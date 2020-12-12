using System;
using System.Collections.Generic;
using System.Linq;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using VoiDPlugins.Library.VMulti.Device;

namespace VoiDPlugins.Library.VMulti
{
    [PluginIgnore]
    public abstract class ButtonHandler : IBinding, IValidateBinding
    {
        protected static Report Report;
        public abstract Dictionary<string, int> Bindings { get; }

        [Property("Property")]
        public string Property { get; set; }
        public virtual string[] ValidProperties => Bindings.Keys.ToArray();

        Action IBinding.Press => () => Press(Property);
        Action IBinding.Release => () => Release(Property);

        public virtual void Press(string input)
        {
            EnableBit(Bindings[input]);
        }

        public virtual void Release(string input)
        {
            DisableBit(Bindings[input]);
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