using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core
{
    /// <summary>
    /// Level of the log entry
    /// </summary>
    public enum LogLevel
    {
        Error = 3,
        Warning = 2,
        Information = 1,
        Debug = 0
    }

    /// <summary>
    /// Interface class for logging
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Initializes the logger object
        /// </summary>
        bool Init();

        /// <summary>
        /// Logs a message of the specified type
        /// </summary>
        /// <param name="level">Type of log message</param>
        /// <param name="message">Message to log</param>
        void Log(LogLevel level, string message);
    }
}
