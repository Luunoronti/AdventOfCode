using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

static class Visualizer
{
    enum MessageIds
    {
        Clear,
        BitmapImage,
    }

    // we will use TcpClient to send data over to the client
    // but, we are actually the client, we start the process
    // if it does not exist, and try to connect.

    private static TcpClient? __visualizerTcp;
    private static NetworkStream? __visualizerStream;
    public static void CloseVisualizerPipe()
    {
        if (__visualizerTcp != null)
        {
            __visualizerStream?.Dispose();
            __visualizerStream = null;

            Console.Write($"{CC.Att}===>{CC.Clr} Closing Visualizer connection...");
            __visualizerTcp.Dispose();
            __visualizerTcp = null;
            Console.WriteLine("done.");
        }
    }
    public static void StartVisualizer()
    {
        // if it has already been started from this process, do nothing
        if (__visualizerTcp != null)
        {
            return;
        }

        // start process if does not exist already
        if (Process.GetProcessesByName("AdventOfCodeVisualizer").Length == 0)
        {
            Console.WriteLine($"{CC.Att}===>{CC.Clr} Starting Visualizer process...");

            //Process.Start($"{Program.RootPath}\\..\\AdventOfCodeVisualizer\\bin\\Debug\\AdventOfCodeVisualizer.exe");
            Process.Start($"{Program.RootPath}\\..\\AdventOfCodeVisualizer\\bin\\x86\\Release\\net7.0-windows10.0.19041.0\\win10-x86\\AdventOfCodeVisualizer.exe");
        }

        Console.Write($"{CC.Att}===>{CC.Clr} Opening Visualizer connection...");
        try
        {
            Thread.Sleep(1000);    //TODO: remove
            __visualizerTcp = new TcpClient("127.0.0.1", 58739); // hard-coded values :)
            __visualizerTcp.NoDelay = true;

            __visualizerStream = __visualizerTcp.GetStream();
            Console.WriteLine("done.");

            ClearAll();
        }
        catch
        {
            Console.WriteLine($"{CC.Err}failed, visualization will not be available{CC.Clr}.");
        }

    }
    public static void ClearAll()
    {
        if (__visualizerStream == null)
            return;

        var data = new byte[8];
        var span = data.AsSpan();

        // for now, test only
        BitConverter.TryWriteBytes(span, 8);
        BitConverter.TryWriteBytes(span[4..], (int)MessageIds.Clear);
        __visualizerStream.Write(span[..8]);
    }


    public static void SendBitmap(Bitmap bitmap, int frame = -1, int window = 0, string additionalMessage = null)
    {
        if (__visualizerStream == null || __visualizerTcp == null)
            return;

        var frameIndex = (ushort)(frame == -1 ? 0x8000 : (ushort)frame);
        var windowIndex = (ushort)window;

        using var mem = new MemoryStream();
        bitmap.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
        mem.Position = 0;

        var stringData = new byte[0];
        if (additionalMessage != null)
            stringData = Encoding.UTF8.GetBytes(additionalMessage);

        var size = (int)(8 + 6 + stringData.Length + mem.Length);
        var data = new byte[size];
        var span = data.AsSpan();

        // for now, test only
        BitConverter.TryWriteBytes(span, size);
        BitConverter.TryWriteBytes(span[4..], (int)MessageIds.BitmapImage);
        BitConverter.TryWriteBytes(span[8..], frameIndex);
        BitConverter.TryWriteBytes(span[10..], windowIndex);
        BitConverter.TryWriteBytes(span[12..], (ushort)stringData.Length);

        if (stringData.Length > 0)
        {
            stringData.AsSpan().CopyTo(span[14..]);
        }

        mem.Read(span[(14 + stringData.Length)..]);
        //__visualizerStream.Write(span[..size]);
        __visualizerTcp.Client.Send(span[..size]);
        Thread.Sleep(20);

    }
    public static void SendBitmap(BitmapContext bitmap, int frame = -1, int window = 0, string additionalMessage = null)
    {
        if (__visualizerStream == null)
            return;

        SendBitmap(bitmap.Bitmap, frame, window, additionalMessage);

        Console.CursorLeft = 0;
        Console.Write(++__test);
    }


    static int __test = 0;
}

class BitmapContext : IDisposable
{
    private Bitmap _bitmap;
    private Graphics _graphics;
    private int _scalingFactor;
    private int _width;
    private int _heigth;

    internal Bitmap Bitmap => _bitmap;

