using System;
using TabletDriverPlugin;

namespace WindowsInk
{
    using HSYNTHETICPOINTERDEVICE = IntPtr;

    public class Pen
    {
        private POINTER_PEN_INFO _penInfo;
        private POINTER_INFO _pointerInfo;
        private HSYNTHETICPOINTERDEVICE _penHandle;
        private bool inContact = false;
        private POINTER_FLAGS pointerFlags;

        public Pen()
        {
            _pointerInfo = new POINTER_INFO
            {
                pointerType = POINTER_INPUT_TYPE.PT_PEN,
                pointerId = 1,
                frameId = 0,
                pointerFlags = POINTER_FLAGS.POINTER_FLAG_NONE,
                sourceDevice = IntPtr.Zero,
                hwndTarget = IntPtr.Zero,
                ptPixelLocation = new POINT(),
                ptHimetricLocation = new POINT(),
                ptPixelLocationRaw = new POINT(),
                ptHimetricLocationRaw = new POINT(),
                dwTime = 0,
                historyCount = 1,
                dwKeyStates = 0,
                PerformanceCount = 0,
                ButtonChangeType = POINTER_BUTTON_CHANGE_TYPE.NONE
            };

            _penInfo = new POINTER_PEN_INFO
            {
                pointerInfo = _pointerInfo,
                pointerFlags = PEN_FLAGS.NONE,
                penMask = PEN_MASK.NONE,
                pressure = 0,
                rotation = 0,
                tiltX = 0,
                tiltY = 0
            };

            // Retrieve handle to custom pen
            _penHandle = NativeMethods.CreateSyntheticPointerDevice(POINTER_INPUT_TYPE.PT_PEN, 1, POINTER_FEEDBACK_MODE.INDIRECT);

            // Notify WindowsInk
            _pointerInfo.pointerFlags = POINTER_FLAGS.POINTER_FLAG_NEW;
            Inject();

            // Back to normal state
            _pointerInfo.pointerFlags = POINTER_FLAGS.POINTER_FLAG_INRANGE | POINTER_FLAGS.POINTER_FLAG_PRIMARY;
        }

        public void Inject()
        {
            var pointer = new POINTER_TYPE_INFO
            {
                type = POINTER_INPUT_TYPE.PT_PEN,
                penInfo = _penInfo
            };
            if (!NativeMethods.InjectSyntheticPointerInput(_penHandle, new POINTER_TYPE_INFO[] { pointer }, 1))
            {
                Log.Write("WindowsInk", "Injection Failed");
            }
        }

        public void SetPosition(POINT point)
        {
            _pointerInfo.ptPixelLocation = point;
            _pointerInfo.ptPixelLocationRaw = point;
            _pointerInfo.ptHimetricLocation = point;
            _pointerInfo.ptHimetricLocationRaw = point;
        }

        public void SetPressure(uint pressure)
        {
            if (pressure > 0)
                inContact = true;
            _penInfo.pressure = pressure;
        }

        public void SetPointerFlags(POINTER_FLAGS flags)
        {
            pointerFlags |= flags;
        }

        public void UnsetPointerFlags(POINTER_FLAGS flags)
        {
            pointerFlags &= ~flags;
        }

        public void ClearPointerFlags()
        {
            pointerFlags = 0;
        }
    }
}