using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;

namespace AdventOfCodeVisualizerWPF
{
    delegate void OnMessage(byte[] data);
    enum MessageIds
    {
        Clear,
        BitmapImage,
        Diagram,
    }

    enum FrameType
    {
        Bitmap,
        Diagram,
    }
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;
        public static MainWindow Instance => _instance;

        // this will need more.
        // because we can support more than just 
        // bitmap images
        private List<(byte[], FrameType, string)> Frames = new List<(byte[], FrameType, string)>();

        public void DispatchMessage(byte[] data) => this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new OnMessage(OnMessageInt), data);

        public MainWindow()
        {
            _instance = this;
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            scrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
            scrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            scrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            scrollViewer.MouseMove += OnMouseMove;
            //  scaleslider.ValueChanged += OnSliderValueChanged;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AocVis.Initialize();
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);

            currentGraphViewer = new GraphViewer();
            currentGraphViewer.GraphCanvas.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(32, 32, 32));

            graphPanel = new DockPanel();
            graphPanel.ClipToBounds = true;
            diagramGrid.Children.Add(graphPanel);
            currentGraphViewer.BindToPanel(graphPanel);
        }



        #region Frame handling
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetCurrentFrame((int)e.NewValue - 1);
        }
        private void SetCurrentFrame(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex >= Frames.Count)
                return;

            switch (Frames[frameIndex].Item2)
            {
                case FrameType.Bitmap:
                    SetImageFromBuffer(Frames[frameIndex].Item1);
                    break;
                case FrameType.Diagram:
                    PrepareAndShowDiagram(Frames[frameIndex].Item1);
                    break;
            }
            message.Text = Frames[frameIndex].Item3;
        }
        #endregion

        #region Set visibility of previewers
        private void SetImageVisible()
        {
            image.Visibility = Visibility.Visible;
            diagramGrid.Visibility = Visibility.Collapsed;
        }
        private void SetDiagramVisible()
        {
            diagramGrid.Visibility = Visibility.Visible;
            image.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Diagram preview
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
            public int shape;
            public int id;
            public int nameStringLen;
            public byte colorR;
            public byte colorG;
            public byte colorB;
        }
        struct DiagramConnector
        {
            public static int Size = Marshal.SizeOf(typeof(DiagramConnector));
            public int SourceId;
            public int TargetId;
            public byte colorR;
            public byte colorG;
            public byte colorB;
            public int Weight;
            public int LabelLen;
        }

        private GraphViewer currentGraphViewer;
        private DockPanel graphPanel;
        private bool firstGraphIsSet;

        private unsafe void PrepareAndShowDiagram(byte[] data)
        {
            Graph graph = new Graph();
            graph.Attr.BackgroundColor = new Microsoft.Msagl.Drawing.Color(32, 32, 32);
            fixed (byte* p = data)
            {
                var dh = (DiagramHeader*)p;

                var nodesCount = dh->nodes;
                var connectorsCount = dh->connectors;

                for (int i = dh->nodesOffset, n = 0; n < dh->nodes; n++, i += DiagramNode.Size)
                {
                    var pnode = (DiagramNode*)(p + i);
                    var name = Encoding.UTF8.GetString(data, i + DiagramNode.Size, pnode->nameStringLen);

                    var node = new Microsoft.Msagl.Drawing.Node(pnode->id.ToString()) { LabelText = name, };
                    node.Attr.Color = new Microsoft.Msagl.Drawing.Color(pnode->colorR, pnode->colorG, pnode->colorB);
                    node.Label.FontColor = node.Attr.Color;
                    //node.Attr.Shape = Shape.
                    graph.AddNode(node);

                    i += pnode->nameStringLen;
                }
                for (int i = dh->connectorsOffset, n = 0; n < dh->connectors; n++, i += DiagramConnector.Size)
                {
                    var pconn = (DiagramConnector*)(p + i);
                    var name = Encoding.UTF8.GetString(data, i + DiagramConnector.Size, pconn->LabelLen);

                    var e = graph.AddEdge(pconn->SourceId.ToString(), name, pconn->TargetId.ToString());
                    e.Attr.Color = new Microsoft.Msagl.Drawing.Color(pconn->colorR, pconn->colorG, pconn->colorB);
                    e.Label.FontColor = e.Attr.Color;

                    e.Attr.Weight = pconn->Weight;
                    //e.Attr.Separation = 10;

                    i += pconn->LabelLen;
                }

                //if (dh->diagramAutoLayout == 1)  // we'll need more control over this, but add that later
                //{
                //    //Initialize LayoutManager to SfDiagram
                //    diagram.LayoutManager = new LayoutManager()
                //    {
                //        //Initialize Layout for LayoutManager  
                //        Layout = new DirectedTreeLayout()
                //        {
                //            Type = LayoutType.Hierarchical,
                //            Orientation = TreeOrientation.TopToBottom,
                //            HorizontalSpacing = 30,
                //            VerticalSpacing = 50,
                //            AvoidSegmentOverlapping = true,
                //        }
                //    };
                //}
            }
            graph.Attr.LayerDirection = LayerDirection.LR;
            graph.LayoutAlgorithmSettings.EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.Rectilinear;
            graph.LayoutAlgorithmSettings.NodeSeparation = 20;
            graph.LayoutAlgorithmSettings.LiftCrossEdges = true;
            graph.LayoutAlgorithmSettings.EdgeRoutingSettings.Padding = 5;


            var tr = currentGraphViewer.Transform;
            currentGraphViewer.Graph = graph;
            if (firstGraphIsSet)
                currentGraphViewer.Transform = tr;
            firstGraphIsSet = true;

            SetDiagramVisible();
        }
        void WpfApplicationSample_MouseDown(object sender, MsaglMouseEventArgs e)
        {
            //statusTextBox.Text = "there was a click...";
        }
        #endregion

        #region Bitmap preview
        private void SetImageFromBuffer(byte[] data)
        {
            var img = new BitmapImage();
            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                img.BeginInit();
                img.StreamSource = stream;
                img.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.UriSource = null;
                img.EndInit();
            }
            image.Source = img;
            SetImageVisible();
        }


        #endregion

        #region Net message processing
        private void OnMessageInt(object d)
        {
            byte[] data = d as byte[];

            // first 4 bytes are size, ignore
            var offset = 4;
            // next 4 bytes are message id
            var msgId = (MessageIds)BitConverter.ToInt32(data, 4);

            //var ds = data.Length > 8 ? data.AsSpan()[8..] : null;
            switch (msgId)
            {
                case MessageIds.Clear:
                    OnMessage_Clear();
                    break;
                case MessageIds.BitmapImage:
                    AddMemoryBufferToFrames(data, FrameType.Bitmap);
                    break;
                case MessageIds.Diagram:
                    AddMemoryBufferToFrames(data, FrameType.Diagram);
                    break;
                default:
                    Debug.Fail("Unknown message " + msgId);
                    break;
            }
        }
        private void OnMessage_Clear()
        {
            Frames.Clear();
            slider.Maximum = slider.Minimum = 0;
            message.Text = "";
            msgPanel.Visibility = Visibility.Collapsed;
            image.Source = null;
        }

        private void AddMemoryBufferToFrames(byte[] data, FrameType frameType)
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
                msgPanel.Visibility = Visibility.Visible;
            }

            var tmpData = new byte[data.Length - (8 + 6 + strmsgSize)];
            Buffer.BlockCopy(data, 8 + 6 + strmsgSize, tmpData, 0, tmpData.Length);
            if (asLastFrame)
            {
                Frames.Add((tmpData, frameType, stringMessage));
                slider.Maximum = Frames.Count;
                slider.Minimum = 1;
            }

            if (frameType == FrameType.Diagram)
            {
                if (slider.Value == 0)
                    slider.Value = 1;
            }
            else
            {
                if (slider.Value == Frames.Count - 1)
                    slider.Value = Frames.Count;
            }
        }

        #endregion

        #region Scroll viewer handling
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(scrollViewer);

                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;

                lastDragPoint = posNow;

                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - dX);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - dY);
            }
        }
        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(scrollViewer);
            if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y <
                scrollViewer.ViewportHeight) //make sure we still can use the scrollbars
            {
                scrollViewer.Cursor = Cursors.SizeAll;
                lastDragPoint = mousePos;
                Mouse.Capture(scrollViewer);
            }
        }

        private double scaleValue = 1;

        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                scaleValue += 0.5;
            }
            if (e.Delta < 0)
            {
                scaleValue -= 0.5;
            }

            e.Handled = true;
            scaleValue = Math.Min(20, Math.Max(0.1, scaleValue));

            lastMousePositionOnTarget = Mouse.GetPosition(grid);

            scaleTransform.ScaleX = scaleValue;
            scaleTransform.ScaleY = scaleValue;

            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, grid);
        }

        void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            scrollViewer.Cursor = Cursors.Arrow;
            scrollViewer.ReleaseMouseCapture();
            lastDragPoint = null;
        }

        //void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //    scaleTransform.ScaleX = e.NewValue;
        //    scaleTransform.ScaleY = e.NewValue;

        //    var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2,
        //                                     scrollViewer.ViewportHeight / 2);
        //    lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, grid);
        //}
        void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue)
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2,
                                                         scrollViewer.ViewportHeight / 2);
                        Point centerOfTargetNow =
                              scrollViewer.TranslatePoint(centerOfViewport, grid);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(grid);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / grid.Width;
                    double multiplicatorY = e.ExtentHeight / grid.Height;

                    double newOffsetX = scrollViewer.HorizontalOffset -
                                        dXInTargetPixels * multiplicatorX;
                    double newOffsetY = scrollViewer.VerticalOffset -
                                        dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    scrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }
        #endregion

        private void SliderButton_FrameMM_Button_Click(object sender, RoutedEventArgs e) => slider.Value = Math.Max(slider.Value - 1, 1);

        private void SliderButton_FramePP_Button_Click(object sender, RoutedEventArgs e) => slider.Value = Math.Min(slider.Value + 1, slider.Maximum);

        private void SliderButton_FrameFirst_Button_Click(object sender, RoutedEventArgs e) => slider.Value = 1;

        private void SliderButton_FrameLast_Button_Click(object sender, RoutedEventArgs e) => slider.Value = slider.Maximum;
    }
}
