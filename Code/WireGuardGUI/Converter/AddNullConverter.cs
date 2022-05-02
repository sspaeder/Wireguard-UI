using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WireGuard.Core.ViewModels;

namespace WireGuard.GUI.Converter
{
    /// <summary>
    /// Adds an empty entry to a collection
    /// </summary>
    class AddNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<ConfigViewModel> data = (IEnumerable<ConfigViewModel>)value;

            // Add an dummy object to the items
            List<object> lstData = new List<object>();
            lstData.Add(App.Current.Resources["LBL_SET_EMPTY"].ToString());
            lstData.AddRange(data);

            return lstData;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
