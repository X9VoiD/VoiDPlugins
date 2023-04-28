using System;
using System.ComponentModel;
using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Output;
using OpenTabletDriver.Tablet;
using VoiDPlugins.Filter.MeL.Core;

namespace VoiDPlugins.Filter.MeL
{
    [PluginName("MeL Filter")]
    public class MeLFilter : IDevicePipelineElement
    {
        private readonly MLCore Core = new();
        private bool rateLimit;

        public event Action<IDeviceReport>? Emit;

        public PipelinePosition Position => PipelinePosition.PostTransform;

        public MeLFilter(ISettingsProvider settingsProvider)
        {
            settingsProvider.Inject(this);
        }

        [Setting("Offset"), Unit("ms"), DefaultValue(0f)]
        public float Offset { set; get; }

        [Setting("Samples"), DefaultValue(20)]
        public int Samples { set => Core.Samples = value; }

        [Setting("Complexity"), DefaultValue(2)]
        public int Complexity { set => Core.Complexity = value; }

        [Setting("Weight"), DefaultValue(1.4f)]
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
                }
                catch
                {
                    if (!rateLimit)
                    {
                        Log.Write("MeLFilter", "Unknown error in MeLCore", LogLevel.Error);
                        rateLimit = true;
                    }
                }

                value = report;
            }

            Emit?.Invoke(value);
        }
    }
}