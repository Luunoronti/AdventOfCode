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
        private const int Part1TestLoopCount = 64;
        private const int Part2LiveLoop = 26_501_365;

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

            Log.WriteLine(CC.CursorHide);
            var sum = 0L;

            for (int i = 0; i < Part1TestLoopCount; i++)
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
                Log.Write($"Step {i} / {Part1TestLoopCount}");
                Console.CursorLeft = 0;
            }
            Log.WriteLine(CC.CursorShow);
            return sum;
        }




        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int width, int height)
        {
            return 0;
        }
    }
}