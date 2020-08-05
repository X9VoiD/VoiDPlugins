using System;
using System.Runtime.InteropServices;

namespace WindowsInk
{
    using HSYNTHETICPOINTERDEVICE = IntPtr;
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

    public enum POINTER_FEEDBACK_MODE
    {
        DEFAULT,
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
        POINTER_FLAG_NONE = 0x00000000,
        POINTER_FLAG_NEW = 0x00000001,
        POINTER_FLAG_INRANGE = 0x00000002,
        POINTER_FLAG_INCONTACT = 0x00000004,
        POINTER_FLAG_FIRSTBUTTON = 0x00000010,
        POINTER_FLAG_SECONDBUTTON = 0x00000020,
        POINTER_FLAG_THIRDBUTTON = 0x00000040,
        POINTER_FLAG_FOURTHBUTTON = 0x00000080,
        POINTER_FLAG_FIFTHBUTTON = 0x00000100,
        POINTER_FLAG_PRIMARY = 0x00002000,
        POINTER_FLAG_CONFIDENCE = 0x000004000,
        POINTER_FLAG_CANCELED = 0x000008000,
        POINTER_FLAG_DOWN = 0x00010000,
        POINTER_FLAG_UPDATE = 0x00020000,
        POINTER_FLAG_UP = 0x00040000,
        POINTER_FLAG_WHEEL = 0x00080000,
        POINTER_FLAG_HWHEEL = 0x00100000,
        POINTER_FLAG_CAPTURECHANGED = 0x00200000,
        POINTER_FLAG_HASTRANSFORM = 0x00400000,
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

    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern HSYNTHETICPOINTERDEVICE CreateSyntheticPointerDevice(POINTER_INPUT_TYPE pointerType,
                                                                           UInt32 maxCount, POINTER_FEEDBACK_MODE mode);

        [DllImport("user32.dll")]
        public static extern bool InjectSyntheticPointerInput(HSYNTHETICPOINTERDEVICE device,
            [In, MarshalAs(UnmanagedType.LPArray)] POINTER_TYPE_INFO[] pointerInfo, UInt32 count);
    }
}