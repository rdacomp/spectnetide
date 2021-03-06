using Spect.Net.SpectrumEmu.Abstraction.Devices;

namespace Spect.Net.SpectrumEmu.Abstraction.Configuration
{
    /// <summary>
    /// This class describes configuration information for the 
    /// Spectrum Next feature set device
    /// </summary>
    public class NextDeviceInfo :
        DeviceInfoBase<INextFeatureSetDevice, INoConfiguration, INoProvider>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        /// <param name="device">Next feature set device instance</param>
        public NextDeviceInfo(INextFeatureSetDevice device) 
            : base(null, null, device)
        {
        }
    }
}