using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

static class Visualizer
{
    enum MessageIds
    {
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
            //Console.WriteLine($"{CC.Att}===>{CC.Clr} Starting Visualizer process...");
            //Process.Start($"{Program.RootPath}\\..\\AdventOfCodeVisualizer\\bin\\Debug\\AdventOfCodeVisualizer.exe");
        }

        Console.Write($"{CC.Att}===>{CC.Clr} Opening Visualizer connection...");
        try
        {
            Thread.Sleep(1000);    //TODO: remove
            __visualizerTcp = new TcpClient("127.0.0.1", 58739); // hard-coded values :)
            __visualizerStream = __visualizerTcp.GetStream();
            Console.WriteLine("done.");

        }
        catch
        {
            Console.WriteLine($"{CC.Err}failed, visualization will not be available{CC.Clr}.");
        }

    }

    private static byte[] outputBuff = new byte[1024 * 1024 * 10];


    public static void SendBitmap(Bitmap bitmap, int frame = -1, int window = 0, string additionalMessage = null)
    {
        if (__visualizerStream == null)
            return;

        var span = outputBuff.AsSpan();
        var frameIndex = (ushort)(frame == -1 ? 0x8000 : (ushort)frame);
        var windowIndex = (ushort)window;

        using var mem = new MemoryStream();
        bitmap.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
        mem.Position = 0;

        var stringData = new byte[0];
        if (additionalMessage != null)
            stringData = Encoding.UTF8.GetBytes(additionalMessage);

        var size = (int)(8 + 6 + stringData.Length + mem.Length);
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
        __visualizerStream.Write(span[..size]);



    }


}



