using System.Collections.Generic;
using System.Linq;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Mode")]
    public class VMultiButtonHandler : ButtonHandler, IBinding
    {
        public static Dictionary<string, int> Bindings { get; } = new()
        {
            { "Left", 1 },
            { "Right", 2 },
            { "Middle", 4 }
        };

        [Property("Button"), PropertyValidated(nameof(ValidButtons))]
        public string Button { get; set; }
        public static string[] ValidButtons => Bindings.Keys.ToArray();

        public void Press(IDeviceReport report)
        {
            EnableBit(Bindings[Button]);
        }

        public void Release(IDeviceReport report)
        {
            DisableBit(Bindings[Button]);
        }
    }
}