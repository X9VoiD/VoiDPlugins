using System.Numerics;
using HidSharp;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Platform.Display;
using OpenTabletDriver.Plugin.Platform.Pointer;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.Library.VMulti
{
    [PluginIgnore]
    public abstract class BasePointer<T> : IAbsolutePointer where T : Report, new()
    {
        public T Report;
        protected readonly HidStream Device;

        private Vector2 ScreenToVMulti;

        public BasePointer(byte ReportID, string Name)
        {
            Report = new T
            {
                ReportID = ReportID
            };

            Device = VMultiDevice.Retrieve(Name);

            var VirtualScreen = (Info.Driver as IVirtualDisplayDriver).VirtualScreen;
            ScreenToVMulti = new Vector2(VirtualScreen.Width, VirtualScreen.Height) / 32767;
        }

        protected Vector2 Convert(Vector2 pos)
        {
            return pos / ScreenToVMulti;
        }

        public abstract void SetPosition(Vector2 pos);
    }
}