using System;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet.Interpolator;
using OpenTabletDriver.Plugin.Timers;
using VoiDPlugins.Filter.MeL.Core;

namespace VoiDPlugins.Filter.MeL
{
    [PluginName("MeL")]
    public class MeLInterp : Interpolator
    {
        public MeLInterp(ITimer scheduler) : base(scheduler)
        {
        }

        public override void UpdateState(SyntheticTabletReport report)
        {
            var now = DateTime.UtcNow;
            var reportMs = (now - lastTime).TotalMilliseconds;
            reportMsAvg += (reportMs - reportMsAvg) / 50.0;
            lastTime = now;
            SyntheticReport = new SyntheticTabletReport(report);
            if (reportMs > 100)
                reportMsAvg = 4;
            if (reportMs >= reportMsAvg * 0.75)
                Core.Add(report.Position);
        }

        public override SyntheticTabletReport Interpolate()
        {
            if (Core.IsReady)
            {
                try
                {
                    SyntheticReport.Position = Core.Predict();
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
        private new DateTime lastTime;
        private new double reportMsAvg = 4.0;
    }
}