using System;
using System.Numerics;

namespace OTDPlugins.MeL.Core
{
    public class TimeSeriesPoint
    {
        public TimeSeriesPoint(Vector2 point, DateTime date)
        {
            Point = point;
            Date = date;
        }

        public Vector2 Point { get; }
        public DateTime Date { get; }

    }
}