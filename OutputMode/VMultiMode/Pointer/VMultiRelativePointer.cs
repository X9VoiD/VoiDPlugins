using System;
using System.Numerics;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public class VMultiRelativePointer : BasePointer<RelativeInputReport>
    {
        public VMultiRelativePointer() : base(0x04, "VMultiRel")
        {
            ButtonHandler.SetReport(Report);
        }

        private Vector2 lastPos;
        private DateTime lastTime = DateTime.UtcNow;
        public override void SetPosition(Vector2 pos)
        {
            var now = DateTime.UtcNow;
            if ((now - lastTime).TotalMilliseconds > 20)
            {
                var newPos = Convert(lastPos - pos);
                Report.X = (byte)newPos.X;
                Report.Y = (byte)newPos.Y;
                Device.Write(Report.ToBytes());
            }
            lastPos = pos;
            lastTime = now;
        }
    }
}