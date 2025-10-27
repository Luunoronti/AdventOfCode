namespace TermGlass;


internal sealed partial class MainLoop
{
    private bool ConsumeKeyboardInput(bool running)
    {
        var consumed = 0;
        while (consumed < MaxKeyEventsPerFrame && _input.TryDequeueKey(out var ke))
        {
            consumed++;
            _input.LastKey = ke.Key;
            _input.Ctrl = ke.Ctrl; _input.Shift = ke.Shift; _input.Alt = ke.Alt;

            // Quit: Ctrl+Q
            if (ke.Ctrl && ke.Key == ConsoleKey.Q) { running = false; break; }
            if (ke.Key == ConsoleKey.Escape) { running = false; break; }
            HandleKeys();
        }

        return running;
    }

    private void HandleKeys()
    {
        var k = _input.LastKey;
        var ctrl = _input.Ctrl;

        switch (k)
        {
            case ConsoleKey.Spacebar:
                _input.StepRequested = true;
                break;

            case ConsoleKey.Add:
            case ConsoleKey.OemPlus:
                if (ctrl) ZoomAtCursor(1.25);
                else Pan(0, -1);
                break;

            case ConsoleKey.Subtract:
            case ConsoleKey.OemMinus:
                if (ctrl) ZoomAtCursor(1 / 1.25);
                else Pan(0, +1);
                break;

            case ConsoleKey.LeftArrow: Pan(-1, 0); break;
            case ConsoleKey.RightArrow: Pan(+1, 0); break;
            case ConsoleKey.UpArrow: Pan(0, -1); break;
            case ConsoleKey.DownArrow: Pan(0, +1); break;

            case ConsoleKey.W: Pan(0, -1); break;
            case ConsoleKey.S: Pan(0, +1); break;
            case ConsoleKey.A: Pan(-1, 0); break;
            case ConsoleKey.D: Pan(+1, 0); break;

            case ConsoleKey.F1:
                ToggleHelpWindow();
                _input.Dirty = true;
                break;

            case ConsoleKey.D0:
                {
                    var sx = _t.Width / 2;
                    var sy = _t.Height / 2;
                    _vp.ResetZoomAroundScreenPoint(sx, sy, 1.0);
                    _input.Dirty = true;
                    break;
                }

            // Autoplay presets
            case ConsoleKey.D1: _cfg.AutoPlay = false; break;
            case ConsoleKey.D2: _cfg.AutoPlay = true; _cfg.AutoStepPerSecond = 5; break;
            case ConsoleKey.D3: _cfg.AutoPlay = true; _cfg.AutoStepPerSecond = 30; break;

            // Layer presets
            case ConsoleKey.F5: _cfg.Layers = UiLayers.All; break;
            case ConsoleKey.F6: _cfg.Layers = UiLayers.Map; break;
            case ConsoleKey.F7: _cfg.Layers = UiLayers.Rulers | UiLayers.StatusBar; break;
            case ConsoleKey.F8: _cfg.Layers ^= UiLayers.Overlays; break;

            // Fine tune autoplay speed
            case ConsoleKey.Oem4: _cfg.AutoStepPerSecond = Math.Max(0.2, _cfg.AutoStepPerSecond / 1.25); break;
            case ConsoleKey.Oem6: _cfg.AutoStepPerSecond *= 1.25; break;

            case ConsoleKey.T:
                _tooltipEnabled = !_tooltipEnabled;
                break;

            case ConsoleKey.C:
                {
                    _cfg.ColorMode = _cfg.ColorMode == ColorMode.TrueColor ? ColorMode.Console16 : ColorMode.TrueColor;
                    _t.SetColorMode(_cfg.ColorMode);
                    _t.Clear();
                    _input.Dirty = true;
                    break;
                }
        }
    }

}
