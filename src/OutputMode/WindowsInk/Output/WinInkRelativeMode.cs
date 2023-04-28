using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Output;
using OpenTabletDriver.Platform.Display;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink Relative Mode")]
    public class WinInkRelativeMode : RelativeOutputMode
    {
        // [Setting("Sync")]
        // [ToolTip("Synchronize OS cursor with Windows Ink's current position when pen goes out of range.")]
        // [DefaultValue(true)]
        // public bool Sync { get; set; } = true;

        // [Setting("Forced Sync")]
        // [ToolTip("If this and \"Sync\" is enabled, the OS cursor will always be resynced with Windows Ink's current position.")]
        // [DefaultValue(false)]
        // public bool ForcedSync { get; set; }

        public WinInkRelativeMode(InputDevice inputDevice, IVirtualScreen virtualScreen, ISettingsProvider settingsProvider)
            : base(inputDevice, CreatePointer(inputDevice, virtualScreen))
        {
            settingsProvider.Inject(this);
        }

        private static WinInkRelativePointer CreatePointer(InputDevice inputDevice, IVirtualScreen virtualScreen)
        {
            return new WinInkRelativePointer(inputDevice, virtualScreen)
            {
                Sync = true,
                ForcedSync = false
            };
        }
    }
}