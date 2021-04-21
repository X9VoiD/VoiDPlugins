using System.Collections.Generic;
using HidSharp;
using OpenTabletDriver.Plugin.Attributes;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    [PluginName("Windows Ink")]
    public class WinInkButtonHandler : ButtonHandler
    {
        public override Dictionary<string, int> Bindings => null;
        public readonly static string[] validProperties = new string[]
        {
            "Pen Tip",
            "Pen Button",
            "Eraser (Toggle)",
            "Eraser (Hold)"
        };

        public override string[] ValidProperties => validProperties;

        private enum Button : int
        {
            Press = 1,
            Barrel = 2,
            Eraser = 4,
            Invert = 8,
            InRange = 16
        }

        private bool EraserState;
        private static HidStream Device;
        private static new DigitizerInputReport Report;

        public override void Press(string input)
        {
            switch (input)
            {
                case "Pen Tip":
                    EnableBit((int)(EraserState ? Button.Eraser : Button.Press));
                    break;

                case "Pen Button":
                    EnableBit((int)Button.Barrel);
                    break;

                case "Eraser (Toggle)":
                    if (EraserState)
                    {
                        DisableBit((int)Button.Invert);
                        EraserState = false;
                        EraserStateTransition();
                    }
                    else
                    {
                        // EnableBit((int)Button.Invert);
                        DisableBit((int)Button.Press);
                        EraserState = true;
                        EraserStateTransition();
                    }
                    break;

                case "Eraser (Hold)":
                    // EnableBit((int)Button.Invert);
                    DisableBit((int)Button.Press);
                    EraserState = true;
                    EraserStateTransition();
                    break;
            }
        }

        public override void Release(string input)
        {
            switch (input)
            {
                case "Pen Tip":
                    DisableBit((int)Button.Press);
                    DisableBit((int)Button.Eraser);
                    break;

                case "Pen Button":
                    DisableBit((int)Button.Barrel);
                    break;

                case "Eraser (Hold)":
                    // DisableBit((int)Button.Invert);
                    EraserState = false;
                    EraserStateTransition();
                    break;

            }
        }

        private void EraserStateTransition()
        {
            var buttons = Report.Buttons;
            var pressure = Report.Pressure;

            // Send In-Range but no tips
            DisableBit((int)Button.Press);
            DisableBit((int)Button.Eraser);
            Report.Pressure = 0;
            Device.Write(Report.ToBytes());

            // Send Out-Of-Range
            Report.Buttons = 0;
            Device.Write(Report.ToBytes());

            // Send In-Range but no tips
            EnableBit((int)Button.InRange);
            if (EraserState)
                EnableBit((int)Button.Invert);

            Device.Write(Report.ToBytes());

            // Send Proper Report
            Report.Buttons = buttons;
            Report.Pressure = pressure;
        }

        public static void SetReport(DigitizerInputReport Report)
        {
            ButtonHandler.SetReport(Report);
            WinInkButtonHandler.Report = Report;
            EnableBit((int)Button.InRange);
        }

        public static void SetDevice(HidStream device)
        {
            Device = device;
        }
    }
}