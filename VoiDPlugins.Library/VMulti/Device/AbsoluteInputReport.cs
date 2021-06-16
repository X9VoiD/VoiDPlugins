using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VoiDPlugins.Library.VMulti.Device
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AbsoluteInputReport
    {
        public AbsoluteInputReport(byte reportID)
        {
            Header = new VMultiReportHeader(Unsafe.SizeOf<AbsoluteInputReport>(), reportID);
            X = 0;
            Y = 0;
            Pressure = 0;
        }

        public VMultiReportHeader Header;
        public ushort X;            // X position of the mouse from 0 to 32767
        public ushort Y;            // Y position of the mouse from 0 to 32767
        public ushort Pressure;     // Pressure of the mouse? from 0 to 8191
    }
}