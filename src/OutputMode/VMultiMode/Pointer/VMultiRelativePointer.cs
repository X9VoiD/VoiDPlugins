using System.Numerics;
using OpenTabletDriver;
using OpenTabletDriver.Platform.Pointer;
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

        public VMultiRelativePointer(InputDevice inputDevice)
        {
            var sharedStore = SharedStore.GetStore(inputDevice, STORE_KEY);
            _instance = sharedStore.GetOrUpdate(REL_INSTANCE, createInstance, out var updated);
            _rawPointer = _instance.Pointer;

            sharedStore.SetOrAdd(MODE, REL_INSTANCE);

            static VMultiInstance<RelativeInputReport> createInstance()
            {
                return new VMultiInstance<RelativeInputReport>("VMultiRel", new RelativeInputReport());
            }
        }

        public void SetPosition(Vector2 delta)
        {
            if (delta == Vector2.Zero)
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

        public void MouseDown(MouseButton button)
        {
        }

        public void MouseUp(MouseButton button)
        {
        }
    }
}