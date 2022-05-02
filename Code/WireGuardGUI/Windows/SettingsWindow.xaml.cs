using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WireGuard.GUI.Windows
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method gets called when the abort button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Mehtod gets called when the apply button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            UpdateSource(checkStartOnBoot, CheckBox.IsCheckedProperty);
            UpdateSource(cbSartOnBoot, ComboBox.SelectedItemProperty);

            UpdateSource(cbDefaultConfig, ComboBox.SelectedItemProperty);
            UpdateSource(cbRestoreSession, CheckBox.IsCheckedProperty);

            //TODO: Validate the settings consistents
            //Exampel: When StartOnBoot is true but no config is selected

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Updates an explicit binding
        /// </summary>
        /// <param name="element">element who is to be updated</param>
        /// <param name="dp">dependency property with the data</param>
        private void UpdateSource(Control element, DependencyProperty dp) =>
            element.GetBindingExpression(dp).UpdateSource();
    }
}
