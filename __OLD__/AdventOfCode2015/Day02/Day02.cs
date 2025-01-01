
using AmaAocHelpers;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2015;

//[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
//[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
//[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
//[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
//[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
[ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
[ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
class Day02
{
    public static long Part1(string[] input)
    {
        long sum = 0;

        foreach (var line in input)
        {
            var sp = line.Split('x').Select(int.Parse).ToArray();
            var s1 = 2 * sp[0] * sp[1];
            var s2 = 2 * sp[1] * sp[2];
            var s3 = 2 * sp[2] * sp[0];
            var s = s1 + s2 + s3 + Math.Min(Math.Min(s1 / 2, s2 / 2), s3 / 2);
            sum += s;
        }
        return sum;
    }
    public static long Part2(string[] input)
    {
        long sum = 0;

        foreach (var line in input)
        {
            var sp = line.Split('x').Select(int.Parse).ToList();

            var m1 = sp.Min();
            var m3 = sp.Max();
            sp.Remove(m1);
            sp.Remove(m3);
            var m2 = sp.First();

            var band = 2 * m1 + 2 * m2 + m1 * m2 * m3;
            sum += band;
        }
        return sum;
    }
}
