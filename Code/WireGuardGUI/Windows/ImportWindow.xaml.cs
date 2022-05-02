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
using WireGuard.GUI.ViewModels;

namespace WireGuard.GUI.Windows
{
    /// <summary>
    /// Interaktionslogik für ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        /// <summary>
        /// Variable for the viewmodel
        /// </summary>
        ImportViewModel ivm;

        /// <summary>
        /// Constructor
        /// </summary>
        public ImportWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Eventhandler for datacontext changes
        /// </summary>
        private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ivm = (ImportViewModel)e.NewValue;
            ivm.Init(this, frame1, frame2);
        }
    }
}
