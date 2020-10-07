using System;
using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OTDPlugins.MeL.Core;

namespace OTDPlugins.MeL
{
    [PluginName("MeL")]
    public class MeLFilter : IFilter
    {
        public Vector2 Filter(Vector2 point)
        {
            Core.Add(point);
            try
            {
                return Core.IsReady ? Core.Predict(DateTime.UtcNow, Offset) : point;
            }
            catch
            {
                Log.Write("MeLFilter", "Unknown error in MeLCore", LogLevel.Error);
                return point;
            }
        }

        public FilterStage FilterStage => FilterStage.PostTranspose;

        [Property("Offset"), Unit("ms")]
        public float Offset { set; get; }

        [Property("Samples")]
        public int Samples { set => Core.Samples = value; }

        [Property("Complexity")]
        public int Complexity { set => Core.Complexity = value; }

        [Property("Weight")]
        public float Weight { set => Core.Weight = value; }

        private readonly MLCore Core = new MLCore();
    }
}