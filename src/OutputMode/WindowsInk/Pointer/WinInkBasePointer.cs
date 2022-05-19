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
        private ThinVMultiAbsPointer? _osPointer;
        protected DigitizerInputReport* RawPointer { get; }
        protected VMultiInstance<DigitizerInputReport>? Instance { get; }
        protected SharedStore? SharedStore { get; }
        protected bool Dirty { get; set; }

        [Property("Sync")]
        [ToolTip("Synchronize OS cursor with Windows Ink's current position when pen goes out of range.")]
        [DefaultPropertyValue(true)]
        public bool Sync
        {
            set => _osPointer = value ? new ThinVMultiAbsPointer(_screen) : null;
        }

        [Property("Forced Sync")]
        [ToolTip("If this and \"Sync\" is enabled, the OS cursor will always be resynced with Windows Ink's current position.")]
        [DefaultPropertyValue(false)]
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
                Instance = null;
                SharedStore = null;
                RawPointer = null;
            }
        }

        public void SetEraser(bool isEraser)
        {
            if (!SharedStore?.Get<bool>(MANUAL_ERASER) ?? false)
            {
                WindowsInkButtonHandler.EraserStateTransition(SharedStore!, Instance!, isEraser);
            }
        }

        public void SetPressure(float percentage)
        {
            if (RawPointer != null)
                RawPointer->Pressure = (ushort)(percentage * 8191);
        }

        public void SetTilt(Vector2 tilt)
        {
            if (RawPointer != null)
            {
                RawPointer->XTilt = (byte)tilt.X;
                RawPointer->YTilt = (byte)tilt.Y;
            }
        }

        public void Reset()
        {
            if (RawPointer is not null && _osPointer is not null && !ForcedSync)
            {
                _osPointer.SetPosition(new Vector2(RawPointer->X, RawPointer->Y));
            }
        }

        public void Flush()
        {
            if (RawPointer is not null && Dirty)
            {
                Dirty = false;

                if (ForcedSync)
                    _osPointer?.SetPosition(new Vector2(RawPointer->X, RawPointer->Y));

                Instance!.Write();
            }
        }
    }
}