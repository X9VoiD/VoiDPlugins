using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public class VMultiAbsolutePointer : BasePointer<AbsoluteInputReport>, IAbsolutePointer
    {
        public VMultiAbsolutePointer(IVirtualScreen screen) : base(screen, 0x09, "VMultiAbs")
        {
            ButtonHandler.SetReport(Report);
        }
    }
}