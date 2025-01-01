using System.Collections.Concurrent;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Xml.Linq;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    [AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(16)] //in 6 steps
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    class Day21
    {
        private const char StartPos = 'S';
        private const char Rock = '#';
        private const int Part1LoopCount = 64;
        //private const int Part2LoopCount = 20;
        private const int Part2LoopCount = 26_501_365;

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(StringSpan input, int width, int height)
        {
            //Map2DSpan<bool> availMap = new(width, height, input, (c) => c != Rock);
            //Map2DSpan<bool> occupiedMap = new(width, height);
            //Point startingPosition = new(input.IndexOf(StartPos) % width, input.IndexOf(StartPos) / width);

            //Queue<Point> occupied = new();
            //Queue<Point> occupied2 = new();
            //occupied.Enqueue(startingPosition);

            //var sum = 0L;
            //for (int i = 0; i < Part1LoopCount; i++)
            //{
            //    occupiedMap.AsSpan().Clear();

            //    while (occupied.TryDequeue(out var st))
            //    {
            //        var n = new Point(st.X, st.Y - 1);
            //        var s = new Point(st.X, st.Y + 1);
            //        var e = new Point(st.X + 1, st.Y);
            //        var w = new Point(st.X - 1, st.Y);

            //        if (availMap.At(n.X % width, n.Y % height) && !occupiedMap.At(n.X, n.Y)) { occupied2.Enqueue(n); occupiedMap.At(n.X, n.Y, true); }
            //        if (availMap.At(s.X % width, s.Y % height) && !occupiedMap.At(s.X, s.Y)) { occupied2.Enqueue(s); occupiedMap.At(s.X, s.Y, true); }
            //        if (availMap.At(w.X % width, w.Y % height) && !occupiedMap.At(w.X, w.Y)) { occupied2.Enqueue(w); occupiedMap.At(w.X, w.Y, true); }
            //        if (availMap.At(e.X % width, e.Y % height) && !occupiedMap.At(e.X, e.Y)) { occupied2.Enqueue(e); occupiedMap.At(e.X, e.Y, true); }
            //    }
            //    sum = occupied2.Count;
            //    (occupied, occupied2) = (occupied2, occupied);
            //    Console.CursorLeft = 0;
            //}


            Map2DSpan<bool> availMap = new(width, height, input, (c) => c != Rock);
            Vector2Long startingPosition = new(input.IndexOf(StartPos) % width, input.IndexOf(StartPos) / width);
            return Simulate(availMap, startingPosition, new List<int>() { 64 }, out var minx, out var miny, out var maxx, out var maxy)[0];//sum;
        }




        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int width, int height)
        {
            Map2DSpan<bool> availMap = new(width, height, input, (c) => c != Rock);
            Vector2Long startingPosition = new(input.IndexOf(StartPos) % width, input.IndexOf(StartPos) / width);

            //var v = Extrapolate(availMap, startingPosition, n: 26_501_365);
            return 0;// v;
        }






        private static long Extrapolate(Map2DSpan<bool> grid, Vector2Long start, int n)
        {
            //  The input has a number of properties which together allow for an analytic
            //  solution without the need for purely naive simulation: 
            //    - The grid is square
            //    - The start is in the center of the grid
            //    - The row and column running through the center of the grid are entirely empty
            //    - After reaching the boundary of the first grid, which due to the above constraint
            //      happens in exactly <grid size / 2> steps, the remaining number of steps to
            //      simulate has no residue modulo the grid size
            //
            var x0 = grid.Width / 2;
            var dx = grid.Width;
            var ys = Simulate(grid, start, new List<int>() { x0, x0 + dx, x0 + 2 * dx }, out var minx, out var miny, out var maxx, out var maxy);


            Log.WriteLine($"Simulation completed. Simulated between {minx}, {miny} and {maxx}, {maxy} ({maxx - minx}, {maxy - miny} )");

            //  Solve a system of equations to obtain the quadratic coefficients a, b, and c:
            //  (1) y0 = c, (2) y1 = a + b + c, (3) y2 = 4a + b + c
            var c = ys[0];
            var b = (4 * ys[1] - ys[2] - 3 * ys[0]) / 2;
            var a = ys[1] - ys[0] - b;

            var x = (n - x0) / dx;
            return a * x * x + b * x + c;
        }
        private static List<long> Simulate(Map2DSpan<bool> grid, Vector2Long start, List<int> sampleAt, out long minx, out long miny, out long maxx, out long maxy )
        {
            var ticks = new List<long>(capacity: sampleAt.Count);
            var heads = new HashSet<Vector2Long>();
            var after = new HashSet<Vector2Long>();
            minx = 0;
            maxx = 0;
            miny = 0;
            maxy = 0;

            heads.Add(start);

            var cache = new Dictionary<Vector2Long, ISet<Vector2Long>>();

            var lastPercent = 0;
            for (var i = 1; i <= sampleAt.Max(); i++)
            {
                after.Clear();

                var index = 0;
                foreach (var pos in heads)
                {
                    foreach (var adj in GetEmptyAround(grid, pos, cache))
                    {
                        after.Add(adj);
                        minx = Math.Min(minx, adj.x);
                        miny = Math.Min(miny, adj.y);

                        maxx = Math.Max(maxx, adj.x);
                        maxy = Math.Max(maxy, adj.y);
                    }

                    index++;
                    var f = (float)index / heads.Count;
                    var per = (int)(f * 100);
                    if(lastPercent != per)
                    {
                        lastPercent = per;
                     //   Console.CursorLeft = 0;
                      //  Log.Write($"{per, 5}%, {i,5} / {sampleAt.Max(),5}, Heads: {heads.Count, 10}, after: {after.Count, 10}");
                    }

                }
                if (sampleAt.Contains(i))
                    ticks.Add(after.Count);
                (heads, after) = (after, heads);
            }
            Log.WriteLine();
            return ticks;
        }
        private static IEnumerable<Vector2Long> GetEmptyAround(Map2DSpan<bool> grid, Vector2Long pos, Dictionary<Vector2Long, ISet<Vector2Long>> cache)
        {
            if (cache.TryGetValue(pos, out var cached))
                return cached;

            var ret = new HashSet<Vector2Long>();

            var width = grid.Width;
            var height = grid.Height;

            var v1 = pos + new Vector2Long(0, 1);
            var v2 = pos + new Vector2Long(0, -1);
            var v3 = pos + new Vector2Long(-1, 0);
            var v4 = pos + new Vector2Long(1, 0);

            var x = ((v1.x % width) + width) % width;
            var y = ((v1.y % height) + height) % height;
            if (grid.At((int)x, (int)y, out bool oob) && oob == false)
                ret.Add(v1);

            x = ((v2.x % width) + width) % width;
            y = ((v2.y % height) + height) % height;
            if (grid.At((int)x, (int)y, out oob) && oob == false)
                ret.Add(v2);

            x = ((v3.x % width) + width) % width;
            y = ((v3.y % height) + height) % height;
            if (grid.At((int)x, (int)y, out oob) && oob == false)
                ret.Add(v3);

            x = ((v4.x % width) + width) % width;
            y = ((v4.y % height) + height) % height;
            if (grid.At((int)x, (int)y, out oob) && oob == false)
                ret.Add(v4);

            cache[pos] = ret;
            return ret;
        }

    }
}