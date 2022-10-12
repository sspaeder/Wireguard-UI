using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WireGuard.Core.Classes
{
    /// <summary>
    /// Class with all paths
    /// </summary>
    public static class Path
    {
        /// <summary>
        /// Path to the main folder
        /// </summary>
        public static readonly string BASE_PATH = AppContext.BaseDirectory;

        /// <summary>
        /// Path to the lockfile
        /// </summary>
        public static readonly string PLUGIN_FOLDER = $@"{BASE_PATH}\Plugins";

        /// <summary>
        /// Path to the settings.json
        /// </summary>
        public static readonly string SETTINGS = $@"{BASE_PATH}\settings.json";

        /// <summary>
        /// Location of the server log file
        /// </summary>
        public static readonly string SERVER_LOG = @$"{BASE_PATH}\Log.txt";

        /// <summary>
        /// Name of the Wireguard GUI service
        /// </summary>
        public const string WIREGUARD_GUI_SERVICE = "WireGuardUIService";

        /// <summary>
        /// Name of the wireguard manager service
        /// </summary>
        public const string WIREGUARD_SERVICE_NAME = "WireGuardManager";

        /// <summary>
        /// Location of the user log file
        /// </summary>
        public const string USER_LOG = @"C:\Users\<USER>\AppData\Local\Wireguard GUI\Log.txt";

        /// <summary>
        /// Location of the wireguard log file
        /// </summary>
        public const string WIREGUARD_LOG = @"C:\Program Files\WireGuard\Data\log.bin";

        /// <summary>
        /// Basepath to wireguard installed
        /// </summary>
        public static readonly string WIREGUARD_PATH = $@"{Environment.GetEnvironmentVariable("ProgramFiles")}\WireGuard";

        /// <summary>
        /// Path to the wireguard exe
        /// </summary>
        public static readonly string WIREGUARD_EXE = $@"{WIREGUARD_PATH}\wireguard.exe";

        /// <summary>
        /// Path to the wg exe from wireguard
        /// </summary>
        public static readonly string WIREGUARD_WG_EXE = $@"{WIREGUARD_PATH}\wg.exe";

        /// <summary>
        /// Path to the wiregurad data folder
        /// </summary>
        public static readonly string WIREGUARD_DATA = $@"{WIREGUARD_PATH}\Data";

        /// <summary>
        /// Path to the wiregurad config files
        /// </summary>
        public static readonly string WIREGUARD_CONFIG = $@"{WIREGUARD_DATA}\Configurations";

        /// <summary>
        /// Folder of the lockfile
        /// </summary>
        public static readonly string DATA_FOLDER = $@"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\WireguardUI";

        /// <summary>
        /// Lockfile for the application
        /// </summary>
        public static readonly string LOCK_FILE = $@"{DATA_FOLDER}\Lock.lock";
    }
}
