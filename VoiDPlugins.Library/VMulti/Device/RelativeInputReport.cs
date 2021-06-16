using System.Runtime.CompilerServices;

namespace VoiDPlugins.Library.VMulti.Device
{
    public struct RelativeInputReport
    {
        public RelativeInputReport(byte reportID)
        {
            Header = new VMultiReportHeader(Unsafe.SizeOf<RelativeInputReport>(), reportID);
            X = 0;
            Y = 0;
            WheelPos = 0;
        }

        public VMultiReportHeader Header;
        public byte X;               // X position of the mouse from -127 to 127
        public byte Y;               // Y position of the mouse from -127 to 127
        public byte WheelPos;        // Wheel position of the mouse from -127 to 127
    }
}