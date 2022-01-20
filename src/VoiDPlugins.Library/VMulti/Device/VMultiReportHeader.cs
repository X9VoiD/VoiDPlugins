using System.Runtime.InteropServices;

namespace VoiDPlugins.Library.VMulti.Device
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VMultiReportHeader
    {
        public VMultiReportHeader(int size, byte reportID)
        {
            VMultiID = 0x40;
            ReportLength = (byte)(size - 1);
            ReportID = reportID;
            Buttons = 0;
        }

        public byte VMultiID;
        public byte ReportLength;
        public byte ReportID;
        public byte Buttons;
    }
}