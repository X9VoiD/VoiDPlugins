using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public unsafe class WinInkAbsolutePointer : BasePointer<DigitizerInputReport>, IVirtualTablet
    {
        public WinInkAbsolutePointer(IVirtualScreen screen) : base(screen, "WindowsInk")
        {
            WinInkButtonHandler.SetReport(ReportPointer, ReportBuffer);
            WinInkButtonHandler.SetDevice(Device);
        }

        public void SetPressure(float percentage)
        {
            ReportPointer->Pressure = (ushort)(percentage * 8191);
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