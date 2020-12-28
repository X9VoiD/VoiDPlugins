using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.Filter
{
    [PluginName("Reconstructor")]
    public class Reconstructor : IFilter
    {
        private Vector2? lastAvg;

        public Vector2 Filter(Vector2 point)
        {
            var truePoint = lastAvg.HasValue ? ReverseEMAFunc(point, lastAvg.Value, (float)EMAWeight) : point;
            lastAvg = point;
            return truePoint;
        }

        private static Vector2 ReverseEMAFunc(Vector2 currentEMA, Vector2 lastEMA, float weight)
        {
            return ((currentEMA - lastEMA) / weight) + lastEMA;
        }

        [Property("EMA Weight"), ToolTip
        (
            "Default: 0.35\n\n" +
            "Defines the weight of the latest sample against previous ones [Range: 0.0 - 1.0]\n" +
            "  Lower == More hardware smoothing removed\n" +
            "  1 == No effect"
        )]
        public double EMAWeight
        {
            set
            {
                weight = Math.Clamp(value, 0, 1);
            }
            get => weight;
        }
        private double weight;

        public FilterStage FilterStage => FilterStage.PreInterpolate;
    }
}