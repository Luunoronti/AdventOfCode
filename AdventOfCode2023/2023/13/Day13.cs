namespace AdventOfCode2023
{
    //[Force] // uncomment to force processing this type
    //[AlwaysEnableLog]
    //[DisableLogInDebug]
    [UseLiveDataInDeug]
    //[AlwaysUseTestData]
    class Day13
    {
        public static string TestFile => "2023\\13\\test.txt";
        public static string LiveFile => "2023\\13\\live.txt";





        private static List<Pattern> GetPatterns(string[] lines)
        {
            var current = new Pattern();
            var list = new List<Pattern> { current };

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    current.ConstructPatterns();
                    current = new Pattern();
                    list.Add(current);
                    continue;
                }
                current.AddLine(line);
            }
            return list;
        }

        public static long Part1(string[] lines)
        {
            var patterns = GetPatterns(lines);

            var sum = 0L;
            foreach (var pattern in patterns)
            {
                var hr = pattern.FindHorizontalReflections(fixSmudges: false, invalidPosition: (-1, -1), out _);

                if (hr.Item3)
                {
                    Log.WriteLine($"Found horizontal: {hr.Item1}, {hr.Item2}");
                    // this is rows to top
                    sum += (hr.Item1 * 100);
                }
                else
                {
                    var vr = pattern.FindVerticalReflections(fixSmudges: false, invalidPosition: (-1, -1), out var _);

                    if (vr.Item3)
                    {
                        Log.WriteLine($"Found vertical: {vr.Item1}, {vr.Item2}");

                        var columnsToTheLeft = vr.Item1;
                        sum += columnsToTheLeft;
                    }
                }
                Log.WriteLine();
            }

            return sum;
        }
        public static long Part2(string[] lines)
        {
            var patterns = GetPatterns(lines);

            var sum = 0L;
            for (int i = 0; i < patterns.Count; i++)
            {
                Pattern? pattern = patterns[i];
                // find with no smudge, to see which position is bad
                var hrns = pattern.FindHorizontalReflections(fixSmudges: false, invalidPosition: (-1, -1), out _);
                var hr = pattern.FindHorizontalReflections(fixSmudges: true, invalidPosition: (hrns.Item1, hrns.Item2), out var smudgeFound);

                if (hr.Item3)
                {
                    Log.WriteLine($"Pattern {i}: Found horizontal: {hr.Item1}, {hr.Item2}");
                    // this is rows to top
                    sum += (hr.Item1 * 100);
                }
                else if (hrns.Item3)
                {
                    Log.WriteLine($"Pattern {i}: Found horizontal (same as with no smudge): {hrns.Item1}, {hrns.Item2}");
                    // this is rows to top
                    sum += (hrns.Item1 * 100);
                }
                else
                {
                    var vrns = pattern.FindHorizontalReflections(fixSmudges: false, invalidPosition: (-1, -1), out _);
                    var vr = pattern.FindVerticalReflections(fixSmudges: !smudgeFound, invalidPosition: (vrns.Item1, vrns.Item2), out var _);

                    if (vr.Item3)
                    {
                        Log.WriteLine($"Pattern {i}: Found vertical: {vr.Item1}, {vr.Item2}");
                        var columnsToTheLeft = vr.Item1;
                        sum += columnsToTheLeft;
                    }
                    else if (vrns.Item3)
                    {
                        Log.WriteLine($"Pattern {i}: Found vertical (same as with no smudge): {vrns.Item1}, {vrns.Item2}");
                        var columnsToTheLeft = vrns.Item1;
                        sum += columnsToTheLeft;
                    }
                }
                Log.WriteLine();
            }

            return sum;
        }



        class Pattern
        {
            private List<string> horizontal = new();
            private List<string> rotated = new(); // use this to find columns (treat them as rows)
            internal void AddLine(string line)
            {
                horizontal.Add(line);

            }
            internal void ConstructPatterns()
            {
                rotated.Clear();
                // rotate strings 90 degrees    
                for (int j = 0; j < horizontal[0].Length; j++)
                {
                    var str = "";
                    for (int i = 0; i < horizontal.Count; i++)
                    {
                        str += horizontal[i][j];
                    }
                    rotated.Add(str);
                }
            }


            internal (int, int, bool) FindReflection(List<string> input, bool fixSmudges, (int, int) invalidPosition, out bool smudgeFound)
            {
                smudgeFound = false;
                int smudgesFound = 0;
                for (int i = 0; i < input.Count - 1; i++)
                {
                    //if (i + 1 == invalidPosition.Item1 && i + 2 == invalidPosition.Item2)
                    //    continue;

                    var reflectionFailed = false;
                    for (int pos = i + 1, pos2 = i; pos < input.Count && pos2 >= 0; pos++, pos2--)
                    {
                        var s1 = input[pos];
                        var s2 = input[pos2];

                        if (s1 != s2)
                        {
                            // not same, reflection failed
                            if (fixSmudges)
                            {
                                var difference = 0;
                                for (int si = 0; si < s1.Length; si++)
                                {
                                    if (s1[si] != s2[si])
                                        difference++;
                                }
                                if (difference == 1)
                                {
                                    // one char is different, so we may as well assume that one
                                    // to be a smudge
                                    //Log.WriteLine($"Line {pos2} before change");
                                    //Log.WriteLine(input[pos2]);

                                    input[pos2] = input[pos]; // override one of the strings


                                    //Log.WriteLine($"Line {pos2} after change");
                                    //Log.WriteLine(input[pos2]);

                                    // we must reconstruct rotated pattern
                                    ConstructPatterns();

                                    smudgeFound = true;
                                    smudgesFound++;

                                    if(smudgesFound>1)
                                    {
                                        Log.WriteLine("More than one candidate found to be a smudge!");
                                    }
                                }
                                else
                                {
                                    reflectionFailed = true;
                                }
                            }
                            else
                            {
                                reflectionFailed = true;
                            }
                        }
                    }
                    if (i + 1 == invalidPosition.Item1 && i + 2 == invalidPosition.Item2)
                    {
                        Log.WriteLine("Smudge: found same position as invalid. Looking for more.");
                        reflectionFailed = true;
                    }

                    if (reflectionFailed == false)
                    {
                        if (smudgeFound)
                        {
                            Log.WriteLine("Smudge: found.");
                        }
                        // becase we start at 0, we need to add one
                        return (i + 1, i + 2, true);
                    }


                }

                return (0, 0, false);
            }
            internal (int, int, bool) FindHorizontalReflections(bool fixSmudges, (int, int) invalidPosition, out bool smudgeFound) => FindReflection(horizontal, fixSmudges, invalidPosition, out smudgeFound);
            internal (int, int, bool) FindVerticalReflections(bool fixSmudges, (int, int) invalidPosition, out bool smudgeFound) => FindReflection(rotated, fixSmudges, invalidPosition, out smudgeFound);
        }
    }
}