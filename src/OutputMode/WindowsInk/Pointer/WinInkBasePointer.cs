using System.Numerics;
using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Platform.Display;
using OpenTabletDriver.Platform.Pointer;
using OpenTabletDriver.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using VoiDPlugins.Library.VoiD;
using static VoiDPlugins.OutputMode.WindowsInkConstants;

namespace VoiDPlugins.OutputMode
{
    public unsafe abstract class WinInkBasePointer : IPressureHandler, ITiltHandler, IEraserHandler, ISynchronousPointer
    {
        private readonly Vector2 _conversionFactor;
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

        public WinInkBasePointer(string name, InputDevice inputDevice, IVirtualScreen screen)
        {
            _screen = screen;
            _conversionFactor = new Vector2(32767, 32767) / new Vector2(screen.Width, screen.Height);
            SharedStore = SharedStore.GetStore(inputDevice, STORE_KEY);
            Instance = SharedStore.GetOrUpdate(INSTANCE, createInstance, out var updated);
            RawPointer = Instance.Pointer;

            if (updated)
            {
                SharedStore.SetOrAdd(POINTER, this);
                SharedStore.SetOrAdd(ERASER_STATE, false);
                SharedStore.SetOrAdd(MANUAL_ERASER, false);
                SharedStore.SetOrAdd(TIP_PRESSED, false);
            }

            VMultiInstance<DigitizerInputReport> createInstance()
            {
                return new VMultiInstance<DigitizerInputReport>(name, new DigitizerInputReport());
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

                if (!SharedStore.Get<bool>(TIP_PRESSED))
                    SetPressure(0);

                if (ForcedSync)
                    SyncOSCursor();
                Instance.Write();
            }
        }

        protected Vector2 Convert(Vector2 pos)
        {
            return pos * _conversionFactor;
        }

        protected void SetInternalPosition(Vector2 pos)
        {
            _internalPos = pos;
        }

        private void SyncOSCursor()
        {
            _osPointer?.SetPosition(_internalPos);
        }

        public abstract void SetPosition(Vector2 pos);

        public void MouseDown(MouseButton button)
        {
        }

        public void MouseUp(MouseButton button)
        {
        }
    }
}