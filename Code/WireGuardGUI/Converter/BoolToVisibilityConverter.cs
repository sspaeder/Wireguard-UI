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
    /// Converts a bool to a Visibilty
    /// </summary>
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;

            bool data = (bool)value;
            bool invenrt = false;

            if (parameter != null)
                invenrt = Boolean.Parse(parameter.ToString());

            if (invenrt)
                data = !data;

            if (data)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
