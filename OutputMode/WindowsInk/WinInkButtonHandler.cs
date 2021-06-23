using System;
using HidSharp;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink")]
    public unsafe class WinInkButtonHandler : ButtonHandler, IStateBinding
    {
        public static string[] ValidButtons { get; } = new string[]
        {
            "Pen Tip",
            "Pen Button",
            "Eraser (Toggle)",
            "Eraser (Hold)"
        };

        [Property("Button"), PropertyValidated(nameof(ValidButtons))]
        public string Button { get; set; }

        [Flags]
        private enum ButtonBits : byte
        {
            Press = 1,
            Barrel = 2,
            Eraser = 4,
            Invert = 8,
            InRange = 16
        }

        public static bool IsManuallySet { get; set; }
        private static bool EraserState;
        private static HidStream Device;

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            switch (Button)
            {
                case "Pen Tip":
                    EnableBit((int)(EraserState ? ButtonBits.Eraser : ButtonBits.Press));
                    break;

                case "Pen Button":
                    EnableBit((int)ButtonBits.Barrel);
                    break;

                case "Eraser (Toggle)":
                    IsManuallySet = true;
                    EraserStateTransition(!EraserState);
                    break;

                case "Eraser (Hold)":
                    IsManuallySet = true;
                    EraserStateTransition(true);
                    break;
            }
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            switch (Button)
            {
                case "Pen Tip":
                    DisableBit((int)(ButtonBits.Press | ButtonBits.Eraser));
                    break;

                case "Pen Button":
                    DisableBit((int)ButtonBits.Barrel);
                    break;

                case "Eraser (Hold)":
                    EraserStateTransition(false);
                    break;
            }
        }

        public static void EraserStateTransition(bool isEraser)
        {
            if (EraserState != isEraser)
            {
                EraserState = isEraser;
                var report = (DigitizerInputReport*)ReportPointer;
                var buttons = report->Header.Buttons;
                var pressure = report->Pressure;

                // Send In-Range but no tips
                DisableBit((int)(ButtonBits.Press | ButtonBits.Eraser));
                report->Pressure = 0;
                Device.Write(ReportBuffer);

                // Send Out-Of-Range
                report->Header.Buttons = 0;
                Device.Write(ReportBuffer);

                // Send In-Range but no tips
                EnableBit((int)ButtonBits.InRange);
                if (EraserState)
                    EnableBit((int)ButtonBits.Invert);

                Device.Write(ReportBuffer);

                // Set Proper Report
                if (HasBit(buttons, (int)(ButtonBits.Press | ButtonBits.Eraser)))
                    EnableBit((int)(EraserState ? ButtonBits.Eraser : ButtonBits.Press));
                report->Pressure = pressure;
            }
        }

        public static void SetReport(DigitizerInputReport* report, byte[] reportBuffer)
        {
            SetReport((VMultiReportHeader*)report, reportBuffer);
            EnableBit((int)ButtonBits.InRange);
        }

        public static void SetDevice(HidStream device)
        {
            Device = device;
        }
    }
}