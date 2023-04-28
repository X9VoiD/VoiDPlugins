using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Output;
using OpenTabletDriver.Platform.Display;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Absolute Mode")]
    public class VMultiAbsoluteMode : AbsoluteOutputMode
    {
        public VMultiAbsoluteMode(InputDevice inputDevice, IVirtualScreen screen, ISettingsProvider settingsProvider)
            : base(inputDevice, new VMultiAbsolutePointer(inputDevice, screen))
        {
            settingsProvider.Inject(this);
        }
    }
}