using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Output;
using OpenTabletDriver.Platform.Display;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink Absolute Mode")]
    public class WinInkAbsoluteMode : AbsoluteOutputMode
    {
        // Hardcode the following for now since OTD 0.7.x doesn't allow us to pass
        // these settings to the pointer.

        // [Setting("Sync", "Synchronize OS cursor with Windows Ink's current position when pen goes out of range.")]
        // [DefaultValue(true)]
        // public bool Sync { get; set; } = true;

        // [Setting("Forced Sync", "If this and \"Sync\" is enabled, the OS cursor will always be resynced with Windows Ink's current position.")]
        // [DefaultValue(false)]
        // public bool ForcedSync { get; set; }

        public WinInkAbsoluteMode(InputDevice inputDevice, IVirtualScreen virtualScreen, ISettingsProvider settingsProvider)
            : base(inputDevice, CreatePointer(inputDevice, virtualScreen))
        {
            settingsProvider.Inject(this);
        }

        private static WinInkAbsolutePointer CreatePointer(InputDevice inputDevice, IVirtualScreen virtualScreen)
        {
            return new WinInkAbsolutePointer(inputDevice, virtualScreen)
            {
                Sync = true,
                ForcedSync = false
            };
        }
    }
}