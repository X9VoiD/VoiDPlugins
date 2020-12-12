using System.Numerics;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public class VMultiAbsolutePointer : BasePointer<AbsoluteInputReport>
    {
        public VMultiAbsolutePointer() : base(0x09, "VMultiAbs")
        {
            ButtonHandler.SetReport(Report);
        }

        public override void SetPosition(Vector2 pos)
        {
            var newPos = Convert(pos);
            Report.X = (ushort)newPos.X;
            Report.Y = (ushort)newPos.Y;
            Device.Write(Report.ToBytes());
        }
    }
}