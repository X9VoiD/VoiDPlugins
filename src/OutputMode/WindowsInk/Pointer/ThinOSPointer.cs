using System;
using System.Numerics;
using System.Runtime.InteropServices;
using OpenTabletDriver.Plugin.Platform.Display;

namespace VoiDPlugins.OutputMode
{
    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public INPUT_TYPE type;
        public MOUSEINPUT mouse;
        public static int Size => Marshal.SizeOf(typeof(INPUT));
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MOUSEEVENTF dwFlags;
        public uint time;
        public UIntPtr dwExtraInfo;
    }

    public enum MOUSEEVENTF : uint
    {
        ABSOLUTE = 0x8000,
        MOVE = 0x0001,
        VIRTUALDESK = 0x4000,
        LEFTDOWN = 0x0002,
        LEFTUP = 0x0004,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040,
        RIGHTDOWN = 0x0008,
        RIGHTUP = 0x0010,
        XDOWN = 0x0080,
        XUP = 0x0100,
        MOVE_NOCOALESCE = 0x2000
    }

    public enum INPUT_TYPE
    {
        MOUSE_INPUT,
        KEYBD_INPUT,
        HARDWARE_INPUT
    }

    public class ThinOSPointer
    {
        private readonly Vector2 _conversion;
        private readonly INPUT[] _inputs = new INPUT[]
        {
            new INPUT
            {
                type = INPUT_TYPE.MOUSE_INPUT,
                mouse = new MOUSEINPUT
                {
                    time = 0,
                    dwExtraInfo = UIntPtr.Zero
                }
            }
        };

        public ThinOSPointer(IVirtualScreen screen)
        {
            _conversion = new Vector2(screen.Width, screen.Height) / 65535;
        }

        public void SetPosition(Vector2 pos)
        {
            var converted = pos / _conversion;

            _inputs[0].mouse.dwFlags = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.MOVE | MOUSEEVENTF.VIRTUALDESK;
            _inputs[0].mouse.dx = (int)converted.X;
            _inputs[0].mouse.dy = (int)converted.Y;
            _ = SendInput(1, _inputs, INPUT.Size);
        }

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);
    }
}