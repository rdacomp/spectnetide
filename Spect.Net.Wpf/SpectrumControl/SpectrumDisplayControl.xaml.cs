﻿using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Spect.Net.SpectrumEmu.Abstraction.Providers;
using Spect.Net.SpectrumEmu.Devices.Beeper;
using Spect.Net.SpectrumEmu.Devices.Screen;
using Spect.Net.SpectrumEmu.Devices.Tape;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.Wpf.Providers;

namespace Spect.Net.Wpf.SpectrumControl
{
    /// <summary>
    /// Interaction logic for SpectrumDisplayControl.xaml
    /// </summary>
    public partial class SpectrumDisplayControl
    {
        private readonly ScreenConfiguration _displayPars;
        private readonly BeeperConfiguration _beeperPars;
        private WriteableBitmap _bitmap;
        private bool _isReload;

        /// <summary>
        /// The ZX Spectrum virtual machine view model utilized by this user control
        /// </summary>
        public SpectrumVmViewModel Vm { get; set; }

        public IRomProvider RomProvider { get; set; }

        public IClockProvider ClockProvider { get; set; }

        public KeyboardProvider KeyboardProvider { get; set; }

        public bool AllowKeyboardScan { get; set; }

        public IScreenFrameProvider ScreenFrameProvider { get; set; }

        public WaveEarbitFrameProvider EarBitFrameProvider { get; set; }

        public ITzxLoadContentProvider TzxLoadContentProvider { get; set; }

        public ITzxSaveContentProvider TzxSaveContentProvider { get; set; }

        public SpectrumDisplayControl()
        {
            InitializeComponent();
            if (ViewModelBase.IsInDesignModeStatic) return;

            _beeperPars = new BeeperConfiguration();
            _displayPars = new ScreenConfiguration();

            // --- Sign that we are about to load the control the first time
            _isReload = false;
        }

        public virtual void SetupDefaultProviders()
        {
            RomProvider = new ResourceRomProvider(typeof(Spectrum48).Assembly);
            ClockProvider = new ClockProvider();
            KeyboardProvider = new KeyboardProvider();
            AllowKeyboardScan = true;
            ScreenFrameProvider = new DelegatingScreenFrameProvider();
            EarBitFrameProvider = new WaveEarbitFrameProvider(_beeperPars);
            TzxLoadContentProvider = new TzxEmbeddedResourceLoadContentProvider(Assembly.GetEntryAssembly());
            TzxSaveContentProvider = new TzxTempFileSaveContentProvider();
        }

        /// <summary>
        /// Sets up the display related members
        /// </summary>
        public virtual void SetupDisplay()
        {
            _bitmap = new WriteableBitmap(
                _displayPars.ScreenWidth,
                _displayPars.ScreenLines,
                96,
                96,
                PixelFormats.Bgr32,
                null);
            Display.Source = _bitmap;
            Display.Width = _displayPars.ScreenWidth;
            Display.Height = _displayPars.ScreenLines;
            Display.Stretch = Stretch.Fill;
        }

        /// <summary>
        /// Allows to start the Spectrum virtual machine programatically
        /// </summary>
        public virtual void StartVm()
        {
            Vm.StartVmCommand.Execute(null);    
        }

        /// <summary>
        /// We need to stop sound output when the app exists
        /// </summary>
        public void StopSound()
        {
            EarBitFrameProvider.KillSound();
        }

        /// <summary>
        /// Initialize the Spectrum virtual machine dependencies when the user control is loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Vm = DataContext as SpectrumVmViewModel;
            if (Vm == null)
            {
                return;
            }

            Vm.RomProvider = RomProvider;
            Vm.ClockProvider = ClockProvider;
            Vm.KeyboardProvider = KeyboardProvider;
            Vm.ScreenFrameProvider = ScreenFrameProvider;
            Vm.EarBitFrameProvider = EarBitFrameProvider;
            Vm.LoadContentProvider = TzxLoadContentProvider;
            Vm.SaveContentProvider = TzxSaveContentProvider;

            SetupDisplay();

            Focus();
            Vm.DisplayMode = SpectrumDisplayMode.Fit;
            Vm.TapeSetName = "Pac-Man.tzx";

            if (_isReload && Vm.VmState == SpectrumVmState.Running)
            {
                EarBitFrameProvider.PlaySound();
            }

            // --- Register messages this control listens to
            Messenger.Default.Register<SpectrumVmStateChangedMessage>(this, OnVmStateChanged);
            Messenger.Default.Register<SpectrumDisplayModeChangedMessage>(this, OnDisplayModeChanged);
            Messenger.Default.Register<DelegatingScreenFrameProvider.DisplayFrameMessage>(this, OnDisplayFrame);
            Messenger.Default.Register<FastLoadCompletedMessage>(this, OnFastLoadCompleted);

