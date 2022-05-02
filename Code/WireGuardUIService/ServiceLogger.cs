using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;

namespace WireGuard.WireGuardUIService
{
    /// <summary>
    /// Class for the logging from the service
    /// </summary>
    internal class ServiceLogger : ILog
    {
        private readonly ILogger<Worker> logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public ServiceLogger(ILogger<Worker> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Init() => logger != null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Log(Core.LogLevel level, string message)
        {
            switch(level)
            {
                case Core.LogLevel.Information:
                    logger.LogInformation(message);
                    break;

                case Core.LogLevel.Error:
                    logger.LogError(message);
                    break;
            }
        }
    }
}
