//#define DRAWMAPENABLED
//#define DRAWSEGMENTNORMALS

using System.Diagnostics;
using System.Numerics;

namespace AdventOfCode2023
{
    [Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(62)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(952_408_144_115)] // if != 0, will report failure if expected answer != given answer
    class Day18
    {
        enum Direction : byte { Xnegative = 0, Ynegative = 1, Xpositive = 2, Ypositive = 3 }
        struct DigCommand
        {
            public long x;
            public long y;
            public Direction direction;
            public int steps;
        }
        struct HorizontalSegment
        {
            public double x1;
            public double x2;
            public long y;
        }
        struct AABB
        {
            public double x;
            public double y;
            public double width;
            public double height;
        }
        struct Vertex2D
        {
            public double x;
            public long y;
            public Vertex2D(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        struct Segment
        {
            public Vector2Long Start;
            public Vector2Long End;
            public bool IsVertical => Start.x == End.x;
            public bool IsHorizontal => Start.y == End.y;

            public override string ToString()
            {
                return $"[{Start}] [{End}] ([{End - Start}] ({(IsVertical ? "vertical" : "horizontal")}))";
            }
        }

        private static long CalculateCommandsXYAndGetLength(Span<DigCommand> commands)
        {
            long len = 0;
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

                len += Math.Abs(dx * steps) + Math.Abs(dy * steps);

            }
            return len;
        }
        private static AABB GetAABB(Span<DigCommand> commands)
        {
            double minX = double.MaxValue, maxX = double.MinValue, minY = double.MaxValue, maxY = double.MinValue;
            for (int i = 0; i < commands.Length; i++)
            {
                minX = Math.Min(minX, commands[i].x);
                maxX = Math.Max(maxX, commands[i].x);
                minY = Math.Min(minY, commands[i].y);
                maxY = Math.Max(maxY, commands[i].y);
            }
            return new AABB { x = minX, y = minY, width = maxX - minX + 1, height = maxY - minY + 1 };
        }
        private static List<HorizontalSegment> ConstructHorizontalSegmentsSorted(List<Vertex2D> vertices)
        {
            List<HorizontalSegment> segments = new();
            for (int i = 0; i < vertices.Count; i++)
            {
                var v1 = vertices[i];
                var v2 = i == vertices.Count - 1 ? vertices[0] : vertices[i + 1];

                if (v1.y != v2.y)
                    continue;




                var segment = new HorizontalSegment
                {
                    x1 = Math.Min(v1.x, v2.x),
                    x2 = Math.Max(v1.x, v2.x),
                    y = v1.y,
                };
                segments.Add(segment);
            }
            segments.Sort((s1, s2) => (int)(s2.y - s1.y));
            return segments;
        }
        private static List<Vertex2D> ConvertToMesh(Span<DigCommand> commands)
        {
            var ret = new List<Vertex2D>(commands.Length);
            for (int i = 0; i < commands.Length; i++)
                ret.Add(new Vertex2D((int)commands[i].x, (int)commands[i].y));
            return ret;
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
                    steps = int.Parse(sp[1])
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
                    steps = int.Parse(dd, System.Globalization.NumberStyles.HexNumber)
                };
            }
            return ret;
        }

        private static List<Segment> ConvertToMesh2(Span<DigCommand> commands)
        {
            var ret = new List<Segment>(commands.Length);
            for (int i = 0; i < commands.Length; i++)
            {
                var i_1 = i + 1;
                if (i == commands.Length - 1)
                    i_1 = 0;

                var xmin = Math.Min(commands[i].x, commands[i_1].x);
                var xmax = Math.Max(commands[i].x, commands[i_1].x);
                var ymin = Math.Min(commands[i].y, commands[i_1].y);
                var ymax = Math.Max(commands[i].y, commands[i_1].y);

                ret.Add(new Segment
                {
                    Start = new Vector2Long(xmin, ymin),
                    End = new Vector2Long(xmax, ymax),
                }
                );
            }


            return ret;
        }




        private static List<Segment> GetHorizontalSegments(List<Segment> mesh) => mesh.Where(m => m.Start.y == m.End.y).ToList();
        private static List<Segment> GetVerticalSegments(List<Segment> mesh) => mesh.Where(m => m.Start.x == m.End.x).ToList();
        private static double RayCastToTop(List<Segment> floatMesh, long x)
        {
            // for now, no optimization in getting lines
            var lines = floatMesh.Where(l => l.Start.x <= x && l.End.x >= x).ToList();
            var sum = 0L;
            if (lines.Count > 0)
            {
                if (lines.Count % 2 != 0)
                {
                    Log.WriteLine($"Invalid amount of horizontal lines: {lines.Count} at x {x}");
                }
                for (int i = 0; i < lines.Count - 1; i++)
                {
                    var y0 = lines[i].Start.y;
                    var y1 = lines[i + 1].Start.y;

                    // if there is vertical mesh between these, don't count it
                    sum += Math.Abs(y1 - y0);
                }
            }
            return sum;
        }


        //[RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] lines)
        {
            // create commands
            var commands = CreateCommands(lines).AsSpan();
            // create x, y coordinates for each command
            var trenchLength = CalculateCommandsXYAndGetLength(commands);
            // calculate AABB
            // we will use AABB's x to iterate over our shape
            var aabb = GetAABB(commands);

            var floatMesh = ConvertToMesh2(commands);

            floatMesh.Sort((a, b) => (int)(b.Start.y - a.Start.y));

            var bigInt = BigInteger.Zero;

            // we brute force this mess.
            // if we had access to DXR.. around 20 mil rays, 
            // we would do it in less than a second :)
            // will see, maybe this would be an idea after all
            for (long x = 0; x < aabb.width; x++)
            {
                bigInt += (long)RayCastToTop(floatMesh, x);
            }

            Console.WriteLine(bigInt);

            Log.WriteLine($"Answer won't fit in long, so here it is: {bigInt}");

            return (long)(bigInt + trenchLength);
        }

        //[RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string[] lines, int lineWidth, int count)
        {


            // create commands
            var commands = CreateCommands2(lines).AsSpan();
            // create x, y coordinates for each command
            var trenchLength = CalculateCommandsXYAndGetLength(commands);
            // calculate AABB
            // we will use AABB's x to iterate over our shape
            var aabb = GetAABB(commands);

            var floatMesh = ConvertToMesh2(commands);

            floatMesh.Sort((a, b) => (int)(b.Start.y - a.Start.y));

            var bigInt = BigInteger.Zero;

            // we brute force this mess.
            // if we had access to DXR.. around 20 mil rays, 
            // we would do it in less than a second :)
            // will see, maybe this would be an idea after all
            for (long x = 0; x < aabb.width; x++)
            {
                bigInt += (long)RayCastToTop(floatMesh, x);
            }

            Console.WriteLine(bigInt);

            Log.WriteLine($"Answer won't fit in long, so here it is: {bigInt}");

            return (long)(bigInt + trenchLength);
        }




    }
}