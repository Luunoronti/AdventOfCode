


//TODO: when i get a free time, I'll try to make something of my own




// new idea:
// construct all possible solution for each group at a time,
// for solutions, create all possible solutions for second group, and so on.
// also, because we know part of the solution was good, we just need to check part that is 'new'




using System.Collections.Immutable;

namespace AdventOfCode2023
{
    class Day12
    {
        public static string TestFile => "2023\\12\\test.txt";
        public static string LiveFile => "2023\\12\\live.txt";

        private static long SolveLine(string line, int repeats)
        {
            var parts = line.Split(" ");
            // we construct pattern the same way other code (bellow) does
            var pattern = string.Join('?', Enumerable.Repeat(parts[0], repeats));
            // here, we create a queue of all groups, instead of just enumerable
            var nums = new Queue<int>(Enumerable.Repeat(parts[1].Split(',').Select(int.Parse), repeats).SelectMany(x => x));

            // two queues of solutions
            var list1 = new Queue<string>();
            var list2 = new Queue<string>();

            // make new variables so we don't mess our assignments because of poor naming :)
            var srcList = list1;
            var dstList = list2;

            // we add a start string to our src queue, so we don't have to deal with empty source inside our loop.
            srcList.Enqueue("");

            while (nums.TryDequeue(out var group))  // if there are no more groups to check against, end this loop
            {
                // look for all (valid) solutions for this group
                // and add them to second list

                

                // clear surce (we don't need it anymore) and switch 
                srcList.Clear();
                (dstList, srcList) = (srcList, dstList);
            }

            return 0;
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
    class Day12_SomeoneElsesCode
    {
        public static string TestFile => "2023\\12\\test.txt";
        public static string LiveFile => "2023\\12\\live.txt";


        private static string In(int indent) => "".PadLeft(indent);
        private static int lineSize = 0;
        private static Dictionary<(string, ImmutableStack<int>), long> cache = new();

        private static long Compute(string pattern, ImmutableStack<int> nums, int indent = 0)
        {
            var key = (pattern, nums);
            if (cache.TryGetValue(key, out var value))
            {
                //   Log.WriteLine($"{In(indent)}{pattern}, {nums.ToReadable()}{CC.Frm} (cac) Got value: {value}{CC.Clr}");
                return value;
            }

            if (pattern.Length == 0)
            {
                // the good case is when there are no numbers left at the end of the pattern
                cache[key] = nums.Any() ? 0 : 1;
                return cache[key];
            }


            var c = pattern[0];
            switch (c)
            {
                case '.':
                    // consume one spring and recurse
                    Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('.') Consume one spring and recurse{CC.Clr}");
                    cache[key] = Compute(pattern[1..], nums, indent + 1);
                    //   Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
                    break;
                case '?':
                    // recurse both ways
                    Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('?') Recurse both ways (for '.' and '#'){CC.Clr}");
                    cache[key] = Compute("#" + pattern[1..], nums, indent + 1) + Compute("." + pattern[1..], nums, indent + 1);
                    //   Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
                    break;
                case '#':
                    if (!nums.Any())
                    {
                        Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') No more numbers left, this is no good{CC.Clr}");
                        cache[key] = 0; // no more numbers left, this is no good
                                        //     Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
                    }
                    else
                    {
                        var n = nums.Peek();
                        nums = nums.Pop();

                        var potentiallyDead = pattern.TakeWhile(s => s == '#' || s == '?').Count();
                        if (potentiallyDead < n)
                        {
                            Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') Not enough dead springs {CC.Clr}");
                            cache[key] = 0; // not enough dead springs 
                                            //        Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
                        }
                        else if (pattern.Length == n)
                        {
                            Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') pattern len == n, compute empty string {CC.Clr}");
                            cache[key] = Compute("", nums, indent + 1);
                            //        Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
                        }
                        else if (pattern[n] == '#')
                        {
                            Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') dead spring follows the range -> not good {CC.Clr}");
                            cache[key] = 0; // dead spring follows the range -> not good
                                            //        Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
                        }
                        else
                        {
                            Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('#') compute for next number on stack (n + 1) {CC.Clr}");
                            cache[key] = Compute(pattern[(n + 1)..], nums, indent + 1);
                            //   Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
                        }
                    }
                    break;
            }
            return cache[key];
        }



        private static long Solve(string line, int repeat)
        {
            var parts = line.Split(" ");
            var pattern = string.Join('?', Enumerable.Repeat(parts[0], repeat));
            var nums = string.Join(',', Enumerable.Repeat(parts[1], repeat)).Split(',').Select(int.Parse);

            var range = ImmutableStack.CreateRange(nums.Reverse());
            cache.Clear();
            lineSize = line.Length;
            return Compute(pattern, range);
        }



        public static long Part1(string[] lines)
        {
            Solve(lines[1], 1);
            var sum = 0L;
            foreach (var line in lines)
            {
                //    sum += Solve(line, 1);
            }
            return sum;
        }
        public static long Part2(string[] lines)
        {
            var sum = 0L;
            foreach (var line in lines)
            {
                //    sum += Solve(line, 5);
            }
            return sum;
        }
    }
}