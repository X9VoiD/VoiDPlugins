using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.OutputMode
{
    public unsafe class WinInkAbsolutePointer : WinInkBasePointer, IAbsolutePointer
    {
        private Vector2 _prev;

        public WinInkAbsolutePointer(TabletReference tabletReference, IVirtualScreen screen)
            : base("Windows Ink", tabletReference, screen)
        {
        }

        public void SetPosition(Vector2 pos)
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