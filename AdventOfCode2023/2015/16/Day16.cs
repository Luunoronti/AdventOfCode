
using System.Reflection;

namespace AdventOfCode2015
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    class Day16
    {

        class Aunt
        {
            public int Index;
            public int children = -1;
            public int cats = -1;
            public int samoyeds = -1;
            public int pomeranians = -1;
            public int akitas = -1;
            public int vizslas = -1;
            public int goldfish = -1;
            public int trees = -1;
            public int cars = -1;
            public int perfumes = -1;

            private static Dictionary<string, FieldInfo> _props = new();
            public Aunt(string line)
            {
                line = line.Replace("Sue ", "");
                var ii = line.IndexOf(':');
                Index = int.Parse(line[..ii]);

                line = line[ii..];

                var options = line.Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var o in options)
                {
                    var sp2 = o.Split(": ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    var fld = _props.TryGetValue(sp2[0], out var p) ? p : _props[sp2[0]] = typeof(Aunt).GetField(sp2[0])!;
                    fld.SetValue(this, int.Parse(sp2[1]));
                }
            }
        }

        public static long Part1(string[] lines)
        {
            var aunts = lines.Select(l => new Aunt(l)).ToList();

            // filter out stuff
            var auntsToRemove = new List<Aunt>();

            foreach (var a in aunts)
            {
                if (a.cats != -1 && a.cats != 7 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.trees != -1 && a.trees != 3 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);

                if (a.pomeranians != -1 && a.pomeranians != 3 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.goldfish != -1 && a.goldfish != 5 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);

                if (a.children != -1 && a.children != 3 && auntsToRemove.Contains(a)==false) auntsToRemove.Add(a);
                if (a.samoyeds != -1 && a.samoyeds != 2 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.akitas != -1 && a.akitas != 0 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.vizslas != -1 && a.vizslas != 0 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.cars != -1 && a.cars != 2 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.perfumes != -1 && a.perfumes != 1 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);

            }

            var toStay = aunts.Except(auntsToRemove).ToList();
            return toStay[0].Index;
        }

        public static long Part2(string[] lines)
        {
            var aunts = lines.Select(l => new Aunt(l)).ToList();

            // filter out stuff
            var auntsToRemove = new List<Aunt>();

            foreach (var a in aunts)
            {
                if (a.children != -1 && a.children != 3 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.samoyeds != -1 && a.samoyeds != 2 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.akitas != -1 && a.akitas != 0 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.vizslas != -1 && a.vizslas != 0 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.cars != -1 && a.cars != 2 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.perfumes != -1 && a.perfumes != 1 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);

                //if (!auntsToRemove.Contains(a))
                //    continue;

                if (a.cats != -1 && a.cats <= 7 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.trees != -1 && a.trees <= 3 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);

                if (a.pomeranians != -1 && a.pomeranians >= 3 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
                if (a.goldfish != -1 && a.goldfish >= 5 && auntsToRemove.Contains(a) == false) auntsToRemove.Add(a);
            }

            var toStay = aunts.Except(auntsToRemove).ToList();
            return toStay[0].Index;
        }
    }
}