using System.Collections.Generic;
using System.Linq;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VoiD;
using static VoiDPlugins.OutputMode.VMultiModeConstants;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Mode")]
    public class VMultiButtonHandler : IStateBinding
    {
        private VMultiInstance? _instance;

        public static Dictionary<string, int> Bindings { get; } = new()
        {
            { "Left", 1 },
            { "Right", 2 },
            { "Middle", 4 }
        };

        public static string[] ValidButtons { get; } = Bindings.Keys.ToArray();

        [Property("Button"), PropertyValidated(nameof(ValidButtons))]
        public string? Button { get; set; }

        [TabletReference]
        public TabletReference Reference { set => Initialize(value); }

        private void Initialize(TabletReference tabletReference)
        {
            _instance = SharedStore.GetStore(tabletReference, STORE_KEY).Get<VMultiInstance>(INSTANCE);
        }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            _instance!.EnableButtonBit(Bindings[Button!]);
            _instance.Write();
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            _instance!.DisableButtonBit(Bindings[Button!]);
            _instance.Write();
        }
    }
}