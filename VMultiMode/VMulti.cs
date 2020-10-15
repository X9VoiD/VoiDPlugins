using System.Numerics;
using HidSharp;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VMultiMode
{
    public static class VMulti
    {
        public abstract class VMultiReport
        {
            public byte VMultiID;       // ID to communicate to the Mouse device
            public byte ReportLength;   // Size of the report in bytes.
            public byte ReportID;       // ID of the report.
            public byte Buttons;        // Byte with switches for mouse buttons
            public abstract byte Size { get; }
            public abstract byte[] ToBytes();
        }

        public class VMultiAbsReport : VMultiReport
        {
            public ushort X;            // X position of the mouse from 0 to 32767
            public ushort Y;            // Y position of the mouse from 0 to 32767
            public ushort Pressure;     // Pressure of the mouse? from 0 to 8191
            public override byte[] ToBytes()
            {
                var bytes = new byte[Size];
                bytes[0] = VMultiID;
                bytes[1] = ReportLength;
                bytes[2] = ReportID;
                bytes[3] = Buttons;
                bytes[4] = (byte)(X & 0xFF);
                bytes[5] = (byte)((X & 0xFF00) >> 8);
                bytes[6] = (byte)(Y & 0xFF);
                bytes[7] = (byte)((Y & 0xFF00) >> 8);
                bytes[8] = (byte)(Pressure & 0xFF);
                bytes[9] = (byte)((Pressure & 0xFF00) >> 8);
                return bytes;
            }

            public override byte Size => 10;
        }

        public class VMultiRelReport : VMultiReport
        {
            public byte X;               // X position of the mouse from -127 to 127
            public byte Y;               // Y position of the mouse from -127 to 127
            public byte WheelPos;        // Wheel position of the mouse from -127 to 127
            public override byte[] ToBytes()
            {
                var bytes = new byte[Size];
                bytes[0] = VMultiID;
                bytes[1] = ReportLength;
                bytes[2] = ReportID;
                bytes[3] = Buttons;
                bytes[4] = X;
                bytes[5] = Y;
                bytes[6] = WheelPos;
                return bytes;
            }

            public override byte Size => 7;
        }

        public enum ButtonMask : int
        {
            Left = 1,
            Right = 2,
            Middle = 4,
        }


        public class VMultiHandler<T> where T : VMultiReport, new()
        {
            protected T Report;
            protected HidStream VMultiDev;

            protected void Init(string Name, byte ReportID)
            {
                Report = new T()
                {
                    VMultiID = 0x40,
                    ReportID = ReportID
                };

                Report.ReportLength = Report.Size;

                VMultiDev = null;
                foreach (var device in DeviceList.Local.GetHidDevices(productID: 47820))
                {
                    if (device.GetMaxOutputReportLength() == 65 && device.GetMaxInputReportLength() == 65)
                    {
                        device.TryOpen(out VMultiDev);
                        if (VMultiDev == null)
                        {
                            Log.Write(Name, "Cannot find VirtualHID", LogLevel.Error);
                        }
                    }
                }
            }

            public void MouseDown(MouseButton button)
            {
                switch (button)
                {
                    case MouseButton.Left:
                        EnableBit(ButtonMask.Left);
                        break;
                    case MouseButton.Right:
                        EnableBit(ButtonMask.Right);
                        break;
                    case MouseButton.Middle:
                        EnableBit(ButtonMask.Middle);
                        break;
                }
                VMultiDev.Write(Report.ToBytes());
            }

            public void MouseUp(MouseButton button)
            {
                switch (button)
                {
                    case MouseButton.Left:
                        DisableBit(ButtonMask.Left);
                        break;
                    case MouseButton.Right:
                        DisableBit(ButtonMask.Right);
                        break;
                    case MouseButton.Middle:
                        DisableBit(ButtonMask.Middle);
                        break;
                }
                VMultiDev.Write(Report.ToBytes());
            }

            private void EnableBit(ButtonMask mask)
            {
                Report.Buttons = (byte)(Report.Buttons | (int)mask);
            }

            private void DisableBit(ButtonMask mask)
            {
                Report.Buttons = (byte)(Report.Buttons & ~(int)mask);
            }
        }
    }
}