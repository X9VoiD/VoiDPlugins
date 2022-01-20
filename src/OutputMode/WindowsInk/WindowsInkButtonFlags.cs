using System;

namespace VoiDPlugins.OutputMode
{
    [Flags]
    public enum WindowsInkButtonFlags : byte
    {
        Press = 1,
        Barrel = 2,
        Eraser = 4,
        Invert = 8,
        InRange = 16
    }
}