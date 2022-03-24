using System;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink Relative Mode")]
    public class WinInkRelativeMode : RelativeOutputMode
    {
        private WinInkRelativePointer? _pointer;
        private IVirtualScreen? _virtualScreen;

        [Resolved]
        public IServiceProvider ServiceProvider
        {
            set => _virtualScreen = (IVirtualScreen)value.GetService(typeof(IVirtualScreen))!;
        }

        public override TabletReference Tablet
        {
            get => base.Tablet;
            set
            {
                base.Tablet = value;
                _pointer = new WinInkRelativePointer(value, _virtualScreen!);
            }
        }

        public override IRelativePointer Pointer
        {
            get => _pointer!;
            set { }
        }
    }
}