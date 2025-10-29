
using AdventOfCode.Runtime;

namespace TermGlass;

// Drawing in world vs. screen coordinates
public sealed class Frame
{
    private readonly Terminal _t;
    private readonly Viewport _vp;
    private readonly CellBuffer _buf;
    public readonly InputState Input;
    public readonly VizConfig Cfg;

    // Optional: host/AoC can set these; MainLoop will pick them up each frame
    public TooltipProvider? TooltipProvider
    {
        get; set;
    }

    public Frame(Terminal t, Viewport vp, CellBuffer buf, InputState input, VizConfig cfg)
    {
        _t = t; _vp = vp; _buf = buf; Input = input; Cfg = cfg;
    }

    // Transformations
    public (double wx, double wy) ScreenToWorld(int sx, int sy) => _vp.ScreenToWorld(sx, sy);
    public (int sx, int sy) WorldToScreen(double wx, double wy) => _vp.WorldToScreen(wx, wy);

    // Drawing the world map by sampling the viewport window
    public void DrawWorld(IWorldSource world)
    {
        int W = _t.Width, H = _t.Height;
        for (var sy = 0; sy < H; sy++)
        {
            for (var sx = 0; sx < W; sx++)
            {
                var (wx, wy) = _vp.ScreenToWorld(sx, sy);

                if (wx < 0 || wy < 0 || wx >= world.Width || wy >= world.Height)
                    continue;

                var cell = world.GetCell((int)wx, (int)wy)!.Value;
                _buf.TrySet(sx, sy, cell);
            }
        }
    }

    // (Overlays)
    public void DrawRectWorld(double x, double y, double w, double h, char ch, Rgb fg, Rgb bg)
        => Renderer.DrawRectWorld(_buf, _vp, x, y, w, h, ch, fg, bg, Cfg.Layers.HasFlag(UiLayers.Overlays));

    public void DrawCircleWorld(double cx, double cy, double r, char ch, Rgb fg, Rgb bg)
        => Renderer.DrawCircleWorld(_buf, _vp, cx, cy, r, ch, fg, bg, Cfg.Layers.HasFlag(UiLayers.Overlays));

    public void Draw(int sx, int sy, string text, Rgb fg, Rgb bg)
        => Renderer.DrawTextScreen(_buf, sx, sy, text, fg, bg, Cfg.Layers.HasFlag(UiLayers.Overlays));

    public void DrawWorld(int wx, int wy, string text, Rgb fg, Rgb bg)
    {
        var s = _vp.WorldToScreen(wx, wy);
        Renderer.DrawTextScreen(_buf, s.sx, s.sy, text, fg, bg, Cfg.Layers.HasFlag(UiLayers.Overlays));
    }

    public void Draw(int x, int y, char ch, Rgb fg, Rgb bg)
        => DrawRectWorld(x, y, 1, 1, ch, fg, bg);

    public void Draw(double x, double y, char ch, Rgb fg, Rgb bg)
        => DrawRectWorld(x, y, 1, 1, ch, fg, bg);

    public void Draw(System.Numerics.Vector2 v, char ch, Rgb fg, Rgb bg)
        => DrawRectWorld(v.X, v.Y, 1, 1, ch, fg, bg);


    public void Draw(HashSet<(int x, int y)> map, char ch, Rgb fg, Rgb bg)
    {
        foreach (var (x, y) in map)
            DrawRectWorld(x, y, 1, 1, ch, fg, bg);
    }

    public void Draw(Traveller traveller, bool drawTrail = true, bool drawOrigin = true)
    {
        var bg = new Rgb(10, 10, 10);

        var fg1 = new Rgb(30, 30, 30);
        var fg2 = new Rgb(230, 230, 230);

        char GetFromCard(CardinalDirection dir)
        {
            char c = '◁';

            // current location
            if (dir == CardinalDirection.South)
                c = '▽';
            else if (dir == CardinalDirection.North)
                c = '△';
            else if (dir == CardinalDirection.East)
                c = '▷';
            return c;
        }


        // will use settings from config once we have those
        if (drawTrail)
        {
            int step = 0;
            foreach (var vis in traveller.VisitedLocationsForVisOnly)
            {
                step++;
                var p = (float)step / (float)traveller.VisitedLocationsForVisOnly.Count;
                var c = (int)(p * 255);
                Draw(vis.Key, GetFromCard(vis.Value), bg, new Rgb(80, 80, (byte)(c)));
            }
        }
        if (drawOrigin)
            Draw(traveller.StartLocation, '◎', new Rgb(200, 80, 80), bg);
        Draw(traveller.Location, GetFromCard(traveller.CardinalDirection), new Rgb(255, 230, 120), bg);
    }

    //class TType
    public void Draw<TType>(Map<TType> keypad, Rgb foreground, Rgb background)
    {
        if (keypad == null) return;
        //var maxWidth = keypad.MapActualLinear.Max(p => p.ToString().Length + 2);
        for (int y = 0; y < keypad.SizeY; y++)
        {
            for (int x = 0; x < keypad.SizeX; x++)
            {
                var s = keypad[x, y]?.ToString() ?? " ";
                Draw(x, y, s[0], foreground, background);
            }
        }
    }
}
