using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MSFSStartupManager
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;
            return b ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
