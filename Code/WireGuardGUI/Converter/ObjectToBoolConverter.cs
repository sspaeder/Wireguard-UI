using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WireGuard.GUI
{
    /// <summary>
    /// Class to convert an object to an boolean (checks if the objects is not null
    /// </summary>
    class ObjectToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Converts object to bool
        /// </summary>
        /// <param name="value">object to check</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null;


        //Not needed
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
