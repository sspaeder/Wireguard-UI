using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WireGuard.GUI.Converter
{
    /// <summary>
    /// Converter for an bool to Icon
    /// </summary>
    class BoolToIcoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new BitmapImage(new Uri("pack://application:,,,/Images/ICO/Wireguard.ico"));

            if (value is not bool)
                return new BitmapImage(new Uri("pack://application:,,,/Images/ICO/Wireguard.ico"));

            bool b = (bool)value;

            if (b)
                return new BitmapImage(new Uri("pack://application:,,,/Images/ICO/Wireguard_online.ico"));
            else
                return new BitmapImage(new Uri("pack://application:,,,/Images/ICO/Wireguard.ico"));

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
