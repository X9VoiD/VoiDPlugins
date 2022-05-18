using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using VoiDPlugins.Library.VoiD;
using static VoiDPlugins.OutputMode.WindowsInkConstants;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink")]
    public unsafe partial class WindowsInkButtonHandler : IStateBinding
    {
        private VMultiInstance? _instance;
        private SharedStore? _sharedStore;

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
            _sharedStore = GlobalStore<SharedStore>.Get(tabletReference);
            _instance = _sharedStore.GetData<VMultiInstance>(INSTANCE);
        }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            ref var eraserState = ref GetEraser();
            switch (Button)
            {
                case "Pen Tip":
                    _instance!.EnableButtonBit((int)(eraserState.Value ? WindowsInkButtonFlags.Eraser : WindowsInkButtonFlags.Press));
                    break;

                case "Pen Button":
                    _instance!.EnableButtonBit((int)WindowsInkButtonFlags.Barrel);
                    break;

                case "Eraser (Toggle)":
                    _sharedStore!.GetData<Boxed<bool>>(MANUAL_ERASER).Value = !eraserState.Value;
                    EraserStateTransition(_instance!, ref eraserState, !eraserState.Value);
                    break;

                case "Eraser (Hold)":
                    _sharedStore!.GetData<Boxed<bool>>(MANUAL_ERASER).Value = true;
                    EraserStateTransition(_instance!, ref eraserState, true);
                    break;
            }
            _instance!.Write();
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            switch (Button)
            {
                case "Pen Tip":
                    _instance!.DisableButtonBit((int)(WindowsInkButtonFlags.Press | WindowsInkButtonFlags.Eraser));
                    break;

                case "Pen Button":
                    _instance!.DisableButtonBit((int)WindowsInkButtonFlags.Barrel);
                    break;

                case "Eraser (Hold)":
                    _sharedStore!.GetData<Boxed<bool>>(MANUAL_ERASER).Value = false;
                    EraserStateTransition(_instance!, ref GetEraser(), false);
                    break;
            }
            _instance!.Write();
        }

        public static void EraserStateTransition(VMultiInstance instance, ref Boxed<bool> eraserState, bool isEraser)
        {
            if (eraserState.Value != isEraser)
            {
                eraserState.Value = isEraser;
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
                if (eraserState.Value)
                    instance.EnableButtonBit((int)WindowsInkButtonFlags.Invert);

                instance.Write();

                // Set Proper Report
                if (VMultiInstance.HasBit(buttons, (int)(WindowsInkButtonFlags.Press | WindowsInkButtonFlags.Eraser)))
                    instance.EnableButtonBit((int)(eraserState.Value ? WindowsInkButtonFlags.Eraser : WindowsInkButtonFlags.Press));
                report->Pressure = pressure;
            }
        }

        private ref Boxed<bool> GetEraser()
        {
            return ref _sharedStore!.GetData<Boxed<bool>>(ERASER_STATE);
        }
    }
}