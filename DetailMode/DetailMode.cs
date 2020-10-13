using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using System;
using System.Numerics;

namespace DetailMode
{
  [PluginName("Detail Mode")]
    public class DetailBinding : IBinding
    {
    public string Property { set; get; }
    public Action Press => (Action) (() =>
    {
        DetailFilter.IsActive = !DetailFilter.IsActive;
        DetailFilter.SetPosition = true;
    });
    public Action Release => (Action) (() => DetailFilter.SetPosition = false);
    }

    public class DetailFilter : IFilter
    {
        public static Vector2 StartingPoint;
        public static bool IsActive { set; get; }
        public static bool SetPosition { set; get; }
        public Vector2 Filter(Vector2 OriginalPoint)
        {
            if (DetailFilter.SetPosition)
                DetailFilter.StartingPoint = new Vector2(OriginalPoint.X, OriginalPoint.Y);
            return DetailFilter.IsActive ? new Vector2(OriginalPoint.X * this.Scale + DetailFilter.StartingPoint.X, OriginalPoint.Y * this.Scale + DetailFilter.StartingPoint.Y) : OriginalPoint;
        }

        [SliderProperty("Detail Multiplier", 0.0f, 10f, 0.3f)]
        public float Scale { get; set; }

        public FilterStage FilterStage => FilterStage.PostTranspose;
    }
}
