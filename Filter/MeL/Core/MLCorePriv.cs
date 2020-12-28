using System.Diagnostics;
using System.Numerics;
using MathNet.Numerics;
using VoiDPlugins.Library;

namespace VoiDPlugins.Filter.MeL.Core
{
    partial class MLCore
    {
        private double[] weights;
        private Polynomial xCoeff, yCoeff;
        private RingBuffer<TimeSeriesPoint> timeSeriesPoints;
        private readonly Stopwatch watch = new();

        private bool AddTimeSeriesPoint(Vector2 point, double elapsed)
        {
            this.timeSeriesPoints.Insert(new TimeSeriesPoint(point, elapsed));
            return this.timeSeriesPoints.IsFilled;
        }

        private double[] ConstructTimeDesignMatrix()
        {
            var baseTime = this.timeSeriesPoints[0].Elapsed;
            var data = new double[Samples];
            var index = 0;
            foreach (var timePoint in this.timeSeriesPoints)
                data[index++] = timePoint.Elapsed - baseTime;

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

        private double[] CalcWeight(double ratio)
        {
            var weights = new double[Samples];
            var weightsNormalized = new double[Samples];
            double weight = 1;
            for (int i = 0; i < Samples; i++)
                weights[i] = weight *= ratio;
            for (int i = 0; i < Samples; i++)
                weightsNormalized[i] = weights[i] / weights[^1];
            return weightsNormalized;
        }
    }
}