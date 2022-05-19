using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using VoiDPlugins.Library.VoiD;
using static VoiDPlugins.OutputMode.VMultiModeConstants;

namespace VoiDPlugins.OutputMode
{
    public unsafe class VMultiRelativePointer : IRelativePointer, ISynchronousPointer
    {
        private readonly RelativeInputReport* _rawPointer;
        private readonly VMultiInstance<RelativeInputReport> _instance;
        private Vector2 _error;
        private Vector2 _prev;
        private bool _dirty;

        public VMultiRelativePointer(TabletReference tabletReference)
        {
            _instance = new VMultiInstance<RelativeInputReport>("VMultiRel", new RelativeInputReport());
            var sharedStore = SharedStore.GetStore(tabletReference, STORE_KEY);
            if (!sharedStore.TryAdd(INSTANCE, _instance))
                _instance = sharedStore.Get<VMultiInstance<RelativeInputReport>>(INSTANCE);

            _rawPointer = _instance.Pointer;
        }

        public void SetPosition(Vector2 delta)
        {
            if (delta == Vector2.Zero && _prev == Vector2.Zero)
                return;
            if (_rawPointer is null)
                return;

            delta += _error;
            _error = new Vector2(delta.X % 1, delta.Y % 1);
            _rawPointer->X = (byte)delta.X;
            _rawPointer->Y = (byte)delta.Y;
            _dirty = true;
            _prev = delta;
        }

        public void Reset()
        {
        }

        public void Flush()
        {
            if (_dirty && _instance is not null)
            {
                _dirty = false;
                _instance.Write();
            }
        }
    }
}