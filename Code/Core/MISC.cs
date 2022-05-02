using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core
{
    /// <summary>
    /// Enumeration of the diffrent status of a client
    /// </summary>
    public enum ConfigClientStatus
    {
        Running,        //Config is running
        Pending,        //Config is trying to start
        Stopped         //Config is currently stoped
    }
}
