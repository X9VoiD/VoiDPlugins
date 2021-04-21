using System.Numerics;
using HidSharp;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.Library.VMulti
{
    [PluginIgnore]
    public abstract class BasePointer<T> : IAbsolutePointer, IRelativePointer where T : Report, new()
    {
        public T Report;
        protected readonly HidStream Device;
        protected IVirtualScreen VirtualScreen;

        private Vector2 ScreenToVMulti;
        private Vector2 error;

        public BasePointer(IVirtualScreen screen, byte ReportID, string Name)
        {
            Report = new T
            {
                ReportID = ReportID
            };

            VirtualScreen = screen;
            Device = VMultiDevice.Retrieve(Name);

            ScreenToVMulti = new Vector2(VirtualScreen.Width, VirtualScreen.Height) / 32767;
        }

        protected Vector2 Convert(Vector2 pos)
        {
            return pos / ScreenToVMulti;
        }

        public virtual void SetPosition(Vector2 pos)
        {
            Report.SetCoordinates(Convert(pos));
            Device.Write(Report.ToBytes());
        }

        public virtual void Translate(Vector2 delta)
        {
            delta += error;
            error = new Vector2(delta.X % 1, delta.Y % 1);

            Report.SetCoordinates(delta);
            Device.Write(Report.ToBytes());
        }
    }
}