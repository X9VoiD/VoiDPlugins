using System;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using static VoiDPlugins.OutputMode.WindowsInkConstants;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink")]
    public unsafe class WinInkButtonHandler : IStateBinding
    {
        private VMultiInstance? _instance;

        public static string[] ValidButtons { get; } = new string[]
        {
            "Pen Tip",
            "Pen Button",
            "Eraser (Toggle)",
            "Eraser (Hold)"
        };

        [Property("Button"), PropertyValidated(nameof(ValidButtons))]
        public string? Button { get; set; }

        [Flags]
        private enum ButtonBits : byte
        {
            Press = 1,
            Barrel = 2,
            Eraser = 4,
            Invert = 8,
            InRange = 16
        }

        public bool IsManuallySet { get; set; }

        [TabletReference]
        public TabletReference Reference { set => Initialize(value); }

        private void Initialize(TabletReference tabletReference)
        {
            _instance = VMultiInstanceManager.RetrieveVMultiInstance(tabletReference);
        }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            ref var eraserState = ref GetEraser();
            switch (Button)
            {
                case "Pen Tip":
                    _instance!.EnableButtonBit((int)(eraserState.Value ? ButtonBits.Eraser : ButtonBits.Press));
                    break;

                case "Pen Button":
                    _instance!.EnableButtonBit((int)ButtonBits.Barrel);
                    break;

                case "Eraser (Toggle)":
                    IsManuallySet = true;
                    EraserStateTransition(_instance!, ref eraserState, !eraserState.Value);
                    break;

                case "Eraser (Hold)":
                    IsManuallySet = true;
                    EraserStateTransition(_instance!, ref eraserState, true);
                    break;
            }
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            switch (Button)
            {
                case "Pen Tip":
                    _instance!.DisableButtonBit((int)(ButtonBits.Press | ButtonBits.Eraser));
                    break;

                case "Pen Button":
                    _instance!.DisableButtonBit((int)ButtonBits.Barrel);
                    break;

                case "Eraser (Hold)":
                    EraserStateTransition(_instance!, ref GetEraser(), false);
                    break;
            }
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
                instance.DisableButtonBit((int)(ButtonBits.Press | ButtonBits.Eraser));
                report->Pressure = 0;
                instance.Write();

                // Send Out-Of-Range
                report->Header.Buttons = 0;
                instance.Write();

                // Send In-Range but no tips
                instance.EnableButtonBit((int)ButtonBits.InRange);
                if (eraserState.Value)
                    instance.EnableButtonBit((int)ButtonBits.Invert);

                instance.Write();

                // Set Proper Report
                if (VMultiInstance.HasBit(buttons, (int)(ButtonBits.Press | ButtonBits.Eraser)))
                    instance.EnableButtonBit((int)(eraserState.Value ? ButtonBits.Eraser : ButtonBits.Press));
                report->Pressure = pressure;
            }
        }

        private ref Boxed<bool> GetEraser()
        {
            return ref _instance!.GetData<Boxed<bool>>(ERASER_STATE);
        }
    }
}