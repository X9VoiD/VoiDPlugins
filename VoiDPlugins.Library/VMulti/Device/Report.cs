namespace VoiDPlugins.Library.VMulti.Device
{
    public abstract class Report
    {
        public byte VMultiID = 0x40;
        public byte ReportID;
        public byte Buttons;
        public byte ReportLength => (byte)(Size - 1);
        public abstract byte Size { get; }
        public abstract byte[] ToBytes();
    }
}