using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Devices.Ports
{
    /// <summary>
    /// Emulates the floating point bus of Spectrum +3E
    /// </summary>
    public class SpectrumP3FloppyCommandPortHandler : PortHandlerBase
    {
        private const ushort PORTMASK = 0b1111_1111_1111_1111;
        private const ushort PORT = 0b0011_1111_1111_1101;

        private IFloppyDevice _floppyDevice;

        /// <summary>
        /// Initializes a new port handler with the specified attributes.
        /// </summary>
        /// <param name="parent">Parent device</param>
        public SpectrumP3FloppyCommandPortHandler(IPortDevice parent) : base(parent, PORTMASK, PORT)
        {
        }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public override void OnAttachedToVm(ISpectrumVm hostVm)
        {
            base.OnAttachedToVm(hostVm);
            _floppyDevice = hostVm.FloppyDevice;
        }

        /// <summary>
        /// Handles the read from the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="readValue">The value read from the port</param>
        /// <returns>True, if read handled; otherwise, false</returns>
        public override bool HandleRead(ushort addr, out byte readValue)
        {
            readValue = _floppyDevice.ReadResultByte(out var executionMode);
            _floppyDevice.SetExecutionMode(executionMode);
            return true;
        }

        /// <summary>
        /// Writes the specified value to the port
        /// </summary>
        /// <param name="addr">Full port address</param>
        /// <param name="writeValue">Value to write to the port</param>
        public override void HandleWrite(ushort addr, byte writeValue)
        {
            _floppyDevice.WriteCommandByte(writeValue);
        }
    }
}