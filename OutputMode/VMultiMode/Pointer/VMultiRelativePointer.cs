using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public class VMultiRelativePointer : BasePointer<RelativeInputReport>, IRelativePointer
    {
        public VMultiRelativePointer(IVirtualScreen screen) : base(screen, 0x04, "VMultiRel")
        {
            ButtonHandler.SetReport(Report);
        }
    }
}