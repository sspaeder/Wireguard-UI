namespace WireGuard.Core.PlugIn
{
    /// <summary>
    /// Interface for client plugins
    /// </summary>
    public interface IClientPlugIn
    {
        /// <summary>
        /// Method gets called when the window closes
        /// </summary>
        /// <returns></returns>
        bool ExitHandler();

        /// <summary>
        /// Sets the language to be displayed
        /// </summary>
        /// <param name="language">language to show</param>
        void SetLanguage(string language);

        /// <summary>
        /// Sets the context of the plugin
        /// </summary>
        /// <param name="context">context object to set</param>
        void SetContext(IContext context);

        /// <summary>
        /// Gets the name of the plugin to access it
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the displayname of the plugin
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the start page of the plugin
        /// </summary>
        BasePage StartPage { get; }
    }
}
