using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WireGuard.Core
{
    /// <summary>
    /// Logs the messages to a file
    /// </summary>
    public class Log2File : ILog
    {
        #region Variables

        readonly string fileName;
        readonly string folderPath;
        readonly string completePath;

        #endregion

        #region Constructors

        /// <summary>
        /// Construcotr
        /// </summary>
        /// <param name="fileName">Filename to log to</param>
        /// <param name="folderPath">Path to the folder where the log should be stored</param>
        public Log2File(string fileName, string folderPath)
        {
            this.fileName = fileName;
            this.folderPath = folderPath;
            completePath = $@"{folderPath}\{fileName}";
        }

        /// <summary>
        /// Construcotr of the class
        /// </summary>
        public Log2File()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the object
        /// </summary>
        public bool Init()
        {
            if(!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            return true;
        }

        /// <summary>
        /// Logs a message to the file
        /// </summary>
        /// <param name="level">Level of error</param>
        /// <param name="message">Message to log</param>
        public void Log(LogLevel level, string message)
        {
            switch(level)
            {
                case LogLevel.Debug:
                    Debug(message);
                    break;

                case LogLevel.Information:
                    Information(message);
                    break;

                case LogLevel.Warning:
                    Warning(message);
                    break;

                case LogLevel.Error:
                    Error(message);
                    break;
            }
        }

        /// <summary>
        /// Basemehtod for all log types
        /// </summary>
        /// <param name="type">Type of the message</param>
        /// <param name="message">Message to be logged</param>
        private void BaseLog(string type, string message)
        {
            Monitor.Enter(this);

            using (StreamWriter sw = new StreamWriter(completePath, true))
                sw.WriteLine($"[{type}][{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] {message}");

            Monitor.Exit(this);
        }

        /// <summary>
        /// Logs a debug message to the logfile
        /// </summary>
        /// <param name="message">Message to be logged</param>
        private void Debug(string message) => BaseLog("DBG", message);

        /// <summary>
        /// Loggs an information in the logfile
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public void Information(string message) => BaseLog("INF", message);

        /// <summary>
        /// Loggs a warning in the logfile
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public void Warning(string message) => BaseLog("WAR", message);

        /// <summary>
        /// Logs an error text to the log file
        /// </summary>
        /// <param name="message">Message to be logged</param>
        public void Error(string message) => BaseLog("ERR", message);

        #endregion
    }
}
