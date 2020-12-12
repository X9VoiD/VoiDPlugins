using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public class WinInkRelativePointer : BasePointer<DigitizerInputReport>, IVirtualTablet
    {
        public WinInkRelativePointer() : base(0x05, "WindowsInk")
        {
            WinInkButtonHandler.SetReport(Report);
            WinInkButtonHandler.SetDevice(Device);
        }

        private Vector2 lastPos;
        private DateTime lastTime = DateTime.UtcNow;
        public override void SetPosition(Vector2 pos)
        {
            var now = DateTime.UtcNow;
            if ((now - lastTime).TotalMilliseconds > 100)
            {
                var newPos = Convert(lastPos - pos);
                Report.X = (byte)newPos.X;
                Report.Y = (byte)newPos.Y;
                Device.Write(Report.ToBytes());
            }
            lastPos = pos;
            lastTime = now;
        }

        public void SetPressure(float percentage)
        {
            Report.Pressure = (ushort)(percentage * 8191);
        }
    }
}