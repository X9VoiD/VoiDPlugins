using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using System;
using System.Numerics;
using OpenTabletDriver.Plugin.Output;

namespace PrecisionControl
{
    [PluginName("Precision Control")]
    public class PrecisionControl: IBinding, IValidateBinding, IFilter
    {
        public static Vector2 StartingPoint;
        public static bool IsActive { set; get; }
        public static bool SetPosition { set; get; }
        public string Property { set; get; }

        public Action Press => () =>
        {
            if (Property == "Toggle")
                IsActive = !IsActive;
            else if (Property == "Hold")
                IsActive = true;
            SetPosition = true;
        };

        public Action Release => () =>
        {
            if (Property == "Hold")
                IsActive = false;
        };

        public string[] ValidProperties => new[] { "Toggle", "Hold" };

        public Vector2 Filter(Vector2 OriginalPoint)
        {
            if (SetPosition)
            {
                StartingPoint = OriginalPoint;
                SetPosition = false;
            }

            if (IsActive)
            {
                switch (Info.Driver.OutputMode)
                {
                    case AbsoluteOutputMode _:
                        var delta = (OriginalPoint - StartingPoint) * Scale;
                        return StartingPoint + delta;
                    case RelativeOutputMode _:
                        return OriginalPoint * Scale;
                    default:
                        return OriginalPoint;
                }
            }
            else
            {
                return OriginalPoint;
            }
        }

        [SliderProperty("Precision Multiplier", 0.0f, 10f, 0.3f)]
        public float Scale { get; set; } = 0.3f;

        public FilterStage FilterStage => FilterStage.PostTranspose;
    }
}
