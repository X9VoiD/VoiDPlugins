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
                    var truePoint = truePoints.IsFilled ? ReverseMAFunc(truePoints, point, (int)Math.Round(Window)) : point;
                    truePoints.Insert(truePoint);
                    return truePoint;
                }
                case ReconstructionAlg.ReverseCMA:
                {
                    var truePoint = lastAvg.HasValue ? ReverseCMAFunc(point, lastAvg.Value, (int)Math.Round(Window)) : point;
                    lastAvg = point;
                    return truePoint;
                }
                case ReconstructionAlg.ReverseEMA:
                {
                    var truePoint = lastAvg.HasValue ? ReverseEMAFunc(point, lastAvg.Value, Window) : point;
                    lastAvg = point;
                    return truePoint;
                }
                case ReconstructionAlg.ReverseMCMA:
                {
                    var truePoint = lastAvg.HasValue ? ReverseMCMAFunc(point, lastAvg.Value, Window) : point;
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

        private static Vector2 ReverseCMAFunc(Vector2 currentCMA, Vector2 lastCMA, int window)
        {
            return (window * (currentCMA - lastCMA)) + lastCMA;
        }

        private static Vector2 ReverseEMAFunc(Vector2 currentEMA, Vector2 lastEMA, float weight)
        {
            return (currentEMA - (lastEMA * (1 - weight))) / weight;
        }

        private static Vector2 ReverseMCMAFunc(Vector2 currentMCMA, Vector2 lastMCMA, float weight)
        {
            return ((currentMCMA - lastMCMA) / weight) + lastMCMA;
        }

        [BooleanProperty("Reverse MA", "Set to True if the tablet is using MA algorithm for smoothing/noise reduction")]
        [ToolTip(
            "100% reconstruction accuracy when the tablet smoothing algorithm is MA and the window is exactly known\n\n" +
            "Despite its perfect reconstruction accuracy, Reverse MA completely fails when tablet is not using MA + specified window"
        )]
        public bool ReverseMA
        {
            set
            {
                if (value)
                    mode = ReconstructionAlg.ReverseMA;
            }
        }

        [BooleanProperty("Reverse CMA", "Set to True if the tablet is using CMA algorithm for smoothing/noise reduction")]
        [ToolTip
        (
            "99.999~% reconstruction accuracy when the tablet smoothing algorithm is CMA and the window is exactly known\n\n" +
            "Good reconstruction stability when true tablet smoothing algorithm is unknown\n\n" +
            "Not entirely 100% accurate due to decimal errors in the order of 1x10^-15, which is extremely small"
        )]
        public bool ReverseCMA
        {
            set
            {
                if (value)
                    mode = ReconstructionAlg.ReverseCMA;
            }
        }

        [BooleanProperty("Reverse EMA", "Set to True if the tablet is using EMA algorithm for smoothing/noise reduction")]
        [ToolTip
        (
            "99.999~% reconstruction accuracy when the tablet smoothing algorithm is EMA and the weight is exactly known\n\n" +
            "Great reconstruction stability when true tablet smoothing algorithm is unknown, but harder to determine exact weight\n\n" +
            "Not entirely 100% accurate due to decimal errors in the order of 1x10^-15, which is extremely small"
        )]
        public bool ReverseEMA
        {
            set
            {
                if (value)
                    mode = ReconstructionAlg.ReverseEMA;
            }
        }

        [BooleanProperty("Reverse MCMA", "Set to True if the tablet is using MCMA algorithm for smoothing/noise reduction")]
        [ToolTip
        (
            "99.999~% reconstruction accuracy when the tablet smoothing algorithm is CMA and the weight is exactly known\n\n" +
            "Best reconstruction stability when true tablet smoothing algorithm is unknown, but harder to determine exact weight\n\n" +
            "Not entirely 100% accurate due to decimal errors in the order of 1x10^-15, which is extremely small\n\n" +
            "This is the same as CMA, but instead of \"window\" it uses weights (CMA 3 is equivalent to MCMA 1/3)\n" +
            "As a sidenote, MCMA can reverse hawku's smoothing algorithm"
        )]
        public bool ReverseMCMA
        {
            set
            {
                if (value)
                    mode = ReconstructionAlg.ReverseMCMA;
            }
        }

        private float window;
        [Property("Window"), ToolTip
        (
            "Default: 3\n\n" +
            "MA/CMA:\n" +
            "    Defines in integers how many samples is considered. [Range: 2 - n]\n\n" +
            "EMA/MCMA:\n" +
            "    Defines the weight of the latest sample against previous ones [Range: 0.0 - 1.0]"
        )]
        public float Window
        {
            set
            {
                window = value;
                if (window > 1)
                    truePoints = new RingBuffer<Vector2>((int)(Math.Round(value) - 1));
            }
            get => window;
        }

        public FilterStage FilterStage => FilterStage.PreTranspose;
    }
}