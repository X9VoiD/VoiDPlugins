using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VoiDPlugins.Library.VMulti.Device
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DigitizerInputReport
    {
        public const byte NormalReportID = 0x05;
        public const byte ExtendedReportID = 0x06;

        public DigitizerInputReport()
        {
            Header = new VMultiReportHeader(Unsafe.SizeOf<DigitizerInputReport>(), ExtendedReportID);
            X = 0;
            Y = 0;
            Pressure = 0;
            XTilt = 0;
            YTilt = 0;
        }

        private DigitizerInputReport(byte reportId)
        {
            Header = new VMultiReportHeader(Unsafe.SizeOf<DigitizerInputReport>(), reportId);
            X = 0;
            Y = 0;
            Pressure = 0;
            XTilt = 0;
            YTilt = 0;
        }

        public static DigitizerInputReport Normal()
        {
            return new DigitizerInputReport(NormalReportID);
        }

        public static DigitizerInputReport Extended()
        {
            return new DigitizerInputReport(ExtendedReportID);
        }

        public VMultiReportHeader Header;
        public ushort X;            // X position of the pen from 0 to 32767
        public ushort Y;            // Y position of the pen from 0 to 32767
        public ushort Pressure;     // Pressure level from 0 to 16383
        public byte XTilt;          // X tilt of the pen from -127 to 127
        public byte YTilt;          // Y tilt of the pen from -127 to 127
    }
}