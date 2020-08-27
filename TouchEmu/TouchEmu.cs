using TabletDriverPlugin;
using TabletDriverPlugin.Attributes;
using TabletDriverPlugin.Output;
using TabletDriverPlugin.Platform.Pointer;

namespace TouchEmu
{
    public static class TouchState
    {
        public static IVirtualTablet touchPointer = new TouchPointerHandler();
    }

    [PluginName("Touch Emu"), SupportedPlatform(PluginPlatform.Windows)]
    public class TouchOutputMode : AbsoluteOutputMode
    {
        public override IVirtualTablet VirtualTablet => TouchState.touchPointer;
    }

    public class TouchPointerHandler : IVirtualTablet, IPressureHandler
    {
        private bool _inContact;
        private bool _lastContact;

        public TouchPointerHandler()
        {
            Touch.Init();
            _inContact = false;
            _lastContact = false;
        }

        public void MouseDown(MouseButton button)
        {
            return;
        }

        public void MouseUp(MouseButton button)
        {
            return;
        }

        public void SetPosition(Point pos)
        {
            Touch.SetPosition(new POINT((int)pos.X, (int)pos.Y));
            if (_inContact != _lastContact)
            {
                if (_inContact)
                {
                    Touch.UnsetPointerFlags(POINTER_FLAGS.UP | POINTER_FLAGS.UPDATE);
                    Touch.SetPointerFlags(POINTER_FLAGS.DOWN);
                    _lastContact = _inContact;
                }
                else
                {
                    Touch.UnsetPointerFlags(POINTER_FLAGS.DOWN | POINTER_FLAGS.UPDATE);
                    Touch.SetPointerFlags(POINTER_FLAGS.UP);
                    _lastContact = _inContact;
                }
            }
            else
            {
                Touch.SetPointerFlags(POINTER_FLAGS.UPDATE);
            }
            Touch.SetTarget();
            Touch.Inject();
        }

        public void SetPressure(float percentage)
        {
            var pressure = (uint)(percentage * 1024);
            if (pressure > 0)
            {
                Touch.SetPressure(pressure);
                Touch.SetPointerFlags(POINTER_FLAGS.INCONTACT | POINTER_FLAGS.FIRSTBUTTON);
                _inContact = true;
            }
            else
            {
                Touch.SetPressure(1);
                Touch.UnsetPointerFlags(POINTER_FLAGS.INCONTACT | POINTER_FLAGS.FIRSTBUTTON);
                _inContact = false;
            }
        }
    }
}