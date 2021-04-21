using System;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink Absolute Mode")]
    public class WinInkAbsoluteMode : AbsoluteOutputMode
    {
        [Resolved]
        public IServiceProvider ServiceProvider
        {
            set => Pointer = new WinInkAbsolutePointer((IVirtualScreen)value.GetService(typeof(IVirtualScreen)));
        }

        public override IAbsolutePointer Pointer { get; set; }
    }
}