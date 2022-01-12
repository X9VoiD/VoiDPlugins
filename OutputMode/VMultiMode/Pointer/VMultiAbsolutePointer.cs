using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using VoiDPlugins.Library.VoiD;

namespace VoiDPlugins.OutputMode
{
    public unsafe class VMultiAbsolutePointer : IAbsolutePointer, ISynchronousPointer
    {
        private readonly AbsoluteInputReport* _rawPointer;
        private readonly VMultiInstance<AbsoluteInputReport>? _instance;
        private Vector2 _conversionFactor;

        public VMultiAbsolutePointer(TabletReference tabletReference, IVirtualScreen virtualScreen)
        {
            _instance = GlobalStore<VMultiInstance>.GetOrInitialize(tabletReference, () => new VMultiInstance<AbsoluteInputReport>("VMultiAbs", new AbsoluteInputReport()));
            _rawPointer = _instance.Pointer;
            _conversionFactor = new Vector2(32767, 32767) / new Vector2(virtualScreen.Width, virtualScreen.Height);
        }

        public void SetPosition(Vector2 pos)
        {
            pos *= _conversionFactor;
            _rawPointer->X = (ushort)pos.X;
            _rawPointer->Y = (ushort)pos.Y;
        }

        public void Reset()
        {
        }

        public void Flush()
        {
            _instance!.Write();
        }
    }
}