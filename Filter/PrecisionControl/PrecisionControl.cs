using System;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.Filter
{
    [PluginName("Precision Control")]
    public class PrecisionControlBinding : IStateBinding
    {
        internal static Vector2 StartingPoint;
        internal static bool IsActive { set; get; }
        internal static bool SetPosition { set; get; }

        public static string[] ValidModes => new[] { "Toggle", "Hold" };

        [Property("Mode"), PropertyValidated(nameof(ValidModes))]
        public string Mode { set; get; }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            if (Mode == "Toggle")
                IsActive = !IsActive;
            else if (Mode == "Hold")
                IsActive = true;

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
        public event Action<IDeviceReport> Emit;

        [SliderProperty("Precision Multiplier", 0.0f, 10f, 0.3f), DefaultPropertyValue(0.3f)]
        public float Scale { get; set; }

        public PipelinePosition Position => PipelinePosition.PostTransform;

        public void Consume(IDeviceReport value)
        {
            if (value is ITabletReport report)
            {
                if (PrecisionControlBinding.SetPosition)
                {
                    PrecisionControlBinding.StartingPoint = report.Position;
                    PrecisionControlBinding.SetPosition = false;
                }

                if (PrecisionControlBinding.IsActive)
                {
                    var delta = (report.Position - PrecisionControlBinding.StartingPoint) * Scale;
                    report.Position = PrecisionControlBinding.StartingPoint + delta;
                }
                value = report;
            }

            Emit?.Invoke(value);
        }
    }
}
