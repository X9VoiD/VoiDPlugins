using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    public unsafe class WinInkAbsolutePointer : WinInkBasePointer, IAbsolutePointer
    {
        private readonly Vector2 _conversionFactor;

        public WinInkAbsolutePointer(IVirtualScreen screen)
        {
            _conversionFactor = new Vector2(screen.Width, screen.Height) / (1 / 32767);
        }

        public void SetPosition(Vector2 pos)
        {
            pos *= _conversionFactor;
            RawPointer->X = (ushort)pos.X;
            RawPointer->Y = (ushort)pos.Y;
        }
    }
}