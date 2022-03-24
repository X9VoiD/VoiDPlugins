using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Platform.Pointer;
using OpenTabletDriver.Plugin.Tablet;

namespace VoiDPlugins.OutputMode
{
    [PluginName("VMulti Relative Mode")]
    public class VMultiRelativeMode : RelativeOutputMode
    {
        private VMultiRelativePointer? _pointer;

        public override TabletReference Tablet
        {
            get => base.Tablet;
            set
            {
                base.Tablet = value;
                _pointer = new VMultiRelativePointer(value);
            }
        }

        public override IRelativePointer Pointer
        {
            get => _pointer!;
            set { }
        }
    }
}