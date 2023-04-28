﻿using System;
using System.ComponentModel;
using System.Numerics;
using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Output;
using OpenTabletDriver.Tablet;

namespace VoiDPlugins.Filter
{
    [PluginName("Precision Control")]
    public class PrecisionControlBinding : IStateBinding
    {
        internal static Vector2 StartingPoint;
        internal static bool IsActive { set; get; }
        internal static bool SetPosition { set; get; }

        public static string[] ValidModes => new[] { "Toggle", "Hold" };

        [Setting("Mode"), MemberValidated(nameof(ValidModes))]
        public string? Mode { set; get; }

        public PrecisionControlBinding(ISettingsProvider settingsProvider)
        {
            settingsProvider.Inject(this);
        }

        public void Press(IDeviceReport report)
        {
            if (Mode == "Toggle")
                IsActive = !IsActive;
            else if (Mode == "Hold")
                IsActive = true;

            SetPosition = true;
        }

        public void Release(IDeviceReport report)
        {
            if (Mode == "Hold")
                IsActive = false;
        }
    }

    [PluginName("Precision Control")]
    public class PrecisionControl : IDevicePipelineElement
    {
        public event Action<IDeviceReport>? Emit;

        [RangeSetting("Precision Multiplier", 0.0f, 10f, 0.3f), DefaultValue(0.3f)]
        public float Scale { get; set; }

        public PipelinePosition Position => PipelinePosition.PostTransform;

        public PrecisionControl(ISettingsProvider settingsProvider)
        {
            settingsProvider.Inject(this);
        }

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
