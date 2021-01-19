using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Relative Mode")]
    public class VMultiRelativeMode : RelativeOutputMode
    {
        private readonly VMultiRelativePointer pointer = new();
        public override IRelativePointer Pointer => pointer;
    }
}