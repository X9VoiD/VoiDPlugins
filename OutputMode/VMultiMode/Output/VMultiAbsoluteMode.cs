using System;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Absolute Mode")]
    public class VMultiAbsoluteMode : AbsoluteOutputMode
    {
        [Resolved]
        public IServiceProvider ServiceProvider
        {
            set => Pointer = new VMultiRelativePointer((IVirtualScreen)value.GetService(typeof(IVirtualScreen)));
        }

        public override IAbsolutePointer Pointer { get; set; }
    }
}