using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
enum MessageIds
{
    Clear,
    BitmapImage,
    Diagram,
}

static class Visualizer
{
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

        bool isProgramStarting = false;

        // start process if does not exist already
        if (Process.GetProcessesByName("AdventOfCodeVisualizerWPF").Length == 0)
        {
            Console.WriteLine($"{CC.Att}===>{CC.Clr} Starting Visualizer process...");
            Process.Start($"{Program.RootPath}\\..\\AdventOfCodeVisualizerWPF\\bin\\Debug\\AdventOfCodeVisualizerWPF.exe");
            isProgramStarting = true;
        }

        Console.Write($"{CC.Att}===>{CC.Clr} Opening Visualizer connection...");

        int counter = 0;
        while (true)
        {
            try
            {
                counter++;
                Thread.Sleep(1000);    //TODO: remove
                __visualizerTcp = new TcpClient("127.0.0.1", 58739); // hard-coded values :)
                __visualizerTcp.NoDelay = true;
                __visualizerStream = __visualizerTcp.GetStream();
                Console.WriteLine("done.");
                ClearAll();
                break;
            }
            catch
            {
                if (isProgramStarting && counter < 5)
                {
                    Console.Write('.');
                    continue;
                }
                else
                    Console.WriteLine($"{CC.Err}failed, visualization will not be available{CC.Clr}.");
            }
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

    public static void SendMap2dSpan<T>(Map2DSpan<T> map, Func<T, Map2DSpan<T>, int, int, Color> mapper, int frame = -1, int window = 0, string additionalMessage = null)
    {
        using var bitmap = new Bitmap(map.Width, map.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        // Lock the bitmap's bits.  
        var rect = new Rectangle(0, 0, map.Width, map.Height);
        var bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
        // Get the address of the first line.
        var ptr = bmpData.Scan0;

        // Declare an array to hold the bytes of the bitmap.
        var bytes = Math.Abs(bmpData.Stride) * map.Height;
        var rgbValues = new byte[bytes];
        var span = rgbValues.AsSpan();
        var offset = 0;
        for (var y = 0; y < map.Height; y++)
        {
            for (var x = 0; x < map.Width; x++)
            {
                var c = mapper(map[x, y], map, x, y);
                span[offset] = c.B; offset++;
                span[offset] = c.G; offset++;
                span[offset] = c.R; offset++;
                span[offset] = c.A; offset++;
            }
        }

        // Copy the RGB values back to the bitmap
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

        // Unlock the bits.
        bitmap.UnlockBits(bmpData);

        SendBitmap(bitmap, frame, window, additionalMessage);
    }

    public static void SendRaw(ReadOnlySpan<byte> span)
    {
        if (__visualizerStream == null || __visualizerTcp == null)
            return;

        __visualizerTcp.Client.Send(span);
        Thread.Sleep(20);
    }
    static int __test = 0;
}



class DiagramContext
{
    [StructLayout(LayoutKind.Sequential)]
    struct DiagramHeader
    {
        public static int Size = Marshal.SizeOf(typeof(DiagramHeader));
        public int nodes;
        public int connectors;
        public int nodesOffset;
        public int connectorsOffset;
        public int diagramAutoLayout;
    }
    struct DiagramNode
    {
        public static int Size = Marshal.SizeOf(typeof(DiagramNode));
        public int x;
        public int y;
        public int width;
        public int height;
        public int shape;
        public int id;
        public int nameStringLen;
    }
    struct DiagramConnector
    {
        public static int Size = Marshal.SizeOf(typeof(DiagramConnector));
        public int SourceId;
        public int TargetId;
    }

    List<Node> nodes = new();
    List<Connector> conns = new();

    class Node
    {
        internal int x;
        internal int y;
        internal int width;
        internal int height;
        internal int shape;
        internal int id;
        internal string text;
        internal int textSize;
    }
    class Connector
    {
        internal int srcDd;
        internal int dstId;
        internal string text;
        internal int textSize;
    }

    public void AddNode(int id, string text, int x, int y, int width, int height, int shape) // other data here later
    {
        if (nodes.Any(n => n.id == id)) throw new ArgumentException("Node Id must be unique across Diagram Context");

        nodes.Add(new Node { id = id, text = text, x = x, y = y, width = width, height = height, shape = shape, textSize = Encoding.UTF8.GetByteCount(text) });
    }
    public void AddConnector(int sourceId, int dstId, string text) // other data here later
          => conns.Add(new Connector { srcDd = sourceId, dstId = dstId, text = text, textSize = Encoding.UTF8.GetByteCount(text) });

    public unsafe void Send(int frame = -1, int window = 0, string additionalMessage = null)
    {
        var size = DiagramHeader.Size
            + DiagramNode.Size * nodes.Count
            + nodes.Sum(n => n.textSize)
            + DiagramConnector.Size * conns.Count
            + conns.Sum(c => c.textSize)
            ;

        var stringData = new byte[0];
        if (additionalMessage != null)
            stringData = Encoding.UTF8.GetBytes(additionalMessage);


        var data = new byte[size + 14 + stringData.Length];
        var sp = data.AsSpan();
        var frameIndex = (ushort)(frame == -1 ? 0x8000 : (ushort)frame);
        var windowIndex = (ushort)window;
        

        BitConverter.TryWriteBytes(sp, size + 14 + stringData.Length);
        BitConverter.TryWriteBytes(sp[4..], (int)MessageIds.Diagram);
        BitConverter.TryWriteBytes(sp[8..], frameIndex);
        BitConverter.TryWriteBytes(sp[10..], windowIndex);
        BitConverter.TryWriteBytes(sp[12..], (ushort)stringData.Length);
        if (stringData.Length > 0)
        {
            stringData.AsSpan().CopyTo(sp[14..]);
        }


        fixed (byte* p = data)
        {
            var offset = 14 + stringData.Length;

            var dh = (DiagramHeader*)(p + offset);

            dh->connectors = conns.Count;
            dh->nodes = nodes.Count;
            dh->nodesOffset = DiagramHeader.Size;
            dh->connectorsOffset = DiagramHeader.Size + DiagramNode.Size * nodes.Count + nodes.Sum(n => n.textSize);
            dh->diagramAutoLayout = 1;

            offset += DiagramHeader.Size;

            for (var i = 0; i < nodes.Count; i++)
            {
                var dn = (DiagramNode*)(p + offset);
                var n = nodes[i];
                dn->id = n.id;
                dn->shape = n.shape;
                dn->x = n.x;
                dn->y = n.y;
                dn->width = n.width;
                dn->height = n.height;
                dn->nameStringLen = n.textSize;
                Encoding.UTF8.GetBytes(n.text, 0, n.text.Length, data, offset + DiagramNode.Size);
                offset += DiagramNode.Size + dn->nameStringLen;
            }

            for (var i = 0; i < conns.Count; i++)
            {
                var dc = (DiagramConnector*)(p + offset);
                var n = conns[i];
                dc->SourceId = n.srcDd;
                dc->TargetId = n.dstId;
                //Encoding.UTF8.GetBytes(n.text, 0, n.text.Length, data, offset + DiagramConnector.Size);
                //offset += DiagramConnector.Size + dn->nameStringLen;
                offset += DiagramConnector.Size;
            }
        }
        Visualizer.SendRaw(data);
    }


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
    private static int Clamp(int c) => Math.Max(0, Math.Min(c, 255));
    public static Color Clr(int r, int g, int b) => Color.FromArgb(Clamp(255), Clamp(r), Clamp(g), Clamp(b));
    public static Color Clr(int a, int r, int g, int b) => Color.FromArgb(Clamp(a), Clamp(r), Clamp(g), Clamp(b));

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



