using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public unsafe class WinInkRelativePointer : BasePointer<DigitizerInputReport>, IVirtualTablet
    {
        private Vector2 maxPoint;
        private Vector2 currentPoint;

        public WinInkRelativePointer(IVirtualScreen screen) : base(screen, "WindowsInk")
        {
            WinInkButtonHandler.SetReport(ReportPointer, ReportBuffer);
            WinInkButtonHandler.SetDevice(Device);
            maxPoint = new Vector2(VirtualScreen.Width, VirtualScreen.Height);
            currentPoint = maxPoint / 2;
        }

        public void SetPressure(float percentage)
        {
            ReportPointer->Pressure = (ushort)(percentage * 8191);
        }

        public override void Translate(Vector2 delta)
        {
            currentPoint = Vector2.Clamp(currentPoint + delta, Vector2.Zero, maxPoint);
            base.SetPosition(currentPoint);
        }

        protected override DigitizerInputReport CreateReport()
        {
            return new DigitizerInputReport(0x05);
        }

        protected override void SetCoordinates(Vector2 pos)
        {
            ReportPointer->X = (ushort)pos.X;
            ReportPointer->Y = (ushort)pos.Y;
        }
    }
}