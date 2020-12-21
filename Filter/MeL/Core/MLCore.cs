using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using OpenTabletDriver.Plugin;
using VoiDPlugins.Library;

namespace VoiDPlugins.Filter.MeL.Core
{
    internal partial class MLCore
    {
        public MLCore()
        {
            Weight = 1.4f;
            watch.Start();
        }

        ~MLCore()
        {
            watch.Stop();
        }

        public void Add(Vector2 point)
        {
            Add(point, TimeNow);
        }

        public void Add(Vector2 point, double elapsed)
        {
            if (AddTimeSeriesPoint(point, elapsed))
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

        public Vector2 Predict(double offset = 0)
        {
            var predicted = new Vector2();
            double predictAhead;
            predictAhead = TimeNow - this.timeSeriesPoints.PeekFirst().Elapsed + offset;

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
        public double TimeNow { get => watch.Elapsed.TotalMilliseconds; }
    }
}