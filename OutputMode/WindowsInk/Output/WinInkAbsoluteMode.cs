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
        private WinInkAbsolutePointer? _pointer;

        [Resolved]
        public IServiceProvider ServiceProvider
        {
            set => _pointer = new WinInkAbsolutePointer((IVirtualScreen)value.GetService(typeof(IVirtualScreen))!);
        }

        [OnDependencyLoad]
        public void Initialize()
        {
            _pointer!.Initialize(TabletReference);
        }

        public override IAbsolutePointer Pointer
        {
            get => _pointer!;
            set { }
        }
    }
}