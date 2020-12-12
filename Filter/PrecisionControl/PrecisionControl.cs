using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using System;
using System.Numerics;

namespace VoiDPlugins.Filter
{
    [PluginName("Precision Control")]
    public class PrecisionControl: IBinding, IValidateBinding, IFilter
    {
        private static Vector2 StartingPoint;
        private static bool IsActive { set; get; }
        private static bool SetPosition { set; get; }
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
                var delta = (OriginalPoint - StartingPoint) * Scale;
                return StartingPoint + delta;
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
