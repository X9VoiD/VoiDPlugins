using System.Diagnostics;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using OpenTabletDriver.Plugin;
using VoiDPlugins.Library;

namespace VoiDPlugins.Filter.MeL.Core
{
    internal partial class MLCore
    {
        private double[]? weights;
        private Polynomial? xCoeff, yCoeff;
        private RingBuffer<TimeSeriesPoint>? timeSeriesPoints;
        private readonly Stopwatch watch = new();

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
            predictAhead = TimeNow - this.timeSeriesPoints![0].Elapsed + offset;

            predicted.X = (float)this.xCoeff!.Evaluate(predictAhead);
            predicted.Y = (float)this.yCoeff!.Evaluate(predictAhead);

            return predicted;
        }

        private bool AddTimeSeriesPoint(Vector2 point, double elapsed)
        {
            this.timeSeriesPoints!.Insert(new TimeSeriesPoint(point, elapsed));
            return this.timeSeriesPoints.IsFilled;
        }

        private double[] ConstructTimeDesignMatrix()
        {
            var baseTime = this.timeSeriesPoints![0].Elapsed;
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
                    foreach (var timePoint in timeSeriesPoints!)
                        points[index++] = timePoint.Point.X;
                    break;
                case Axis.Y:
                    foreach (var timePoint in timeSeriesPoints!)
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