#if false

using System.Numerics;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OTDPlugins.MeL.Core;

namespace OTDPlugins.MeL
{
    [PluginName("MeL")]
    public class MeLInterp : IInterpolator
    {
        public void NewReport(Vector2 point, uint pressure)
        {
            Core.Add(point);
        }

        public void Interpolate(InterpolatorArgs output)
        {
            if (Core.IsReady)
                output.Position = Core.Predict(Core.TimeNow, 0);
        }

        [Property("Samples")]
        public int Samples { set => Core.Samples = value; }

        [Property("Complexity")]
        public int Complexity { set => Core.Complexity = value; }

        [Property("Weight")]
        public int Weight { set => Core.Weight = value; }

        private readonly MLCore Core = new MLCore();
    }
}

#endif