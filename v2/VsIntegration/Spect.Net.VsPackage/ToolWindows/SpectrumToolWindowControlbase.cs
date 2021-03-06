using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Spect.Net.SpectrumEmu.Machine;
using Spect.Net.VsPackage.ToolWindows.SpectrumEmulator;

namespace Spect.Net.VsPackage.ToolWindows
{
    /// <summary>
    /// This class is intended to be the base user control for all Spectrum-related
    /// tool windows controls.
    /// </summary>
    public abstract class SpectrumToolWindowControlBase : UserControl
    {
        public static readonly DependencyProperty ZoomAllowedProperty = DependencyProperty.Register(
            "ZoomAllowed", typeof(bool), typeof(SpectrumToolWindowControlBase), new PropertyMetadata(default(bool)));

        public bool ZoomAllowed
        {
            get => (bool)GetValue(ZoomAllowedProperty);
            set => SetValue(ZoomAllowedProperty, value);
        }

        /// <summary>
        /// Signs whether the control is loaded
        /// </summary>
        public bool IsControlLoaded { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Controls.UserControl" /> class.</summary>
        protected SpectrumToolWindowControlBase()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            IsControlLoaded = false;
            Loaded += (s, e) =>
            {
                var emuVm = SpectNetPackage.Default.EmulatorViewModel;
                emuVm.VmStateChanged += OnInternalVmStateChanged;
                emuVm.MachineInstanceChanged += OnInternalMachineInstanceChanged;
                SpectNetPackage.Default.Options.ZoomFactorChanged += OnZoomFactorChanged;
                IsControlLoaded = true;
                SetZoomFactor();
            };
            Unloaded += (s, e) =>
            {
                IsControlLoaded = false;
                var emuVm = SpectNetPackage.Default.EmulatorViewModel;
                emuVm.VmStateChanged -= OnInternalVmStateChanged;
                emuVm.MachineInstanceChanged -= OnInternalMachineInstanceChanged;
                SpectNetPackage.Default.Options.ZoomFactorChanged -= OnZoomFactorChanged;
            };
        }

        /// <summary>
        /// Dispatch the vm state changed message on the UI thread
        /// </summary>
        private void OnInternalVmStateChanged(object sender, VmStateChangedEventArgs e)
        {
            DispatchOnUiThread(() => OnVmStateChanged(e.OldState, e.NewState));
        }

        /// <summary>
        /// Override this message to respond to vm state changes events.
        /// </summary>
        protected virtual void OnVmStateChanged(VmState oldState, VmState newState)
        {
        }

        /// <summary>
        /// Dispatch the instance changed message on the UI thread
        /// </summary>
        private void OnInternalMachineInstanceChanged(object sender, MachineInstanceChangedEventArgs e)
        {
            DispatchOnUiThread(() => OnMachineInstanceChanged(e.OldMachine, e.NewMachine));
        }

        /// <summary>
        /// Override this message to respond to machine instance changes events.
        /// </summary>
        protected virtual void OnMachineInstanceChanged(SpectrumMachine oldMachine, SpectrumMachine newMachine)
        {
        }

        /// <summary>
        /// This method dispatches the specified action asynchronously on the UI thread with the provided
        /// priority
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="priority">Dispatch priority. By default, it is the highest (Send)</param>
        /// <remarks>
        /// This implementation does not handle the result of the async dispatch
        /// </remarks>
        protected void DispatchOnUiThread(Action action, DispatcherPriority priority = DispatcherPriority.Send)
        {
#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
            Dispatcher.InvokeAsync(() =>
#pragma warning restore VSTHRD001 // Avoid legacy thread switching APIs
            {
                try
                {
                    action();
                }
                catch
                {
                    // --- We intentionally ignore this exception
                }
            }, priority);
        }

        /// <summary>
        /// Respond to the event when zoom factor changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnZoomFactorChanged(object sender, ZoomFactorChangedEventArgs e)
        {
            SetZoomFactor();
        }

        /// <summary>
        /// Sets the zoom factor
        /// </summary>
        private void SetZoomFactor()
        {
            if (!ZoomAllowed) return;
            if (!(FindName("MainPanel") is Grid mainPanel)) return;

            var scale = ConvertZoomFactor(SpectNetPackage.Default.Options.ToolWindowZoomFactor);
            mainPanel.LayoutTransform = new ScaleTransform(scale, scale);
            mainPanel.UseLayoutRounding = true;
        }

        /// <summary>
        /// Converts the zoom factor to a double number
        /// </summary>
        private static double ConvertZoomFactor(ZoomFactor value)
        {
            switch (value)
            {
                case ZoomFactor.Zero50:
                    return 0.5;
                case ZoomFactor.Zero80:
                    return 0.8;
                case ZoomFactor.One:
                    return 1.0;
                case ZoomFactor.One20:
                    return 1.2;
                case ZoomFactor.One33:
                    return 1.33;
                case ZoomFactor.One50:
                    return 1.5;
                case ZoomFactor.Two:
                    return 2.0;
                case ZoomFactor.Two50:
                    return 2.5;
                default:
                    return 1.0;
            }
        }
    }
}