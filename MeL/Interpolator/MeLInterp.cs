#if false

using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Tablet.Interpolator;
using OpenTabletDriver.Plugin.Timers;
using OTDPlugins.MeL.Core;

namespace OTDPlugins.MeL
{
    [PluginName("MeL")]
    public class MeLInterp : Interpolator
    {
        public MeLInterp(ITimer scheduler) : base(scheduler)
        {
        }

        public override void UpdateState(ITabletReport report)
        {
            SyntheticReport = new SyntheticTabletReport(report);
            Core.Add(report.Position);
        }

        public override ITabletReport Interpolate()
        {
            if (Core.IsReady)
            {
                try
                {
                    SyntheticReport.Position = Core.Predict(Core.TimeNow, 0);
                }
                catch
                {
                    Log.Write("MeLInterp", "Unknown error in MeLCore");
                }
            }
            return SyntheticReport;
        }

        [Property("Samples")]
        public int Samples { set => Core.Samples = value; }

        [Property("Complexity")]
        public int Complexity { set => Core.Complexity = value; }

        [Property("Weight")]
        public float Weight { set => Core.Weight = value; }

        private readonly MLCore Core = new MLCore();
        private SyntheticTabletReport SyntheticReport;
    }
}

#endif