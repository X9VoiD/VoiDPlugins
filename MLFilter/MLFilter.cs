using MLFilter;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using TabletDriverPlugin;
using TabletDriverPlugin.Attributes;
using TabletDriverPlugin.Tablet;

namespace OTDPlugins
{

    [PluginName("MLFilter")]
    public class MLFilter : Notifier, IFilter
    {
        public virtual Point Filter(Point point)
        {
            DateTime date = DateTime.Now;
            CalcReportRate(date);
            var predicted = new Point();
            var feedPoint = new Point();
            bool fed = false;

            if (Feed)
            {
                if (AddPoint(point))
                {
                    foreach (var lastPoint in _lastPoints)
                        feedPoint += lastPoint;
                    feedPoint.X /= _lastPoints.Count;
                    feedPoint.Y /= _lastPoints.Count;
                    fed = AddTimeSeriesPoint(feedPoint, date);
                }
            }
            else
            {
                fed = AddTimeSeriesPoint(point, date);
            }

            if (fed)
            {
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
                    return point;
                }

                double predictAhead;
                predictAhead = (date - _timeSeriesPoints.First.Value.Date).TotalMilliseconds + Compensation;

                predicted.X = (float)xCoeff.Evaluate(predictAhead);
                predicted.Y = (float)yCoeff.Evaluate(predictAhead);

                if (Normalize)
                {
                    predicted.X *= ScreenWidth;
                    predicted.Y *= ScreenHeight;
                }

                Point finalPoint = new Point();

                if (Feed || AvgSamples == 0)
                    finalPoint = predicted;
                else
                {
                    if (AddPoint(predicted) && AvgSamples > 0)
                    {
                        foreach (var tempPoint in _lastPoints)
                        {
                            finalPoint += tempPoint;
                        }
                        finalPoint.X /= _lastPoints.Count;
                        finalPoint.Y /= _lastPoints.Count;
                    }
                }

                var now = DateTime.Now;

                _lastTime = date;
                return finalPoint;
            }
            _lastTime = date;
            return point;
        }

        #region Private Functions

        private bool AddTimeSeriesPoint(Point point, DateTime time)
        {
            _timeSeriesPoints.AddLast(new TimeSeriesPoint(point, time));
            if (_timeSeriesPoints.Count > Samples)
                _timeSeriesPoints.RemoveFirst();
            if (_timeSeriesPoints.Count == Samples)
                return true;
            return false;
        }

        private bool AddPoint(Point point)
        {
            _lastPoints.AddLast(point);
            if (_lastPoints.Count > AvgSamples)
                _lastPoints.RemoveFirst();
            if (_lastPoints.Count == AvgSamples)
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
                foreach (var timePoint in _timeSeriesPoints)
                {
                    points[++index] = timePoint.Point.X / ScreenWidth;
                }

            else if (axis == Axis.Y)
                foreach (var timePoint in _timeSeriesPoints)
                {
                    points[++index] = timePoint.Point.Y / ScreenHeight;
                }

            return points;
        }

        private void CalcReportRate(DateTime now)
        {
            _reportRate = 1000.0 / (now - _lastTime).TotalMilliseconds;
            _reportRateAvg.AddLast(_reportRate);
            if (_reportRateAvg.Count > 10)
                _reportRateAvg.RemoveFirst();
        }

        private double CalcReportRateAvg()
        {
            double avg = 0;
            foreach (var sample in _reportRateAvg)
                avg += sample;
            return avg / _reportRateAvg.Count;
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

        #endregion Private Functions

        private double _compensation = 0, _weight = 2;
        private int _samples = 20, _avgSamples, _degree = 2;
        private LinkedList<TimeSeriesPoint> _timeSeriesPoints = new LinkedList<TimeSeriesPoint>();
        private LinkedList<double> _reportRateAvg = new LinkedList<double>();
        private double _reportRate;
        private DateTime _lastTime = DateTime.Now;
        private bool _normalize, _feed;
        private int _screenWidth, _screenHeight;
        private LinkedList<Point> _lastPoints = new LinkedList<Point>();

        #region Controls

        [UnitProperty("Offset", "ms")]
        public double Compensation
        {
            set => CompensationFunc(ref _compensation, value);
            get => _compensation;
        }

        [Property("Samples")]
        public int Samples
        {
            set => SamplesFunc(ref _samples, value);
            get => _samples;
        }

        [Property("Complexity")]
        public int Degree
        {
            set => DegreeFunc(ref _degree, value);
            get => _degree;
        }

        [Property("Weight")]
        public double Weight
        {
            set => RaiseAndSetIfChanged(ref _weight, value);
            get => _weight;
        }

        [BooleanProperty("Normalize", "Preprocess the input. Set Screen Dimensions below when enabling Normalization.")]
        public bool Normalize
        {
            set => RaiseAndSetIfChanged(ref _normalize, value);
            get => _normalize;
        }

        [UnitProperty("Screen Width", "px")]
        public int ScreenWidth
        {
            set => RaiseAndSetIfChanged(ref _screenWidth, value);
            get => _screenWidth;
        }

        [UnitProperty("Screen Height", "px")]
        public int ScreenHeight
        {
            set => RaiseAndSetIfChanged(ref _screenHeight, value);
            get => _screenHeight;
        }

        [Property("Averaging Samples")]
        public int AvgSamples
        {
            set => RaiseAndSetIfChanged(ref _avgSamples, value);
            get => _avgSamples;
        }

        [BooleanProperty("Feed to Filter", "")]
        public bool Feed
        {
            set => RaiseAndSetIfChanged(ref _feed, value);
            get => _feed;
        }

        #endregion Controls

        #region Control Utility Functions

        private void CompensationFunc(ref double a, double value)
        {
            RaiseAndSetIfChanged(ref a, value);
        }

        private void SamplesFunc(ref int a, int value)
        {
            int minimum = Degree + 1;

            if (value <= minimum)
            {
                RaiseAndSetIfChanged(ref a, minimum);
                return;
            }

            RaiseAndSetIfChanged(ref a, value);
        }

        private void DegreeFunc(ref int a, int value)
        {
            RaiseAndSetIfChanged(ref a, value);
        }

        #endregion Control Utility Functions

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
        public TimeSeriesPoint(Point point, DateTime date)
        {
            Point = point;
            Date = date;
        }

        public Point Point { get; }
        public DateTime Date { get; }

    }
}