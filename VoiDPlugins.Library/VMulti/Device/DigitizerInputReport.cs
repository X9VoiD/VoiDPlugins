using System.Numerics;
using System.Runtime.CompilerServices;

namespace VoiDPlugins.Library.VMulti.Device
{
    public sealed class DigitizerInputReport : Report
    {
        public ushort X;            // X position of the pen from 0 to 32767
        public ushort Y;            // Y position of the pen from 0 to 32767
        public ushort Pressure;     // Pressure level from 0 to 8191
        public byte XTilt;          // X tilt of the pen from -127 to 127
        public byte YTilt;          // Y tilt of the pen from -127 to 127

        private byte[] bytes = new byte[12];

        public override byte[] ToBytes()
        {
            Unsafe.WriteUnaligned(ref bytes[0], VMultiID);
            Unsafe.WriteUnaligned(ref bytes[1], ReportLength);
            Unsafe.WriteUnaligned(ref bytes[2], ReportID);
            Unsafe.WriteUnaligned(ref bytes[3], Buttons);
            Unsafe.WriteUnaligned(ref bytes[4], X);
            Unsafe.WriteUnaligned(ref bytes[6], Y);
            Unsafe.WriteUnaligned(ref bytes[8], Pressure);
            Unsafe.WriteUnaligned(ref bytes[10], XTilt);
            Unsafe.WriteUnaligned(ref bytes[11], YTilt);
            return bytes;
        }

        public override void SetCoordinates(Vector2 coordinates)
        {
            X = (ushort)coordinates.X;
            Y = (ushort)coordinates.Y;
        }

        public override byte Size => 12;
    }
}