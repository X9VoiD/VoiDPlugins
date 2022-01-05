using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.OutputMode
{
    public unsafe class VMultiRelativePointer : IRelativePointer, ISynchronousPointer
    {
        private RelativeInputReport* _rawPointer;
        private VMultiInstance<RelativeInputReport>? _instance;
        private Vector2 _error;

        public void Initialize(TabletReference tabletReference)
        {
            _instance = VMultiInstanceManager.RetrieveVMultiInstance("VMultiRel", tabletReference, () => new RelativeInputReport());
            _rawPointer = _instance.Pointer;
        }

        public void SetPosition(Vector2 delta)
        {
            delta += _error;
            _error = new Vector2(delta.X % 1, delta.Y % 1);
            _rawPointer->X = (byte)delta.X;
            _rawPointer->Y = (byte)delta.Y;
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