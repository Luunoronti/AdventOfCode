
using System.Text;
using static System.Windows.Forms.LinkLabel;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(62)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(952_408_144_115)] // if != 0, will report failure if expected answer != given answer
    class Day18
    {
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
        private static void DigTrenches(Span<DigCommand> commands, Map2DSpan<int> map)
        {
            for (int i = 0; i < commands.Length; ++i)
            {
                DigCommand cmd = commands[i];
                var dir = cmd.direction;
                var steps = cmd.steps;
                var dx = dir switch { Direction.Xpositive => 1, Direction.Xnegative => -1, _ => 0 };
                var dy = dir switch { Direction.Ypositive => 1, Direction.Ynegative => -1, _ => 0 };

                for (int x = cmd.x, y = cmd.y, s = 0; s < steps; x += dx, y += dy, ++s)
                {
                    map.At(x, y, cmd.color);
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

        private static void DrawMap(Map2DSpan<int> map)
        {
            if (Log.Enabled == false) return;

            var sb = new StringBuilder();
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    var at = map.At(x, y);
                    if (at <= 0)
                        sb.Append($"{CC.Clr}{' '}");
                    else
                    {
                        var r = (byte)(at >> 16);
                        var g = (byte)(at >> 8);
                        var b = (byte)(at >> 0);
                        sb.Append($"{CC.FgRGB(r, g, b)}{'█'}");
                    }
                }
                sb.AppendLine(CC.Clr);
            }
            Log.WriteLine(sb.ToString());
        }
        private static void PrintCommands(Span<DigCommand> commands)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                var c = commands[i];

                var r = (byte)(c.color >> 16);
                var g = (byte)(c.color >> 8);
                var b = (byte)(c.color >> 0);

                Log.WriteLine($"({c.x}, {c.y}) => {c.steps} steps in {c.direction} with {CC.FgRGB(r, g, b)}█{CC.Clr} {c.color:X}");
            }
        }
        private static void FloodFillNonZero(Map2DSpan<int> map, int sx, int sy, int color = 0x808080)
        {
            // we work with queue instead of recursion
            var fillQueue = new Queue<Point>();

            fillQueue.Enqueue(new Point(sx, sy));

            while (fillQueue.TryDequeue(out var p))
            {
                var x = p.X;
                var y = p.Y;

                var at = map.At(x, y, out var outOfBounds);
                if (outOfBounds)
                    continue;
                if (at > 0)
                    continue;

                map.At(x, y, color);

                AddPointIfZero(fillQueue, map, x + 1, y);
                AddPointIfZero(fillQueue, map, x, y + 1);
                AddPointIfZero(fillQueue, map, x - 1, y);
                AddPointIfZero(fillQueue, map, x, y - 1);
            }

            void AddPointIfZero(Queue<Point> queue, Map2DSpan<int> map, int x, int y)
            {
                var at = map.At(x, y, out var outOfBounds);
                if (!outOfBounds && at == 0)
                    fillQueue.Enqueue(new Point { X = x, Y = y });
            }
        }
        private static bool IsPointInsideTrench(Map2DSpan<int> map, int px, int py)
        {
            var hits = 0;
            if (map.At(px, py) > 0) return false;

            for (int x = px - 1, y = py - 1; px >= 0 && py >= 0; px--, py--)
            {
                if (map.At(px, py) > 0)
                    hits++;
            }
            return hits % 2 != 0;
        }

        readonly static int[] sps_xes = new int[8] { 1, 1, 0, -1, -1, -1, -1, 1 };
        readonly static int[] sps_ys = new int[8] { 0, 1, 1, 1, 0, -1, -1, -1 };
        private static Point GetFloodFillStartPosition(Map2DSpan<int> map, int x, int y)
        {
            for (int i = 0; i < sps_xes.Length; i++)
            {
                if (IsPointInsideTrench(map, x + sps_xes[i], y + sps_ys[i])) return new Point(x + sps_xes[i], y + sps_ys[i]);
            }
            throw new Exception("Unable to find start point for flood fill.");
        }
        private static int CountNonZeroColors(Map2DSpan<int> map)
        {
            var len = map.Length;
            var s = map.AsSpan();
            var sum = 0;
            for (int i = 0; i < len; i++)
            {
                if (s[i] > 0)
                    sum++;
            }
            return sum;
        }
        //[RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] lines, int lineWidth, int count)
        {
            //if (Log.Enabled)
            //{
            //    Log.WriteLine("Press <Enter> to continue...");
            //    Console.ReadLine();
            //}

            // create commands
            var commands = CreateCommands(lines).AsSpan();
            // create x, y coordinates for each command
            CalculateCommandsXY(commands);
            // create AABB to see how much data we need to generate
            var aabb = GetAABB(commands);
            Log.WriteLine($"AABB: ({aabb.x}, {aabb.y}, width: {aabb.width}, height: {aabb.height})");
            // create our dig map
            var map = new Map2DSpan<int>(aabb.width, aabb.height);
            // estimate our starting position (will be saved in first commands object)
            CalculateWorldPositions(commands, aabb);
            // now, process commands to create trenches. also, color them accordingly
            DigTrenches(commands, map);

            //DrawMap(map);
            // now, get start position of fill
            var fillStart = GetFloodFillStartPosition(map, commands[0].x, commands[0].y);
            FloodFillNonZero(map, fillStart.X, fillStart.Y);

            // count non-zero values in our map
            var sum = CountNonZeroColors(map);
            return sum;
        }
        //[RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string[] lines, int lineWidth, int count)
        {
            // create commands
            var commands = CreateCommands2(lines).AsSpan();
            CalculateCommandsXY(commands);
            var aabb = GetAABB(commands);

            Log.WriteLine($"AABB area: {((long)aabb.width * aabb.height):N0} m2");
            Log.WriteLine($"Initial vertex count: {commands.Length}");

            // convert our vertices to a single mesh, with double precision
            var vertices = ConvertToMesh(commands);
            // triangulate mesh
            var triangles = Triangulate(vertices);

            var area = ComputeArea(triangles);
            return area;
        }


        private static List<Vertex2D> ConvertToMesh(Span<DigCommand> commands)
        {
            var ret = new List<Vertex2D>(commands.Length);
            for (int i = 0; i < commands.Length; i++)
                ret.Add(new Vertex2D(commands[i].x, commands[i].y));
            return ret;
        }

        private static double GetLen(Vertex2D v1, Vertex2D v2) => Math.Sqrt(Math.Pow(v2.x - v1.x, 2) + Math.Pow(v2.y - v1.y, 2));
        private static double ComputeArea(Triangle triangle)
        {
            // side lengths
            var s1 = GetLen(triangle.v1, triangle.v2);
            var s2 = GetLen(triangle.v2, triangle.v3);
            var s3 = GetLen(triangle.v3, triangle.v1);
            // semi perimeter
            var sp = ((s1 + s2 + s3) / 2);
            // Heron’s 
            return Math.Sqrt(sp * (sp - s1) * (sp - s2) * (sp - s3));
        }
        private static long ComputeArea(List<Triangle> triangles)
        {
            var area = 0l;
            for (int i = 0; i < triangles.Count; i++)
            {
                var triangle = triangles[i];
                // floor it (or ceil?) - let's Round for now and see the result
                area += (long)Math.Floor(ComputeArea(triangle));
            }
            return area;
        }
        struct Vertex2D
        {
            public double x;
            public double y;

            public Vertex2D(double x, double y)
            {
                this.x = x;
                this.y = y;
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





        // mesh triangulation. we will need to move it to some tools stuff
        private static List<Triangle> Triangulate(List<Vertex2D> mesh)
        {
            var result = new List<Triangle>();
            var tempPolygon = new List<Vertex2D>(mesh);
            //var convPolygon = new List<Vertex2D>();

            int begin_ind = 0;
            int cur_ind;
            int begin_ind1;
            int N = mesh.Count;
            int Range;

            if (Square(tempPolygon) < 0)
                tempPolygon.Reverse();

            while (N >= 3)
            {
                while ((PMSquare(tempPolygon[begin_ind], tempPolygon[(begin_ind + 1) % N],
                          tempPolygon[(begin_ind + 2) % N]) < 0) ||
                          (Intersect(tempPolygon, begin_ind, (begin_ind + 1) % N, (begin_ind + 2) % N) == true))
                {
                    begin_ind++;
                    begin_ind %= N;
                }
                cur_ind = (begin_ind + 1) % N;

                result.Add(new Triangle (tempPolygon[begin_ind], tempPolygon[cur_ind], tempPolygon[(begin_ind + 2) % N]));

                //if (triangulate == false)
                //{
                //    begin_ind1 = cur_ind;
                //    while ((PMSquare(tempPolygon[cur_ind], tempPolygon[(cur_ind + 1) % N],
                //                    tempPolygon[(cur_ind + 2) % N]) > 0) && ((cur_ind + 2) % N != begin_ind))
                //    {
                //        if ((Intersect(tempPolygon, begin_ind, (cur_ind + 1) % N, (cur_ind + 2) % N) == true) ||
                //            (PMSquare(tempPolygon[begin_ind], tempPolygon[(begin_ind + 1) % N],
                //                      tempPolygon[(cur_ind + 2) % N]) < 0))
                //            break;
                //        convPolygon.Add(tempPolygon[(cur_ind + 2) % N]);
                //        cur_ind++;
                //        cur_ind %= N;
                //    }
                //}

                Range = cur_ind - begin_ind;
                if (Range > 0)
                {
                    tempPolygon.RemoveRange(begin_ind + 1, Range);
                }
                else
                {
                    tempPolygon.RemoveRange(begin_ind + 1, N - begin_ind - 1);
                    tempPolygon.RemoveRange(0, cur_ind + 1);
                }
                N = tempPolygon.Count;
                begin_ind++;
                begin_ind %= N;
            }

            return result;
        }
        private static double PMSquare(Vertex2D v1, Vertex2D v2) => v2.x * v1.y - v1.x * v2.y;
        private static double PMSquare(Vertex2D p1, Vertex2D p2, Vertex2D p3) => ((p3.x - p1.x) * (p2.y - p1.y)) - ((p2.x - p1.x) * (p3.y - p1.y));

        private static double Square(List<Vertex2D> mesh)
        {
            var S = 0d;
            if (mesh.Count >= 3)
            {
                for (int i = 0; i < mesh.Count - 1; i++)
                    S += PMSquare(mesh[i], mesh[i + 1]);
                S += PMSquare(mesh[^1], mesh[0]);
            }
            return S;
        }
        private static bool Intersect(List<Vertex2D> mesh, int vertex1Ind, int vertex2Ind, int vertex3Ind)
        {
            double s1, s2, s3;
            for (int i = 0; i < mesh.Count; i++)
            {
                if ((i == vertex1Ind) || (i == vertex2Ind) || (i == vertex3Ind))
                    continue;
                s1 = PMSquare(mesh[vertex1Ind], mesh[vertex2Ind], mesh[i]);
                s2 = PMSquare(mesh[vertex2Ind], mesh[vertex3Ind], mesh[i]);
                if (((s1 < 0) && (s2 > 0)) || ((s1 > 0) && (s2 < 0)))
                    continue;
                s3 = PMSquare(mesh[vertex3Ind], mesh[vertex1Ind], mesh[i]);
                if (((s3 >= 0) && (s2 >= 0)) || ((s3 <= 0) && (s2 <= 0)))
                    return true;
            }
            return false;
        }
    }


}