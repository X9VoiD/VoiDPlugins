namespace VoiDPlugins.Library.VMulti.Device
{
    public class RelativeInputReport : Report
    {
        public byte X;               // X position of the mouse from -127 to 127
        public byte Y;               // Y position of the mouse from -127 to 127
        public byte WheelPos;        // Wheel position of the mouse from -127 to 127

        public override byte[] ToBytes()
        {
            var bytes = new byte[Size];
            bytes[0] = VMultiID;
            bytes[1] = ReportLength;
            bytes[2] = ReportID;
            bytes[3] = Buttons;
            bytes[4] = X;
            bytes[5] = Y;
            bytes[6] = WheelPos;
            return bytes;
        }

        public override byte Size => 7;
    }
}