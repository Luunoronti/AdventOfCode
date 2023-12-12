

//!!!!!!!!!!!!!
//
// this code is copied from
// https://github.com/encse/adventofcode/blob/master/2023/Day12/Solution.cs
// I've got first part via brute force permutation generator.
// but it failed miserably at second part.
// I still don't understand this code (mostly because it's db linq), but i've got it here to debug and get a grasp of it.
// it does, however, work



using System.Collections.Immutable;
using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

namespace AdventOfCode2023
{
    //[Force] // uncomment to force processing this type
    //[AlwaysEnableLog]
    //[DisableLogInDebug]
    [UseLiveDataInDeug]
    //[AlwaysUseTestData]
    class Day12
    {
        public static string TestFile => "2023\\12\\test.txt";
        public static string LiveFile => "2023\\12\\live.txt";


        private static long Solve(string[] input, int repeat) => (
       from line in input
       let parts = line.Split(" ")
       let pattern = Unfold(parts[0], '?', repeat)
       let numString = Unfold(parts[1], ',', repeat)
       let nums = numString.Split(',').Select(int.Parse)
       select
           Compute(pattern, ImmutableStack.CreateRange(nums.Reverse()), new Cache())
   ).Sum();

        private static string Unfold(string st, char join, int unfold) =>
            string.Join(join, Enumerable.Repeat(st, unfold));

        private static long Compute(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            if (!cache.ContainsKey((pattern, nums)))
            {
                cache[(pattern, nums)] = Dispatch(pattern, nums, cache);
            }
            return cache[(pattern, nums)];
        }

        private static long Dispatch(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            return pattern.FirstOrDefault() switch
            {
                '.' => ProcessDot(pattern, nums, cache),
                '?' => ProcessQuestion(pattern, nums, cache),
                '#' => ProcessHash(pattern, nums, cache),
                _ => ProcessEnd(pattern, nums, cache),
            };
        }

        private static long ProcessEnd(string _, ImmutableStack<int> nums, Cache __)
        {
            // the good case is when there are no numbers left at the end of the pattern
            return nums.Any() ? 0 : 1;
        }

        private static long ProcessDot(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            // consume one spring and recurse
            return Compute(pattern[1..], nums, cache);
        }

        private static long ProcessQuestion(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            // recurse both ways
            return Compute("." + pattern[1..], nums, cache) + Compute("#" + pattern[1..], nums, cache);
        }

        private static long ProcessHash(string pattern, ImmutableStack<int> nums, Cache cache)
        {
            // take the first number and consume that many dead springs, recurse

            if (!nums.Any())
            {
                return 0; // no more numbers left, this is no good
            }

            var n = nums.Peek();
            nums = nums.Pop();

            var potentiallyDead = pattern.TakeWhile(s => s == '#' || s == '?').Count();

            if (potentiallyDead < n)
            {
                return 0; // not enough dead springs 
            }
            else if (pattern.Length == n)
            {
                return Compute("", nums, cache);
            }
            else if (pattern[n] == '#')
            {
                return 0; // dead spring follows the range -> not good
            }
            else
            {
                return Compute(pattern[(n + 1)..], nums, cache);
            }
        }

        public static long Part1(string[] lines)
        {
            return Solve(lines, 1);

        }
        public static long Part2(string[] lines)
        {

            return Solve(lines, 5);
        }
    }
}