            // --- Now, the control is fully loaded and ready to work
            Messenger.Default.Send(new SpectrumControlFullyLoaded(this));
        }

        /// <summary>
        /// Cleanup when the user control is closed
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            EarBitFrameProvider.PauseSound();

            // --- Unregister messages this control listens to
            Messenger.Default.Unregister<SpectrumVmStateChangedMessage>(this);
            Messenger.Default.Unregister<SpectrumDisplayModeChangedMessage>(this);
            Messenger.Default.Unregister<DelegatingScreenFrameProvider.DisplayFrameMessage>(this);
            Messenger.Default.Unregister<FastLoadCompletedMessage>(this);

            // --- Sign that the next time we load the control, it is a reload
            _isReload = true;
        }

        /// <summary>
        /// Manage the size change of the control
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (Vm == null) return;
            ResizeFor(args.NewSize.Width, args.NewSize.Height);
        }


        /// <summary>
        /// Respond to the state changes of the Spectrum virtual machine
        /// </summary>
        /// <remarks>
        /// This method is called from a background thread!
        /// </remarks>
        private void OnVmStateChanged(SpectrumVmStateChangedMessage message)
        {
            Dispatcher.Invoke(() =>
                {
                    switch (message.NewState)
                    {
                        case SpectrumVmState.Stopped:
                            EarBitFrameProvider.KillSound();
                            break;
                        case SpectrumVmState.Running:
                            EarBitFrameProvider.PlaySound();
                            break;
                        case SpectrumVmState.Paused:
                            EarBitFrameProvider.PauseSound();
                            break;
                    }
                },
                DispatcherPriority.Normal);
        }

        /// <summary>
        /// Responds to the change of display mode
        /// </summary>
        private void OnDisplayModeChanged(SpectrumDisplayModeChangedMessage message)
        {
            ResizeFor(ActualWidth, ActualHeight);
        }

        /// <summary>
        /// It is time to restart playing the sound
        /// </summary>
        private void OnFastLoadCompleted(FastLoadCompletedMessage msg)
        {
            Dispatcher.Invoke(() =>
            {
                Vm.SpectrumVm.BeeperDevice.Reset();
                EarBitFrameProvider.PlaySound();
            });
        }

        private void ResizeFor(double width, double height)
        {
            if (Vm.DisplayMode >= SpectrumDisplayMode.Normal && Vm.DisplayMode <= SpectrumDisplayMode.Zoom5)
            {
                var scale = (int) Vm.DisplayMode;
                PixelScale.ScaleX = PixelScale.ScaleY = scale;
                return;
            }
            var widthFactor = (int)(width / _displayPars.ScreenWidth);
            var heightFactor = (int)height / _displayPars.ScreenLines;
            var factor = Math.Min(widthFactor, heightFactor);
            if (factor < (int)SpectrumDisplayMode.Normal) factor = (int)SpectrumDisplayMode.Normal;
            else if (factor > (int)SpectrumDisplayMode.Zoom5) factor = (int)SpectrumDisplayMode.Zoom5;
            PixelScale.ScaleX = PixelScale.ScaleY = factor;
        }

        /// <summary>
        /// The new screen frame is ready, it is time to display it
        /// </summary>
        /// <param name="message">Message with the screen buffer</param>
        /// <remarks>
        /// This method is called from a background thread!
        /// </remarks>
        private void OnDisplayFrame(DelegatingScreenFrameProvider.DisplayFrameMessage message)
        {
            // --- Refresh the screen
            Dispatcher.Invoke(() =>
                {
                    RefreshSpectrumScreen(message.Buffer);
                    Messenger.Default.Send(new SpectrumScreenRefreshedMessage());
                    if (AllowKeyboardScan)
                    {
                        KeyboardProvider.Scan();
                    }
                },
                DispatcherPriority.Normal
            );
        }

        /// <summary>
        /// Refreshes the spectrum screen
        /// </summary>
        private void RefreshSpectrumScreen(byte[] currentBuffer)
        {
            var width = _displayPars.ScreenWidth;
            var height = _displayPars.ScreenLines;

            _bitmap.Lock();
            unsafe
            {
                var stride = _bitmap.BackBufferStride;
                // Get a pointer to the back buffer.
                var pBackBuffer = (int)_bitmap.BackBuffer;

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var addr = pBackBuffer + y * stride + x * 4;
                        var pixelData = currentBuffer[y * width + x];
                        *(uint*)addr = Spectrum48ScreenDevice.SpectrumColors[pixelData & 0x0F];
                    }
                }
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            _bitmap.Unlock();
        }
    }
}
