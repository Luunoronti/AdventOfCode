namespace TermGlass;


internal sealed partial class MainLoop
{
    private void HandleMouse()
    {
        var wheel = _input.ConsumeWheel();
        if (wheel != 0)
        {
            var factor = Math.Pow(1.1, wheel);
            ZoomAtCursor(factor);
            _input.Dirty = true;
        }

        // don't pan scene while dragging a window
        if (Window.IsDragging) return;

        if (_input.MouseLeftDragging || _input.MouseRightDragging)
        {
            var (dx, dy) = _input.ConsumeDragDelta();
            var mul = _cfg.PanSpeed / _vp.Zoom;
            if (dx != 0 || dy != 0)
            {
                _vp.Offset(-dx * mul, -dy * mul);
                _input.Dirty = true;
            }
        }
    }
    private void ZoomAtCursor(double factor)
    {
        var sx = _input.MouseX.Clamp(0, _t.Width - 1);
        var sy = _input.MouseY.Clamp(0, _t.Height - 1);
        var (wx, wy) = _vp.ScreenToWorld(sx, sy);
        _vp.ZoomAround(wx, wy, factor, minZoom: 0.1, maxZoom: 40.0);
    }
}
