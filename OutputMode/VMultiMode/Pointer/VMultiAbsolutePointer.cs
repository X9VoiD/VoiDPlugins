using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public unsafe class VMultiAbsolutePointer : BasePointer<AbsoluteInputReport>, IAbsolutePointer
    {
        public VMultiAbsolutePointer(IVirtualScreen screen) : base(screen, "VMultiAbs")
        {
            ButtonHandler.SetReport((VMultiReportHeader*)ReportPointer, ReportBuffer);
        }

        protected override AbsoluteInputReport CreateReport()
        {
            return new AbsoluteInputReport(0x09);
        }

        protected override void SetCoordinates(Vector2 pos)
        {
            ReportPointer->X = (ushort)pos.X;
            ReportPointer->Y = (ushort)pos.Y;
        }
    }
}