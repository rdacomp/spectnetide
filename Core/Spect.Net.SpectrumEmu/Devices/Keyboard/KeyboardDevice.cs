using System;
using Spect.Net.SpectrumEmu.Abstraction.Devices;
using Spect.Net.SpectrumEmu.Abstraction.Providers;

#pragma warning disable 67

namespace Spect.Net.SpectrumEmu.Devices.Keyboard
{
    /// <summary>
    /// This device manages the keyboard of the Spectrum virtual machine
    /// </summary>
    public class KeyboardDevice: IKeyboardDevice
    {
        private IKeyboardProvider _keyboardProvider;
        private byte[] _lineStatus = new byte[8];

        /// <summary>
        /// The virtual machine that hosts the device
        /// </summary>
        public ISpectrumVm HostVm { get; private set; }

        /// <summary>
        /// Signs that the device has been attached to the Spectrum virtual machine
        /// </summary>
        public void OnAttachedToVm(ISpectrumVm hostVm)
        {
            HostVm = hostVm;
            var keyboardInfo = HostVm.GetDeviceInfo<IKeyboardDevice>();
            _keyboardProvider = (IKeyboardProvider)keyboardInfo?.Provider;
            _keyboardProvider?.SetKeyStatusHandler(SetStatus);
        }

        /// <summary>
        /// #of frames rendered
        /// </summary>
        public int FrameCount { get; set; }

        /// <summary>
        /// Overflow from the previous frame, given in #of tacts 
        /// </summary>
        public int Overflow { get; set; }

        /// <summary>
        /// Allow the device to react to the start of a new frame
        /// </summary>
        public void OnNewFrame()
        {
            FrameCount++;
        }

        /// <summary>
        /// Allow the device to react to the completion of a frame
        /// </summary>
        public void OnFrameCompleted()
        {
            HostVm.KeyboardProvider?.EmulateKeyStroke();
        }

        /// <summary>
        /// Allow external entities respond to frame completion
        /// </summary>
        public event EventHandler FrameCompleted;

        /// <summary>
        /// Sets the status of the specified Spectrum keyboard key
        /// </summary>
        /// <param name="key">Key code</param>
        /// <param name="isDown">True, if the key is down; otherwise, false</param>
        public void SetStatus(SpectrumKeyCode key, bool isDown)
        {
            var lineIndex = (byte)key / 5;
            var lineMask = 1 << (byte) key%5;
            _lineStatus[lineIndex] = isDown
                ? (byte)(_lineStatus[lineIndex] | lineMask)
                : (byte)(_lineStatus[lineIndex] & ~lineMask);

        }

        /// <summary>
        /// Gets the status of the specified Spectrum keyboard key.
        /// </summary>
        /// <param name="key">Key code</param>
        /// <returns>True, if the key is down; otherwise, false</returns>
        public bool GetStatus(SpectrumKeyCode key)
        {
            var lineIndex = (byte)key / 5;
            var lineMask = 1 << (byte)key % 5;
            return (_lineStatus[lineIndex] & lineMask) != 0;
        }

        /// <summary>
        /// Gets the byte we would get when querying the I/O address with the
        /// specified byte as the highest 8 bits of the address line
        /// </summary>
        /// <param name="lines">The highest 8 bits of the address line</param>
        /// <returns>
        /// The status value to be received when querying the I/O
        /// </returns>
        public byte GetLineStatus(byte lines)
        {
            byte status = 0;
            lines = (byte)~lines;

            var lineIndex = 0;
            while (lines > 0)
            {
                if ((lines & 0x01) != 0)
                {
                    status |= _lineStatus[lineIndex];
                }
                lineIndex++;
                lines >>= 1;
            }
            return (byte)~status;
        }

        /// <summary>
        /// Resets this device
        /// </summary>
        public void Reset()
        {
            for (var i = 0; i < _lineStatus.Length; i++)
            {
                _lineStatus[i] = 0;
            }
        }

        /// <summary>
        /// Gets the state of the device so that the state can be saved
        /// </summary>
        /// <returns>The object that describes the state of the device</returns>
        IDeviceState IDevice.GetState() => new KeyboardDeviceState(this);

        /// <summary>
        /// Sets the state of the device from the specified object
        /// </summary>
        /// <param name="state">Device state</param>
        public void RestoreState(IDeviceState state) => state.RestoreDeviceState(this);

        public class KeyboardDeviceState : IDeviceState
        {
            public byte[] LineStatus { get; set; }

            public KeyboardDeviceState()
            {
            }

            public KeyboardDeviceState(KeyboardDevice device)
            {
                LineStatus = device._lineStatus;
            }

            /// <summary>
            /// Restores the dvice state from this state object
            /// </summary>
            /// <param name="device">Device instance</param>
            public void RestoreDeviceState(IDevice device)
            {
                if (!(device is KeyboardDevice keyboard)) return;

                keyboard._lineStatus = LineStatus;
            }
        }
    }
}

#pragma warning restore 67
