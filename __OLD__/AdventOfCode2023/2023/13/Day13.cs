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
            var list = new List<Pattern>();
            var tmpString = new List<string>();

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    if (tmpString.Count > 0)
                    {
                        list.Add(new Pattern(tmpString));
                        tmpString.Clear();
                    }
                }
                else
                {
                    tmpString.Add(line);
                }
            }
            if (tmpString.Count > 0)
            {
                list.Add(new Pattern(tmpString));
                tmpString.Clear();
            }
            return list;
        }

        public static long Part1(string[] lines)
        {
            var patterns = GetPatterns(lines);

            var sum = 0L;
            for (int i = 0; i < patterns.Count; i++)
            {
                Pattern? pattern = patterns[i];
                var horizontal = pattern.HorizontalReflections;
                var vertical = pattern.VerticalReflections;

                Log.WriteLine($"Pattern {CC.Att}{i:D02}{CC.Clr}: Found horizontal reflections: {CC.Val}{string.Join(", ", horizontal)}{CC.Clr}");
                Log.WriteLine($"Pattern {CC.Att}{i:D02}{CC.Clr}: Found vertical reflections: {CC.Val}{string.Join(", ", vertical)}{CC.Clr}");

                if (horizontal.Count > 0) sum += horizontal[0].Line1 * 100;
                else if (vertical.Count > 0) sum += vertical[0].Line1;
                else Log.WriteLine($"{CC.Err}Pattern {i:D02} has no reflections!{CC.Clr}");

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
                var horizontal = pattern.HorizontalReflections;
                var vertical = pattern.VerticalReflections;

                var horizontal_Smudge = pattern.HorizontalReflectionsWithSmudge;
                var vertical_Smudge = pattern.VerticalReflectionsWithSmudge;

                var horizontal_Smudge_distinct = horizontal_Smudge.Except(horizontal).ToList();
                var vertical_Smudge_distinct = vertical_Smudge.Except(vertical).ToList();


                Log.WriteLine($"Pattern {CC.Att}{i:D02}{CC.Clr}: Found horizontal reflections: {CC.Val}{string.Join(", ", horizontal)}{CC.Clr}");
                Log.WriteLine($"Pattern {CC.Att}{i:D02}{CC.Clr}: Found horizontal reflections (smudge): {CC.Val}{string.Join(", ", horizontal_Smudge)}{CC.Clr}");
                Log.WriteLine($"Pattern {CC.Att}{i:D02}{CC.Clr}: Found horizontal reflections (smudge, except): {CC.Val}{string.Join(", ", horizontal_Smudge_distinct)}{CC.Clr}");

                Log.WriteLine($"Pattern {CC.Att}{i:D02}{CC.Clr}: Found vertical reflections: {CC.Val}{string.Join(", ", vertical)}{CC.Clr}");
                Log.WriteLine($"Pattern {CC.Att}{i:D02}{CC.Clr}: Found vertical reflections (smudge): {CC.Val}{string.Join(", ", vertical_Smudge)}{CC.Clr}");
                Log.WriteLine($"Pattern {CC.Att}{i:D02}{CC.Clr}: Found vertical reflections (smudge, except): {CC.Val}{string.Join(", ", vertical_Smudge_distinct)}{CC.Clr}");




                if (horizontal_Smudge_distinct.Count > 0) sum += horizontal_Smudge_distinct[0].Line1 * 100;
                else if (vertical_Smudge_distinct.Count > 0) sum += vertical_Smudge_distinct[0].Line1;
                else if (horizontal.Count > 0) sum += horizontal[0].Line1 * 100;
                else if (vertical.Count > 0) sum += vertical[0].Line1;
                else Log.WriteLine($"{CC.Err}Pattern {i:D02} has no reflections!{CC.Clr}");

                Log.WriteLine();
            }

            return sum;
        }



        struct Reflection
        {
            public int Line1;
            public int Line2;
            public override string ToString() => $"{Line1}-{Line2}";
        }
        class Pattern
        {
            internal List<Reflection> VerticalReflections { get; }
            internal List<Reflection> VerticalReflectionsWithSmudge { get; }
            internal List<Reflection> HorizontalReflections { get; }
            internal List<Reflection> HorizontalReflectionsWithSmudge { get; }

            public Pattern(List<string> lines)
            {
                HorizontalReflections = FindReflections(lines).ToList();
                HorizontalReflectionsWithSmudge = FindReflectionsWithSmudge(lines).ToList();

                lines = RotatePattern(lines);

                VerticalReflections = FindReflections(lines).ToList();
                VerticalReflectionsWithSmudge = FindReflectionsWithSmudge(lines).ToList();
            }


            private static IEnumerable<Reflection> FindReflections(List<string> input)
            {
                for (int i = 0; i < input.Count - 1; i++)
                {
                    var reflectionFailed = false;
                    for (int pos = i + 1, pos2 = i; pos < input.Count && pos2 >= 0; pos++, pos2--)
                    {
                        var s1 = input[pos];
                        var s2 = input[pos2];
                        if (s1 != s2)
                        {
                            reflectionFailed = true;
                        }
                    }

                    if (reflectionFailed == false)
                    {
                        // becase we start at 0, we need to add one
                        yield return new Reflection { Line1 = i + 1, Line2 = i + 2 };
                    }
                }
            }
            private static IEnumerable<Reflection> FindReflectionsWithSmudge(List<string> input)
            {
                for (int i = 0; i < input.Count - 1; i++)
                {
                    var reflectionFailed = false;
                    var smudgeCountForProble = 0;
                    for (int pos = i + 1, pos2 = i; pos < input.Count && pos2 >= 0; pos++, pos2--)
                    {
                        var s1 = input[pos];
                        var s2 = input[pos2];

                        // count differences between strings.
                        var diffCount = 0;
                        for (int si = 0; si < s1.Length; si++)
                            if (s1[si] != s2[si])
                                diffCount++;

                        if (diffCount == 0)
                        {
                            continue;
                        }
                        if (diffCount > 1)
                        {
                            reflectionFailed = true;
                            break;
                        }

                        if (diffCount == 1)
                        {
                            if (smudgeCountForProble > 0)
                            {
                                reflectionFailed = true;
                                break;
                            }
                            smudgeCountForProble++;
                        }
                    }

                    if (reflectionFailed == false)
                    {
                        // becase we start at 0, we need to add one
                        yield return new Reflection { Line1 = i + 1, Line2 = i + 2 };
                    }
                }
            }


            private static List<string> RotatePattern(List<string> input)
            {
                var rotated = new List<string>();
                // rotate strings 90 degrees    
                for (int j = 0; j < input[0].Length; j++)
                {
                    var str = "";
                    for (int i = 0; i < input.Count; i++)
                    {
                        str += input[i][j];
                    }
                    rotated.Add(str);
                }
                return rotated;
            }


        }
    }
}