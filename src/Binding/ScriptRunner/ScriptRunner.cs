using System;
using System.Diagnostics;
using OpenTabletDriver;
using OpenTabletDriver.Attributes;
using OpenTabletDriver.Tablet;

namespace VoiDPlugins.Binding.ScriptRunner
{
    [PluginName("Script Runner")]
    public class ScriptRunner : IStateBinding
    {
        [Setting("Run")]
        public string? Script { get; set; }

        public void Press(IDeviceReport report)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(Script!)
                    {
                        UseShellExecute = true
                    }
                };
                process.Start();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        public void Release(IDeviceReport report)
        {
            return;
        }
    }
}