using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;

namespace OemKill
{
    [SupportedPlatform(PluginPlatform.Windows)]
    [PluginName("OEM Kill")]
    public class OemKillTool : ITool
    {
        public bool Initialize()
        {
            var processList = Process.GetProcesses();
            var oemProcesses = from process in processList
                               where OemProcesses.Contains(process.ProcessName)
                               select process;

            if (oemProcesses.ToList().Count == 0)
            {
                Log.Write("OemKill", "No oem process found");
                return true;
            }

            foreach (var process in oemProcesses)
            {
                try
                {
                    process.Kill();
                    Log.Write("OemKill", "Killing " + process.ProcessName);
                }
                catch (Exception e)
                {
                    Log.Write("OemKill", "Failed. Reason: " + e.Message, LogLevel.Error);
                    return false;
                }
            }

            Log.Write("OemKill", "Oem process killed successfully");
            return true;
        }

        readonly List<string> OemProcesses = new List<string>
        {
            // XP-Pen
            "PentabletService",
            "PenTablet",

            // Wacom
            "WacomDesktopCenter",
            "Wacom_Tablet",
            "Pen_Tablet",

            // Gaomon
            "Gaomon Tablet",
            "TabletDriverCore",

            // VEIKK
            "TabletDriverCenter",
            "TabletDriverSetting"
        };

        public void Dispose() { }
    }
}