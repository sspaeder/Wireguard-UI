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
    /// Converter for the conversion if a setting should be displayed or not
    /// </summary>
    class AvailabelVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Value is a string of available settings
            string[] data = value as string[];

            //if value is not a string, return null
            if (data == null)
                return null;

            //If the star is existent or the length is 0, show all
            if (data.Contains("*") || data.Length == 0)
                return Visibility.Visible;

            //If the NONE value is present, show nothing
            if (data.Contains("NONE"))
                return Visibility.Collapsed;

            //Check if the control is availabel or not
            if (data.Contains(parameter.ToString()))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        //Not needed
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
