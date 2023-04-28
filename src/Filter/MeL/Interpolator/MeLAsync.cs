using System.ComponentModel;
using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Output;
using OpenTabletDriver.Tablet;
using VoiDPlugins.Filter.MeL.Core;

namespace VoiDPlugins.Filter.MeL
{
    [PluginName("MeL Async")]
    public class MeLAsync : AsyncDevicePipelineElement
    {
        private readonly MLCore Core = new MLCore();
        private readonly HPETDeltaStopwatch watch = new HPETDeltaStopwatch();
        private float? reportMsAvg;

        public MeLAsync(ISettingsProvider settingsProvider, ITimer scheduler) : base(scheduler)
        {
            settingsProvider.Inject(this);
        }

        public override PipelinePosition Position => PipelinePosition.PostTransform;

        [Setting("Samples"), DefaultValue(20)]
        public int Samples { set => Core.Samples = value; }

        [Setting("Complexity"), DefaultValue(2)]
        public int Complexity { set => Core.Complexity = value; }

        [Setting("Weight"), DefaultValue(1.4f)]
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