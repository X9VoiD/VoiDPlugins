using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink Relative Mode")]
    public class WinInkRelativeMode : RelativeOutputMode
    {
        private readonly WinInkRelativePointer pointer = new();
        public override IRelativePointer Pointer => pointer;
    }
}