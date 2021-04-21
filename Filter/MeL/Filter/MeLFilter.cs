using System;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using VoiDPlugins.Filter.MeL.Core;

namespace VoiDPlugins.Filter.MeL
{
    [PluginName("MeL")]
    public class MeLFilter : IPositionedPipelineElement<IDeviceReport>
    {
        private readonly MLCore Core = new MLCore();
        private bool rateLimit;

        public event Action<IDeviceReport> Emit;

        public PipelinePosition Position => PipelinePosition.PostTransform;

        [Property("Offset"), Unit("ms"), DefaultPropertyValue(0)]
        public float Offset { set; get; }

        [Property("Samples"), DefaultPropertyValue(20)]
        public int Samples { set => Core.Samples = value; }

        [Property("Complexity"), DefaultPropertyValue(2)]
        public int Complexity { set => Core.Complexity = value; }

        [Property("Weight"), DefaultPropertyValue(1.4f)]
        public float Weight { set => Core.Weight = value; }

        public void Consume(IDeviceReport value)
        {
            if (value is ITabletReport report)
            {
                Core.Add(report.Position);
                try
                {
                    report.Position = Core.IsReady ? Core.Predict(Offset) : report.Position;
                    rateLimit = false;
                    Emit?.Invoke(report);
                }
                catch
                {
                    if (!rateLimit)
                    {
                        Log.Write("MeLFilter", "Unknown error in MeLCore", LogLevel.Error);
                        rateLimit = true;
                    }
                    Emit?.Invoke(report);
                }
            }
        }
    }
}