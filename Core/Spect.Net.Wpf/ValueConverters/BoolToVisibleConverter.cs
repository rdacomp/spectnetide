using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Spect.Net.Wpf.ValueConverters
{
    /// <summary>
    /// This converter creates a visibility value from an bool.
    /// </summary>
    public class BoolToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}