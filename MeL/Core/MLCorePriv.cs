using System;
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics;

namespace OTDPlugins.MeL.Core
{
    partial class MLCore
    {
        private double[] weights;
        private Polynomial xCoeff, yCoeff;
        private readonly LinkedList<TimeSeriesPoint> timeSeriesPoints = new LinkedList<TimeSeriesPoint>();

        private bool AddTimeSeriesPoint(Vector2 point, DateTime time)
        {
            this.timeSeriesPoints.AddLast(new TimeSeriesPoint(point, time));
            if (this.timeSeriesPoints.Count > Samples)
                this.timeSeriesPoints.RemoveFirst();
            return this.timeSeriesPoints.Count == Samples;
        }

        private double[] ConstructTimeDesignMatrix()
        {
            var baseTime = this.timeSeriesPoints.First.Value.Date;
            var data = new double[Samples];
            var index = 0;
            foreach (var timePoint in this.timeSeriesPoints)
                data[index++] = (timePoint.Date - baseTime).TotalMilliseconds;

            return data;
        }

        private double[] ConstructTargetMatrix(Axis axis)
        {
            var points = new double[Samples];
            var index = 0;

            switch (axis)
            {
                case Axis.X:
                    foreach (var timePoint in timeSeriesPoints)
                        points[index++] = timePoint.Point.X;
                    break;
                case Axis.Y:
                    foreach (var timePoint in timeSeriesPoints)
                        points[index++] = timePoint.Point.Y;
                    break;
            }

            return points;
        }

        private double[] CalcWeight(int samples, double ratio)
        {
            var weights = new List<double>();
            var weightsNormalized = new List<double>();
            double weight = 1;
            for (int i = 0; i < samples; i++)
                weights.Add(weight *= ratio);
            foreach (var _weight in weights)
                weightsNormalized.Add(_weight / weights[^1]);
            return weightsNormalized.ToArray();
        }
    }
}