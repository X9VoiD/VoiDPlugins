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

        [Property("Sync")]
        [ToolTip("Synchronize OS cursor with Windows Ink's current position when pen goes out of range.")]
        [DefaultPropertyValue(true)]
        public bool Sync { get; set; } = true;

        [Property("Forced Sync")]
        [ToolTip("If this and \"Sync\" is enabled, the OS cursor will always be resynced with Windows Ink's current position.")]
        [DefaultPropertyValue(false)]
        public bool ForcedSync { get; set; }

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
                _pointer = new WinInkRelativePointer(value, _virtualScreen!)
                {
                    Sync = Sync,
                    ForcedSync = ForcedSync
                };
            }
        }

        public override IRelativePointer Pointer
        {
            get => _pointer!;
            set { }
        }
    }
}