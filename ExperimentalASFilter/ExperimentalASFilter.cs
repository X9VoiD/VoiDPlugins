using ExpASFilter;
using MathNet.Numerics;
using MathNet.Numerics.LinearRegression;
using System;
using System.Collections.Generic;
using TabletDriverPlugin;
using TabletDriverPlugin.Attributes;
using TabletDriverPlugin.Tablet;

namespace OpenTabletDriverPlugins
{

    [PluginName("Experimental AS Filter")]
    public class ExperimentalASFilter : Notifier, IFilter
    {
        public virtual Point Filter(Point point)
        {
            DateTime date = DateTime.Now;
            CalcReportRate(date);
            var reportRateAvg = CalcReportRateAvg();

            if (AddTimeSeriesPoint(point, date))
            {
                var predicted = new Point();
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
                if (Weighted)
                {
                    var weights = CalcWeight();
                    xCoeff = new Polynomial(Fit.PolynomialWeighted(timeMatrix, x, weights, Degree));
                    yCoeff = new Polynomial(Fit.PolynomialWeighted(timeMatrix, y, weights, Degree));
                }
                else
                {
                    xCoeff = new Polynomial(Fit.Polynomial(timeMatrix, x, Degree, DirectRegressionMethod.Svd));
                    yCoeff = new Polynomial(Fit.Polynomial(timeMatrix, y, Degree, DirectRegressionMethod.Svd));
                }

                double predictAhead;
                if (Sync)
                    predictAhead = 1000.0 / reportRateAvg * Ahead;
                else
                    predictAhead = (date - _timeSeriesPoints.First.Value.Date).TotalMilliseconds + Compensation;

                predicted.X = (float)xCoeff.Evaluate(predictAhead);
                predicted.Y = (float)yCoeff.Evaluate(predictAhead);
                if (Normalize)
                {
                    predicted.X *= ScreenWidth;
                    predicted.Y *= ScreenHeight;
                }

                var now = DateTime.Now;
                if ((now - date).TotalMilliseconds > CalcReportRateAvg())
                    Log.Write("ExpASFilter", now + ": CPU choking hard. Latency higher than normal. We missed a hz.");

                _lastTime = date;
                return predicted;
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
                {
                    points[++index] = timePoint.Point.X;
                }

            else if (axis == Axis.Y)
                foreach (var timePoint in _timeSeriesPoints)
                {
                    points[++index] = timePoint.Point.Y;
                }

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

        private double[] CalcWeight()
        {
            var weights = new List<double>();
            var weightsNormalized = new List<double>();
            double weight = 1;
            foreach (var point in _timeSeriesPoints)
                weights.Add(weight *= 2);
            foreach (var _weight in weights)
                weightsNormalized.Add(_weight / weights[^1]);
            return weightsNormalized.ToArray();
        }

        #endregion Private Functions

        private int _samples = 6, _degree = 1, _ahead = 8;
        private LinkedList<TimeSeriesPoint> _timeSeriesPoints = new LinkedList<TimeSeriesPoint>();
        private LinkedList<double> _reportRateAvg = new LinkedList<double>();
        private double _reportRate;
        private DateTime _lastTime = DateTime.Now;
        private double _compensation;
        private bool _normalize, _sync, _weighted;
        private int _screenWidth, _screenHeight;

        #region Controls

        [UnitProperty("Compensation", "ms")]
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

        [Property("Degree")]
        public int Degree
        {
            set => DegreeFunc(ref _degree, value);
            get => _degree;
        }

        [BooleanProperty("Normalize", "Preprocess the input before performing regression. (may improve accuracy) Set Screen Dimensions below when enabling Normalization.")]
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

        [BooleanProperty("Synchronize to Report Rate", "Compensation will be ignored in respect for tablet's report rate. Set below how many tablet reports to predict ahead.")]
        public bool Sync
        {
            set => RaiseAndSetIfChanged(ref _sync, value);
            get => _sync;
        }

        [Property("Reports Ahead")]
        public int Ahead
        {
            set => RaiseAndSetIfChanged(ref _ahead, value);
            get => _ahead;
        }

        [BooleanProperty("Exponential Weighted Polynomial Regression", "Well...")]
        public bool Weighted
        {
            set => RaiseAndSetIfChanged(ref _weighted, value);
            get => _weighted;
        }

        #endregion Controls

        #region Control Utility Functions

        private void CompensationFunc(ref double a, double value)
        {
            if (value > 30)
                Log.Write("ExpASFilter", "Unrealistic latency compensation. [Compensation: " + value + "ms]", true);
            RaiseAndSetIfChanged(ref a, value);
        }

        private void SamplesFunc(ref int a, int value)
        {
            int minimum = Degree + 1;
            var suggested = Math.Pow(Degree, 2);

            if (value > 12)
                Log.Write("ExpASFilter",
                    "Samples too high. Expect higher latency." +
                    "[Samples: " + value + "]", true);

            else if (value <= minimum)
            {
                Log.Write("ExpASFilter",
                    "Samples too low for selected degree." +
                    "[Samples: " + value + ", Required Minimum: " + minimum + "]", true);
                RaiseAndSetIfChanged(ref a, minimum);
                return;
            }

            if (value <= suggested)
                Log.Write("ExpASFilter",
                    "Samples might not be enough for an accurate prediction." +
                    "[Samples: " + value + ", Suggestion: " + suggested + "]");

            RaiseAndSetIfChanged(ref a, value);
        }

        private void DegreeFunc(ref int a, int value)
        {
            if (value == 0)
            {
                Log.Write("ExpASFilter", "Degree cannot be zero", true);
            }
            else if (value > 5)
                Log.Write("ExpASFilter", "Degree too high, might cause instability and inaccuracy issues" +
                    "[Suggestion: Degree <= 5]");
            RaiseAndSetIfChanged(ref a, value);
        }

        #endregion Control Utility Functions

        public FilterStage FilterStage => FilterStage.PostTranspose;

    }
}

namespace ExpASFilter
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