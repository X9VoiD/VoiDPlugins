using System;
using System.Collections.Generic;
using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins
{
    [PluginName("Reconstructor")]
    public class Reconstructor : IFilter
    {
        private RingBuffer<Vector2> truePoints;
        private Vector2? lastAvg;
        private ReconstructionAlg mode;

        public Vector2 Filter(Vector2 point)
        {
            switch (mode)
            {
                case ReconstructionAlg.ReverseMA:
                {
                    var truePoint = truePoints.IsFilled ? ReverseMAFunc(truePoints, point, MAWindow) : point;
                    truePoints.Insert(truePoint);
                    return truePoint;
                }
                case ReconstructionAlg.ReverseEMA:
                {
                    var truePoint = lastAvg.HasValue ? ReverseEMAFunc(point, lastAvg.Value, (float)EMAWeight) : point;
                    lastAvg = point;
                    return truePoint;
                }
                default:
                    return point;
            }
        }

        private static Vector2 ReverseMAFunc(IEnumerable<Vector2> trueHistory, Vector2 input, int window)
        {
            Vector2 sum = new Vector2();
            foreach (var item in trueHistory)
                sum += item;

            return (input * window) - sum;
        }

        private static Vector2 ReverseEMAFunc(Vector2 currentEMA, Vector2 lastEMA, float weight)
        {
            return ((currentEMA - lastEMA) / weight) + lastEMA;
        }

        [BooleanProperty("Reverse MA", "Set to True if the tablet is using MA algorithm for smoothing/noise reduction")]
        [ToolTip(
            "100% reconstruction accuracy when the tablet smoothing algorithm is MA and the window is exactly known\n\n" +
            "Warning: Reverse MA completely fails when tablet is not using MA + specified window"
        )]
        public bool ReverseMA
        {
            set
            {
                if (value)
                    mode = ReconstructionAlg.ReverseMA;
            }
        }

        [BooleanProperty("Reverse EMA", "Set to True if the tablet is using EMA algorithm for smoothing/noise reduction")]
        [ToolTip
        (
            "100% reconstruction accuracy when the tablet smoothing algorithm is EMA and the weight is exactly known\n\n" +
            "Great reconstruction stability when true tablet smoothing algorithm is unknown, but determining exact weight is hard\n\n" +
            "As a sidenote, hawku uses EMA for his smoothing filter"
        )]
        public bool ReverseEMA
        {
            set
            {
                if (value)
                    mode = ReconstructionAlg.ReverseEMA;
            }
        }

        [Property("MA Window"), ToolTip
        (
            "Default: 3\n\n" +
            "Defines in integers how many samples is considered. [Range: 2 - n]"
        )]
        public int MAWindow
        {
            set
            {
                window = value;
                if (window > 1)
                    truePoints = new RingBuffer<Vector2>(value - 1);
            }
            get => window;
        }

        [Property("EMA Weight"), ToolTip
        (
            "Default: 0.35\n\n" +
            "Defines the weight of the latest sample against previous ones [Range: 0.0 - 1.0]"
        )]
        public double EMAWeight
        {
            set
            {
                weight = Math.Clamp(value, 0, 1);
            }
            get => weight;
        }

        private int window;
        private double weight;

        public FilterStage FilterStage => FilterStage.PreTranspose;
    }
}