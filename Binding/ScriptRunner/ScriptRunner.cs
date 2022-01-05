using System;
using System.Diagnostics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.Binding.ScriptRunner
{
    [PluginName("Script Runner")]
    public class ScriptRunner : IStateBinding
    {
        [Property("Run")]
        public string? Script { get; set; }

        public void Press(TabletReference tablet, IDeviceReport report)
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

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            return;
        }
    }
}