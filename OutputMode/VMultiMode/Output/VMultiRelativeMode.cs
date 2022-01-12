using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Relative Mode")]
    public class VMultiRelativeMode : RelativeOutputMode
    {
        private VMultiRelativePointer? _pointer;

        [OnDependencyLoad]
        public void Initialize()
        {
            _pointer = new VMultiRelativePointer(TabletReference);
        }

        public override IRelativePointer Pointer
        {
            get => _pointer!;
            set { }
        }
    }
}