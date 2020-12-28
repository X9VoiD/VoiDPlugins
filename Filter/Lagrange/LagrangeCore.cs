using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using VoiDPlugins.Library;

namespace VoiDPlugins.Filter
{
    public class LagrangeCore
    {
        private RingBuffer<TimeSeriesPoint> Points;
        private readonly Stopwatch Watch = new();
        private float? prevElapsed;
        public int Samples { set => Points = new(value); }
        public bool Jitter;
        public bool IsFilled { get => Points.IsFilled; }
        public float TimeNow { get => (float)Watch.Elapsed.TotalMilliseconds; }
        public float ReportInterval = 4;

        public LagrangeCore()
        {
            Watch.Start();
        }

        ~LagrangeCore()
        {
            Watch.Stop();
        }

        public void Add(Vector2 point)
        {
            var now = TimeNow;
            Points.Insert(new TimeSeriesPoint(point, now));
            if (prevElapsed.HasValue)
            {
                var interval = now - prevElapsed;
                if (interval < ReportInterval * 1.5)
                    ReportInterval += (float)((interval - ReportInterval) * 0.1);
            }
            prevElapsed = now;
        }

        public Vector2 Predict(float offset = 0)
        {
            Vector2 lagrange = new(0, 0);
            var i = 0;
            foreach (var z in Points)
            {
                lagrange += z.Point * Decompose(TimeNow + offset, (float)z.Elapsed, i);
                i++;
            }

            return lagrange;
        }

        private float Decompose(float interpTime, float time, int decompIndex)
        {
            float decomposed = 1;
            int i = 0;
            foreach (var point in Points)
            {
                if (i != decompIndex)
                {
                    decomposed *= (float)((interpTime - point.Elapsed) / (time - point.Elapsed));
                }
                i++;
            }
            return decomposed;
        }
    }
}