using System;
using System.Collections.Generic;
using TabletDriverPlugin;
using TabletDriverPlugin.Attributes;
using TabletDriverPlugin.Tablet;

namespace OpenTabletDriverPlugins
{
    using static Math;

    [PluginName("Advanced Anti-Smoothing")]
    public class AdvancedAntiSmoothing : Notifier, IFilter
    {
        public virtual Point Filter(Point point)
        {
            TimeSpan timeSpanDelta = DateTime.Now - _lastTime;

            _lastPredictedPoint ??= point;
            float timeDelta = (float) timeSpanDelta.TotalMilliseconds;

            float XVelocity = (point.X - (_lastPoint ?? point).X) / timeDelta;
            float YVelocity = (point.Y - (_lastPoint ?? point).Y) / timeDelta;

            AddVelocitySamples(XVelocity, YVelocity);

            float XVelocityAvg = Average(_xVelocityAvg);
            float YVelocityAvg = Average(_yVelocityAvg);

            float XAcceleration = (XVelocity - _lastVelocityX) / timeDelta;
            float YAcceleration = (YVelocity - _lastVelocityY) / timeDelta;

            AddAccelSamples(XAcceleration, YAcceleration);

            float XAccelerationAvg = Average(_xAccelAvg);
            float YAccelerationAvg = Average(_yAccelAvg);

            float predictedXVelocity = XVelocityAvg + XAccelerationAvg * (Compensation + timeDelta);
            float predictedYVelocity = YVelocityAvg + YAccelerationAvg * (Compensation + timeDelta);

            Point predicted;
            if (PurePredict)
                predicted = new Point(_lastPredictedPoint.X, _lastPredictedPoint.Y);
            else
                predicted = new Point(point.X, point.Y);

            if (Abs(XVelocity) < 2000 && Abs(predictedXVelocity) < 2000 &&
                Abs(YVelocity) < 2000 && Abs(predictedYVelocity) < 2000)
            {
                predicted.X += predictedXVelocity * timeDelta * (Strength / 100f);
                predicted.Y += predictedYVelocity * timeDelta * (Strength / 100f);
            }
            else
            {
                TabletDriverPlugin.Log.Write("AdvAntiSmoothing", DateTime.Now + ": No prediction applied.");
                predicted = point;
            }

            if (PurePredict && predicted.DistanceFrom(point) > 80 && XVelocity < 1 && YVelocity < 1)
            {
                TabletDriverPlugin.Log.Write("AdvAntiSmoothing", DateTime.Now + "Cursor resnapped.");
                predicted = point;
            }

            _lastVelocityX = XVelocity;
            _lastVelocityY = YVelocity;
            _lastPredictedPoint = predicted;
            _lastPoint = point;
            _lastTime = DateTime.Now;
            return predicted;
        }

        private void AddVelocitySamples(float X, float Y)
        {
            _xVelocityAvg.AddLast(X);
            _yVelocityAvg.AddLast(Y);
            if (_xVelocityAvg.Count > Samples)
            {
                for (int i = _xVelocityAvg.Count; i > Samples; --i)
                {
                    _xVelocityAvg.RemoveFirst();
                    _yVelocityAvg.RemoveFirst();
                }
            }
        }

        private void AddAccelSamples(float X, float Y)
        {
            _xAccelAvg.AddLast(X);
            _yAccelAvg.AddLast(Y);
            if (_xAccelAvg.Count > Samples)
            {
                for (int i = _xAccelAvg.Count; i > Samples; --i)
                {
                    _xAccelAvg.RemoveFirst();
                    _yAccelAvg.RemoveFirst();
                }
            }
        }

        private float Average(LinkedList<float> samples)
        {
            float weight = 2, sumOfWeights = 0, avg = 0;

            foreach (float sample in samples)
            {
                avg += sample * weight;
                weight *= 2;
                sumOfWeights += weight;
            }

            avg /= sumOfWeights;
            return avg;
        }

        private bool _purePredict;
        private Point _lastPoint = null, _lastPredictedPoint = null;
        private float _compensation = 20, _lastVelocityX = 0, _lastVelocityY = 0, _strength = 100;
        private int _samples = 20;
        private LinkedList<float> _xVelocityAvg = new LinkedList<float>();
        private LinkedList<float> _yVelocityAvg = new LinkedList<float>();
        private LinkedList<float> _xAccelAvg = new LinkedList<float>();
        private LinkedList<float> _yAccelAvg = new LinkedList<float>();
        private DateTime _lastTime;

        [BooleanProperty("Pure Prediction", "Enable pure predicted cursor. (Use with Caution! Inaccurate, not yet ready)")]
        public bool PurePredict
        {
            set => this.RaiseAndSetIfChanged(ref _purePredict, value);
            get => _purePredict;
        }

        [UnitProperty("Compensation", "ms")]
        public float Compensation
        {
            set => this.RaiseAndSetIfChanged(ref _compensation, value);
            get => _compensation;
        }

        [UnitProperty("Strength", "%")]
        public float Strength
        {
            set => this.RaiseAndSetIfChanged(ref _strength, value);
            get => _strength;
        }

        [Property("Samples")]
        public int Samples
        {
            set => this.RaiseAndSetIfChanged(ref _samples, value);
            get => _samples;
        }

        public FilterStage FilterStage => FilterStage.PostTranspose;

    }
}