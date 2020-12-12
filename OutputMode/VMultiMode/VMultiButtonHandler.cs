using System.Collections.Generic;
using OpenTabletDriver.Plugin.Attributes;
using VoiDPlugins.Library.VMulti;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Mode")]
    public class VMultiButtonHandler : ButtonHandler
    {
        private readonly Dictionary<string, int> bindings = new()
        {
            { "Left", 1 },
            { "Right", 2 },
            { "Middle", 4 }
        };

        public override Dictionary<string, int> Bindings => bindings;
    }
}