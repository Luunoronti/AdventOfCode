
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdventOfCode2015
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    class Day12
    {
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string input) => (JsonConvert.DeserializeObject(input) as JArray).Sum(o => GetAllNumbers(o, ignoreRed: false));

        private static int GetAllNumbers(JToken token, bool ignoreRed)
        {
            var sum = 0;
            foreach (var o in token.Children())
            {
                if (o.Type == JTokenType.Integer)
                {
                    sum += o.Value<int>();
                }
                else if (ignoreRed && o.Type == JTokenType.String)
                {
                    //if (o.Parent.Type != JTokenType.Array)
                    {
                        var s = o.Value<string>();
                        if (s == "red")
                        {
                            if (o.Parent.Type != JTokenType.Array)
                            {
                                Log.WriteLine($"Removed \"red\" from {o.Parent.Type} ({o.Parent.Parent.Type})");
                                return -int.MaxValue;
                            }
                        }
                    }
                }
                else
                {
                    var s = GetAllNumbers(o, ignoreRed);
                    if (s == -int.MaxValue)
                    {
                        sum = 0;
                        return 0;
                    }
                    else
                    {
                        sum += s;
                    }
                }
            }

            return sum;
        }

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string input) => (JsonConvert.DeserializeObject(input) as JArray).Sum(o => GetAllNumbers(o, ignoreRed: true));
    }
}