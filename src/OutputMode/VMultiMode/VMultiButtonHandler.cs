using System.Collections.Generic;
using System.Linq;
using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Tablet;
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

        [Setting("Button"), MemberValidated(nameof(ValidButtons))]
        public string? Button { get; set; }

        public VMultiButtonHandler(InputDevice inputDevice, ISettingsProvider settingsProvider)
        {
            settingsProvider.Inject(this);
            try
            {
                var sharedStore = SharedStore.GetStore(inputDevice, STORE_KEY);
                var mode = sharedStore.Get<int>(MODE);
                _instance = sharedStore.Get<VMultiInstance>(mode);
            }
            catch
            {
                Log.WriteNotify("VMulti",
                          "VMulti bindings are being used without an active VMulti output mode.",
                          LogLevel.Error);
            }
        }

        public void Press(IDeviceReport report)
        {
            if (_instance == null)
                return;

            _instance.EnableButtonBit(Bindings[Button!]);
            _instance.Write();
        }

        public void Release(IDeviceReport report)
        {
            if (_instance == null)
                return;

            _instance.DisableButtonBit(Bindings[Button!]);
            _instance.Write();
        }
    }
}