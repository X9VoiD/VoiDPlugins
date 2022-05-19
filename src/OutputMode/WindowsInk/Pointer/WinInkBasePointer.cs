using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using VoiDPlugins.Library.VoiD;
using static VoiDPlugins.OutputMode.WindowsInkConstants;

namespace VoiDPlugins.OutputMode
{
    public unsafe abstract class WinInkBasePointer : IPressureHandler, ITiltHandler, IEraserHandler, ISynchronousPointer
    {
        private readonly IVirtualScreen _screen;
        private ThinOSPointer? _osPointer;
        private Vector2 _internalPos;
        protected DigitizerInputReport* RawPointer { get; }
        protected VMultiInstance<DigitizerInputReport> Instance { get; }
        protected SharedStore SharedStore { get; }
        protected bool Dirty { get; set; }

        public bool Sync
        {
            set => _osPointer = value ? new ThinOSPointer(_screen) : null;
        }

        public bool ForcedSync { get; set; }

        public WinInkBasePointer(string name, TabletReference tabletReference, IVirtualScreen screen)
        {
            _screen = screen;
            Instance = new VMultiInstance<DigitizerInputReport>(name, new DigitizerInputReport());
            SharedStore = SharedStore.GetStore(tabletReference, STORE_KEY);
            if (SharedStore.TryAdd(INSTANCE, Instance))
            {
                SharedStore.TryAdd(POINTER, this);
                SharedStore.TryAdd(ERASER_STATE, false);
                SharedStore.TryAdd(MANUAL_ERASER, false);
                RawPointer = Instance.Pointer;
            }
            else
            {
                Instance = SharedStore.Get<VMultiInstance<DigitizerInputReport>>(INSTANCE);
                RawPointer = Instance.Pointer;
            }
        }

        public void SetEraser(bool isEraser)
        {
            if (!SharedStore.Get<bool>(MANUAL_ERASER))
            {
                WindowsInkButtonHandler.EraserStateTransition(SharedStore, Instance, isEraser);
            }
        }

        public void SetPressure(float percentage)
        {
            RawPointer->Pressure = (ushort)(percentage * 8191);
        }

        public void SetTilt(Vector2 tilt)
        {
            RawPointer->XTilt = (byte)tilt.X;
            RawPointer->YTilt = (byte)tilt.Y;
        }

        public void Reset()
        {
            if (_osPointer is not null && !ForcedSync)
                SyncOSCursor();
        }

        public void Flush()
        {
            if (Dirty)
            {
                Dirty = false;

                if (ForcedSync)
                    SyncOSCursor();
                Instance.Write();
            }
        }

        protected void SetInternalPosition(Vector2 pos)
        {
            _internalPos = pos;
        }

        private void SyncOSCursor()
        {
            _osPointer?.SetPosition(_internalPos);
        }
    }
}