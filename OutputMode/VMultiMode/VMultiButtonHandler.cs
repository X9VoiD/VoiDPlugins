using System.Collections.Generic;
using System.Linq;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Mode")]
    public class VMultiButtonHandler : ButtonHandler, IStateBinding
    {
        public static Dictionary<string, int> Bindings { get; } = new()
        {
            { "Left", 1 },
            { "Right", 2 },
            { "Middle", 4 }
        };

        public static string[] ValidButtons { get; } = Bindings.Keys.ToArray();

        [Property("Button"), PropertyValidated(nameof(ValidButtons))]
        public string Button { get; set; }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            EnableBit(Bindings[Button]);
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            DisableBit(Bindings[Button]);
        }
    }
}