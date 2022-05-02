using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WireGuard.Core.ViewModels;
using WireGuard.GUI.ViewModels;

namespace WireGuard.GUI.Converter
{
    /// <summary>
    /// Replaces a null with an value
    /// </summary>
    class NullReplaceConverter : IValueConverter
    {
        //Converts from string to object
        //String is always targettype because of binding
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //If value is null return the empty etnry 
            //See AddNullConverter
            if (value == null)
                return App.Current.Resources["LBL_SET_EMPTY"].ToString();

            MainViewModel mvm = App.Current.MainWindow.DataContext as MainViewModel;
            ConfigViewModel cvm = mvm.Collection.Configs.FirstOrDefault(x => x.Name == value.ToString());

            //If an element is found, return the element elese the empty entry
            if (cvm != null)
                return cvm;
            else //Can appear because of not existent config
                return App.Current.Resources["LBL_SET_EMPTY"].ToString();
        }

        //Converts to string
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Check if value is null
            if (value != null)
            {
                //If value is a string it only can be an empty entry
                //See AddNullConverter
                if (value is string)
                    return null;

                MainViewModel mvm = App.Current.MainWindow.DataContext as MainViewModel;
                ConfigViewModel cvm = mvm.Collection.Configs.FirstOrDefault(x => x.DisplayName == value.ToString());

                //If an element is found, return the name, else return nothing
                if (cvm != null)
                    return cvm.Name;
                else //Should not appear
                    return null;
            }
            else //No value selected
                return null;
        }
    }
}
