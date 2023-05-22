using System;
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
        private Vector2 _delta;
        private bool _dirty;

        public VMultiRelativePointer(TabletReference tabletReference)
        {
            var sharedStore = SharedStore.GetStore(tabletReference, STORE_KEY);
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
            _dirty = true;
            _delta = delta;
        }

        public void Reset()
        {
        }

        public void Flush()
        {
            if (_dirty && _instance is not null)
            {
                _dirty = false;
                Send(_delta);
            }
        }

        private void Send(Vector2 delta)
        {
            var remaining = delta + _error;
            while (Math.Abs(remaining.X) > 127 || Math.Abs(remaining.Y) > 127)
            {
                var partialDelta = new Vector2(Math.Clamp(remaining.X, -127, 127), Math.Clamp(remaining.Y, -127, 127));
                _rawPointer->X = (byte)partialDelta.X;
                _rawPointer->Y = (byte)partialDelta.Y;
                _instance.Write();
                remaining -= partialDelta;
            }

            _error = new Vector2(remaining.X % 1, remaining.Y % 1);
            _rawPointer->X = (byte)remaining.X;
            _rawPointer->Y = (byte)remaining.Y;
            _instance.Write();
        }
    }
}