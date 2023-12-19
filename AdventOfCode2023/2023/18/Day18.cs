#define DRAWMAPENABLED
//#define DRAWSEGMENTNORMALS

using System.Drawing.Text;
using System.Numerics;
using static System.Windows.Forms.DataFormats;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(62)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(952_408_144_115)] // if != 0, will report failure if expected answer != given answer
    class Day18
    {
        private static Log.MapContext mapDrawer;


        //[RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] lines, int lineWidth, int count)
        {
            // create commands
            var commands = CreateCommands(lines).AsSpan();
            // create x, y coordinates for each command
            CalculateCommandsXY(commands);
            // create AABB to see how much data we need to generate
            var aabb = GetAABB(commands);
            // recalculate positions to positive coordinates (this step may not be required in final version, but it's nice to have it for now)
            CalculateWorldPositions(commands, aabb);


            // init map
#if DRAWMAPENABLED
            mapDrawer = Log.CreateRectangularMapContext(aabb.width, aabb.height);
            mapDrawer.Init(5);
            // mapDrawer.SetBackgroundPostProcess((@in, _) => (r: (byte)Math.Max(0, @in.r - 5), g: (byte)Math.Max(0, @in.g - 5), b: (byte)Math.Max(0, @in.b - 5)));
            mapDrawer.FillForegroundColor(180, 180, 180);
            mapDrawer.FillBackgroundColor(0, 0, 0);
#endif
            // convert our map to vertices and perform greedy meshing
            var vertices = ConvertToMesh(commands);


            // we need a better way to know out starting position
            // or a better way to calculate normal
            var fillStart = new Point(vertices[0].x, vertices[0].y - 1);
            StartGreedyMeshing(vertices, fillStart.X, fillStart.Y, Direction.Ynegative, aabb.width, aabb.height);

            // just an endless loop to draw map
            mapDrawer.DrawAndWait();
            Log.WriteLine("Completed");
            while (true)
            {
                //                mapDrawer.DrawAndWait();
                Thread.Sleep(1); // be nice to OS
            }

            return 0;// sum;
        }
        //[RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string[] lines, int lineWidth, int count)
        {
            Console.ReadLine();

            // create commands
            var commands = CreateCommands2(lines).AsSpan();
            CalculateCommandsXY(commands);
            var aabb = GetAABB(commands);

            Log.WriteLine($"AABB x: {aabb.x}, y: {aabb.y}, width: {aabb.width}, height: {aabb.height}");
            Log.WriteLine($"AABB area: {((long)aabb.width * aabb.height):N0} m2");
            Log.WriteLine($"Initial vertex count: {commands.Length}");

            // idea: If we could create a triangles representation of our mesh
            // and sum their areas, we could solve the problem quite fast and easy



            // convert our vertices to a single mesh, with double precision
            var vertices = ConvertToMesh(commands);
            return 0;// area;
        }




        private static void DrawSegmentsToMapDrawer(Span<Segment> segments, int width)
        {
            void VerticalLine(Span<char> buffer, Span<int> colorBuffer, int width, Segment segment)
            {
                var min = Math.Min(segment.y1, segment.y2);
                var max = Math.Max(segment.y1, segment.y2);

                for (int i = min; i < max; i++)
                {
                    var cu = buffer.At(segment.x1, i, width);
                    if (cu == '─')
                        buffer.At(segment.x1, i, width, '┼');
                    else
                        buffer.At(segment.x1, i, width, '│');

                    colorBuffer.At(segment.x1, i, width, segment.color);
                }
#if DRAWSEGMENTNORMALS
                var nx = segment.x1 + segment.normal.x;
                var ny = min + (max - min) / 2;
                buffer.At(nx, ny, width, (segment.normal.x > 0 ? '>' : '<'));
                colorBuffer.At(nx, ny, width, segment.color);
#endif
            }
            void HorizontalLine(Span<char> buffer, Span<int> colorBuffer, int width, Segment segment)
            {
                var min = Math.Min(segment.x1, segment.x2);
                var max = Math.Max(segment.x1, segment.x2);

                for (int i = min; i < max; i++)
                {
                    var cu = buffer.At(i, segment.y1, width);
                    if (cu == '│')
                        buffer.At(i, segment.y1, width, '┼');
                    else
                        buffer.At(i, segment.y1, width, '─');

                    //buffer.At(i, segment.y1, width, '─');
                    colorBuffer.At(i, segment.y1, width, segment.color);
                }

#if DRAWSEGMENTNORMALS
                var nx = min + (max - min) / 2;
                var ny = segment.y1 + segment.normal.y;
                buffer.At(nx, ny, width, (segment.normal.y > 0 ? 'v' : '^'));
                colorBuffer.At(nx, ny, width, segment.color);
#endif
            }

            if (Log.Enabled == false) return;

            var charBuffer = new char[mapDrawer.Width * mapDrawer.Height];
            charBuffer.AsSpan().Fill(' ');
            var colorBuffer = new int[mapDrawer.Width * mapDrawer.Height];

            foreach (var segment in segments)
            {
                if (segment.x1 == segment.x2) VerticalLine(charBuffer, colorBuffer, width, segment);
                else HorizontalLine(charBuffer, colorBuffer, width, segment);
            }

            mapDrawer.SetContent(charBuffer);
            mapDrawer.SetForegroundColors(colorBuffer);
        }
        private static void FillRectangleColors(List<Rectangle> rectangles, int width)
        {
            var colorBuffer = new int[mapDrawer.Width * mapDrawer.Height].AsSpan();
            foreach (var rect in rectangles)
            {
                for (int y = rect.y; y < rect.y + rect.height; y++)
                {
                    for (var x = rect.x; x < rect.x + rect.width; x++)
                    {
                        colorBuffer.At(x, y, width, rect.color);
                    }
                }
            }
            mapDrawer.SetBackgroundColors(colorBuffer);
        }
        private static List<Vertex2D> ConvertToMesh(Span<DigCommand> commands)
        {
            var ret = new List<Vertex2D>(commands.Length);
            for (int i = 0; i < commands.Length; i++)
                ret.Add(new Vertex2D(commands[i].x, commands[i].y, commands[i].color));
            return ret;
        }


        #region Structs
        enum Direction : byte
        {
            Xnegative = 0,
            Ynegative = 1,
            Xpositive = 2,
            Ypositive = 3
        }
        struct DigCommand
        {
            public int x;
            public int y;
            public Direction direction;
            public int steps;
            public int color;
        }
        struct AABB
        {
            public int x;
            public int y;
            public int width;
            public int height;
        }
        struct Vertex2D
        {
            public int x;
            public int y;
            public int color;
            public Vertex2D(int x, int y, int color)
            {
                this.x = x;
                this.y = y;
                this.color = color;
            }
        }
        struct Triangle
        {
            public Vertex2D v1;
            public Vertex2D v2;
            public Vertex2D v3;

            public Triangle(Vertex2D v1, Vertex2D v2, Vertex2D v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }
        }
        struct Vector2D
        {
            public static Vector2D Up = new Vector2D(0, -1);
            public static Vector2D Down = new Vector2D(0, 1);
            public static Vector2D Left = new Vector2D(-1, 0);
            public static Vector2D Right = new Vector2D(1, 0);

            public int x;
            public int y;

            public Vector2D(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public static bool operator ==(Vector2D a, Vector2D b) => (a.x == b.x) && (a.y == b.y);
            public static bool operator !=(Vector2D a, Vector2D b) => (a.x != b.x) || (a.y != b.y);

            public override readonly bool Equals(object obj)
            {
                if (obj is Vector2D v2) return this == v2;
                return false;
            }
        }
        class Rectangle
        {
            public int x;
            public int y;
            public int width;
            public int height;

            public int Left
            {
                get => x;
                set
                {
                    if (x < value)
                    {
                        var delta = value - x;
                        x = value;
                        width -= delta;
                    }
                    else
                    {
                        var delta = x - value;
                        x = value;
                        width += delta;
                    }
                }
            }
            public int Right
            {
                get { return x + width; }
                set => width = value - x;
            }
            public int Top
            {
                get => y;
                set
                {
                    if (y < value)
                    {
                        var delta = value - y;
                        y = value;
                        height -= delta;
                    }
                    else
                    {
                        var delta = y - value;
                        y = value;
                        height += delta;
                    }
                }
            }
            public int Bottom
            {
                get { return y + height; }
                set => height = value - y;
            }

            public int color;
        }
        struct Segment
        {
            public int x1;
            public int y1;
            public int x2;
            public int y2;

            public int XMin => Math.Min(x1, x2);
            public int XMax => Math.Max(x1, x2);

            public int YMin => Math.Min(y1, y2);
            public int YMax => Math.Max(y1, y2);

            public Vector2D normal;
            public int color;

            public long Length => Math.Abs(x1 - x2) + Math.Abs(y2 - y1); // this works for aligned segments only, which we have
        }
        enum NormalDirection { SmallerToHigher, HigherToSmaller }
        #endregion

        #region Greedy meshing
        private static Span<Segment> ConstructSegments(List<Vertex2D> vertices)
        {
            var segments = new Segment[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                var v1 = vertices[i];
                var v2 = i == vertices.Count - 1 ? vertices[0] : vertices[i + 1];

                var y_1 = v1.y;
                var y_2 = v2.y;
                if (v2.x == v1.x)
                {
                    //if(v1.y < v2.y)
                    //{
                    //    y_2++;
                    //}
                    //else
                    //{
                    //    y_1++;
                    //}
                }


                var segment = new Segment { x1 = v1.x, y1 = y_1, x2 = v2.x, y2 = y_2, color = v1.color };

                segments[i] = segment;
            }
            return segments.AsSpan();
        }

        private static NormalDirection GetNormalFromStartPosition(Segment segment0, int startX, int startY)
        {
            var normX = 0;
            var normY = 0;
            if (segment0.x1 == segment0.x2)
                normY = Math.Sign(startY - segment0.y1);
            if (segment0.y1 == segment0.y2)
                normX = Math.Sign(startX - segment0.x1);

            return normX != 0 ? normX < 0 ? NormalDirection.HigherToSmaller : NormalDirection.SmallerToHigher : normY < 0 ? NormalDirection.HigherToSmaller : NormalDirection.SmallerToHigher;
        }
        private static void AssignNormals(Span<Segment> segments, NormalDirection normalDirection)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                var s = segments[i];

                if (s.x1 == s.x2)
                {
                    if (s.y1 < s.y2)
                    {
                        segments[i].normal = normalDirection == NormalDirection.SmallerToHigher ? Vector2D.Left : Vector2D.Right;
                    }
                    else
                    {
                        segments[i].normal = normalDirection == NormalDirection.SmallerToHigher ? Vector2D.Right : Vector2D.Left;
                    }
                }
                else
                {
                    if (s.x1 < s.x2)
                    {
                        segments[i].normal = normalDirection == NormalDirection.SmallerToHigher ? Vector2D.Down : Vector2D.Up;
                    }
                    else
                    {
                        segments[i].normal = normalDirection == NormalDirection.SmallerToHigher ? Vector2D.Up : Vector2D.Down;
                    }
                }
            }
        }

        private static ReadOnlySpan<Segment> GetVerticalSegments(Span<Segment> segments)
        {
            // count segments
            var sum = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].x1 == segments[i].x2)
                    sum++;
            }
            var offset = 0;
            var buffer = new Segment[sum];
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].x1 == segments[i].x2)
                    buffer[offset++] = segments[i];
            }
            return buffer.AsSpan();
        }
        private static ReadOnlySpan<Segment> GetHorizontalSegments(Span<Segment> segments)
        {
            // count segments
            var sum = 0;
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].y1 == segments[i].y2)
                    sum++;
            }
            var offset = 0;
            var buffer = new Segment[sum];
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].y1 == segments[i].y2)
                    buffer[offset++] = segments[i];
            }
            return buffer.AsSpan();
        }


        private static void StartGreedyMeshing(List<Vertex2D> vertices, int startX, int startY, Direction direction, int width, int heigth)
        {
            // construct our segments
            var segments = ConstructSegments(vertices);
            var seg0Normal = GetNormalFromStartPosition(segments[0], startX, startY);
            AssignNormals(segments, seg0Normal);

            DrawSegmentsToMapDrawer(segments, width);

            // sort segments, and create two sets, vertical lines and horizontal lines
            var horizontalSegments = GetHorizontalSegments(segments);
            var verticalSegments = GetVerticalSegments(segments);

            var rectangles = CreateRectangles(segments, horizontalSegments, verticalSegments, width, heigth);
            FillRectangleColors(rectangles, width);

            Log.WriteLine($"{rectangles.Count} rectangles created.");
        }


        private static Rectangle CreateRectangleLeftToRight(Segment segment, Segment prevSegment, Segment nextSegment,
            ReadOnlySpan<Segment> horizontalSegments, ReadOnlySpan<Segment> verticalSegments, int width, int heigth)
        {
            var startx = segment.x1 + 1;
            var miny = Math.Min(segment.y1, segment.y2);
            var maxy = Math.Max(segment.y1, segment.y2);

            if (prevSegment.normal == Vector2D.Down && miny == prevSegment.y1)
                miny++;

            if (prevSegment.normal == Vector2D.Down && maxy == prevSegment.y1)
                maxy++;

            if (nextSegment.normal == Vector2D.Down && miny == nextSegment.y1)
                miny++;

            if (miny >= maxy) return default;

            var maxAllowedX = width;

            for (int i = 0; i < verticalSegments.Length; i++)
            {
                var s = verticalSegments[i];
                if (s.x1 < startx) continue;

                if (s.y2 < miny && s.y1 < miny) continue;
                if (s.y2 >= maxy && s.y1 >= maxy) continue;

                maxAllowedX = Math.Min(maxAllowedX, s.x1);
            }

            // for now, grow till the end, just to test
            var rect = new Rectangle
            {
                color = segment.color,
                x = startx,
                y = miny,
                width = maxAllowedX - startx,
                height = maxy - miny
            };

            return rect;
        }

        private static Rectangle CreateRectangleRightToLeft(Segment segment, Segment prevSegment, Segment nextSegment,
           ReadOnlySpan<Segment> horizontalSegments, ReadOnlySpan<Segment> verticalSegments,
           int width, int heigth)
        {
            var startx = segment.x1 - 1;
            var miny = Math.Min(segment.y1, segment.y2);
            var maxy = Math.Max(segment.y1, segment.y2);

            if (prevSegment.normal == Vector2D.Down && miny == prevSegment.y1)
                miny++;

            if (prevSegment.normal == Vector2D.Down && maxy == prevSegment.y1)
                maxy++;

            if (nextSegment.normal == Vector2D.Down && miny == nextSegment.y1)
                miny++;

            if (miny >= maxy) return default;

            var minAllowedX = 0;

            for (int i = 0; i < verticalSegments.Length; i++)
            {
                var s = verticalSegments[i];
                if (s.x1 > startx) continue;

                if (s.y2 < miny && s.y1 < miny) continue;
                if (s.y2 >= maxy && s.y1 >= maxy) continue;

                minAllowedX = Math.Max(minAllowedX, s.x1);
            }

            // for now, grow till the end, just to test
            var rect = new Rectangle
            {
                color = segment.color,
                x = minAllowedX + 1,
                y = miny,
                width = startx - minAllowedX,
                height = maxy - miny
            };

            return rect;
        }

        private static Rectangle CreateRectangleTopToBottom(Segment segment, Segment prevSegment, Segment nextSegment,
         ReadOnlySpan<Segment> horizontalSegments, ReadOnlySpan<Segment> verticalSegments,
         int width, int heigth)
        {
            var starty = segment.y1 + 1;
            var minx = Math.Min(segment.x1, segment.x2);
            var maxx = Math.Max(segment.x1, segment.x2);

            if (prevSegment.normal == Vector2D.Right && minx == prevSegment.x1)
                minx++;

            if (minx >= maxx) return default;

            var maxAllowedY = heigth;

            for (int i = 0; i < horizontalSegments.Length; i++)
            {
                var s = horizontalSegments[i];
                if (s.y1 < starty) continue;

                if (s.x2 < minx && s.x1 < minx) continue;
                if (s.x2 >= maxx && s.x1 >= maxx) continue;

                maxAllowedY = Math.Min(maxAllowedY, s.y1);
            }

            // for now, grow till the end, just to test
            var rect = new Rectangle
            {
                color = segment.color,
                x = minx,
                y = starty,
                width = maxx - minx,
                height = maxAllowedY - starty
            };

            return rect;
        }

        private static Rectangle CreateRectangleBottomToTop(Segment segment, Segment prevSegment, Segment nextSegment,
         ReadOnlySpan<Segment> horizontalSegments, ReadOnlySpan<Segment> verticalSegments,
         int width, int heigth)
        {
            var starty = segment.y1 - 1;
            var minx = Math.Min(segment.x1, segment.x2);
            var maxx = Math.Max(segment.x1, segment.x2);

            if (nextSegment.normal == Vector2D.Right && minx == nextSegment.x1)
                minx++;

            if (minx >= maxx) return default;

            var minAllowedY = 0;

            for (int i = 0; i < horizontalSegments.Length; i++)
            {
                var s = horizontalSegments[i];
                if (s.y1 > starty) continue;

                if (s.x2 < minx && s.x1 < minx) continue;
                if (s.x2 >= maxx && s.x1 >= maxx) continue;

                minAllowedY = Math.Max(minAllowedY, s.y1);
            }

            // for now, grow till the end, just to test
            var rect = new Rectangle
            {
                color = segment.color,
                x = minx,
                y = minAllowedY + 1,
                width = maxx - minx,
                height = starty - minAllowedY
            };

            return rect;
        }

        private static List<Rectangle> CreateRectangles(Span<Segment> segments, ReadOnlySpan<Segment> horizontalSegments, ReadOnlySpan<Segment> verticalSegments, int width, int heigth)
        {
            static void Create(List<Rectangle> currentRects, Vector2D normal, Span<Segment> segments, ReadOnlySpan<Segment> horizontalSegments, ReadOnlySpan<Segment> verticalSegments, int width, int heigth)
            {
                for (int i = 0; i < segments.Length; i++)
                {
                    var seg = segments[i];
                    if (seg.normal != normal)
                        continue;
                    var prev = i > 0 ? segments[i - 1] : segments[^1];
                    var next = i < segments.Length - 1 ? segments[i + 1] : segments[0];

                    Rectangle rect = default;

                    if (normal == Vector2D.Right) rect = CreateRectangleLeftToRight(seg, prev, next, horizontalSegments, verticalSegments, width, heigth);
                    //else if (normal == Vector2D.Left) rect = CreateRectangleRightToLeft(seg, prev, next, horizontalSegments, verticalSegments, width, heigth);
                    //else if (normal == Vector2D.Up) rect = CreateRectangleBottomToTop(seg, prev, next, horizontalSegments, verticalSegments, width, heigth);
                    //else if (normal == Vector2D.Down) rect = CreateRectangleTopToBottom(seg, prev, next, horizontalSegments, verticalSegments, width, heigth);

                    if (rect.width > 0 && rect.height > 0)
                    {
                        currentRects.Add(rect);
                        //if(currentRects.Count > 10)
                        //    return;
                    }
                }
            }

            static bool IsValueWithin(int v, int v1, int v2)
            {
                return v >= v1 && v <= v2;
            }
            static bool IsSegmentOnRect(int x1, int x2, int left, int right)
            {
                if (x1 == left || x2 == right) return true;
                if (x1 >= left && x1 <= right) return true;
                if (x2 >= left && x2 <= right) return true;

                if (left >= x1 && left <= x2) return true;
                if (right >= x1 && right <= x2) return true;
                return false;
            }
            static bool IsRectOnRect(int left1, int right1, int left2, int right2)
            {
                if (left1 == left2 || right1 == right2) return true;
                if (left1 >= left2 && left1 <= right2) return true; // rect pt1 on rect 2
                if (right1 >= left2 && right1 <= right2) return true; // rect pt2 on rect 2

                if (left2 >= left1 && left2 <= right1) return true; // rect pt1 on rect 1
                if (right2 >= left1 && right2 <= right1) return true; // rect pt2 on rect 1
                return false;
            }

            static void GreedyGrowTopBottom(List<Rectangle> currentRects, ReadOnlySpan<Segment> horizontalSegments, int width, int heigth)
            {
                for (int i = 0; i < currentRects.Count; i++)
                {
                    var rect = currentRects[i];

                    var minimum = 0;
                    var maximum = heigth;

                    // do not grow beyond any segments
                    for (int s = 0; s < horizontalSegments.Length; s++)
                    {
                        var seg = horizontalSegments[s];

                        if (IsSegmentOnRect(seg.XMin, seg.XMax, rect.Left, rect.Right))
                        {
                            if (seg.y1 <= rect.Top) minimum = Math.Max(minimum, seg.y1 + 1);
                            else if (seg.y1 >= rect.Bottom) maximum = Math.Min(maximum, seg.y1);
                        }
                    }

                    // do not grow beyond any rectangle
                    for (int i1 = 0; i1 < currentRects.Count; i1++)
                    {
                        var r2 = currentRects[i1];
                        if (r2 == rect) continue;

                        if (IsRectOnRect(rect.Left, rect.Right, r2.Left, r2.Right))
                        {
                            if (r2.Bottom < rect.Top) minimum = Math.Max(minimum, r2.Bottom);
                            else if (r2.Top > rect.Bottom) maximum = Math.Min(maximum, r2.Top);
                        }
                    }

                    rect.Top = minimum;
                    rect.Bottom = maximum;
                }
            }

            var ret = new List<Rectangle>();

            Create(ret, Vector2D.Right, segments, horizontalSegments, verticalSegments, width, heigth);
            GreedyGrowTopBottom(ret, horizontalSegments, width, heigth);


            //Create(ret, Vector2D.Left, segments, horizontalSegments, verticalSegments, width, heigth);
            //Create(ret, Vector2D.Down, segments, horizontalSegments, verticalSegments, width, heigth);
            //Create(ret, Vector2D.Up, segments, horizontalSegments, verticalSegments, width, heigth);

            // greedy grow those rectangles


            ret.Clear();


            for (int i = 0; i < segments.Length; i++)
            {
                var seg = segments[i];
                var prev = i > 0 ? segments[i - 1] : segments[^1];
                var next = i < segments.Length - 1 ? segments[i + 1] : segments[0];
               
                Rectangle rect = default;

                if (seg.normal == Vector2D.Right) rect = new Rectangle { x = seg.x1 + 1, y = seg.YMin + 2, width = 1, height = 1, color = seg.color };
                else if (seg.normal == Vector2D.Left) rect = new Rectangle { x = seg.x1 - 1, y = seg.YMin + 2, width = 1, height = 1, color = seg.color };
               
                else if (seg.normal == Vector2D.Up) rect = new Rectangle { x = seg.XMin + 1, y = seg.y1-1 , width = 1, height = 1, color = seg.color };
                else if (seg.normal == Vector2D.Down) rect = new Rectangle { x = seg.XMin + 1, y = seg.y1 + 1, width = 1, height = 1, color = seg.color };

                ret.Add(rect);
            }


            return ret;
        }


        #endregion





        #region Draw and fill approach (on my beloved 2d map)

        private static void CalculateCommandsXY(Span<DigCommand> commands)
        {
            int x = 0, y = 0;
            for (int i = 0; i < commands.Length; i++)
            {
                var cmd = commands[i];
                var dir = cmd.direction;
                var steps = cmd.steps;

                var dx = dir switch { Direction.Xpositive => 1, Direction.Xnegative => -1, _ => 0 };
                var dy = dir switch { Direction.Ypositive => 1, Direction.Ynegative => -1, _ => 0 };

                commands[i].x = x;
                commands[i].y = y;

                x += dx * steps;
                y += dy * steps;
            }
        }
        private static AABB GetAABB(Span<DigCommand> commands)
        {
            int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
            for (int i = 0; i < commands.Length; i++)
            {
                minX = Math.Min(minX, commands[i].x);
                maxX = Math.Max(maxX, commands[i].x);
                minY = Math.Min(minY, commands[i].y);
                maxY = Math.Max(maxY, commands[i].y);
            }
            return new AABB { x = minX, y = minY, width = maxX - minX + 1, height = maxY - minY + 1 };
        }
        private static void CalculateWorldPositions(Span<DigCommand> commands, AABB aabb)
        {
            if (aabb.x < 0 || aabb.y < 0)
            {
                for (int i = 0; i < commands.Length; i++)
                {
                    commands[i].x -= (aabb.x);
                    commands[i].y -= (aabb.y);
                }
            }
        }

        private static DigCommand[] CreateCommands(string[] lines)
        {
            var ret = new DigCommand[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string? line = lines[i];
                var sp = line.Split(' ');

                var dc = sp[0][0];
                ret[i] = new DigCommand
                {
                    direction = dc switch { 'R' => Direction.Xpositive, 'D' => Direction.Ypositive, 'L' => Direction.Xnegative, 'U' => Direction.Ynegative },
                    steps = int.Parse(sp[1]),
                    color = int.Parse(sp[2][2..8], System.Globalization.NumberStyles.HexNumber) // it's aways 6 chars hex color
                };
            }
            return ret;
        }
        private static DigCommand[] CreateCommands2(string[] lines)
        {
            var ret = new DigCommand[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string? line = lines[i];
                var sp = line.Split(' ');

                var dd = sp[2][2..7];
                var dir = sp[2][7];
                ret[i] = new DigCommand
                {
                    direction = dir switch { '0' => Direction.Xpositive, '1' => Direction.Ypositive, '2' => Direction.Xnegative, '3' => Direction.Ynegative },
                    steps = int.Parse(dd, System.Globalization.NumberStyles.HexNumber),
                    color = 0x808080
                };
            }
            return ret;
        }
        #endregion

    }
}