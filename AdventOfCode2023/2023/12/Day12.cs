

//!!!!!!!!!!!!!
//
// this code is copied from
// https://github.com/encse/adventofcode/blob/master/2023/Day12/Solution.cs
// I've got first part via brute force permutation generator.
// but it failed miserably at second part.
// I still don't understand this code (mostly because it's db linq, and I am too old for functional programming :),
// but i've got it here to debug and get a grasp of it.



// after rewriting and a bunch of logs, I understand it a bit.
//TODO: when i get a free time, I'll try to make something of my own


using System.Collections.Immutable;
using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

namespace AdventOfCode2023
{
    //[Force] // uncomment to force processing this type
    //[AlwaysEnableLog]
    //[DisableLogInDebug]
    //[UseLiveDataInDeug]
    //[AlwaysUseTestData]
    class Day12
    {
        public static string TestFile => "2023\\12\\test.txt";
        public static string LiveFile => "2023\\12\\live.txt";


        private static string In(int indent) => "".PadLeft(indent);
        private static int lineSize = 0;
        private static long Compute(string pattern, ImmutableStack<int> nums, Cache cache, int indent = 0)
        {
            var key = (pattern, nums);
            if (cache.TryGetValue(key, out var value))
            {
             //   Log.WriteLine($"{In(indent)}{pattern}, {nums.ToReadable()}{CC.Frm} (cac) Got value: {value}{CC.Clr}");
                return value;
            }

            var c = pattern.FirstOrDefault();
            switch (c)
            {
                case '.':
                    // consume one spring and recurse
                    Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('.') Consume one spring and recurse{CC.Clr}");
                    cache[key] = Compute(pattern[1..], nums, cache, indent + 1);
                 //   Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
                    break;
                case '?':
                    // recurse both ways
                    Log.WriteLine($"{pattern.PadLeft(lineSize)}, {nums.ToReadable()}{CC.Frm} ('?') Recurse both ways (for '.' and '#'){CC.Clr}");
                    cache[key] = Compute("#" + pattern[1..], nums, cache, indent + 1) + Compute("." + pattern[1..], nums, cache, indent + 1);
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
                            cache[key] = Compute("", nums, cache, indent + 1);
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
                            cache[key] = Compute(pattern[(n + 1)..], nums, cache, indent + 1);
                         //   Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
                        }
                    }
                    break;
                default:
                    // the good case is when there are no numbers left at the end of the pattern
                    cache[key] = nums.Any() ? 0 : 1;
                 //   Log.WriteLine($"{In(indent)}{key.pattern}, {key.nums.ToReadable()} => {cache[key]}");
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
            var cache = new Cache();
            lineSize = line.Length;
            return Compute(pattern, range, cache);
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