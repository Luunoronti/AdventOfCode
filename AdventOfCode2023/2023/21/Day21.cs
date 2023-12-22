using System.Collections.Concurrent;
using System.Transactions;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    [AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(16)] //in 6 steps
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    class Day21
    {
        private const char StartPos = 'S';
        private const char Rock = '#';
        private const int Part1LoopCount = 6;//64;
        //private const int Part2LoopCount = 20;
        private const int Part2LoopCount = 26_501_365;

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(StringSpan input, int width, int height)
        {
            Map2DSpan<bool> availMap = new(width, height, input, (c) => c != Rock);
            Map2DSpan<bool> occupiedMap = new(width, height);
            Point startingPosition = new(input.IndexOf(StartPos) % width, input.IndexOf(StartPos) / width);

            Queue<Point> occupied = new();
            Queue<Point> occupied2 = new();
            occupied.Enqueue(startingPosition);

            var sum = 0L;
            for (int i = 0; i < Part1LoopCount; i++)
            {
                occupiedMap.AsSpan().Clear();

                while (occupied.TryDequeue(out var st))
                {
                    var n = new Point(st.X, st.Y - 1);
                    var s = new Point(st.X, st.Y + 1);
                    var e = new Point(st.X + 1, st.Y);
                    var w = new Point(st.X - 1, st.Y);

                    if (availMap.At(n.X % width, n.Y % height) && !occupiedMap.At(n.X, n.Y)) { occupied2.Enqueue(n); occupiedMap.At(n.X, n.Y, true); }
                    if (availMap.At(s.X % width, s.Y % height) && !occupiedMap.At(s.X, s.Y)) { occupied2.Enqueue(s); occupiedMap.At(s.X, s.Y, true); }
                    if (availMap.At(w.X % width, w.Y % height) && !occupiedMap.At(w.X, w.Y)) { occupied2.Enqueue(w); occupiedMap.At(w.X, w.Y, true); }
                    if (availMap.At(e.X % width, e.Y % height) && !occupiedMap.At(e.X, e.Y)) { occupied2.Enqueue(e); occupiedMap.At(e.X, e.Y, true); }
                }
                sum = occupied2.Count;
                (occupied, occupied2) = (occupied2, occupied);
                Console.CursorLeft = 0;
            }
            return sum;
        }




        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int width, int height)
        {
            ConcurrentDictionary<Point, int> visitedCount = new();

            var mapDrawer = Log.CreateRectangularMapContext(width, height);
            //mapDrawer.Init(0);
            //mapDrawer.SetBackgroundPostProcess((@in, pos)
            //=>
            //{
            //    var key = new Point(pos.x, pos.y);
            //    if (visitedCount.ContainsKey(key))
            //        return ((byte)(Math.Min(255, visitedCount[key] * 5)), 0, 0);
            //    return (0, 0, 0);
            //});

            //mapDrawer.SetContent(input, replaceDots: true);
            //mapDrawer.FillForegroundColor(30, 30, 30);
            //mapDrawer.FillBackgroundColor(0, 0, 0);





            List<long> deltas = new();
            List<long> deltaDeltas = new();
            List<long> values = new();

            Dictionary<(int, int), int> tileDeltas = new();
            Dictionary<(int, int), int> tileLastValues = new();
            var top = Console.CursorTop;

            int valueAtStartTile = 0;
            int valueAtStartLeftTile = 0;

            for (int j = 0; j < 60; j++)
            {
                Console.CursorTop = top;

                Console.WriteLine($"Step: {j}");

                Map2DSpan<bool> availMap = new(width, height, input, (c) => c != Rock);
                Map2DSpan<bool> occupiedMap = new(width, height);
                Point startingPosition = new(input.IndexOf(StartPos) % width, input.IndexOf(StartPos) / width);

                Queue<Point> occupied = new();
                Queue<Point> occupied2 = new();
                occupied.Enqueue(startingPosition);

                visitedCount.Clear();

                var sum = 0L;
                for (int i = 0; i < j * 10 + Part2LoopCount; i++)
                {
                    // process only the tiles that are not yet full
                    occupiedMap.AsSpan().Clear();

                    while (occupied.TryDequeue(out var st))
                    {
                        var n = new Point(st.X, st.Y - 1);
                        var s = new Point(st.X, st.Y + 1);
                        var e = new Point(st.X + 1, st.Y);
                        var w = new Point(st.X - 1, st.Y);

                        if (availMap.At(n.X % width, n.Y % height) && !occupiedMap.At(n.X, n.Y)) { occupied2.Enqueue(n); occupiedMap.At(n.X, n.Y, true); visitedCount.AddOrUpdate(n, 0, (_, i) => i + 1); }
                        if (availMap.At(s.X % width, s.Y % height) && !occupiedMap.At(s.X, s.Y)) { occupied2.Enqueue(s); occupiedMap.At(s.X, s.Y, true); visitedCount.AddOrUpdate(s, 0, (_, i) => i + 1); }
                        if (availMap.At(w.X % width, w.Y % height) && !occupiedMap.At(w.X, w.Y)) { occupied2.Enqueue(w); occupiedMap.At(w.X, w.Y, true); visitedCount.AddOrUpdate(w, 0, (_, i) => i + 1); }
                        if (availMap.At(e.X % width, e.Y % height) && !occupiedMap.At(e.X, e.Y)) { occupied2.Enqueue(e); occupiedMap.At(e.X, e.Y, true); visitedCount.AddOrUpdate(e, 0, (_, i) => i + 1); }
                    }
                    sum = occupied2.Count;
                    (occupied, occupied2) = (occupied2, occupied);
                }

                //mapDrawer.Draw();

                values.Add(sum);
                if (values.Count > 1)
                    deltas.Add(values[^1] - values[^2]);

                if (deltas.Count > 1)
                    deltaDeltas.Add(deltas[^1] - deltas[^2]);
                Log.WriteLine($"Curr: {sum}, Deltas: {string.Join(", ", deltas)}, Deltas^2: {string.Join(", ", deltaDeltas)}");


             

                // amount of numbers for each 'tile'
                List<int> sumsForTiles = new();
                for (var ty = 0; ty < height / 11; ty++)
                {
                    for (var tx = 0; tx < width / 11; tx++)
                    {
                        var isStart = false;
                        var leftToStart = false;

                        var sumForTime = 0;
                        for (var y = 0; y < 11; y++)
                        {
                            for (var x = 0; x < 11; x++)
                            {
                                var point = new Point((tx * 11) + x, (ty * 11) + y);
                                if (visitedCount.TryGetValue(point, out var r))
                                {
                                    sumForTime += r;
                                }
                                if (point == startingPosition)
                                    isStart = true;
                                if (point == new Point(startingPosition.X - 11, startingPosition.Y))
                                    leftToStart = true;
                            }
                        }
                        if (isStart)
                        {
                            var startV = sumForTime;
                            Console.WriteLine($"Delta at start : ({startV} - {valueAtStartTile}): {startV - valueAtStartTile}");
                            valueAtStartTile = startV;
                        }
                        //if (leftToStart)
                        //{
                        //    var startV = sumForTime;
                        //    Console.WriteLine($"Delta at left : ({startV} - {valueAtStartLeftTile}): {startV - valueAtStartLeftTile}");
                        //    valueAtStartLeftTile = startV;
                        //}

                        if (tileLastValues.TryGetValue((tx, ty), out var sft))
                            tileDeltas[(tx, ty)] = sumForTime - sft;
                        else 
                            tileDeltas[(tx, ty)] = sumForTime;

                        tileLastValues[(tx, ty)] = sumForTime;
                    }
                }
                for (var ty = 0; ty < height / 11; ty++)
                {
                    for (var tx = 0; tx < width / 11; tx++)
                    {
                        if (tileDeltas.TryGetValue((tx, ty), out var sft)) Console.Write($"{sft,5} ");
                        else Console.Write($"{"",5} ");
                    }
                    Console.WriteLine();
                }





                Thread.Sleep(500);

                Console.ReadKey(true);
            }


            //mapDrawer.SetForegroundPostProcess((@in, pos) =>
            //{
            //    var m = _mapMemory[pos.y * width + pos.x];
            //    var per = (float)BitOperations.PopCount(m) / (float)bitCountMax;

            //    var clr = (byte)Math.Max(30, Math.Min(255, (int)(250 * per)));
            //    return (20, clr, clr);
            //});





            return 0;
        }
    }
}