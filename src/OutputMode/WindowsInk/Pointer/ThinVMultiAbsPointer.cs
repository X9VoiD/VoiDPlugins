using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    internal unsafe class ThinVMultiAbsPointer
    {
        private readonly AbsoluteInputReport* _rawPointer;
        private readonly VMultiInstance<AbsoluteInputReport>? _instance;
        private Vector2 _conversionFactor;

        public ThinVMultiAbsPointer(IVirtualScreen virtualScreen)
        {
            _instance = new VMultiInstance<AbsoluteInputReport>("VMultiAbs", new AbsoluteInputReport());
            _rawPointer = _instance.Pointer;
            _conversionFactor = new Vector2(32767, 32767) / new Vector2(virtualScreen.Width, virtualScreen.Height);
        }

        public void SetPosition(Vector2 pos)
        {
            pos *= _conversionFactor;
            _rawPointer->X = (ushort)pos.X;
            _rawPointer->Y = (ushort)pos.Y;
            _instance!.Write();
        }
    }
}