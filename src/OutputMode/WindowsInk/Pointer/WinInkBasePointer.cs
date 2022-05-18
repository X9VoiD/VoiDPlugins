using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using VoiDPlugins.Library.VoiD;
using static VoiDPlugins.OutputMode.WindowsInkConstants;

namespace VoiDPlugins.OutputMode
{
    public unsafe abstract class WinInkBasePointer : IPressureHandler, ITiltHandler, IEraserHandler, ISynchronousPointer
    {
        protected DigitizerInputReport* RawPointer { get; }
        protected VMultiInstance<DigitizerInputReport>? Instance { get; }
        protected SharedStore? SharedStore { get; }
        protected bool Dirty { get; set; }

        public WinInkBasePointer(string name, TabletReference tabletReference)
        {
            Instance = new VMultiInstance<DigitizerInputReport>(name, new DigitizerInputReport());
            SharedStore = GlobalStore<SharedStore>.Set(tabletReference, () => new SharedStore());
            SharedStore.InitializeData(INSTANCE, Instance);
            SharedStore.InitializeData(POINTER, this);
            SharedStore.InitializeData(ERASER_STATE, new Boxed<bool>(false));
            SharedStore.InitializeData(MANUAL_ERASER, new Boxed<bool>(false));
            RawPointer = Instance.Pointer;
        }

        public void SetEraser(bool isEraser)
        {
            if (!SharedStore!.GetData<Boxed<bool>>(MANUAL_ERASER).Value)
            {
                WindowsInkButtonHandler.EraserStateTransition(Instance!, ref GetEraser(), isEraser);
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
        }

        public void Flush()
        {
            if (Dirty)
            {
                Dirty = false;
                Instance!.Write();
            }
        }

        private ref Boxed<bool> GetEraser()
        {
            return ref SharedStore!.GetData<Boxed<bool>>(ERASER_STATE);
        }
    }
}