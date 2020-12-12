using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink Relative Mode")]
    public class WinInkRelativeMode : AbsoluteOutputMode
    {
        private readonly WinInkRelativePointer pointer = new WinInkRelativePointer();
        public override IAbsolutePointer Pointer => pointer;
    }
}