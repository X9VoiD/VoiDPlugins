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
        public Vector2 Filter(Vector2 point)
        {
            if (ReverseCMA)
            {
                if (lastAvg.HasValue)
                {
                    var truePoint = ReverseCMAFunc(point, lastAvg.Value, Window);
                    lastAvg = point;
                    return truePoint;
                }
                else
                {
                    lastAvg = point;
                    return point;
                }
            }
            else if (ReverseMA)
            {
                if (truePoints.IsFilled)
                {
                    var truePoint = ReverseMAFunc(truePoints, point, Window);
                    truePoints.Insert(truePoint);
                    return truePoint;
                }
                else
                {
                    truePoints.Insert(point);
                    return point;
                }
            }
            else
                return point;
        }

        private static Vector2 ReverseMAFunc(IEnumerable<Vector2> trueHistory, Vector2 input, int window)
        {
            Vector2 sum = new Vector2();
            foreach (var item in trueHistory)
                sum += item;

            return (input * window) - sum;
        }

        private static Vector2 ReverseCMAFunc(Vector2 avgCurrent, Vector2 avgLast, int window)
        {
            return (window * (avgCurrent - avgLast)) + avgLast;
        }

        [BooleanProperty("Reverse MA", "Set to True if the tablet is using MA algorithm for noise reduction")]
        [ToolTip(
            "100% reconstruction accuracy when the tablet smoothing algorithm is MA and the window is exactly known\n\n" +
            "Despite its perfect reconstruction accuracy, Reverse MA completely fails when tablet is not using MA + specified window"
        )]
        public bool ReverseMA { get; set; }

        [BooleanProperty("Reverse CMA", "Set to True if the tablet is using CMA algorithm for noise reduction")]
        [ToolTip
        (
            "99.999~% reconstruction accuracy when the tablet smoothing algorithm is CMA and the window is exactly known\n\n" +
            "Better stability and reconstruction accuracy when true tablet smoothing algorithm is unknown\n\n" +
            "Not entirely 100% accurate due to decimal errors in the order of 1x10^-15, which is extremely small."
        )]
        public bool ReverseCMA { get; set; }

        private int window;
        [Property("Window"), ToolTip("Default: 3\n\nDefines in integers how strong the smoothing of the tablet is")]
        public int Window
        {
            set
            {
                window = value;
                truePoints = new RingBuffer<Vector2>(value - 1);
            }
            get => window;
        }

        public FilterStage FilterStage => FilterStage.PreTranspose;

    }
}