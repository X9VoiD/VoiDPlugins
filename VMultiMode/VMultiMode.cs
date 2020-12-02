using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;
using static VoiDPlugins.VMultiMode.VMulti;

namespace VoiDPlugins.VMultiMode
{
    [PluginName("VMulti Absolute Output Mode"), SupportedPlatform(PluginPlatform.Windows)]
    public class VMultiAbsMode : AbsoluteOutputMode
    {
        private readonly IAbsolutePointer AbsHandler = new VMultiAbsHandler();
        public override IAbsolutePointer Pointer => AbsHandler;
    }

    [PluginName("VMulti Relative Output Mode"), SupportedPlatform(PluginPlatform.Windows)]
    public class VMultiRelMode : AbsoluteOutputMode
    {
        private readonly IAbsolutePointer RelHandler = new VMultiRelHandler();
        public override IAbsolutePointer Pointer => RelHandler;
    }

    public class VMultiAbsHandler : VMultiHandler<VMultiAbsReport>, IAbsolutePointer
    {
        public VMultiAbsHandler()
        {
            Init("VMultiAbs", 0x09);
        }

        public void SetPosition(Vector2 pos)
        {
            var newPos = pos / ScreenToVMulti;
            Report.X = (ushort)newPos.X;
            Report.Y = (ushort)newPos.Y;
            VMultiDev.Write(Report.ToBytes());
        }
    }

    public class VMultiRelHandler : VMultiHandler<VMultiRelReport>, IAbsolutePointer
    {
        private ushort prevX, prevY;

        public VMultiRelHandler()
        {
            Init("VMultiRel", 0x04);
        }

        public void SetPosition(Vector2 pos)
        {
            var X = (ushort)pos.X;
            var Y = (ushort)pos.Y;
            Report.X = (byte)(X - prevX);
            Report.Y = (byte)(Y - prevY);
            prevX = X;
            prevY = Y;
            VMultiDev.Write(Report.ToBytes());
        }
    }
}