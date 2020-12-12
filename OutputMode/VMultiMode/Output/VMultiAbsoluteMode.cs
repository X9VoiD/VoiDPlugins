using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Absolute Mode")]
    public class VMultiAbsoluteMode : AbsoluteOutputMode
    {
        private readonly VMultiAbsolutePointer pointer = new VMultiAbsolutePointer();
        public override IAbsolutePointer Pointer => pointer;
    }
}