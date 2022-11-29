using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using VoiDPlugins.Library.VoiD;
using static VoiDPlugins.OutputMode.WindowsInkConstants;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink")]
    public unsafe partial class WindowsInkButtonHandler : IStateBinding
    {
        private VMultiInstance _instance = null!;
        private SharedStore _sharedStore = null!;

        public static string[] ValidButtons { get; } = new string[]
        {
            "Pen Tip",
            "Pen Button",
            "Eraser (Toggle)",
            "Eraser (Hold)"
        };

        [Property("Button"), PropertyValidated(nameof(ValidButtons))]
        public string? Button { get; set; }

        [TabletReference]
        public TabletReference Reference { set => Initialize(value); }

        private void Initialize(TabletReference tabletReference)
        {
            try
            {
                _sharedStore = SharedStore.GetStore(tabletReference, STORE_KEY);
                _instance = _sharedStore.Get<VMultiInstance>(INSTANCE);
            }
            catch
            {
                Log.WriteNotify("WinInk",
                          "Windows Ink bindings are being used without an active Windows Ink output mode.",
                          LogLevel.Error);
            }
        }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            if (_instance == null)
                return;

            var eraserState = _sharedStore.Get<bool>(ERASER_STATE);
            switch (Button)
            {
                case "Pen Tip":
                    _sharedStore.Set(TIP_PRESSED, true);
                    _instance.EnableButtonBit((int)(eraserState ? WindowsInkButtonFlags.Eraser : WindowsInkButtonFlags.Press));
                    break;

                case "Pen Button":
                    _instance.EnableButtonBit((int)WindowsInkButtonFlags.Barrel);
                    break;

                case "Eraser (Toggle)":
                    _sharedStore.Set(MANUAL_ERASER, !eraserState);
                    EraserStateTransition(_sharedStore, _instance, !eraserState);
                    break;

                case "Eraser (Hold)":
                    _sharedStore.Set(MANUAL_ERASER, true);
                    EraserStateTransition(_sharedStore, _instance, true);
                    break;
            }
            _instance.Write();
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            if (_instance == null)
                return;

            switch (Button)
            {
                case "Pen Tip":
                    _sharedStore.Set(TIP_PRESSED, false);
                    _instance.DisableButtonBit((int)(WindowsInkButtonFlags.Press | WindowsInkButtonFlags.Eraser));
                    break;

                case "Pen Button":
                    _instance.DisableButtonBit((int)WindowsInkButtonFlags.Barrel);
                    break;

                case "Eraser (Hold)":
                    _sharedStore.Set(MANUAL_ERASER, false);
                    EraserStateTransition(_sharedStore, _instance, false);
                    break;
            }
            _instance.Write();
        }

        internal static void EraserStateTransition(SharedStore store, VMultiInstance instance, bool isEraser)
        {
            var eraserState = store.Get<bool>(ERASER_STATE);
            if (eraserState != isEraser)
            {
                store.Set(ERASER_STATE, isEraser);
                eraserState = isEraser;
                var report = (DigitizerInputReport*)instance.Header;
                var buttons = report->Header.Buttons;
                var pressure = report->Pressure;

                // Send In-Range but no tips
                instance.DisableButtonBit((int)(WindowsInkButtonFlags.Press | WindowsInkButtonFlags.Eraser));
                report->Pressure = 0;
                instance.Write();

                // Send Out-Of-Range
                report->Header.Buttons = 0;
                instance.Write();

                // Send In-Range but no tips
                instance.EnableButtonBit((int)WindowsInkButtonFlags.InRange);
                if (eraserState)
                    instance.EnableButtonBit((int)WindowsInkButtonFlags.Invert);

                instance.Write();

                // Set Proper Report
                if (VMultiInstance.HasBit(buttons, (int)(WindowsInkButtonFlags.Press | WindowsInkButtonFlags.Eraser)))
                    instance.EnableButtonBit((int)(eraserState ? WindowsInkButtonFlags.Eraser : WindowsInkButtonFlags.Press));
                report->Pressure = pressure;
            }
        }
    }
}