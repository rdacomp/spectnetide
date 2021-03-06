using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Devices.Screen;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// This class implements a base class for port handling that implements the
    /// "floating bus" feature of the ULA.
    /// </summary>
    /// <remarks>
    /// http://ramsoft.bbk.org.omegahg.com/floatingbus.html
    /// </remarks>
    public class UlaGenericPortDeviceBase : GenericPortDeviceBase
    {
        private IMemoryDevice _memoryDevice;

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _memoryDevice = hostVm.MemoryDevice;
        }

        /// <summary>
        /// Get the memory device
        /// </summary>
        public IMemoryDevice MemoryDevice => _memoryDevice;

        /// <summary>
        /// Define how to handle an unattached port
        /// </summary>
        /// <param name="addr">Port address</param>
        /// <returns>Port value for the unhandled port address</returns>
        public override byte UnhandledRead(ushort addr)
        {
            var tact = HostVm.CurrentFrameTact % ScreenDevice.RenderingTactTable.Length;
            var rt = ScreenDevice.RenderingTactTable[tact];
            var memAddr = (ushort)0;
            switch (rt.Phase)
            {
                case ScreenRenderingPhase.BorderFetchPixelAttr:
                case ScreenRenderingPhase.DisplayB1FetchA2:
                case ScreenRenderingPhase.DisplayB2FetchA1:
                    memAddr = rt.AttributeToFetchAddress;
                    break;
            }

            if (memAddr == 0) return 0xFF;
            var readValue = _memoryDevice.Read(memAddr, true);
            return readValue;
        }
    }
}