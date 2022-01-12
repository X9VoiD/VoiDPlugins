using System.Numerics;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library;
using VoiDPlugins.Library.VMulti;
using VoiDPlugins.Library.VMulti.Device;
using static VoiDPlugins.OutputMode.WindowsInkConstants;

namespace VoiDPlugins.OutputMode
{
    public unsafe abstract class WinInkBasePointer : IPressureHandler, ITiltHandler, IEraserHandler, ISynchronousPointer
    {
        protected DigitizerInputReport* RawPointer { get; }
        protected VMultiInstance<DigitizerInputReport>? Instance { get; }

        public WinInkBasePointer(TabletReference tabletReference)
        {
            Instance = VMultiInstanceManager.RetrieveVMultiInstance("WindowsInk", tabletReference, () => new DigitizerInputReport());
            Instance.InitializeData(POINTER, this);
            Instance.InitializeData(ERASER_STATE, new Boxed<bool>(false));
            Instance.InitializeData(MANUAL_ERASER, new Boxed<bool>(false));
            RawPointer = Instance.Pointer;
        }

        public void SetEraser(bool isEraser)
        {
            if (!Instance!.GetData<Boxed<bool>>(MANUAL_ERASER).Value)
            {
                WindowsInkButtonHandler.EraserStateTransition(Instance, ref GetEraser(), isEraser);
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
            Instance!.DisableButtonBit((int)WindowsInkButtonFlags.InRange);
        }

        public void Flush()
        {
            Instance!.Write();
        }

        private ref Boxed<bool> GetEraser()
        {
            return ref Instance!.GetData<Boxed<bool>>(ERASER_STATE);
        }
    }
}