using System.Numerics;

namespace VoiDPlugins.Filter
{
    public class TimeSeriesPoint
    {
        public TimeSeriesPoint(Vector2 point, double elapsed)
        {
            Point = point;
            Elapsed = elapsed;
        }

        public readonly Vector2 Point;
        public double Elapsed;
    }
}