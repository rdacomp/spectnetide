using Spect.Net.SpectrumEmu.Abstraction.Machine;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

namespace Spect.Net.SpectrumEmu.Machine
{
    /// <summary>
    /// Default implementation of the Spectrum debug info provider
    /// </summary>
    public class SpectrumDebugInfoProvider: VmComponentProviderBase, ISpectrumDebugInfoProvider
    {
        /// <summary>
        /// The currently defined breakpoints
        /// </summary>
        public BreakpointCollection Breakpoints { get; }

        /// <summary>
        /// Gets or sets an imminent breakpoint
        /// </summary>
        public ushort? ImminentBreakpoint { get; set; }

        /// <summary>
        /// Us this method to prepare the breakpoints when running the
        /// virtual machine in debug mode
        /// </summary>
        public void PrepareBreakpoints()
        {
        }

        /// <summary>
        /// Resets the current hit count of breakpoints
        /// </summary>
        public void ResetHitCounts()
        {
            foreach (var bp in Breakpoints.Values)
            {
                bp.CurrentHitCount = 0;
            }
        }

        /// <summary>
        /// Checks if the virtual machine should stop at the specified address
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>
        /// True, if the address means a breakpoint to stop; otherwise, false
        /// </returns>
        public bool ShouldBreakAtAddress(ushort address)
        {
            return Breakpoints.ContainsKey(address);
        }

        public SpectrumDebugInfoProvider()
        {
            Breakpoints = new BreakpointCollection();
        }
    }
}