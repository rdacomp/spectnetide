using Spect.Net.SpectrumEmu.Abstraction.Devices.Floppy;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Spect.Net.VsPackage.Dialogs.Vfdd
{
    /// <summary>
    /// Converts an ExportFormat value to checkbox state
    /// </summary>
    public class DiskFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FloppyFormat formatValue
                && Enum.TryParse<FloppyFormat>(parameter?.ToString() ?? "", out var format))
            {
                return formatValue == format;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue
                && Enum.TryParse<FloppyFormat>(parameter?.ToString() ?? "", out var format))
            {
                return boolValue ? format : 0;
            }
            return 0;
        }
    }
}