    private Color _selectedColor;

    public Color SelectedColor
    {
        get => _selectedColor;
        set => _selectedColor = value;
    }
    public int ScalingFactor
    {
        get => _scalingFactor;
        set => _scalingFactor = value;
    }

    #region Color
    private static int Clr(int c) => Math.Max(0, Math.Min(c, 255));
    private static Color Clr(int r, int g, int b) => Color.FromArgb(Clr(255), Clr(r), Clr(g), Clr(b));
    private static Color Clr(int a, int r, int g, int b) => Color.FromArgb(Clr(a), Clr(r), Clr(g), Clr(b));

    public void SetSelectedColor(int a, int r, int g, int b) => SelectedColor = Clr(a, r, g, b);
    public void SetSelectedColor(int r, int g, int b) => SelectedColor = Clr(r, g, b);
    public void SetSelectedColor(Color color) => SelectedColor = color;
    #endregion
    
    internal BitmapContext(int width, int height, Color clearColor, int scalingFactor = 1)
    {
        _width = width;
        _heigth = height;

        _scalingFactor = scalingFactor;
        _bitmap = new Bitmap(width * scalingFactor, height * scalingFactor, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        _graphics = Graphics.FromImage(_bitmap);
        _graphics.Clear(clearColor);
        _selectedColor = Color.White;
    }
    internal BitmapContext(int width, int height, int scalingFactor = 1)
        : this(width, height, Color.Transparent, scalingFactor)
    {
    }

    public void Dispose()
    {
        _graphics?.Dispose();
        _bitmap?.Dispose();
    }

    private static SolidBrush GetSolidBrush(Color color) => _solidBruses.TryGetValue(color, out var brush) ? brush : _solidBruses[color] = new SolidBrush(color);
    private static Pen GetPen(Color color) => _pens.TryGetValue(color, out var brush) ? brush : _pens[color] = new Pen(color);

    #region Rectangle
    public void FillRectangle(int x, int y, int width, int height) => FillRectangle(new Rectangle(x, y, width, height), _selectedColor);
    public void FillRectangle(int x, int y, int width, int height, Color color) => FillRectangle(new Rectangle(x, y, width, height), color);
    public void FillRectangle(Rectangle rectangle) => FillRectangle(rectangle, _selectedColor);
    public void FillRectangle(Rectangle rectangle, Color color) => _graphics.FillRectangle(GetSolidBrush(color), new Rectangle(rectangle.X * _scalingFactor, rectangle.Y * _scalingFactor, rectangle.Width * _scalingFactor, rectangle.Height * _scalingFactor));

    public void DrawRectangle(int x, int y, int width, int height) => DrawRectangle(new Rectangle(x, y, width, height), _selectedColor);
    public void DrawRectangle(int x, int y, int width, int height, Color color) => DrawRectangle(new Rectangle(x, y, width, height), color);
    public void DrawRectangle(Rectangle rectangle) => DrawRectangle(rectangle, _selectedColor);
    public void DrawRectangle(Rectangle rectangle, Color color) => _graphics.DrawRectangle(GetPen(color), new Rectangle(rectangle.X * _scalingFactor, rectangle.Y * _scalingFactor, rectangle.Width * _scalingFactor, rectangle.Height * _scalingFactor));
    #endregion


    #region Set pixel
    public void SetPixel(int x, int y, Color color) => FillRectangle(new Rectangle(x, y, 1, 1), color);
    public void SetPixel(int x, int y) => FillRectangle(new Rectangle(x, y, 1, 1), _selectedColor);
    public void SetPixel(int x, int y, int r, int g, int b) => FillRectangle(new Rectangle(x, y, 1, 1), Clr(r, g, b));

    public void SetPixel(int pos, Color color) => SetPixel(pos % _width, pos / _width, color);
    public void SetPixel(int pos) => SetPixel(pos % _width, pos / _width, _selectedColor);
    public void SetPixel(int pos, int r, int g, int b) => SetPixel(pos % _width, pos / _width, Clr(r, g, b));
    #endregion

    private static readonly Dictionary<Color, SolidBrush> _solidBruses = new();
    private static readonly Dictionary<Color, Pen> _pens = new();
    public static Color RandomizeColor(int minR = 0, int minG = 0, int minB = 0, int steps = 1)
    {
        var random = new Random();
        return Clr(minR + random.Next(6) * 20, minG + random.Next(6) * 20, minB + random.Next(6) * 20);
    }
}



