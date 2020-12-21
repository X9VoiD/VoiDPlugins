using System.Numerics;

namespace VoiDPlugins.Filter.MeL.Core
{
    public class TimeSeriesPoint
    {
        public TimeSeriesPoint(Vector2 point, double elapsed)
        {
            Point = point;
            Elapsed = elapsed;
        }

        public Vector2 Point { get; }
        public double Elapsed { get; }

    }
}