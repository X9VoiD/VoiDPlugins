using System;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink Absolute Mode")]
    public class WinInkAbsoluteMode : AbsoluteOutputMode
    {
        private WinInkAbsolutePointer? _pointer;
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
                _pointer = new WinInkAbsolutePointer(value, _virtualScreen!);
            }
        }

        public override IAbsolutePointer Pointer
        {
            get => _pointer!;
            set { }
        }
    }
}