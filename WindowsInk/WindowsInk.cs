using System.Numerics;
using HidSharp;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;
using static VoiDPlugins.WindowsInk.VMulti;

namespace VoiDPlugins.WindowsInk
{
    [PluginName("Artist Mode (Windows Ink)"), SupportedPlatform(PluginPlatform.Windows)]
    public class WindowsInk : AbsoluteOutputMode
    {
        private readonly IVirtualTablet InkHandler = new InkHandler();
        public override IVirtualTablet VirtualTablet => InkHandler;
    }

    [PluginName("Artist Mode (Relative Windows Ink)"), SupportedPlatform(PluginPlatform.Windows)]
    public class WindowsInkRelative : RelativeOutputMode
    {
        private readonly IVirtualMouse InkHandler = new InkHandler();
        public override IVirtualMouse VirtualMouse => InkHandler;
    }

    public class InkHandler : IVirtualTablet, IPressureHandler, IVirtualMouse
    {
        private InkReport InkReport;
        private readonly HidStream VMultiDev;
        private bool EraserState;
        private Vector2 LastPos;
        private readonly float Width = Info.Driver.VirtualScreen.Width;
        private readonly float Height = Info.Driver.VirtualScreen.Height;

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
                    if (VMultiDev == null)
                    {
                        Log.Write("WindowsInk", "Cannot find VirtualHID", LogLevel.Error);
                    }
                }
            }

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
            InkReport.X = (ushort)(pos.X / Width * 32767);
            InkReport.Y = (ushort)(pos.Y / Height * 32767);
            VMultiDev.Write(InkReport);
        }

        public void Move(float dX, float dY)
        {
            LastPos.X += dX;
            LastPos.Y += dY;
            InkReport.X = (ushort)(LastPos.X / Width * 32767);
            InkReport.Y = (ushort)(LastPos.Y / Height * 32767);
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