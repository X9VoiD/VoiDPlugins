using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public unsafe class VMultiRelativePointer : BasePointer<RelativeInputReport>, IRelativePointer
    {
        public VMultiRelativePointer(IVirtualScreen screen) : base(screen, "VMultiRel")
        {
            ButtonHandler.SetReport((VMultiReportHeader*)ReportPointer, ReportBuffer);
        }

        protected override RelativeInputReport CreateReport()
        {
            return new RelativeInputReport(0x04);
        }

        public override void SetPosition(Vector2 pos)
        {
            SetCoordinates(pos);
            Device.Write(ReportBuffer);
        }

        protected override void SetCoordinates(Vector2 pos)
        {
            ReportPointer->X = (byte)pos.X;
            ReportPointer->Y = (byte)pos.Y;
        }
    }
}