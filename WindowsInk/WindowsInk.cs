using System;
using System.Numerics;
using HidSharp;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using static VoiDPlugins.WindowsInk.VMulti;

namespace VoiDPlugins.WindowsInk
{

    [PluginName("Artist Mode (Windows Ink)"), SupportedPlatform(PluginPlatform.Windows)]
    public class WindowsInk : AbsoluteOutputMode
    {
        private readonly IAbsolutePointer InkHandler = new InkHandler();
        public override IAbsolutePointer Pointer => InkHandler;
    }

    [PluginName("Artist Mode (Relative Windows Ink)"), SupportedPlatform(PluginPlatform.Windows)]
    public class WindowsInkRelative : RelativeOutputMode
    {
        private readonly IRelativePointer InkHandler = new InkHandler();
        public override IRelativePointer Pointer => InkHandler;
    }

    public class InkHandler : IAbsolutePointer, IRelativePointer, IVirtualTablet, IVirtualMouse
    {
        private readonly IVirtualScreen VirtualScreen = (Info.Driver as IVirtualDisplayDriver).VirtualScreen;
        private readonly HidStream VMultiDev;
        private InkReport InkReport;
        private Vector2 LastPos;
        private bool EraserState;
        private readonly Vector2 ScreenMax, ScreenToVMulti;

        public InkHandler()
        {
            InkReport = new InkReport()
            {
                VMultiID = 0x40,
                ReportLength = 0x0a,
                ReportID = 0x05,
                Buttons = (byte)ButtonMask.InRange
            };

            VMultiDev = null;
            foreach (var device in DeviceList.Local.GetHidDevices(productID: 47820))
            {
                if (device.GetMaxOutputReportLength() == 65 && device.GetMaxInputReportLength() == 65)
                {
                    device.TryOpen(out VMultiDev);
                    if (VMultiDev != null)
                        break;
                }
            }

            if (VMultiDev == null)
            {
                Log.Write("WindowsInk", "Cannot find VirtualHID", LogLevel.Error);
                Log.Write("WindowsInk", "Install VMulti driver here: https://github.com/X9VoiD/vmulti-bin/releases/latest");
            }

            ScreenMax = new Vector2(VirtualScreen.Width, VirtualScreen.Height) * 32767;
            ScreenToVMulti = ScreenMax / 32767;
            EraserState = false;
        }

        public void MouseDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    if (!EraserState)
                    {
                        EnableBit(ButtonMask.Press);
                    }
                    else
                    {
                        EnableBit(ButtonMask.Eraser);
                    }
                    break;
                case MouseButton.Right:
                    EnableBit(ButtonMask.Barrel);
                    break;
                case MouseButton.Middle:
                    EnableBit(ButtonMask.Invert);
                    DisableBit(ButtonMask.Press);
                    EraserState = true;
                    StateChange();
                    break;
            }
        }

        public void MouseUp(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    DisableBit(ButtonMask.Press);
                    DisableBit(ButtonMask.Eraser);
                    break;
                case MouseButton.Right:
                    DisableBit(ButtonMask.Barrel);
                    break;
                case MouseButton.Middle:
                    DisableBit(ButtonMask.Invert);
                    EraserState = false;
                    StateChange();
                    break;
            }
        }

        public void SetPosition(Vector2 pos)
        {
            var newPos = pos / ScreenToVMulti;
            InkReport.X = (ushort)newPos.X;
            InkReport.Y = (ushort)newPos.Y;
            VMultiDev.Write(InkReport);
        }

        public void Translate(Vector2 delta)
        {
            var newPos = LastPos + delta;
            LastPos.X = Math.Clamp(newPos.X, 0, ScreenMax.X);
            LastPos.Y = Math.Clamp(newPos.Y, 0, ScreenMax.Y);
            var reportPos = LastPos / ScreenToVMulti;
            InkReport.X = (ushort)reportPos.X;
            InkReport.Y = (ushort)reportPos.Y;
            VMultiDev.Write(InkReport);
        }

        public void SetPressure(float percentage)
        {
            InkReport.Pressure = (ushort)(percentage * 8191);
        }

        private void EnableBit(ButtonMask mask)
        {
            InkReport.Buttons = (byte)(InkReport.Buttons | (int)mask);
        }

        private void DisableBit(ButtonMask mask)
        {
            InkReport.Buttons = (byte)(InkReport.Buttons & ~(int)mask);
        }

        private void StateChange()
        {
            var prevState = InkReport.Buttons;
            var prevPressure = InkReport.Pressure;
            InkReport.Buttons = 0;
            InkReport.Pressure = 0;
            VMultiDev.Write(InkReport);
            InkReport.Buttons = prevState;
            InkReport.Pressure = prevPressure;
        }
    }
}