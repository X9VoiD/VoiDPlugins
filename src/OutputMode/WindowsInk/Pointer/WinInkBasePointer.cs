using System.Numerics;
using OpenTabletDriver.Plugin;
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
        protected VMultiInstance<DigitizerInputReport> Instance { get; }
        protected SharedStore SharedStore { get; }
        protected bool Dirty { get; set; }

        public WinInkBasePointer(string name, TabletReference tabletReference)
        {
            Log.Write("WinInk", "Initializing pointer");
            Instance = new VMultiInstance<DigitizerInputReport>(name, new DigitizerInputReport());
            SharedStore = SharedStore.GetStore(tabletReference, STORE_KEY);
            SharedStore.Add(INSTANCE, Instance);
            SharedStore.Add(POINTER, this);
            SharedStore.Add(ERASER_STATE, false);
            SharedStore.Add(MANUAL_ERASER, false);
            RawPointer = Instance.Pointer;
            Log.Write("WinInk", "Pointer initialized");
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
        }

        public void Flush()
        {
            Log.Write("WinInk", $"Flush({Dirty})", LogLevel.Debug);
            if (Dirty)
            {
                Dirty = false;
                Instance!.Write();
            }
        }
    }
}