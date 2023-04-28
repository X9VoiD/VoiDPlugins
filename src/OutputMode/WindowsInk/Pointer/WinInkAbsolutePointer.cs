using System.Numerics;
using OpenTabletDriver;
using OpenTabletDriver.Platform.Display;

namespace VoiDPlugins.OutputMode
{
    public unsafe class WinInkAbsolutePointer : WinInkBasePointer
    {
        private Vector2 _prev;

        public WinInkAbsolutePointer(InputDevice inputDevice, IVirtualScreen screen)
            : base("Windows Ink", inputDevice, screen)
        {
        }

        public override void SetPosition(Vector2 pos)
        {
            if (pos == _prev)
                return;

            SetInternalPosition(pos);
            Instance.EnableButtonBit((int)WindowsInkButtonFlags.InRange);
            pos = Convert(pos);
            RawPointer->X = (ushort)pos.X;
            RawPointer->Y = (ushort)pos.Y;
            Dirty = true;
            _prev = pos;
        }
    }
}