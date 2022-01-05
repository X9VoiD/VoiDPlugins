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
        private WinInkRelativePointer? _pointer;

        [Resolved]
        public IServiceProvider ServiceProvider
        {
            set => _pointer = new WinInkRelativePointer((IVirtualScreen)value.GetService(typeof(IVirtualScreen))!);
        }

        [OnDependencyLoad]
        public void Initialize()
        {
            _pointer!.Initialize(TabletReference);
        }

        public override IRelativePointer Pointer
        {
            get => _pointer!;
            set { }
        }
    }
}