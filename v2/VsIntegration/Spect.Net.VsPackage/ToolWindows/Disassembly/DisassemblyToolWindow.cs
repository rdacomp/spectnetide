using System.Runtime.InteropServices;
using Spect.Net.VsPackage.VsxLibrary.ToolWindow;

namespace Spect.Net.VsPackage.ToolWindows.Disassembly
{
    /// <summary>
    /// This class implements the Z80 Disassembly tool window.
    /// </summary>
    [Guid("149E947C-6296-4BCE-A939-A5CD3AA6195F")]
    [Caption("Z80 Disassembly")]
    public class DisassemblyToolWindow :
        SpectrumToolWindowPane<DisassemblyToolWindowControl, DisassemblyToolWindowViewModel>
    {
        protected override DisassemblyToolWindowViewModel GetVmInstance()
        {
            return SpectNetPackage.Default.DisassemblyViewModel;
        }
    }
}