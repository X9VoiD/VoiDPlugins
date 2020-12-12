using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Touch Emu"), SupportedPlatform(PluginPlatform.Windows)]
    public class TouchOutputMode : AbsoluteOutputMode
    {
        private readonly IAbsolutePointer TouchPointer = new TouchPointerHandler();
        public override IAbsolutePointer Pointer => TouchPointer;
    }
}