using System;

namespace WindowsInk
{
    public static class VMulti
    {
        // HID report structure for XP-Pen drivers. It is used to send Windows Ink packets.
        public struct InkReport
        {
            public byte VMultiID;       // ID to communicate to the Digitizer device
            public byte ReportLength;   // Size of the report in bytes.
            public byte ReportID;       // ID of the report.
            public byte Buttons;        // Byte with switches for pen buttons / states
            public ushort X;            // X position of the pen from 0 to 32767
            public ushort Y;            // Y position of the pen from 0 to 32767
            public ushort Pressure;     // Pressure level from 0 to 8191
            public byte[] ToBytes()
            {
                var bytes = new byte[10];
                bytes[0] = VMultiID;
                bytes[1] = ReportLength;
                bytes[2] = ReportID;
                bytes[3] = Buttons;
                bytes[4] = (byte)(X & 0xFF);
                bytes[5] = (byte)((X & 0xFF00) >> 8);
                bytes[6] = (byte)(Y & 0xFF);
                bytes[7] = (byte)((Y & 0xFF00) >> 8);
                bytes[8] = (byte)(Pressure & 0xFF);
                bytes[9] = (byte)((Pressure & 0xFF00) >> 8);
                return bytes;
            }
            public static implicit operator byte[](InkReport report)
            {
                return report.ToBytes();
            }
        }

        public enum ButtonMask : int
        {
            Press = 1,
            Barrel = 2,
            Eraser = 4,
            Invert = 8,
            InRange = 16
        }
    }
}