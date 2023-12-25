using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using AdventOfCodeVisualizer.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;

namespace AdventOfCodeVisualizer.Views;

enum MessageIds
{
    BitmapImage,
}

public sealed partial class MainPage : Page
{
    private static MainPage _instance;
    public static void OnMessage(byte[] data)
    {
        _instance.OnMessageInt(data);
    }

    private void OnMessageInt(byte[] data)
    {
        // first 4 bytes are size, ignore
        var offset = 4;
        // next 4 bytes are message id
        var msgId = (MessageIds)BitConverter.ToInt32(data.AsSpan()[4..]);

        //var ds = data.Length > 8 ? data.AsSpan()[8..] : null;
        switch (msgId)
        {
            case MessageIds.BitmapImage:
                OnMessage_BitmapImage(data);
                break;
            default:
                Debug.Fail("Unknown message " + msgId);
                break;
        }
    }

    private List<(byte[], int, string)> Frames = new();


    private async void SetImageFromBuffer(byte[] data, int offset)
    {
        // produce bitmap itself
        // get buffer as RAB
        var imgBuff = data.AsBuffer(offset, data.Length - offset).AsStream().AsRandomAccessStream();
        // decode image
        var decoder = BitmapDecoder.CreateAsync(imgBuff).GetAwaiter().GetResult();
        imgBuff.Seek(0);


        // create a bitmap transform.
        // you'll have to call this with width & height corresponding to desired Image's with & height
        var transform = new BitmapTransform();
        transform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;
        transform.ScaledWidth = (uint)image.Width;
        transform.ScaledHeight = (uint)image.Height;

        using var bmp = await decoder.GetSoftwareBitmapAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, transform, ExifOrientationMode.RespectExifOrientation, ColorManagementMode.ColorManageToSRgb);
        var source = new SoftwareBitmapSource();
        await source.SetBitmapAsync(bmp);

        // create bitmap
        //var output = new WriteableBitmap((int)decoder.PixelHeight, (int)decoder.PixelWidth);
        //output.SetSource(imgBuff);
        image.Source = source;

       
    }

    private void slider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var index = (int)e.NewValue - 1;
        if (index >= 0 && index < Frames.Count)
        {
            SetImageFromBuffer(Frames[index].Item1, Frames[index].Item2);
            message.Text = Frames[index].Item3;
        }
    }

    private void OnMessage_BitmapImage(byte[] data)
    {
        // frame index (0 or more)
        var frameIndex = BitConverter.ToUInt16(data, 8);

        // if last bit in frame index is set,
        // add as next frame - adds to the frame history, regardless of current frame count
        var asLastFrame = (frameIndex & 0x8000) == 0x8000;
        frameIndex = (ushort)(frameIndex & ~0x8000);


        // window index: used for multiple windows. 0: main window,
        // if more than 0, other windows will open and show that image
        var windowIndex = BitConverter.ToUInt16(data, 8 + 2);


        // there may also be a string message in there
        var strmsgSize = BitConverter.ToUInt16(data, 8 + 4);
        var stringMessage = "";
        if (strmsgSize > 0)
        {
            stringMessage = Encoding.UTF8.GetString(data, 8 + 6, strmsgSize);
        }


        if (asLastFrame)
        {
            Frames.Add((data, (ushort)(8 + 6 + strmsgSize), stringMessage));
            slider.Maximum = Frames.Count;
            slider.Minimum = 1;
        }
        
        if (slider.Value == Frames.Count - 1)
            slider.Value = Frames.Count;

        ////image.Source = output;

        //if (asLastFrame)
        //{
        //    Frames.Add(output);

        //    slider.Maximum = Frames.Count;
        //    slider.Minimum = 1;
        //    if (image.Source == null)
        //    {
        //        image.Source = output;
        //    }
        //}
        //else
        //{
        //    if (frameIndex >= Frames.Count)
        //    {
        //        Frames.AddRange(Enumerable.Range(Frames.Count, frameIndex).Select(i => (BitmapSource)null!));
        //    }
        //    Frames[frameIndex] = output;
        //    if (slider.Value == frameIndex + 1)
        //    {
        //        image.Source = output;
        //    }
        //    slider.Maximum = Frames.Count;
        //    slider.Minimum = 1;
        //}
    }


    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        _instance = this;
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

   
}
