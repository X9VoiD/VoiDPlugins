using System;
using System.Runtime.InteropServices;

namespace WindowsInk
{
    using HANDLE = IntPtr;
    using HWND = IntPtr;
    using DWORD = UInt32;

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public enum POINTER_INPUT_TYPE
    {
        PT_POINTER,
        PT_TOUCH,
        PT_PEN,
        PT_MOUSE,
        PT_TOUCHPAD
    }

    public enum POINTER_DEVICE_TYPE
    {
        INTEGRATED_PEN,
        EXTERNAL_PEN,
        TOUCH,
        TOUCH_PAD,
        MAX
    }

    public enum POINTER_FEEDBACK_MODE
    {
        DEFAULT = 1,
        INDIRECT,
        NONE
    }

    public enum POINTER_BUTTON_CHANGE_TYPE
    {
        NONE,
        FIRSTBUTTON_DOWN,
        FIRSTBUTTON_UP,
        SECONDBUTTON_DOWN,
        SECONDBUTTON_UP,
        THIRDBUTTON_DOWN,
        THIRDBUTTON_UP,
        FOURTHBUTTON_DOWN,
        FOURTHBUTTON_UP,
        FIFTHBUTTON_DOWN,
        FIFTHBUTTON_UP
    }

    public enum POINTER_FLAGS
    {
        NONE = 0x00000000,
        NEW = 0x00000001,
        INRANGE = 0x00000002,
        INCONTACT = 0x00000004,
        FIRSTBUTTON = 0x00000010,
        SECONDBUTTON = 0x00000020,
        THIRDBUTTON = 0x00000040,
        FOURTHBUTTON = 0x00000080,
        FIFTHBUTTON = 0x00000100,
        PRIMARY = 0x00002000,
        CONFIDENCE = 0x000004000,
        CANCELED = 0x000008000,
        DOWN = 0x00010000,
        UPDATE = 0x00020000,
        UP = 0x00040000,
        WHEEL = 0x00080000,
        HWHEEL = 0x00100000,
        CAPTURECHANGED = 0x00200000,
        HASTRANSFORM = 0x00400000,
    }

    public enum PEN_FLAGS
    {
        NONE = 0x00000000,
        BARREL = 0x00000001,
        INVERTED = 0x00000002,
        ERASER = 0x00000004
    }

    public enum PEN_MASK
    {
        NONE = 0x00000000,
        PRESSURE = 0x00000001,
        ROTATION = 0x00000002,
        TILT_X = 0x00000004,
        TILT_Y = 0x00000008
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTER_INFO
    {
        public POINTER_INPUT_TYPE pointerType;
        public uint pointerId;
        public uint frameId;
        public POINTER_FLAGS pointerFlags;
        public HANDLE sourceDevice;
        public HWND hwndTarget;
        public POINT ptPixelLocation;
        public POINT ptHimetricLocation;
        public POINT ptPixelLocationRaw;
        public POINT ptHimetricLocationRaw;
        public DWORD dwTime;
        public uint historyCount;
        public int InputData;
        public DWORD dwKeyStates;
        public ulong PerformanceCount;
        public POINTER_BUTTON_CHANGE_TYPE ButtonChangeType;
    }

    public struct POINTER_PEN_INFO
    {
        public POINTER_INFO pointerInfo;
        public PEN_FLAGS pointerFlags;
        public PEN_MASK penMask;
        public uint pressure;
        public uint rotation;
        public int tiltX;
        public int tiltY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTER_TYPE_INFO
    {
        public POINTER_INPUT_TYPE type;
        public POINTER_PEN_INFO penInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct POINTER_DEVICE_INFO
    {
        public DWORD displayOrientation;
        public IntPtr device;
        public POINTER_DEVICE_TYPE pointerDeviceType;
        public IntPtr monitor;
        public ulong startingCursorId;
        public ushort maxActiveContacts;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 520)]
        public string productString;
    }

    public static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr CreateSyntheticPointerDevice(POINTER_INPUT_TYPE pointerType,
                                                                           ulong maxCount, POINTER_FEEDBACK_MODE mode);

        [DllImport("user32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InjectSyntheticPointerInput(IntPtr device, [In, MarshalAs(UnmanagedType.LPArray)] POINTER_TYPE_INFO[] pointerInfo, uint count);

        [DllImport("user32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetPointerDevices(ref uint deviceCount, IntPtr pointerDevices);
    }
}