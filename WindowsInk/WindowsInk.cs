using TabletDriverPlugin;
using TabletDriverPlugin.Attributes;
using TabletDriverPlugin.Output;
using TabletDriverPlugin.Platform.Pointer;

namespace WindowsInk
{
    public static class InkState
    {
        public static IPointerHandler inkPointer = new InkPointerHandler();
    }

    [PluginName("Windows Ink"), SupportedPlatform(PluginPlatform.Windows)]
    public class InkOutputMode : AbsoluteOutputMode
    {
        public override IPointerHandler PointerHandler => InkState.inkPointer;
    }

    public class InkPointerHandler : IPointerHandler, IPressureHandler
    {
        private Point _lastPos;
        private bool _inContact;
        private bool _lastContact;

        public InkPointerHandler()
        {
            Pen.Init();
            _inContact = false;
            _lastContact = false;
        }

        public Point GetPosition()
        {
            return _lastPos;
        }

        public void SetPosition(Point pos)
        {
            Pen.SetPosition(new POINT((int)pos.X, (int)pos.Y));
            if (_inContact != _lastContact)
            {
                if (_inContact)
                {
                    Pen.UnsetPointerFlags(POINTER_FLAGS.UP | POINTER_FLAGS.UPDATE);
                    Pen.SetPointerFlags(POINTER_FLAGS.DOWN);
                    _lastContact = _inContact;
                }
                else
                {
                    Pen.UnsetPointerFlags(POINTER_FLAGS.DOWN | POINTER_FLAGS.UPDATE);
                    Pen.SetPointerFlags(POINTER_FLAGS.UP);
                    _lastContact = _inContact;
                }
            }
            else
            {
                Pen.SetPointerFlags(POINTER_FLAGS.UPDATE);
            }
            Pen.Inject();
            _lastPos = pos;
        }

        public void SetPressure(float percentage)
        {
            var pressure = (uint)(percentage * 1024);
            if (pressure > 0)
            {
                Pen.SetPressure(pressure);
                Pen.SetPointerFlags(POINTER_FLAGS.INCONTACT | POINTER_FLAGS.FIRSTBUTTON);
                _inContact = true;
            }
            else
            {
                Pen.SetPressure(1);
                Pen.UnsetPointerFlags(POINTER_FLAGS.INCONTACT | POINTER_FLAGS.FIRSTBUTTON);
                _inContact = false;
            }
        }
    }
}