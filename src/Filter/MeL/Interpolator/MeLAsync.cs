using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Timing;
using VoiDPlugins.Filter.MeL.Core;

namespace VoiDPlugins.Filter.MeL
{
    [PluginName("MeL Async")]
    public class MeLAsync : AsyncPositionedPipelineElement<IDeviceReport>
    {
        private readonly MLCore Core = new MLCore();
        private readonly HPETDeltaStopwatch watch = new HPETDeltaStopwatch();
        private float? reportMsAvg;

        public override PipelinePosition Position => PipelinePosition.PostTransform;

        [Property("Samples"), DefaultPropertyValue(20)]
        public int Samples { set => Core.Samples = value; }

        [Property("Complexity"), DefaultPropertyValue(2)]
        public int Complexity { set => Core.Complexity = value; }

        [Property("Weight"), DefaultPropertyValue(1.4f)]
        public float Weight { set => Core.Weight = value; }

        protected override void ConsumeState()
        {
            if (State is ITabletReport report)
            {
                var reportMs = (float)watch.Restart().TotalMilliseconds;

                if (reportMs < 50f)
                    reportMsAvg = (reportMsAvg + ((reportMs - reportMsAvg) / 50.0f)) ?? reportMs;

                if (reportMs >= reportMsAvg * 0.75f)
                    Core.Add(report.Position);
            }
        }

        protected override void UpdateState()
        {
            if (State is ITabletReport report && Core.IsReady)
            {
                try
                {
                    report.Position = Core.Predict();
                    State = report;
                }
                catch
                {
                    Log.Write("MeLInterp", "Unknown error in MeLCore");
                }
            }

            if (PenIsInRange() || State is not ITabletReport)
            {
                OnEmit();
            }
        }
    }
}