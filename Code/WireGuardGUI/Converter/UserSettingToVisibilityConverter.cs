using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WireGuard.GUI.Converter
{
    /// <summary>
    /// Converts the user availablesetting to a visibility
    /// (In fact, it hides the stettings button if there are no settings to make)
    /// </summary>
    class UserSettingToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;

            string[] data = (string[])value;

            if (data.Contains("NONE"))
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
