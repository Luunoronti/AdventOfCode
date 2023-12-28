
using AmaAocHelpers;
using AmaAocHelpers.Tools.Maps;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode0;

//[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
//[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
//[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
//[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
//[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
[ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
[ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
class Day06
{
    enum Operation
    {
        Toggle,
        On,
        Off,
    }
    public static long Part1(string[] input)
    {
        var map = new Map2d<bool>(1000, 1000);

        foreach (var line in input)
        {
            var l = line.Replace("through", ",");
            var op = line.StartsWith("toggle ") ? Operation.Toggle : line.StartsWith("turn on ") ? Operation.On : Operation.Off;
            var ind = op == Operation.Toggle ? 7 : op == Operation.On ? 8 : 9;
            l = l[ind..];

            var data = l.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
            var dx = data[0];
            var dy = data[1];
            var dw = data[2] - dx;
            var dh = data[3] - dy;

            for (var y = 0; y <= dh; y++)
                for (var x = 0; x <= dw; x++)
                    map[dx + x, dy + y] = op == Operation.On ? true : op == Operation.Off ? false : !map[dx + x, dy + y];
        }
        //Visualizer.SendMap2dSpan(map, (b, m, x, y) => b ? Color.Cyan : Color.Black);
        var count = map.Count(m => m);
        return count;
    }

    public static long Part2(string[] input)
    {
        var map = new Map2d<int>(1000, 1000);

        foreach (var line in input)
        {
            var l = line.Replace("through", ",");
            var op = line.StartsWith("toggle ") ? Operation.Toggle : line.StartsWith("turn on ") ? Operation.On : Operation.Off;
            var ind = op == Operation.Toggle ? 7 : op == Operation.On ? 8 : 9;
            l = l[ind..];

            var data = l.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
            var dx = data[0];
            var dy = data[1];
            var dw = data[2] - dx;
            var dh = data[3] - dy;

            for (var y = 0; y <= dh; y++)
            {
                for (var x = 0; x <= dw; x++)
                {
                    if (op == Operation.Toggle)
                        map[dx + x, dy + y] += 2;
                    else if (op == Operation.On)
                        map[dx + x, dy + y] += 1;
                    else if (op == Operation.Off)
                        map[dx + x, dy + y] = Math.Max(0, map[dx + x, dy + y] - 1);
                }
            }
        }
        //Visualizer.SendMap2dSpan(map, (b, m, x, y) => BitmapContext.Clr(b, b, b));
        var count = map.Sum(m => m);
        return count;
    }

}
