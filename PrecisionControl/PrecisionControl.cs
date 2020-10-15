using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using System;
using System.Numerics;
using System.Linq;

namespace PrecisionControl
{
    [PluginName("Precision Control")]
    public class PrecisionControl: IBinding, IValidateBinding, IFilter
    {
        public static Vector2 StartingPoint;
        public float _Scale;
        public static bool IsActive { set; get; }
        public static bool SetPosition { set; get; }
        public string Property { set; get; }

        public Action Press => (Action)(() =>
            {
                IsActive = !IsActive;
                SetPosition = true;
            });

        public Action Release => (Action)(() => SetPosition = false);

        public string[] ValidProperties
        {
            get { return new[]{ "Toggle Precision Control" }; }
        }

        public Vector2 Filter(Vector2 OriginalPoint)
        {
            if (SetPosition && Info.Driver.OutputMode is OpenTabletDriver.Plugin.Output.AbsoluteOutputMode)
                StartingPoint = new Vector2(OriginalPoint.X, OriginalPoint.Y);
            return IsActive ? new Vector2(OriginalPoint.X * Scale + StartingPoint.X, OriginalPoint.Y * Scale + StartingPoint.Y) : OriginalPoint;
        }

        [SliderProperty("Precision Multiplier", 0.0f, 10f, 0.3f)]
        public float Scale
        {
            get => _Scale;
            set { _Scale = value; }
        }

        public FilterStage FilterStage => FilterStage.PostTranspose;
    }
}
