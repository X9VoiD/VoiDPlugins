using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.OutputMode
{
    public unsafe class WinInkAbsolutePointer : WinInkBasePointer, IAbsolutePointer
    {
        private readonly Vector2 _conversionFactor;

        public WinInkAbsolutePointer(TabletReference tabletReference, IVirtualScreen screen) : base(tabletReference)
        {
            _conversionFactor = new Vector2(32767, 32767) / new Vector2(screen.Width, screen.Height);
        }

        public void SetPosition(Vector2 pos)
        {
            Instance!.EnableButtonBit((int)WindowsInkButtonFlags.InRange);
            pos *= _conversionFactor;
            RawPointer->X = (ushort)pos.X;
            RawPointer->Y = (ushort)pos.Y;
        }
    }
}