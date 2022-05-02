using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class LogContentMessage : Message
    {
        /// <summary>
        /// 
        /// </summary>
        public LogContentMessage() : base("logcontent")
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToJSON() => System.Text.Json.JsonSerializer.Serialize(this);
    }
}
