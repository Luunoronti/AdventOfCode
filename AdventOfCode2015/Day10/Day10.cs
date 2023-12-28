
using System.Text;
using AmaAocHelpers;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode0;

//[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
//[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
//[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
//[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
//[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
[ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
[ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
class Day10
{
    private static int Compute(string input, int totalCount)
    {
        var sb = new StringBuilder();
        for (int count = 0; count < totalCount; count++)
        {
            List<(char, int)> list = new();

            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                if (list.Count == 0)
                {
                    list.Add((c, 1));
                    continue;
                }
                var li = list.Count - 1;
                if (list[li].Item1 == c)
                {
                    list[li] = (c, list[li].Item2 + 1);
                    continue;
                }
                list.Add((c, 1));
            }

            sb.Clear();
            foreach (var l in list)
            {
                sb.Append($"{l.Item2}{l.Item1}");
            }

            input = sb.ToString();
            Log.WriteLine(input);
        }
        return input.Length;

    }
    public static long Part1(string input) => Compute(input, 40);

    public static long Part2(string input) => Compute(input, 50);
}
