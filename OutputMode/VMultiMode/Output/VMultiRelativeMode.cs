using System;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Relative Mode")]
    public class VMultiRelativeMode : RelativeOutputMode
    {
        [Resolved]
        public IServiceProvider ServiceProvider
        {
            set => Pointer = new VMultiRelativePointer((IVirtualScreen)value.GetService(typeof(IVirtualScreen)));
        }

        public override IRelativePointer Pointer { get; set; }
    }
}