using System.Collections.Generic;
using System.Linq;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.Library.VMulti
{
    [PluginIgnore]
    public abstract class ButtonHandler : IBinding
    {
        protected static Report Report;
        public abstract Dictionary<string, int> Bindings { get; }

        [Property("Property"), PropertyValidated(nameof(ValidProperties))]
        public string Property { get; set; }
        public virtual string[] ValidProperties => Bindings.Keys.ToArray();

        public virtual void Press(IDeviceReport report)
        {
            EnableBit(Bindings[Property]);
        }

        public virtual void Release(IDeviceReport report)
        {
            DisableBit(Bindings[Property]);
        }

        public static void SetReport(Report report)
        {
            Report = report;
        }

        public static void EnableBit(int bit)
        {
            Report.Buttons = (byte)(Report.Buttons | bit);
        }

        public static void DisableBit(int bit)
        {
            Report.Buttons = (byte)(Report.Buttons & ~bit);
        }
    }
}