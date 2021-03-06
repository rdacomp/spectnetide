using System.ComponentModel;
using Spect.Net.VsPackage.Vsx.Output;

namespace Spect.Net.VsPackage.Z80Programs
{
    [DisplayName("Z80 Build Output")]
    [AutoActivate(true)]
    [ClearWithSolution(false)]
    public class Z80BuildOutputPane : OutputPaneDefinition
    {
    }

    [DisplayName("ZX Spectrum Output")]
    [AutoActivate(true)]
    [ClearWithSolution(false)]
    public class SpectrumVmOutputPane : OutputPaneDefinition
    {
    }
}