using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using HidSharp;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.Library.VMulti
{
    [PluginIgnore]
    public abstract unsafe class BasePointer<T> : IAbsolutePointer, IRelativePointer where T : unmanaged
    {
        protected readonly byte[] ReportBuffer;
        protected readonly T* ReportPointer;
        protected readonly HidStream Device;
        protected IVirtualScreen VirtualScreen;

        private Vector2 ScreenToVMulti;
        private Vector2 error;

        public BasePointer(IVirtualScreen screen, string Name)
        {
            ReportBuffer = GC.AllocateArray<byte>(Unsafe.SizeOf<T>(), pinned: true);
            ReportPointer = (T*)Unsafe.AsPointer(ref ReportBuffer[0]);

            T report = CreateReport();
            *ReportPointer = report;

            VirtualScreen = screen;
            Device = VMultiDevice.Retrieve(Name);

            ScreenToVMulti = new Vector2(VirtualScreen.Width, VirtualScreen.Height) / 32767;
        }

        protected Vector2 Convert(Vector2 pos)
        {
            return pos / ScreenToVMulti;
        }

        protected abstract T CreateReport();
        protected abstract void SetCoordinates(Vector2 pos);

        public virtual void SetPosition(Vector2 pos)
        {
            SetCoordinates(Convert(pos));
            Device.Write(ReportBuffer);
        }

        public virtual void Translate(Vector2 delta)
        {
            delta += error;
            error = new Vector2(delta.X % 1, delta.Y % 1);

            SetCoordinates(delta);
            Device.Write(ReportBuffer);
        }
    }
}