using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2015
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(4)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(11)] // if != 0, will report failure if expected answer != given answer
    class Day03
    {
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(StringSpan input, int lineWidth, int count)
        {
            Dictionary<(int, int), int> homes = new();
            int x = 0;
            int y = 0;
            for (int i = 0; i < input.Length; i++)
            {
                var dir = input[i] switch
                {
                    '^' => (0, -1),
                    '>' => (1, 0),
                    'v' => (0, 1),
                    '<' => (-1, 0),
                };

                x += dir.Item1;
                y += dir.Item2;

                if (homes.ContainsKey((x, y)))
                    homes[(x, y)]++;
                else
                    homes[(x, y)] = 1;
            }

            return homes.Count;
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int lineWidth, int count)
        {
                Dictionary<(int, int), int> homes = new();
                int x = 0;
                int y = 0;

                int x2 = 0;
                int y2 = 0;

                for (int i = 0; i < input.Length; i++)
                {
                    if (i == 0)
                    {
                        homes[(x, y)] = 1;
                    }
                    var dir = input[i] switch
                    {
                        '^' => (0, -1),
                        '>' => (1, 0),
                        'v' => (0, 1),
                        '<' => (-1, 0),
                    };

                    if (i % 2 == 0)
                    {
                        x += dir.Item1;
                        y += dir.Item2;

                        if (homes.ContainsKey((x, y))) homes[(x, y)]++;
                        else homes[(x, y)] = 1;
                    }
                    else
                    {
                        x2 += dir.Item1;
                        y2 += dir.Item2;

                        if (homes.ContainsKey((x2, y2))) homes[(x2, y2)]++;
                        else homes[(x2, y2)] = 1;
                    }
                }

                return homes.Count;
            }
    }
}