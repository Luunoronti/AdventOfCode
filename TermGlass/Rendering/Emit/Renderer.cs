
namespace TermGlass;

// =================== Rendering helpers ===================

public static class Renderer
{
    public static void DrawWorld(CellBuffer buf, Viewport vp, IWorldSource world, bool enabled)
    {
        if (!enabled) return;
        int W = buf.Width, H = buf.Height;
        for (var sy = 1; sy < H - 1; sy++)
        {
            for (var sx = 4; sx < W; sx++)
            {
                var (wx, wy) = vp.ScreenToWorld(sx, sy);
                var cell = Sample(world, wx, wy, 1.0 / vp.Zoom);
                buf.Set(sx, sy, cell);
            }
        }
    }

    private static Cell Sample(IWorldSource world, double wx, double wy, double worldPerPixel)
    {
        if (worldPerPixel <= 1.0)
        {
            const double eps = 1e-9;
            var x = (int)Math.Floor(wx + eps);
            var y = (int)Math.Floor(wy + eps);
            var c = world.GetCell(x, y);
            return c ?? new Cell(' ', Rgb.White, Rgb.Black);
        }
        else
        {
            // Downsample: average 2x2 neighborhood; pick mode character; average BOTH fg and bg
            var step = Math.Max(1.0, worldPerPixel * 0.8);
            var x0 = (int)Math.Floor(wx - step * 0.5);
            var y0 = (int)Math.Floor(wy - step * 0.5);

            int fr = 0, fg = 0, fb = 0, br = 0, bg = 0, bb = 0, n = 0;
            var chars = new char[4];
            var count = 0;

            for (var dy = 0; dy < 2; dy++)
                for (var dx = 0; dx < 2; dx++)
                {
                    var c = world.GetCell(x0 + dx, y0 + dy);
                    if (c.HasValue)
                    {
                        var v = c.Value;
                        fr += v.Fg.R; fg += v.Fg.G; fb += v.Fg.B;
                        br += v.Bg.R; bg += v.Bg.G; bb += v.Bg.B;
                        chars[count++] = v.Ch;
                        n++;
                    }
                    else
                    {
                        chars[count++] = ' ';
                    }
                }

            if (n == 0) return new Cell(' ', Rgb.White, Rgb.Black);

            var ch = ModeChar(chars);
            var avgFg = new Rgb((byte)(fr / n), (byte)(fg / n), (byte)(fb / n));
            var avgBg = new Rgb((byte)(br / n), (byte)(bg / n), (byte)(bb / n));
            return new Cell(ch, avgFg, avgBg);
        }
    }

    private static char ModeChar(char[] arr)
    {
        var dict = new Dictionary<char, int>();
        foreach (var c in arr) dict[c] = dict.TryGetValue(c, out var n) ? n + 1 : 1;
        var best = -1; var bestC = ' ';
        foreach (var kv in dict) if (kv.Value > best) { best = kv.Value; bestC = kv.Key; }
        return bestC;
    }

    public static void DrawRectWorld(CellBuffer buf, Viewport vp, double x, double y, double w, double h, char ch, Rgb fg, Rgb bg, bool enabled)
    {
        if (!enabled) return;
        var (sx0, sy0) = vp.WorldToScreen(x, y);
        var (sx1, sy1) = vp.WorldToScreen(x + w, y + h);
        int x0 = Math.Min(sx0, sx1 - 1), x1 = Math.Max(sx0, sx1 - 1);
        int y0 = Math.Min(sy0, sy1 - 1), y1 = Math.Max(sy0, sy1 - 1);
        for (var sy = y0; sy <= y1; sy++)
            for (var sx = x0; sx <= x1; sx++)
                buf.TrySet(sx, sy, new Cell(ch, fg, bg));
    }

    public static void DrawCircleWorld(CellBuffer buf, Viewport vp, double cx, double cy, double r, char ch, Rgb fg, Rgb bg, bool enabled)
    {
        if (!enabled) return;
        var (scx, scy) = vp.WorldToScreen(cx, cy);
        var rr = (int)Math.Round(r * vp.Zoom);
        for (var sy = scy - rr; sy <= scy + rr; sy++)
            for (var sx = scx - rr; sx <= scx + rr; sx++)
            {
                var (wx, wy) = vp.ScreenToWorld(sx, sy);
                var d2 = (wx - cx) * (wx - cx) + (wy - cy) * (wy - cy);
                if (Math.Abs(Math.Sqrt(d2) - r) <= 0.6 / vp.Zoom)
                    buf.TrySet(sx, sy, new Cell(ch, fg, bg));
            }
    }

