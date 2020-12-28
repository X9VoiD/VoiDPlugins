using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet.Interpolator;
using OpenTabletDriver.Plugin.Timers;

namespace VoiDPlugins.Filter
{
    [PluginName("Lagrange")]
    public class Lagrange : Interpolator
    {
        private readonly LagrangeCore Core = new();

        private SyntheticTabletReport Report;

        public Lagrange(ITimer timer) : base(timer)
        {
        }

        public override SyntheticTabletReport Interpolate()
        {
            if (Core.IsFilled)
                Report.Position = Core.Predict();
            return Report;
        }

        public override void UpdateState(SyntheticTabletReport report)
        {
            Report = report;
            Core.Add(report.Position);
        }

        [Property("Samples")]
        public int Samples { set => Core.Samples = value; }

        [Property("Offset"), Unit("ms")]
        public float Offset { get; set; }

        [BooleanProperty("Mode 2", "Enable if tablet report is jittery. (most of 266+hz tablets)")]
        public bool Jitter { set => Core.Jitter = value; }
    }
}
