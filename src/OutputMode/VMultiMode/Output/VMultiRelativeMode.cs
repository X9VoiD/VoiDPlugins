using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Output;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Relative Mode")]
    public class VMultiRelativeMode : RelativeOutputMode
    {
        public VMultiRelativeMode(InputDevice inputDevice, ISettingsProvider settingsProvider)
            : base(inputDevice, new VMultiRelativePointer(inputDevice))
        {
            settingsProvider.Inject(this);
        }
    }
}