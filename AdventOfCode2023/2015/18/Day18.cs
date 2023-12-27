using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2015
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    [RequestsVisualizer]
    [RuntimeWatchdog(5)]
    class Day18
    {
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(StringSpan input, int lineWidth, int count)
        {
            var map = new Map2DSpan<int>(lineWidth, count, input, (c) => c == '#' ? 1 : 0);
            var map2 = new Map2DSpan<int>(lineWidth, count);
            Visualizer.SendMap2dSpan(map, (v, _, _, _) => v == 0 ? Color.Black : Color.Cyan);

            for (var i = 0; i < 100; i++)
            {
                map2.Map(map, (v, x, y) =>
                {
                    if (v == 0)
                    {
                        var count = map.CountAdjenced(x, y, (i, _) => i > 0);
                        return count == 3 ? 1 : 0;
                    }
                    else
                    {
                        var count = map.CountAdjenced(x, y, (i, _) => i > 0);
                        return count == 3 || count == 2 ? 1 : 0;
                    }
                });
                (map, map2) = (map2, map);
                Visualizer.SendMap2dSpan(map, (v, _, _, _) => v == 0 ? Color.Black : Color.Cyan);
            }
            return map.Count((i) => i > 0);
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int lineWidth, int count)
        {
            var map = new Map2DSpan<int>(lineWidth, count, input, (c) => c == '#' ? 1 : 0);
            var map2 = new Map2DSpan<int>(lineWidth, count);
            Visualizer.SendMap2dSpan(map, (v, _, _, _) => v == 0 ? Color.Black : Color.Cyan);

            for (int i = 0; i < 100; i++)
            {
                map.At(0, 0, 1);
                map.At(0, map.Height - 1, 1);
                map.At(map.Width - 1, 0, 1);
                map.At(map.Width - 1, map.Height - 1, 1);

                map2.Map(map, (v, x, y) =>
                {
                    if (v == 0)
                    {
                        var count = map.CountAdjenced(x, y, (i, _) => i > 0);
                        return count == 3 ? 1 : 0;
                    }
                    else
                    {
                        var count = map.CountAdjenced(x, y, (i, _) => i > 0);
                        return count == 3 || count == 2 ? 1 : 0;
                    }
                });

                map2.At(0, 0, 1);
                map2.At(0, map.Height - 1, 1);
                map2.At(map.Width - 1, 0, 1);
                map2.At(map.Width - 1, map.Height - 1, 1);

                (map, map2) = (map2, map);
                Visualizer.SendMap2dSpan(map, (v, _, _, _) => v == 0 ? Color.Black : Color.Cyan);
            }
            return map.Count((i) => i > 0);
        }
    }
}