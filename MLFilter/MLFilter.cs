using MLFilter;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;

namespace OTDPlugins
{
    [PluginName("MLFilter")]
    public class MLFilter : Notifier, IFilter
    {
        public virtual Vector2 Filter(Vector2 point)
        {
            DateTime date = DateTime.Now;

            if (AddTimeSeriesPoint(point, date))
            {
                var predicted = new Vector2();
                var timeMatrix = ConstructTimeDesignMatrix();
                double[] x, y;
                if (Normalize)
                {
                    x = ConstructNormalizedTargetMatrix(Axis.X);
                    y = ConstructNormalizedTargetMatrix(Axis.Y);
                }
                else
                {
                    x = ConstructTargetMatrix(Axis.X);
                    y = ConstructTargetMatrix(Axis.Y);
                }

                Polynomial xCoeff, yCoeff;
                var weights = CalcWeight(Weight);
                try
                {
                    xCoeff = new Polynomial(Fit.PolynomialWeighted(timeMatrix, x, weights, Degree));
                    yCoeff = new Polynomial(Fit.PolynomialWeighted(timeMatrix, y, weights, Degree));
                }
                catch
                {
                    Log.Write("MLFilter", "Error in calculation", LogLevel.Error);
                    return point;
                }

                double predictAhead;
                predictAhead = (date - _timeSeriesPoints.First.Value.Date).TotalMilliseconds + Offset;

                predicted.X = (float)xCoeff.Evaluate(predictAhead);
                predicted.Y = (float)yCoeff.Evaluate(predictAhead);
                if (Normalize)
                {
                    predicted.X *= ScreenWidth;
                    predicted.Y *= ScreenHeight;
                }

                _lastTime = date;
                return predicted;
            }
            _lastTime = date;
            return point;
        }

        private bool AddTimeSeriesPoint(Vector2 point, DateTime time)
        {
            _timeSeriesPoints.AddLast(new TimeSeriesPoint(point, time));
            if (_timeSeriesPoints.Count > Samples)
                _timeSeriesPoints.RemoveFirst();
            if (_timeSeriesPoints.Count == Samples)
                return true;
            return false;
        }

        private double[] ConstructTimeDesignMatrix()
        {
            DateTime baseTime = _timeSeriesPoints.First.Value.Date;

            var data = new double[Samples];
            var index = -1;
            foreach (var timePoint in _timeSeriesPoints)
            {
                ++index;
                data[index] = (timePoint.Date - baseTime).TotalMilliseconds;
            }

            return data;
        }

        private double[] ConstructTargetMatrix(Axis axis)
        {
            var points = new double[Samples];
            var index = -1;

            if (axis == Axis.X)
                foreach (var timePoint in _timeSeriesPoints)
                    points[++index] = timePoint.Point.X;

            else if (axis == Axis.Y)
                foreach (var timePoint in _timeSeriesPoints)
                    points[++index] = timePoint.Point.Y;

            return points;
        }

        private double[] ConstructNormalizedTargetMatrix(Axis axis)
        {
            var points = new double[Samples];
            var index = -1;

            if (axis == Axis.X)
            {
                foreach (var timePoint in _timeSeriesPoints)
                {
                    points[++index] = timePoint.Point.X / ScreenWidth;
                }
            }
            else if (axis == Axis.Y)
            {
                foreach (var timePoint in _timeSeriesPoints)
                {
                    points[++index] = timePoint.Point.Y / ScreenHeight;
                }
            }

            return points;
        }

        private double[] CalcWeight(double ratio)
        {
            var weights = new List<double>();
            var weightsNormalized = new List<double>();
            double weight = 1;
            foreach (var point in _timeSeriesPoints)
                weights.Add(weight *= ratio);
            foreach (var _weight in weights)
                weightsNormalized.Add(_weight / weights[^1]);
            return weightsNormalized.ToArray();
        }

        private LinkedList<TimeSeriesPoint> _timeSeriesPoints = new LinkedList<TimeSeriesPoint>();
        private DateTime _lastTime = DateTime.Now;
        private LinkedList<Vector2> _lastPoints = new LinkedList<Vector2>();

        private double _offset = 0;
        [UnitProperty("Offset", "ms")]
        public double Offset
        {
            set
            {
                if (value == 0)
                {
                    Log.Write("MLFilter", "Mode: Low Latency Jitter Reduction");
                }
                else if (value > 0)
                {
                    Log.Write("MLFilter", "Mode: Latency Compensation");
                    if (value > 18)
                    {
                        Log.Write("MLFilter", "Unrealistic latency compensation. [Compensation: " + value + "ms]", LogLevel.Warning);
                    }
                }
                else if (value < 0)
                {
                    Log.Write("MLFilter", "Mode: AI Smoothing");
                }
                _offset = value;
            }
            get => _offset;
        }

        private int _samples = 20;

        [Property("Samples")]
        public int Samples
        {
            set
            {
                int minimum = Degree + 1;

                if (value <= minimum)
                {
                    Log.Write("MLFilter",
                              "Samples too low for selected degree." +
                              $"[Samples:{value} Requirement: >{minimum}]", LogLevel.Warning);
                    _samples = value;
                    return;
                }
            }
            get => _samples;
        }

        private int _degree;
        [Property("Complexity")]
        public int Degree
        {
            set
            {
                if (value == 0)
                {
                    Log.Write("MLFilter", "Complexity cannot be zero", LogLevel.Error);
                    Log.Write("MLFilter", "Complexity: 1");
                    _degree = 1;
                    return;
                }
                else if (value > 10)
                {
                    Log.Write("MLFilter", "Degree too high, might cause instability and inaccuracy issues" +
                              "[Suggestion: (Degree <= 10, Normalization: enable)]", LogLevel.Warning);
                }
                _degree = value;
            }
            get => _degree;
        }

        [Property("Weight")]
        public double Weight { set; get; }

        [BooleanProperty("Normalize", "Preprocess the input. Set Screen Dimensions below when enabling Normalization.")]
        public bool Normalize { set; get; }

        [UnitProperty("Screen Width", "px")]
        public int ScreenWidth { set; get; }

        [UnitProperty("Screen Height", "px")]
        public int ScreenHeight { set; get; }

        public FilterStage FilterStage => FilterStage.PostTranspose;

    }
}

namespace MLFilter
{
    enum Axis {
        X,
        Y
    }
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