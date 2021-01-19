using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using System;
using System.Numerics;

namespace VoiDPlugins.Filter
{
    [PluginName("Precision Control")]
    public class PrecisionControlBinding: IBinding, IValidateBinding
    {
        internal static Vector2 StartingPoint;
        internal static bool IsActive { set; get; }
        internal static bool SetPosition { set; get; }

        [Property("Property")]
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
    }

    [PluginName("Precision Control")]
    public class PrecisionControl : IFilter
    {
        public Vector2 Filter(Vector2 OriginalPoint)
        {
            if (PrecisionControlBinding.SetPosition)
            {
                PrecisionControlBinding.StartingPoint = OriginalPoint;
                PrecisionControlBinding.SetPosition = false;
            }

            if (PrecisionControlBinding.IsActive)
            {
                var delta = (OriginalPoint - PrecisionControlBinding.StartingPoint) * Scale;
                return PrecisionControlBinding.StartingPoint + delta;
            }
            else
            {
                return OriginalPoint;
            }
        }

        [SliderProperty("Precision Multiplier", 0.0f, 10f, 0.3f), DefaultPropertyValue(0.3f)]
        public float Scale { get; set; }

        public FilterStage FilterStage => FilterStage.PostTranspose;
    }
}
