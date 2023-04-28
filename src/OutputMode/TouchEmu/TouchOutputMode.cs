using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Output;
using OpenTabletDriver.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Touch Emu"), SupportedPlatform(SystemPlatform.Windows)]
    public class TouchOutputMode : AbsoluteOutputMode
    {
        public TouchOutputMode(InputDevice tablet, ISettingsProvider settingsProvider) : base(tablet, new TouchPointerHandler())
        {
            settingsProvider.Inject(this);
        }
    }
}