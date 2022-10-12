using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core
{
    /// <summary>
    /// Class for the logging of events
    /// </summary>
    public static class LogManager
    {
        #region Variables

        static List<ILog> loggers;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the log manager
        /// </summary>
        public static void Init()
        {
            loggers = new List<ILog>();
        }

        /// <summary>
        /// Adds a logger to the log manager
        /// </summary>
        /// <param name="log">log object to take part</param>
        public static void Register(ILog log)
        {
            if(log.Init())
                loggers.Add(log);
        }

        /// <summary>
        /// Logs a message to all registrated logger
        /// </summary>
        /// <param name="level">Level of the log message</param>
        /// <param name="message">Message to log</param>
        private static void Log(LogLevel level, string message)
        {
            if(level >= LogLevel)
                foreach(ILog log in loggers)
                    log.Log(level, message);
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="message">Message to show</param>
        /// <param name="function">function who calls the log operation</param>
        /// <param name="cls">class who calls the operation</param>
        public static void Debug(string message, string cls, [CallerMemberName] string function = "") => Log(LogLevel.Debug ,$"{cls}.{function}: {message}");

        /// <summary>
        /// Logs an information
        /// </summary>
        /// <param name="message">Message to log</param>
        public static void Information(string message) => Log(LogLevel.Information, message);

        /// <summary>
        /// Logs a warning
        /// </summary>
        /// <param name="message">warning message to log</param>
        public static void Warning(string message) => Log(LogLevel.Warning, message);

        /// <summary>
        /// Logs an error
        /// </summary>
        /// <param name="message">error message to log</param>
        public static void Error(string message) => Log(LogLevel.Error, message);

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="ex">Exception to log</param>
        public static void Error(Exception ex) => Error($"{ex.Message}\n{ex.StackTrace}");

        /// <summary>
        /// Sets the log level of from a string
        /// </summary>
        /// <param name="logLvl">Log level to apply</param>
        public static void SetLogLvlFromString(string logLvl)
        {
            switch(logLvl)
            {
                case "Debug":
                    LogLevel = LogLevel.Debug;
                    break;

                case "Info":
                    LogLevel = LogLevel.Information;
                    break;

                case "Warning":
                    LogLevel= LogLevel.Warning;
                    break;

                case "Error":
                    LogLevel= LogLevel.Error;
                    break;

                default:
                    throw new ArgumentException($"Unknown log level: {logLvl}.\nValid are Debug, Info, Warning, Error.");
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the loglevel of the manager
        /// </summary>
        public static LogLevel LogLevel { get; set; } = LogLevel.Information;

        #endregion
    }
}
