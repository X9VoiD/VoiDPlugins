using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public class VMultiAbsolutePointer : BasePointer<AbsoluteInputReport>, IAbsolutePointer
    {
        public VMultiAbsolutePointer() : base(0x09, "VMultiAbs")
        {
            ButtonHandler.SetReport(Report);
        }
    }
}