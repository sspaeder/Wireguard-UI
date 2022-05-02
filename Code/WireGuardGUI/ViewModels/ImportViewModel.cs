using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WireGuard.Core;
using WireGuard.Core.Messages;
using WireGuard.Core.PlugIn;
using WireGuard.Core.ViewModels;
using WireGuard.GUI.Pages;
using WireGuard.GUI.Windows;


namespace WireGuard.GUI.ViewModels
{
    /// <summary>
    /// Viewmodel for the import actions
    /// </summary>
    class ImportViewModel : BaseViewModel
    {
        #region Variables

        /// <summary>
        /// Variable for the plugin controller to 
        /// </summary>
        PlugInController<IClientPlugIn> plugIns;

        /// <summary>
        /// Window to be represented by the viewmodel
        /// </summary>
        ImportWindow window;

        /// <summary>
        /// Frame to display pages
        /// </summary>
        Frame firstFrame;

        /// <summary>
        /// Frame to dispaly the pages
        /// </summary>
        Frame secondFrame;

        /// <summary>
        /// Variable for the displayed plugin
        /// </summary>
        IClientPlugIn displayed;

        /// <summary>
        /// Context for displayed plugins
        /// </summary>
        PlugInContext context;

        /// <summary>
        /// Client to communicate to the Server
        /// </summary>
        Client client;

        /// <summary>
        /// Collection of configurations
        /// </summary>
        ConfigCollectionViewModel confColVM;

        /// <summary>
        /// Determines if the window is closing without exit message
        /// </summary>
        bool exitRequested = false;

        #endregion

        #region Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">Client to communicate to the server</param>
        /// <param name="ccvm">Collection with the configurations</param>
        public ImportViewModel(Client client, ConfigCollectionViewModel ccvm)
        {
            confColVM = ccvm;

            plugIns = new PlugInController<IClientPlugIn>();

            if (System.IO.Directory.Exists(Path.PLUGIN_FOLDER))
            {
                foreach (string dll in System.IO.Directory.GetFiles(Path.PLUGIN_FOLDER, "*.dll"))
                    plugIns.Load(dll);
            }
            else
                System.IO.Directory.CreateDirectory(Path.PLUGIN_FOLDER);

            this.client = client;

            context = new PlugInContext(client);
            context.GoToPageEvent += Context_GoToPageEvent;
            context.RequestExitEvent += Context_RequestExitEvent;
            context.ConfigChanged += Context_ConfigChanged;

            StartPlugInCmd = new RelayCommand(StartPlugInMethod);

            foreach (IClientPlugIn p in plugIns)
            {
                p.SetContext(context);
                p.SetLanguage(System.Globalization.CultureInfo.CurrentCulture.ToString());
            }
        }

        /// <summary>
        /// Initalizes the ViewModel 
        /// </summary>
        /// <param name="win"></param>
        /// <param name="f1">Frame to display pages</param>
        /// <param name="f2">Frame to display pages</param>
        public void Init(ImportWindow win, Frame f1, Frame f2)
        {
            window = win;

            window.Closing += Window_Closing;
            window.Loaded += Window_Loaded;

            firstFrame = f1;
            secondFrame = f2;
        }

        /// <summary>
        /// Method to navigate to a page
        /// </summary>
        /// <param name="page">Page to navigate to</param>
        private async void GotToPage(BasePage page)
        {
            BasePage first = firstFrame.Content as BasePage;
            BasePage second = secondFrame.Content as BasePage;

            if(first == null && second == null)
            {
                firstFrame.Content = page;
                await page.AnimateIn();
            }
            else if (first == null)
            {
                firstFrame.Content = page;
                page.RenderSize = firstFrame.RenderSize;
                await Task.WhenAll(new Task[] { page.AnimateIn(), second.AnimateOut() });
                secondFrame.Content = null;
            }
            else if (second == null)
            {
                secondFrame.Content = page;
                page.RenderSize = secondFrame.RenderSize;
                await Task.WhenAll(new Task[] { first.AnimateOut(), page.AnimateIn() });
                firstFrame.Content = null;
            }
        }

        #endregion

        #region Handler Methods

        /// <summary>
        /// Method to handel the window loading
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GotToPage(new StartPage() { DataContext = this });
        }

        /// <summary>
        /// Handler method before the window is closing
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(exitRequested == false)
            {
                if (displayed != null)
                    e.Cancel = displayed.ExitHandler();

                if (!e.Cancel)
                    context.Clear();   
            }
            else
                exitRequested = false;
        }

        /// <summary>
        /// Event when a plugin requests a page change
        /// </summary>
        /// <param name="page">page to change to</param>
        private void Context_GoToPageEvent(BasePage page) => GotToPage(page);

        /// <summary>
        /// Requests to close the window without exit message
        /// </summary>
        private void Context_RequestExitEvent()
        {
            exitRequested = true;
            window.Close();
        }

        /// <summary>
        /// Handels the changings of a configuration
        /// </summary>
        /// <param name="file">File to add or to remove</param>
        /// <param name="delete">Should the file be deleted or added</param>
        private void Context_ConfigChanged(string file, bool delete = false)
        {
            if(delete)
            {
                ConfigViewModel cvm = confColVM.Configs.First(x => x.Name == file);

                if (cvm == null)
                    return;

                confColVM.Remove(cvm);
            }
            else
            {
                confColVM.Add(file);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to start a plugin
        /// </summary>
        public RelayCommand StartPlugInCmd { get; }

        /// <summary>
        /// Method to start a plugin
        /// </summary>
        /// <param name="parameter">Name of the plugin to start</param>
        private void StartPlugInMethod(object parameter)
        {
            // File import
            if(parameter == null)
            {
                FileImportAction?.Invoke();
                return;
            }

            if (parameter is not string)
                return;

            string plugInName = parameter.ToString();

            displayed = plugIns.Find(x => x.Name == plugInName);

            if (displayed == null)
                return;

            displayed.SetContext(context);
            GotToPage(displayed.StartPage);
        }

        #endregion

        #region Propertys

        /// <summary>
        /// Action for the fileimport
        /// </summary>
        public Action FileImportAction { get; init; }

        /// <summary>
        /// Gets the number of plugins loaded
        /// </summary>
        public int Count { get => plugIns.Count; }

        /// <summary>
        /// Gets all loaded plugins
        /// </summary>
        public IEnumerable<IClientPlugIn> LoadedPlugIns { get => plugIns.OrderBy(x => x.Name); }

        #endregion

    }
}
