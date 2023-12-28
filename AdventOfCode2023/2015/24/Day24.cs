
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
    class Day24
    {
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] input, int lineWidth, int count)
        {
            var packages = input.Select(int.Parse).ToList();
            packages.Sort();
            packages.Reverse();

            var we_3 = packages.Sum() / 3;

            List<int> group1 = new();

            for (int i = 0; i < packages.Count; i++)
            {
                var sum = group1.Sum();
                var missing = we_3 - sum;

            }

            return 0;
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int lineWidth, int count)
        {
            return 0;
        }
    }
}