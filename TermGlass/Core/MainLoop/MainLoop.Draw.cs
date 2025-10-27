namespace TermGlass;


internal sealed partial class MainLoop
{
    private void Draw(bool completed)
    {
        _input.Dirty = false;

        _buf.AlphaBlendEnabled = _cfg.ColorMode == ColorMode.TrueColor;
        _buf.Fill(new Cell(' ', Rgb.White, Rgb.Black));

        var frame = new Frame(_t, _vp, _buf, _input, _cfg);
        _draw(frame, completed);
        _statusFromFrame = _status?.Invoke() ?? "";

        if (_cfg.Layers.HasFlag(UiLayers.Rulers)) DrawRulers();

        // windows — on top of content and rulers
        Window.DrawAll(_buf);

        // tooltip — above everything except the status bar
        DrawTooltipIfAny();

        if (_cfg.Layers.HasFlag(UiLayers.StatusBar)) DrawStatusBar();

        _t.Draw(_buf);

        // FPS only when autoplay
        if (_cfg.AutoPlay)
        {
            _frameCounter++;
            var now = DateTime.UtcNow;
            var elapsed = (now - _fpsLastTime).TotalSeconds;
            if (elapsed >= 1.0)
            {
                _fps = _frameCounter / elapsed;
                _frameCounter = 0;
                _fpsLastTime = now;
            }
        }
    }

}
