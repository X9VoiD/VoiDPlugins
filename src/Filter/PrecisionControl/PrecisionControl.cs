using System;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Timing;

namespace VoiDPlugins.Filter
{
    [PluginName("Precision Control")]
    public class PrecisionControlBinding : IStateBinding
    {
        internal static Vector2 StartingPoint;
        internal static bool IsActive { set; get; }
        internal static bool IsRelative { set; get; }
        internal static bool SetPosition { set; get; }
        internal static float CurrentScale { set; get; }

        public static string[] ValidModes => new[] { "Toggle", "Hold" };
        public static string[] ValidRelativeModes => new[] { "Absolute", "Relative" };

        [Property("Mode"), PropertyValidated(nameof(ValidModes))]
        public string? Mode { set; get; }

        [Property("Relative Mode"), PropertyValidated(nameof(ValidRelativeModes))]
        public string? RelativeMode { set; get; }

        [SliderProperty("Precision Multiplier", 0.0f, 10f, 0.3f), DefaultPropertyValue(0.3f)]
        public float Scale { get; set; }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            if (Mode == "Toggle")
                IsActive = !IsActive;
            else if (Mode == "Hold")
                IsActive = true;

            IsRelative = (RelativeMode == "Relative");


            CurrentScale = Scale;
            SetPosition = true;
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            if (Mode == "Hold")
                IsActive = false;
        }
    }

    [PluginName("Precision Control")]
    public class PrecisionControl : IPositionedPipelineElement<IDeviceReport>
    {
        public event Action<IDeviceReport>? Emit;

        private readonly HPETDeltaStopwatch _stopwatch = new HPETDeltaStopwatch();

        [SliderProperty("Timeout", 100f, 1000f, 1f), DefaultPropertyValue(50), Unit("ms")]
        [ToolTip("Time period after which the position is reset in relative mode.")]
        public double Timeout
        {
            get { return timeout.TotalMilliseconds; }
            set { timeout = TimeSpan.FromMilliseconds(value); }
        }
        private TimeSpan timeout;

        public PipelinePosition Position => PipelinePosition.PostTransform;
        internal static Vector2 LastPosition;
        internal static Vector2 NewZero;

        public void Consume(IDeviceReport value)
        {
            if (value is ITabletReport report)
            {
                if (PrecisionControlBinding.SetPosition)
                {
                    PrecisionControlBinding.StartingPoint = report.Position;
                    NewZero = report.Position;
                    PrecisionControlBinding.SetPosition = false;
                }

                if (PrecisionControlBinding.IsRelative)
                {
                    var deltaTime = _stopwatch.Restart();

                    if (deltaTime > timeout) {
                        PrecisionControlBinding.StartingPoint = LastPosition;
                        NewZero = report.Position;
                    }
                }

                if (PrecisionControlBinding.IsActive)
                {
                    var relativeTo = PrecisionControlBinding.IsRelative ? NewZero : PrecisionControlBinding.StartingPoint;
                    var delta = (report.Position - relativeTo) * PrecisionControlBinding.CurrentScale;
                    report.Position = PrecisionControlBinding.StartingPoint + delta;
                }

                LastPosition = report.Position;

                value = report;
            }

            Emit?.Invoke(value);
        }
    }
}
