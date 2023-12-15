
namespace AdventOfCode2023
{
    //[Force] // uncomment to force processing this type
    //[AlwaysEnableLog]
    //[DisableLogInDebug]
    //[UseLiveDataInDeug]
    //[AlwaysUseTestData]
    [ExpectedTestAnswerPart1(21)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(525152)] // if != 0, will report failure if expected answer != given answer
    class Day12
    {
        private static long ProcessPattern(ReadOnlySpan<char> pattern, ReadOnlySpan<int> groups, Dictionary<(int, int), long> cache)
        {
            // these cache operations reduce Part 2 time, from ~30 minutes on my PC, to milliseconds
            if (cache.TryGetValue((pattern.Length, groups.Length), out var ret))
                return ret;

            var sum = 0L;

            var isLast = groups.Length == 1;
            var group = groups[0];

            if (!isLast)
                group++; // add '.' to the group. it is required for any but last group

            if (group > pattern.Length)
                return cache[(pattern.Length, groups.Length)] = 0;

            for (int i = 0; i <= pattern.Length - group; i++)
            {
                // first of all, check if there are any '#' characters before start of our group proposed place
                // if so, those are not satisfied, and this pattern is not valid
                if (pattern[..i].IndexOf('#') != -1)
                    break;

                // then check if we can actually put the group in proposed space
                if (!IsPlacementPossible(pattern.Slice(i, group), isLast: isLast))
                    continue;

                // and, if this is last group, check if there are any '#' characters
                // after the group
                if (isLast && pattern[(i + group)..].IndexOf('#') != -1)
                    continue;

                if (isLast)
                {
                    sum++;
                }
                else
                {
                    // process next group for this propsed pattern
                    sum += ProcessPattern(pattern[(i + group)..], groups[1..], cache);
                }
            }
            return cache[(pattern.Length, groups.Length)] = sum;

            static bool IsPlacementPossible(ReadOnlySpan<char> span, bool isLast)
            {
                var dot = span.IndexOf('.');
                if (dot != -1)
                {
                    if (!isLast)
                    {
                        if (dot != span.Length - 1)
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                if (!isLast && span[^1] == '#') return false;
                return true;
            }
        }

        // THIS METHOD CONTAINS GOTO!! :D
        // Please don't judge me, but sometimes it's best for readability, 
        // and it's being used a lot in <insert frameworks for some popular gaming platforms I can't mention here> initialization routines
        private static long SolveLine(string line, int repeats)
        {
            var parts = line.Split(" ");
            // we construct pattern the same way other code (bellow) does
            var pattern = string.Join('?', Enumerable.Repeat(parts[0], repeats));
            var nums = new ReadOnlySpan<int>(Enumerable.Repeat(parts[1].Split(',').Select(int.Parse), repeats).SelectMany(x => x).ToArray());
            return ProcessPattern(pattern.AsSpan(), nums, new Dictionary<(int, int), long>());
        }

        public static long Part1(string[] lines) => lines.Select(l => SolveLine(l, 1)).Sum();
        public static long Part2(string[] lines) => lines.Select(l => SolveLine(l, 5)).Sum();
    }










    //[Force] // uncomment to force processing this type
    //[AlwaysEnableLog]
    //[DisableLogInDebug]
    //[UseLiveDataInDeug]
    //[AlwaysUseTestData]

    //!!!!!!!!!!!!!
    //
    // this code is copied from
    // https://github.com/encse/adventofcode/blob/master/2023/Day12/Solution.cs
    // I've got first part via brute force permutation generator.
    // but it failed miserably at second part.
    // I still don't understand this code (mostly because it's db linq, and I am too old for functional programming :),
    // but i've got it here to debug and get a grasp of it.

    // after rewriting and a bunch of logs, I understand it a bit.
    //class Day12_SomeoneElsesCode
    //{
    //    public static string TestFile => "2023\\12\\test.txt";
    //    public static string LiveFile => "2023\\12\\live.txt";


    //    private static string In(int indent) => "".PadLeft(indent);
    //    private static int lineSize = 0;
    //    private static Dictionary<(string, ImmutableStack<int>), long> cache = new();

    //    private static long Compute(string pattern, ImmutableStack<int> nums, int indent = 0)
    //    {
    //        var key = (pattern, nums);
    //        if (cache.TryGetValue(key, out var value))
    //        {
    //            //   Log.WriteLine($"{In(indent)}{pattern}, {nums.ToReadable()}{CC.Frm} (cac) Got value: {value}{CC.Clr}");
    //            return value;
    //        }

    //        if (pattern.Length == 0)
    //        {
    //            // the good case is when there are no numbers left at the end of the pattern
    //            cache[key] = nums.Any() ? 0 : 1;
    //            return cache[key];
    //        }


    //        var c = pattern[0];
    //        switch (c)
    //        {
    //            case '.':
    //                // consume one spring and recurse
    //                Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('.') Consume one spring and recurse{CC.Clr}");
    //                cache[key] = Compute(pattern[1..], nums, indent + 1);
    //                //   Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
    //                break;
    //            case '?':
    //                // recurse both ways
    //                Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('?') Recurse both ways (for '.' and '#'){CC.Clr}");
    //                cache[key] = Compute("#" + pattern[1..], nums, indent + 1) + Compute("." + pattern[1..], nums, indent + 1);
    //                //   Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
    //                break;
    //            case '#':
    //                if (!nums.Any())
    //                {
    //                    Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') No more numbers left, this is no good{CC.Clr}");
    //                    cache[key] = 0; // no more numbers left, this is no good
    //                                    //     Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
    //                }
    //                else
    //                {
    //                    var n = nums.Peek();
    //                    nums = nums.Pop();

    //                    var potentiallyDead = pattern.TakeWhile(s => s == '#' || s == '?').Count();
    //                    if (potentiallyDead < n)
    //                    {
    //                        Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') Not enough dead springs {CC.Clr}");
    //                        cache[key] = 0; // not enough dead springs 
    //                                        //        Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
    //                    }
    //                    else if (pattern.Length == n)
    //                    {
    //                        Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') pattern len == n, compute empty string {CC.Clr}");
    //                        cache[key] = Compute("", nums, indent + 1);
    //                        //        Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
    //                    }
    //                    else if (pattern[n] == '#')
    //                    {
    //                        Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') dead spring follows the range -> not good {CC.Clr}");
    //                        cache[key] = 0; // dead spring follows the range -> not good
    //                                        //        Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
    //                    }
    //                    else
    //                    {
    //                        Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') compute for next number on stack (n + 1) {CC.Clr}");
    //                        cache[key] = Compute(pattern[(n + 1)..], nums, indent + 1);
    //                        //   Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
    //                    }
    //                }
    //                break;
    //        }
    //        return cache[key];
    //    }



    //    public static long Solve(string line, int repeat)
    //    {
    //        var parts = line.Split(" ");
    //        var pattern = string.Join('?', Enumerable.Repeat(parts[0], repeat));
    //        var nums = string.Join(',', Enumerable.Repeat(parts[1], repeat)).Split(',').Select(int.Parse);

    //        var range = ImmutableStack.CreateRange(nums.Reverse());
    //        cache.Clear();
    //        lineSize = line.Length;
    //        return Compute(pattern, range);
    //    }



    //    public static long Part1(string[] lines)
    //    {
    //        Solve(lines[1], 1);
    //        var sum = 0L;
    //        foreach (var line in lines)
    //        {
    //            //    sum += Solve(line, 1);
    //        }
    //        return sum;
    //    }
    //    public static long Part2(string[] lines)
    //    {
    //        var sum = 0L;
    //        foreach (var line in lines)
    //        {
    //            //    sum += Solve(line, 5);
    //        }
    //        return sum;
    //    }
    //}
}