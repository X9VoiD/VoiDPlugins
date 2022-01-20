using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VoiDPlugins.Library.VMulti.Device
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RelativeInputReport
    {
        public RelativeInputReport()
        {
            Header = new VMultiReportHeader(Unsafe.SizeOf<RelativeInputReport>(), 0x04);
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