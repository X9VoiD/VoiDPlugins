using System;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink Relative Mode")]
    public class WinInkRelativeMode : RelativeOutputMode
    {
        [Resolved]
        public IServiceProvider ServiceProvider
        {
            set => Pointer = new WinInkRelativePointer((IVirtualScreen)value.GetService(typeof(IVirtualScreen)));
        }

        public override IRelativePointer Pointer { get; set; }
    }
}