using System.Collections.Generic;
using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using VoiDPlugins.Library.VoiD;
using static VoiDPlugins.OutputMode.VMultiModeConstants;

namespace VoiDPlugins.OutputMode
{
    public unsafe class VMultiAbsolutePointer : IAbsolutePointer, ISynchronousPointer
    {
        private readonly AbsoluteInputReport* _rawPointer;
        private readonly VMultiInstance<AbsoluteInputReport> _instance;
        private Vector2 _conversionFactor;
        private Vector2 _prev;
        private bool _dirty;

        public VMultiAbsolutePointer(TabletReference tabletReference, IVirtualScreen virtualScreen)
        {
            var sharedStore = SharedStore.GetStore(tabletReference, STORE_KEY);
            _instance = sharedStore.GetOrUpdate(ABS_INSTANCE, createInstance, out _);
            _rawPointer = _instance.Pointer;
            _conversionFactor = new Vector2(32767, 32767) / new Vector2(virtualScreen.Width, virtualScreen.Height);

            sharedStore.SetOrAdd(MODE, ABS_INSTANCE);

            static VMultiInstance<AbsoluteInputReport> createInstance()
            {
                return new VMultiInstance<AbsoluteInputReport>("VMultiAbs", new AbsoluteInputReport());
            }
        }

        public void SetPosition(Vector2 pos)
        {
            if (pos == _prev)
                return;

            pos *= _conversionFactor;
            _rawPointer->X = (ushort)pos.X;
            _rawPointer->Y = (ushort)pos.Y;
            _dirty = true;
            _prev = pos;
        }

        public void Reset()
        {
        }

        public void Flush()
        {
            if (_dirty)
            {
                _dirty = false;
                _instance.Write();
            }
        }
    }
}