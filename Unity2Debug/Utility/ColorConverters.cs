using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Unity2Debug.Utility
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isValid && !isValid)
                return Brushes.Red;
            else
                return Brushes.Black;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) { return default; }
    }
}
