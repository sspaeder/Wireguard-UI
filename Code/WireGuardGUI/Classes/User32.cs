using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.GUI.Classes
{
    /// <summary>
    /// Class with helper functions for the User32.dll
    /// </summary>
    internal static class User32
    {
        #region Enumerations

        /// <summary>
        /// Enumeration of window visible states
        /// </summary>
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow"/>
        public enum WindowState
        {
            HIDE = 0,
            SHOW_NORMAL = 1,
            SHOW_MINIMIZED = 2,
            SHOW_MAXIMIZED = 3,
            SHOW_NO_ACTIVATE = 4,
            SHOW = 5,
            MINIMIZE = 6,
            SHOW_MIN_NO_ACTIVE = 7,
            SHOW_NA = 8,
            RESTORE = 9,
            SHOW_DEFAULT = 10,
            FORCEMINIMIZE = 11
        }

        #endregion

        #region Methods

        /// <summary>
        /// Method to show a window
        /// </summary>
        /// <param name="hwnd">Handel for the process to show</param>
        /// <param name="nCmdShow">The state for the window</param>
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow"/>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, WindowState nCmdShow);

        /// <summary>
        /// Finds a window from its caption
        /// </summary>
        /// <param name="ZeroOnly">Must be zero</param>
        /// <param name="lpWindowName">Name of the window</param>
        /// <see href="https://www.pinvoke.net/default.aspx/user32.FindWindow"/>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        /// <summary>
        /// Actiavates a window
        /// </summary>
        /// <param name="hWnd">Handel to the window</param>
        /// <see href="https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow"/>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion
    }
}
