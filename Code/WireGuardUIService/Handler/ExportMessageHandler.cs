using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WireGuard.Core;
using WireGuard.Core.Messages;
using System.CodeDom;

namespace WireGuard.WireGuardUIService.Handler
{
    internal class ExportMessageHandler : MessageHandler
    {
        public ExportMessageHandler(Context context) : base(context)
        {

        }

        public override Type Type => typeof(ExportMessage);

        public override void Handel(Server server, Message message)
        {
            ExportMessage export = (ExportMessage)message;

            try
            {
                LogManager.Debug("Creating archive", nameof(ExportMessageHandler), nameof(Handel));

                using (FileStream fs = new FileStream(export.FilePath, FileMode.OpenOrCreate))
                {
                    using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        string info = "";

                        // Writing user log
                        if (File.Exists(Core.Classes.Path.USER_LOG.Replace("<USER>", export.Username)))
                        {
                            LogManager.Debug("Compress user.log", nameof(ExportMessageHandler), nameof(Handel));
                            archive.CreateEntryFromFile(Core.Classes.Path.USER_LOG.Replace("<USER>", export.Username), "User.log");
                        }
                        else
                            info += "\tNo user log found\n";

                        // Writing server log
                        if (File.Exists(Core.Classes.Path.SERVER_LOG))
                        {
                            LogManager.Debug("Compress server.log", nameof(ExportMessageHandler), nameof(Handel));
                            archive.CreateEntryFromFile(Core.Classes.Path.SERVER_LOG, "Server.log");
                        }
                        else
                            info += "\tNo server log found\n";

                        //Writing wireguard log
                        if (File.Exists(Core.Classes.Path.WIREGUARD_LOG))
                        {
                            LogManager.Debug("Compress wg.log", nameof(ExportMessageHandler), nameof(Handel));
                            archive.CreateEntryFromFile(Core.Classes.Path.WIREGUARD_LOG, "wg.log");
                        }
                        else
                            info += "\tNo wireguard log found\n";

                        // Writing additional informations
                        LogManager.Debug("Compress informations", nameof(ExportMessageHandler), nameof(Handel));
                        ZipArchiveEntry entry = archive.CreateEntry("Info.txt");

                        FileVersionInfo fviWG = FileVersionInfo.GetVersionInfo(Core.Classes.Path.WIREGUARD_EXE);
                        Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                        using (StreamWriter sw = new StreamWriter(entry.Open()))
                        {
                            sw.WriteLine($"User: {export.Username}");
                            sw.WriteLine($"Export date: {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}");
                            sw.WriteLine($"Windows Version: {Environment.OSVersion}");
                            sw.WriteLine($"WireGuard Version: {fviWG.ProductVersion}");
                            sw.WriteLine($"RHRK WG Version: v{v.Major}.{v.Minor}.{v.Build}");
                            sw.WriteLine($"Running .Net: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
                            sw.WriteLine("Info:");

                            if (String.IsNullOrEmpty(info))
                                sw.WriteLine("\tNo additonal informations");
                            else
                                sw.WriteLine(info);
                        }
                    }
                }

                LogManager.Debug("Archive created", nameof(ExportMessageHandler), nameof(Handel));

                server.Send(new ResultMessage() { Error = 0 });
            }
            catch (Exception ex)
            {
                LogManager.Error("Error reading wg logfile");
                LogManager.Error(ex);

                server.Send(new ResultMessage() { Error = -1, ErrorMsg = ex.Message });
            }
        }
    }
}
