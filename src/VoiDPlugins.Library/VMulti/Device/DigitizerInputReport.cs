using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VoiDPlugins.Library.VMulti.Device
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DigitizerInputReport
    {
        public DigitizerInputReport()
        {
            Header = new VMultiReportHeader(Unsafe.SizeOf<DigitizerInputReport>(), 0x05);
            X = 0;
            Y = 0;
            Pressure = 0;
            XTilt = 0;
            YTilt = 0;
        }

        public VMultiReportHeader Header;
        public ushort X;            // X position of the pen from 0 to 32767
        public ushort Y;            // Y position of the pen from 0 to 32767
        public ushort Pressure;     // Pressure level from 0 to 8191
        public byte XTilt;          // X tilt of the pen from -127 to 127
        public byte YTilt;          // Y tilt of the pen from -127 to 127
    }
}