using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using Spect.Net.VsPackage.Utility;
using Spect.Net.VsPackage.Vsx;

namespace Spect.Net.VsPackage.ToolWindows.Memory
{
    /// <summary>
    /// Interaction logic for MemoryToolWindowControl.xaml
    /// </summary>
    public partial class MemoryToolWindowControl : ISupportsMvvm<MemoryToolWindowViewModel>
    {
        /// <summary>
        /// The view model behind this control
        /// </summary>
        public MemoryToolWindowViewModel Vm { get; private set; }

        /// <summary>
        /// Sets the view model instance
        /// </summary>
        /// <param name="vm">View model instance to set</param>
        void ISupportsMvvm<MemoryToolWindowViewModel>.SetVm(MemoryToolWindowViewModel vm)
        {
            if (Vm != null)
            {
                Vm.ScreenRefreshed -= OnScreenRefreshed;
            }
            DataContext = Vm = vm;
            Vm.ScreenRefreshed += OnScreenRefreshed;
        }

        public MemoryToolWindowControl()
        {
            InitializeComponent();
            PreviewKeyDown += (s, e) => MemoryDumpListBox.HandleListViewKeyEvents(e);
            PreviewKeyDown += (s, arg) => Vm.HandleDebugKeys(arg);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            Prompt.CommandLineEntered += OnCommandLineEntered;
        }

        /// <summary>
        /// The control is loaded
        /// </summary>
        private void OnLoaded(object s, RoutedEventArgs e)
        {
            Vm.MachineViewModel.MemViewPointChanged += OnMemViewPointChanged;
            if (!Vm.ViewInitializedWithSolution)
            {
                Vm.ViewInitializedWithSolution = true;
                if (Vm.VmStopped)
                {
                    Vm.SetRomViewMode(0);                    
                }
                else
                {
                    Vm.SetFullViewMode();
                }
            }
            ScrollToTop(Vm.MachineViewModel.MemViewPoint);
            Vm.RefreshViewMode();
        }

        private void OnMemViewPointChanged(object sender, EventArgs e)
        {
            ScrollToTop(Vm.MachineViewModel.MemViewPoint);
            Vm.RefreshViewMode();
        }

        private void OnUnloaded(object s, RoutedEventArgs e)
        {
            Prompt.IsValid = true;
            Prompt.CommandText = "";
            Vm.MachineViewModel.MemViewPointChanged -= OnMemViewPointChanged;
        }

        /// <summary>
        /// We refresh the memory map.
        /// </summary>
        private void OnScreenRefreshed(object sender, EventArgs args)
        {
            if (IsControlLoaded && Vm.ScreenRefreshCount % 25 == 0)
            {
                DispatchOnUiThread(() =>
                {
                    RefreshVisibleItems();
                    if (Vm.FullViewMode)
                    {
                        Vm.UpdatePageInformation();
                    }
                });
            }
        }

        /// <summary>
        /// When a valid address is provided, we scroll the memory window to that address
        /// </summary>
        private void OnCommandLineEntered(object sender, CommandLineEventArgs e)
        {
            e.Handled = Vm.ProcessCommandline(e.CommandLine,
                out var validationMessage, out var topAddr);
            if (validationMessage != null)
            {
                Prompt.IsValid = false;
                Prompt.ValidationMessage = validationMessage;
            }
            if (topAddr != null)
            {
                ScrollToTop(topAddr.Value);
            }
        }

        /// <summary>
        /// This method refreshes only those memory items that are visible in the 
        /// current viewport
        /// </summary>
        private void RefreshVisibleItems()
        {
            var stack = MemoryDumpListBox.GetInnerStackPanel();
            for (var i = 0; i < stack.Children.Count; i++)
            {
                if ((stack.Children[i] as FrameworkElement)?.DataContext is MemoryLineViewModel memLine)
                {
                    Vm.RefreshItemIfItMayChange((ushort)memLine.BaseAddress);
                }
            }
        }

        /// <summary>
        /// Scrolls the disassembly item with the specified address into view
        /// </summary>
        /// <param name="address"></param>
        public void ScrollToTop(ushort address)
        {
            address &= 0xFFF7;
            var sw = MemoryDumpListBox.GetScrollViewer();
            sw?.ScrollToVerticalOffset(address/16.0);
        }
    }
}