    public static void DrawTextScreen(CellBuffer buf, int sx, int sy, string text, Rgb fg, Rgb bg, bool enabled)
    {
        if (!enabled) return;
        PutText(buf, sx, sy, text, fg, bg);
    }

    internal static void PutText(CellBuffer buf, int sx, int sy, string text, Rgb fg, Rgb bg)
    {
        for (var i = 0; i < text.Length; i++)
            buf.TrySet(sx + i, sy, new Cell(text[i], fg, bg));
    }

    public static void PutTextKeepBg(CellBuffer buf, int sx, int sy, string text, Rgb fg)
    {
        for (var i = 0; i < text.Length; i++)
        {
            var x = sx + i;
            if ((uint)x >= (uint)buf.Width || (uint)sy >= (uint)buf.Height) break;
            var cur = buf[x, sy];
            buf.TrySet(x, sy, new Cell(text[i], fg, cur.Bg));
        }
    }

    public static void DrawTooltipBox(CellBuffer buf, int x0, int y0, string text,
                                      byte bgAlpha = 180, byte borderAlpha = 220)
    {
        var lines = SplitLines(text);
        DrawTooltipBox(buf, x0, y0, lines, bgAlpha, borderAlpha);
    }

    public static void DrawTooltipBox(CellBuffer buf, int x0, int y0, IReadOnlyList<string> lines, byte bgAlpha = 180, byte borderAlpha = 220)
    {
        int W = buf.Width, H = buf.Height;
        if (lines == null || lines.Count == 0) return;

        const int padX = 2;
        var maxLineLen = 0;
        for (var i = 0; i < lines.Count; i++)
            if (lines[i] != null)
                maxLineLen = Math.Max(maxLineLen, lines[i].Length);

        var w = Math.Clamp(maxLineLen + padX * 2, 6, W);

        if (x0 + w >= W) x0 = Math.Max(0, W - w - 1);
        var h = Math.Min(lines.Count, Math.Max(1, H - 1 - y0));
        if (h < lines.Count) h = lines.Count;
        if (y0 + h >= H - 1) y0 = Math.Max(0, H - 1 - h);

        var bg = new Rgb(20, 20, 20);
        var bd = new Rgb(255, 255, 255);
        var fg = new Rgb(245, 245, 245);

        var opaque = bgAlpha == 255 && borderAlpha == 255;
        var opaqueMode = opaque || !buf.AlphaBlendEnabled;


        FrameDrawer.DrawSingleFrame(x0, y0, x0 + w, y0 + h + 1, buf, bd, borderAlpha, bg, 255, false);

        for (var row = 1; row <= lines.Count; row++)
        {
            var y = y0 + row;
            if ((uint)y >= (uint)H) break;

            for (var x = 1; x < w; x++)
            {
                if (opaqueMode)
                {
                    buf.TrySet(x0 + x, y, new Cell(' ', fg, bg));
                }
                else
                {
                    buf.BlendBgAndFg(x0 + x, y, bg, bgAlpha, bg, bgAlpha);
                }
            }

            // text
            var line = lines[row - 1] ?? string.Empty;
            var inner = Math.Max(0, w - padX * 2);
            if (inner > 0 && line.Length > inner) line = line.AsSpan(0, inner).ToString();

            if (opaqueMode)
                PutText(buf, x0 + padX, y, line, fg, bg);
            else
                PutTextKeepBg(buf, x0 + padX, y, line, fg);
        }
    }

    private static List<string> SplitLines(string s)
    {
        if (string.IsNullOrEmpty(s)) return new List<string>();
        // support \r\n, \n, \r
        return new List<string>(s.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'));
    }

    private static string Truncate(string s, int len)
        => len <= 0 || s.Length <= len ? s : s.AsSpan(0, len).ToString();
}
