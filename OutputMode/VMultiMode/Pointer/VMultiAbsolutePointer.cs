using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public unsafe class VMultiAbsolutePointer : IAbsolutePointer, ISynchronousPointer
    {
        private AbsoluteInputReport* _rawPointer;
        private VMultiInstance<AbsoluteInputReport>? _instance;
        private Vector2 _conversionFactor;

        public VMultiAbsolutePointer(IVirtualScreen virtualScreen)
        {
            _conversionFactor = new Vector2(virtualScreen.Width, virtualScreen.Height) / (1 / 32767);
        }

        public void Initialize(TabletReference tabletReference)
        {
            _instance = VMultiInstanceManager.RetrieveVMultiInstance("VMultiAbs", tabletReference, () => new AbsoluteInputReport());
            _rawPointer = _instance.Pointer;
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