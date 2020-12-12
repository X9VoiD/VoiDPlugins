using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Relative Mode")]
    public class VMultiRelativeMode : AbsoluteOutputMode
    {
        private readonly VMultiRelativePointer pointer = new VMultiRelativePointer();
        public override IAbsolutePointer Pointer => pointer;
    }
}