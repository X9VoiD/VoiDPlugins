using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Timing;

namespace VoiDPlugins.Filter
{
    [PluginName("ReconstructorV2")]
    public class ReconstructorV2 : IPositionedPipelineElement<IDeviceReport>
    {
        private Vector2? lastSample;
        private HPETDeltaStopwatch watch = new();
        private float rc;

        [Property("RC value"), DefaultPropertyValue(0.5f), ToolTip
        (
            "Default: 0.5\n\n" +
            "Defines the RC constant of the low-pass filter to inverse\n" +
            "  Higher == More hardware smoothing removed"
        )]
        public float RCValue
        {
            set => rc = Math.Clamp(value, 0.0001f, 1000f);
            get => rc;
        }

        public event Action<IDeviceReport> Emit;

        public PipelinePosition Position => PipelinePosition.PreTransform;

        public void Consume(IDeviceReport value)
        {
            if (value is ITabletReport report)
            {
                var deltaTime = (float)watch.Restart().TotalMilliseconds;
                if (deltaTime < 100f)
                {
                    var alpha = CalcAlpha(RCValue, deltaTime);
                    var truePoint = lastSample.HasValue ? LowPassInverse(report.Position, lastSample.Value, alpha) : report.Position;
                    lastSample = report.Position;
                    report.Position = truePoint;
                    value = report;
                }
                else
                {
                    lastSample = report.Position;
                }
            }

            Emit?.Invoke(value);
        }

        private static Vector2 LowPassInverse(Vector2 currentSample, Vector2 lastSample, float alpha)
        {
            return ((currentSample - lastSample) / alpha) + lastSample;
        }

        private static float CalcAlpha(float RCValue, float deltaTime)
        {
            return deltaTime / (RCValue + deltaTime);
        }
    }
}