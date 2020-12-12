using System;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using OpenTabletDriver.Plugin;

namespace VoiDPlugins.Filter.MeL.Core
{
    internal partial class MLCore
    {
        public MLCore()
        {
            Weight = 1.4f;
        }

        public void Add(Vector2 point)
        {
            Add(point, MLCore.TimeNow);
        }

        public void Add(Vector2 point, DateTime time)
        {
            if (AddTimeSeriesPoint(point, time))
            {
                IsReady = true;
                var timeMatrix = ConstructTimeDesignMatrix();
                var x = ConstructTargetMatrix(Axis.X);
                var y = ConstructTargetMatrix(Axis.Y);
                try
                {
                    this.xCoeff = new Polynomial(Fit.PolynomialWeighted(timeMatrix, x, this.weights, Complexity));
                    this.yCoeff = new Polynomial(Fit.PolynomialWeighted(timeMatrix, y, this.weights, Complexity));
                }
                catch
                {
                    Log.Write("MLCore", "Error in calculation", LogLevel.Error);
                    IsReady = false;
                }
            }
        }

        public Vector2 Predict(DateTime now, float offset)
        {
            var predicted = new Vector2();
            double predictAhead;
            predictAhead = (now - this.timeSeriesPoints.PeekFirst().Date).TotalMilliseconds + offset;

            predicted.X = (float)this.xCoeff.Evaluate(predictAhead);
            predicted.Y = (float)this.yCoeff.Evaluate(predictAhead);

            return predicted;
        }

        public bool IsReady { private set; get; }

        private int samples;
        public int Samples
        {
            set
            {
                this.timeSeriesPoints = new RingBuffer<TimeSeriesPoint>(value);
                this.samples = value;
            }
            get => this.samples;
        }

        public int Complexity { set; get; } = 2;
        public float Weight { set => this.weights = CalcWeight(value).ToArray(); }
        public static DateTime TimeNow { get => DateTime.UtcNow; }
    }
}