
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2015
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(12)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(19)] // if != 0, will report failure if expected answer != given answer
    class Day08
    {
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(StringSpan input)
        {
            var charsCount = 0;
            var bytesCount = input.Length;

            for (int i = 0; i < bytesCount - 1; i++)
            {
                var c1 = input[i];
                var c2 = input[i + 1];

                if (c1 == '"')
                {
                    // skip this
                    continue;
                }
                if (c1 == '\\' && c2 == '\\')
                {
                    // escape two chars
                    charsCount++;
                    i++;
                }
                else if (c1 == '\\' && c2 == '"')
                {
                    // escape two chars
                    charsCount++;
                    i++;
                }
                else if (c1 == '\\' && c2 == 'x')
                {
                    // escape 4 chars chars
                    charsCount++;
                    i += 3;
                }
                else
                {
                    charsCount++;
                }
            }



            return bytesCount - charsCount;
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string[] input)
        {
            var charsCount = 0;
            var bytesCount = 0;// input.Length;

            foreach (var line in input)
            {
                bytesCount += line.Length;

                for (int i = 0; i < line.Length; i++)
                {
                    var c1 = line[i];
                    // first and last "
                    if (i == 0 || i == line.Length - 1)
                    {
                        charsCount += 3; // original " plus \"
                        continue;
                    }
                    else if (c1 == '\\' || c1 == '"')
                    {
                        charsCount += 2; // original \ plus \
                        continue;
                    }
                    else
                    {
                        charsCount++;
                    }
                }
            }
            return charsCount - bytesCount;
        }
    }
}