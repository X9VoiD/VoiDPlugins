using OpenTabletDriver.Plugin.Attributes;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.Library.VMulti
{
    [PluginIgnore]
    public unsafe class ButtonHandler
    {
        protected static byte[] ReportBuffer;
        protected static VMultiReportHeader* ReportPointer;

        public static void SetReport(VMultiReportHeader* report, byte[] reportBuffer)
        {
            ReportBuffer = reportBuffer;
            ReportPointer = report;
        }

        public static void EnableBit(int bit)
        {
            ReportPointer->Buttons = (byte)(ReportPointer->Buttons | bit);
        }

        public static void DisableBit(int bit)
        {
            ReportPointer->Buttons = (byte)(ReportPointer->Buttons & ~bit);
        }
    }
}