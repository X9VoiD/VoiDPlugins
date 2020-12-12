using System.Collections.Generic;

namespace VoiDPlugins.Tool
{
    internal static class OemData
    {
        public static readonly List<string> OemProcesses = new List<string>
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

        public static readonly List<string> OemDrivers = new List<string>
        {
            // Gaomon, Huion
            "Graphics Tablet"
        };
    }
}