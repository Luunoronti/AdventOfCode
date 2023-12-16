#define DRAWMAPENABLED

using System.Runtime.InteropServices;
using System.Text;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(46)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(51)] // if != 0, will report failure if expected answer != given answer
    class Day16
    {
        [Flags]
        enum BeamDirection : byte
        {
            Left = 0x01,
            Right = 0x02,
            Up = 0x04,
            Down = 0x08,
        }

        // try to make this struct as small as possible
        // to fit as many as possible (we use recursion, so stack size is important)
        [StructLayout(LayoutKind.Sequential)]
        ref struct ProcessParameters
        {
            public StringSpan Input; // each span is sizeof(ref) + sizeof(int)
            public Span<byte> Map;
            public short Width;
            public short Height;
            public short X;
            public short Y;
            public BeamDirection Direction;

            // helper method that will get called in few places, so make it here
            public void StepInDirection()
            {
                switch (Direction)
                {
                    case BeamDirection.Left: X--; break;
                    case BeamDirection.Right: X++; break;
                    case BeamDirection.Up: Y--; break;
                    case BeamDirection.Down: Y++; break;
                }
            }
        }


        private static int dm_consoleTop;
        private static void InitDrawMap()
        {
#if DRAWMAPENABLED
            dm_consoleTop = Console.CursorTop;
            Log.Write(CC.CursorHide);
#endif
        }
        private static void CloseDrawMap()
        {
#if DRAWMAPENABLED
            Log.Write(CC.CursorShow);
#endif
        }
        private static void DrawMap(ProcessParameters parameters, int sleep = 0)
        {
#if DRAWMAPENABLED
            if (Log.Enabled == false) return;
            var sb = new StringBuilder();

            for (int y = 0; y < parameters.Height; y++)
            {
                for (int x = 0; x < parameters.Width; x++)
                {
                    var c = parameters.Input.GetAt(x, y, parameters.Width, parameters.Height, out _);
                    var m = parameters.Map.GetAt(x, y, parameters.Width, parameters.Height, out _);

                    // replace dot because our terminal (and Cascadia Nerd Cove font)
                    // shows 3 dots (...) as it's own glyph, which makes it a bit harder to read
                    if (c == '.') c = CC.DotReplacement;

                    var colorFlag = CC.Frm;
                    if (m != 0)
                        colorFlag = CC.Sys;

                    if (x == parameters.X && y == parameters.Y)
                        sb.Append($"{CC.HBg}");
                    
                    sb.Append($"{colorFlag}{c}");
                    
                    if (x == parameters.X && y == parameters.Y)
                        sb.Append($"{CC.Clr}");
                }

                sb.AppendLine($"{CC.Clr}");
            }

            Console.CursorTop = dm_consoleTop;
            Console.CursorLeft = 0;
            Log.WriteLine(sb.ToString());

            Thread.Sleep(sleep);
#endif
        }

        private static void ProcessBeam(ProcessParameters parameters)
        {
            // loop exit logic inside
            while (true)
            {
                var c = parameters.Input.GetAt(parameters.X, parameters.Y, parameters.Width, parameters.Height, out var outOfBounds);
                // beam has left the map
                if (outOfBounds)
                    return;

                var m = parameters.Map.GetAt(parameters.X, parameters.Y, parameters.Width, parameters.Height, out outOfBounds);

                // beam has left the map - this should not happen, map is the same dimensions as input
                if (outOfBounds)
                    return;

                // if beam has already been here (with same direction)
                // it means we have a loop. drop this beam
                if ((m & (byte)parameters.Direction) == (byte)parameters.Direction)
                    return;

                // we 'energize' our spot
                // and save our direction
                parameters.Map.SetAt((byte)(m | (byte)parameters.Direction), parameters.X, parameters.Y, parameters.Width, parameters.Height, out _);

                // if our current place is empty space ('.'), continue in same direction
                if (c == '.')
                {
                    parameters.StepInDirection();
                    DrawMap(parameters, 1000);
                    continue;
                }

                // we encounter a mirror
                if (c == '\\')
                {
                    // if we go from left (to right), our current direction is down   (to == towards :)
                    // if we go from down (to up), our current direction is left
                    // if we go from right (to left), our current direction is up
                    // if we go from up (to down), our current direction is right
                    parameters.Direction = parameters.Direction switch
                    {
                        BeamDirection.Right => BeamDirection.Down,
                        BeamDirection.Left => BeamDirection.Up,
                        BeamDirection.Up => BeamDirection.Left,
                        BeamDirection.Down => BeamDirection.Right,
                        _ => throw new NotImplementedException(),
                    };
                    parameters.StepInDirection();
                    DrawMap(parameters, 1000);

                    continue;
                }
                if (c == '/')
                {
                    // if we go from left (to right), our current direction is up   (to == towards :)
                    // if we go from down (to up), our current direction is right
                    // if we go from right (to left), our current direction is down
                    // if we go from up (to down), our current direction is left
                    parameters.Direction = parameters.Direction switch
                    {
                        BeamDirection.Right => BeamDirection.Up,
                        BeamDirection.Left => BeamDirection.Down,
                        BeamDirection.Up => BeamDirection.Right,
                        BeamDirection.Down => BeamDirection.Left,
                        _ => throw new NotImplementedException(),
                    };
                    parameters.StepInDirection();
                    DrawMap(parameters, 1000);

                    continue;
                }

                // splitters: we use recursion here
                // this may bite us with the amount of stack
                // we use for our parameters.. will see
                if (c == '-')
                {
                    if (parameters.Direction == BeamDirection.Up || parameters.Direction == BeamDirection.Down)
                    {
                        // we split (run concurrent, but return from this)
                        var param1 = new ProcessParameters
                        {
                            Direction = BeamDirection.Right,
                            X = (short)(parameters.X + 1),
                            Y = parameters.Y,
                            Height = parameters.Height,
                            Width = parameters.Width,
                            Input = parameters.Input,
                            Map = parameters.Map
                        };
                        DrawMap(param1, 1000);
                        ProcessBeam(param1);

                        var param2 = new ProcessParameters
                        {
                            Direction = BeamDirection.Left,
                            X = (short)(parameters.X - 1),
                            Y = parameters.Y,
                            Height = parameters.Height,
                            Width = parameters.Width,
                            Input = parameters.Input,
                            Map = parameters.Map
                        };
                        DrawMap(param2, 1000);
                        ProcessBeam(param2);

                        return;
                    }
                    // else just continue
                    parameters.StepInDirection();
                    DrawMap(parameters, 1000);
                    continue;
                }
                if (c == '|')
                {
                    if (parameters.Direction == BeamDirection.Left || parameters.Direction == BeamDirection.Right)
                    {
                        // we split (run concurrent, but return from this)
                        ProcessBeam(new ProcessParameters
                        {
                            Direction = BeamDirection.Down,
                            X = parameters.X,
                            Y = (short)(parameters.Y + 1),
                            Height = parameters.Height,
                            Width = parameters.Width,
                            Input = parameters.Input,
                            Map = parameters.Map
                        });

                        ProcessBeam(new ProcessParameters
                        {
                            Direction = BeamDirection.Up,
                            X = parameters.X,
                            Y = (short)(parameters.Y - 1),
                            Height = parameters.Height,
                            Width = parameters.Width,
                            Input = parameters.Input,
                            Map = parameters.Map
                        });
                        return;
                    }
                    // else just continue
                    parameters.StepInDirection();
                    DrawMap(parameters, 1000);
                    continue;
                }
            }
        }

        private static byte[]? _mapMemory = null;
        private static unsafe long EnergizeMapWithBeam(StringSpan lines, int width, int heigth, int beamStartX, int beamStartY, BeamDirection initialDirection)
        {
            // need a field map
            if (_mapMemory == null || _mapMemory.Length != (width * heigth))
                _mapMemory = new byte[width * heigth];

            // we could also use stackalloc like this:
            // var map2 = stackalloc byte[width * height];
            // var map = new Span<byte>(map2, width * height);
            // but this would limit our possible map size, as
            // stack space is limited, especially given the fact
            // that we use recursion 

            var map = _mapMemory.AsSpan();
            map.Clear(); // clear map after last use

            InitDrawMap();
            // process with beam from starting point (0, 0), to the right
            ProcessBeam(new ProcessParameters
            {
                Direction = initialDirection,
                X = (short)beamStartX,
                Y = (short)beamStartY,
                Height = (short)heigth,
                Width = (short)width,
                Input = lines,
                Map = map
            });
            CloseDrawMap();

            // now, simply count marked positions
            var sum = 0L;
            for (int i = 0; i < map.Length; i++)
            {
                if (map[i] != 0)
                    sum++;
            }
            return sum;
        }

        [RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(StringSpan input, int lineWidth, int count) => EnergizeMapWithBeam(input, lineWidth, count, 0, 0, BeamDirection.Right);

        [RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int lineWidth, int count)
        {
            // our method is fast enough to just search for each possibility independently
            // we could try to keep the same map from steps before, and skip 
            // beams that were already traversed
            // also, because we are using one map buffer, we can't use threads
            long sum = 0L;

            for (int i = 0; i < lineWidth; i++)
            {
                sum = Math.Max(sum, EnergizeMapWithBeam(input, lineWidth, count, i, 0, BeamDirection.Down));
                sum = Math.Max(sum, EnergizeMapWithBeam(input, lineWidth, count, i, count - 1, BeamDirection.Up));
            }

            for (int i = 0; i < count; i++)
            {
                sum = Math.Max(sum, EnergizeMapWithBeam(input, lineWidth, count, 0, 0, BeamDirection.Right));
                sum = Math.Max(sum, EnergizeMapWithBeam(input, lineWidth, count, lineWidth - 1, count - 1, BeamDirection.Left));
            }
            return sum;
        }

    }
}