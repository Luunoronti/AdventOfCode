
using AdventOfCode2023;
using System.Diagnostics;
using System.Text;
using StringSpan = System.ReadOnlySpan<char>;

static partial class Log
{
    public class MapContext : IDisposable
    {
        private int _consoleTop;
        private readonly char[] _chars;
        private readonly int[] _foregrounds;
        private readonly int[] _background;
        private Func<(byte, byte, byte), int, int, (byte, byte, byte)>? _backgroundPostProcess; // in, x, y, out
        private Func<(byte, byte, byte), int, int, (byte, byte, byte)>? _foregroundPostProcess; // in, x, y, out

        internal MapContext(int width, int heigth)
        {
            Width = width;
            Height = heigth;
            var len = Width * Height;
            _chars = new char[len];
            _foregrounds = new int[len];
            _background = new int[len];
        }
        public int Width { get; }
        public int Height { get; }

        public void SetContent(StringSpan content, bool replaceDots = false)
        {
            var charSpan = _chars.AsSpan();

            if (replaceDots)
            {
                if (content.Length != charSpan.Length) throw new ArgumentOutOfRangeException(nameof(content));
                for (int i = 0; i < charSpan.Length; i++)
                {
                    var c = content[i];
                    // replace dot because our terminal (and Cascadia Nerd Cove font)
                    // shows 3 dots (...) as it's own glyph, which makes it a bit harder to read
                    if (c == '.') c = CC.DotReplacement;
                    charSpan[i] = c;
                }
            }
            else
            {
                content.CopyTo(charSpan);
            }
        }
        public void SetColors(Span<int> foreground, Span<int> background)
        {
            foreground.CopyTo(_foregrounds.AsSpan());
            background.CopyTo(_background.AsSpan());
        }
        public void SetForegroundColors(Span<int> foreground)
        {
            foreground.CopyTo(_foregrounds.AsSpan());
        }
        public void SetBackgroundColors(Span<int> background)
        {
            background.CopyTo(_background.AsSpan());
        }

        public void SetColor(int x, int y, int foreground, int background)
        {
            _foregrounds[y * Width + x] = foreground;
            _background[y * Width + x] = background;
        }
        public void SetColor(int x, int y, byte foreR, byte foreG, byte foreB, byte bkR, byte bkG, byte bkB)
        {
            _foregrounds.AsSpan().SetAt(foreR, foreG, foreB, 0, x, y, Width, Height, out _);
            _background.AsSpan().SetAt(bkR, bkG, bkB, 0, x, y, Width, Height, out _);
        }
        public void SetBackgroundColor(int x, int y, byte r, byte g, byte b)
        {
            _background.AsSpan().SetAt(r, g, b, 0, x, y, Width, Height, out _);
        }
        public void SetForegroundColor(int x, int y, byte r, byte g, byte b)
        {
            _foregrounds.AsSpan().SetAt(r, g, b, 0, x, y, Width, Height, out _);
        }
        public void FillForegroundColor(int color) => _foregrounds.AsSpan().Fill(color);
        public void FillForegroundColor(byte r, byte g, byte b) => _foregrounds.AsSpan().Fill(r | g << 8 | b << 16);
        public void FillBackgroundColor(int color) => _background.AsSpan().Fill(color);
        public void FillBackgroundColor(byte r, byte g, byte b) => _background.AsSpan().Fill(r | g << 8 | b << 16);

        public void SetBackgroundPostProcess(Func<(byte, byte, byte), int, int, (byte, byte, byte)> postProcessFunction) => _backgroundPostProcess = postProcessFunction;
        public void SetForegroundPostProcess(Func<(byte, byte, byte), int, int, (byte, byte, byte)> postProcessFunction) => _foregroundPostProcess = postProcessFunction;
        public void Draw()
        {
            // post background
            if (_backgroundPostProcess != null)
            {
                var span = _background.AsSpan();
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        var c = span.GetBytesAt(x, y, Width, Height, out _);
                        var p = _backgroundPostProcess((c.a, c.b, c.c), x, y);
                        span.SetAt(p.Item1, p.Item2, p.Item3, 0, x, y, Width, Height, out _);
                    }
                }
            }
            // post foreground
            if (_foregroundPostProcess != null)
            {
                var span = _foregrounds.AsSpan();
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        var c = span.GetBytesAt(x, y, Width, Height, out _);
                        var p = _foregroundPostProcess((c.a, c.b, c.c), x, y);
                        span.SetAt(p.Item1, p.Item2, p.Item3, 0, x, y, Width, Height, out _);
                    }
                }
            }

            DrawRectangularMap(_chars, _foregrounds, _background, Width, Height);
        }

        public void DrawAndWait(int time)
        {
            var sw = Stopwatch.StartNew();
            Draw();
            sw.Stop();
            var remaining = time - sw.ElapsedMilliseconds;
            if (remaining > 0)
            {
                Thread.Sleep((int)remaining);
            }
        }

        public void Init()
        {
            _consoleTop = Console.CursorTop;
            Write(CC.CursorHide);
        }
        public void Close()
        {
            Write(CC.CursorShow);
        }

        private void DrawRectangularMap(StringSpan content, Span<int> foreground, Span<int> background, int width, int height)
        {
            if (Enabled == false) return;
            var sb = new StringBuilder(width * height * 40 );
            var lastR = -1; var lastG = -1; var lastB = -1;
            var lastBR = -1; var lastBG = -1; var lastBB = -1;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var c = content.GetAt(x, y, width, height, out _);
                    var (fr, fg, fb, _) = foreground.GetBytesAt(x, y, width, height, out _);
                    var (br, bg, bb, _) = background.GetBytesAt(x, y, width, height, out _);

                    if (lastR != fr || lastG != fg || lastB != fb)
                    {
                        sb.Append($"\u001b[38;2;{fr};{fg};{fb}m");
                        lastR = fr;
                        lastG = fg;
                        lastB = fb;
                    }
                    if (lastBR != br || lastBG != bg || lastBB != bb)
                    {
                        sb.Append($"\u001b[48;2;{br};{bg};{bb}m");
                        lastBR = br;
                        lastBG = bg;
                        lastBB = bb;
                    }
                    sb.Append(c);
                }
                sb.AppendLine($"{CC.Clr}");
                lastR = lastG = lastB = -1;
                lastBR = lastBG = lastBB = -1;
            }
            Console.CursorTop = _consoleTop;
            Console.CursorLeft = 0;
            WriteLine(sb.ToString());
        }

        public void Dispose()
        {
            Close();
        }
    }

}



