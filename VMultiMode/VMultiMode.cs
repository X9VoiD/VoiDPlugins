using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;
using static VMultiMode.VMulti;

namespace VMultiMode
{
    internal static class VMultiState
    {
        public static VMultiAbsHandler AbsHandler = null;
        public static VMultiRelHandler RelHandler = null;
    }

    [PluginName("VMulti Absolute Output Mode"), SupportedPlatform(PluginPlatform.Windows)]
    public class VMultiAbsMode : AbsoluteOutputMode
    {
        public override IVirtualTablet VirtualTablet => VMultiState.AbsHandler ?? new VMultiAbsHandler(Output);
    }

    [PluginName("VMulti Relative Output Mode"), SupportedPlatform(PluginPlatform.Windows)]
    public class VMultiRelMode : AbsoluteOutputMode
    {
        public override IVirtualTablet VirtualTablet => VMultiState.RelHandler ?? new VMultiRelHandler(Output);
    }

    public class VMultiAbsHandler : VMultiHandler<VMultiAbsReport>, IVirtualTablet
    {
        public VMultiAbsHandler(Area screenArea)
        {
            VMultiState.AbsHandler = this;
            Init("VMultiAbs", 0x09);
            ScreenArea = screenArea;
        }

        public void SetPosition(Vector2 pos)
        {
            Report.X = (ushort)(pos.X / ScreenArea.Width * 32767);
            Report.Y = (ushort)(pos.Y / ScreenArea.Height * 32767);
            VMultiDev.Write(Report.ToBytes());
        }
    }

    public class VMultiRelHandler : VMultiHandler<VMultiRelReport>, IVirtualTablet
    {
        ushort prevX, prevY;
        public VMultiRelHandler(Area screenArea)
        {
            VMultiState.RelHandler = this;
            Init("VMultiRel", 0x04);
            ScreenArea = screenArea;
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