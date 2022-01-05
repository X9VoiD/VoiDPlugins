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
        private VMultiAbsolutePointer? _pointer;

        [Resolved]
        public IServiceProvider ServiceProvider
        {
            set => _pointer = new VMultiAbsolutePointer((IVirtualScreen)value.GetService(typeof(IVirtualScreen))!);
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