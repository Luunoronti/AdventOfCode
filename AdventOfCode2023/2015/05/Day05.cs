namespace AdventOfCode2015
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    class Day05
    {
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] input)
        {
            var niceStrings = 0;
            var naughtyStrings = 0;
            foreach (var line in input)
            {
                // check for 'ab', 'cd', 'pq', 'xy'
                var i1 = line.IndexOf("ab");
                var i2 = line.IndexOf("cd");
                var i3 = line.IndexOf("pq");
                var i4 = line.IndexOf("xy");

                if (i1 >= 0 || i2 >= 0 || i3 >= 0 || i4 >= 0)
                {
                    naughtyStrings++;
                    continue;
                }

                // count vovels (and double letters)
                var hasDoubleLetter = false;
                Dictionary<char, int> vowels = new() { { 'a', 0 }, { 'e', 0 }, { 'i', 0 }, { 'o', 0 }, { 'u', 0 } };
                for (int i = 0; i < line.Length; i++)
                {
                    var c = line[i];
                    if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u')
                    {
                        vowels[c]++;
                    }
                    if (i < line.Length - 1)
                    {
                        if (c == line[i + 1])
                            hasDoubleLetter = true;
                    }
                }

                if (!hasDoubleLetter)
                {
                    naughtyStrings++;
                    continue;
                }

                if (vowels.Sum(v => v.Value) < 3)
                {
                    naughtyStrings++;
                    continue;
                }
                niceStrings++;
            }
            return niceStrings;
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string[] input)
        {
            var niceStrings = 0;

            foreach (var line in input)
            {
                //var pairs = new Dictionary<char, int>();
                var pairsFound = false;
                var foundTwoLettersDivided = false;
                for (int i = 0; i < line.Length - 1; i++)
                {
                    var c1 = line[i];
                    var c2 = line[i + 1];

                    // look for this pair in the rest of the string
                    for (var j = i + 2; j < line.Length-1; j++)
                    {
                        if (line[j] == c1 && line[j + 1] == c2)
                        {
                            pairsFound = true;
                            break;
                        }
                    }

                    if (i < line.Length - 2)
                    {
                        var c3 = line[i + 2];

                        if (c1 == c3)
                            foundTwoLettersDivided = true;
                    }
                }


                if (foundTwoLettersDivided && pairsFound)
                    niceStrings++;
            }

            return niceStrings;
        }
    }
}