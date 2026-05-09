using FireDotNetLibrary;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FireDotNetUi.Converters
{
    [ValueConversion(typeof(InflationOption), typeof(string))]
    public class InflationOptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            InflationOption option = (InflationOption)value;
            return option.GetDisplayDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}