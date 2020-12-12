namespace VoiDPlugins.Library.VMulti.Device
{
    public class AbsoluteInputReport : Report
    {
        public ushort X;            // X position of the mouse from 0 to 32767
        public ushort Y;            // Y position of the mouse from 0 to 32767
        public ushort Pressure;     // Pressure of the mouse? from 0 to 8191

        public override byte[] ToBytes()
        {
            var bytes = new byte[Size];
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

        public override byte Size => 10;
    }
}