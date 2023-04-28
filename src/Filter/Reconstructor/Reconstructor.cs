using System;
using System.ComponentModel;
using System.Numerics;
using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Output;
using OpenTabletDriver.Tablet;

namespace VoiDPlugins.Filter
{
    [PluginName("Reconstructor")]
    public class Reconstructor : IDevicePipelineElement
    {
        private Vector2? lastAvg;
        private float weight;

        [Setting("EMA Weight"), DefaultValue(0.5f), ToolTip
        (
            "Default: 0.5\n\n" +
            "Defines the weight of the latest sample against previous ones [Range: 0.0 - 1.0]\n" +
            "  Lower == More hardware smoothing removed\n" +
            "  1 == No effect"
        )]
        public float EMAWeight
        {
            set => weight = Math.Clamp(value, 0, 1);
            get => weight;
        }

        public event Action<IDeviceReport>? Emit;

        public PipelinePosition Position => PipelinePosition.PreTransform;

        public Reconstructor(ISettingsProvider settingsProvider)
        {
            settingsProvider.Inject(this);
        }

        public void Consume(IDeviceReport value)
        {
            if (value is ITabletReport report)
            {
                var truePoint = lastAvg.HasValue ? ReverseEMAFunc(report.Position, lastAvg.Value, (float)EMAWeight) : report.Position;
                lastAvg = report.Position;
                report.Position = truePoint;
                value = report;
            }

            Emit?.Invoke(value);
        }

        private static Vector2 ReverseEMAFunc(Vector2 currentEMA, Vector2 lastEMA, float weight)
        {
            return ((currentEMA - lastEMA) / weight) + lastEMA;
        }
    }
}