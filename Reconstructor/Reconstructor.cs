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

        [BooleanProperty("Reverse MA", "Set to True if the tablet is using MA algorithm for smoothing/noise reduction")]
        [ToolTip(
            "100% reconstruction accuracy when the tablet smoothing algorithm is MA and the window is exactly known\n\n" +
            "Despite its perfect reconstruction accuracy, Reverse MA completely fails when tablet is not using MA + specified window"
        )]
        public bool ReverseMA
        {
            set
            {
                if (value == true)
                    mode = ReconstructionAlg.ReverseMA;
            }
        }

        [BooleanProperty("Reverse CMA", "Set to True if the tablet is using CMA algorithm for smoothing/noise reduction")]
        [ToolTip
        (
            "99.999~% reconstruction accuracy when the tablet smoothing algorithm is CMA and the window is exactly known\n\n" +
            "Better reconstruction stability when true tablet smoothing algorithm is unknown\n\n" +
            "Not entirely 100% accurate due to decimal errors in the order of 1x10^-15, which is extremely small"
        )]
        public bool ReverseCMA
        {
            set
            {
                if (value == true)
                    mode = ReconstructionAlg.ReverseCMA;
            }
        }

        [BooleanProperty("Reverse EMA", "Set to True if the tablet is using EMA algorithm for smoothing/noise reduction")]
        [ToolTip
        (
            "99.999~% reconstruction accuracy when the tablet smoothing algorithm is EMA and the window/weight is exactly known\n\n" +
            "Best reconstruction stability when true tablet smoothing algorithm is unknown, but harder to determine exact window/weight\n\n" +
            "Not entirely 100% accurate due to decimal errors in the order of 1x10^-15, which is extremely small"
        )]
        public bool ReverseEMA
        {
            set
            {
                if (value == true)
                    mode = ReconstructionAlg.ReverseEMA;
            }
        }

        private float window;
        [Property("Window"), ToolTip
        (
            "Default: 3\n\n" +
            "ReverseMA/ReverseCMA:\n" +
            "    Defines in integers how strong the smoothing of the tablet is. [Range: 2 - n]\n\n" +
            "ReverseEMA:\n" +
            "    Defines the weight of the latest sample against previous one. [Range: 0.0 - 1.0]"
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