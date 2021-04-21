using OpenTabletDriver.Plugin.Attributes;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.Library.VMulti
{
    [PluginIgnore]
    public class ButtonHandler
    {
        protected static Report Report;

        public static void SetReport(Report report)
        {
            Report = report;
        }

        public static void EnableBit(int bit)
        {
            Report.Buttons = (byte)(Report.Buttons | bit);
        }

        public static void DisableBit(int bit)
        {
            Report.Buttons = (byte)(Report.Buttons & ~bit);
        }
    }
}