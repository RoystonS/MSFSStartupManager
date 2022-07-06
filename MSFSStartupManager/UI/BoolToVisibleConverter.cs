using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MSFSStartupManager
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = (bool)value;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
