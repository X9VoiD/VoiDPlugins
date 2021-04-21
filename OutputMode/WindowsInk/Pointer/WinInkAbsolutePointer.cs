using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public class WinInkAbsolutePointer : BasePointer<DigitizerInputReport>, IVirtualTablet
    {
        public WinInkAbsolutePointer(IVirtualScreen screen) : base(screen, 0x05, "WindowsInk")
        {
            WinInkButtonHandler.SetReport(Report);
            WinInkButtonHandler.SetDevice(Device);
        }

        public void SetPressure(float percentage)
        {
            Report.Pressure = (ushort)(percentage * 8191);
        }
    }
}