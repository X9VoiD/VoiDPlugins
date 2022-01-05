using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using static VoiDPlugins.OutputMode.WindowsInkConstants;

namespace VoiDPlugins.OutputMode
{
    public unsafe abstract class WinInkBasePointer : IPressureHandler, ITiltHandler, IEraserHandler, ISynchronousPointer
    {
        protected DigitizerInputReport* RawPointer { get; private set; }
        protected VMultiInstance<DigitizerInputReport>? Instance { get; private set; }

        public void Initialize(TabletReference tabletReference)
        {
            Instance = VMultiInstanceManager.RetrieveVMultiInstance("WindowsInk", tabletReference, () => new DigitizerInputReport());
            Instance.InitializeData(POINTER, this);
            Instance.InitializeData(ERASER_STATE, false);
            Instance.InitializeData(MANUAL_ERASER, false);
            RawPointer = Instance.Pointer;
        }

        public void SetEraser(bool isEraser)
        {
            if (!Instance!.GetData<bool>(MANUAL_ERASER))
            {
                WinInkButtonHandler.EraserStateTransition(Instance, ref GetEraser(), isEraser);
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
            Instance!.Write();
        }

        private ref bool GetEraser()
        {
            return ref Instance!.GetData<bool>(ERASER_STATE);
        }
    }
}