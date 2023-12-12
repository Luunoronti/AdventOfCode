using System.DirectoryServices;

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

        private static long[] GetGroups(string line) => line.Split(' ')[1].SplitAsArrayOfLongs(',');


        private static long GetPossiblePermutations(string line, IEnumerable<long> groups, string current, List<string> allPossible)
        {
            if (groups.Count() == 0)
            {
                allPossible.Add(current.PadRight(line.Length, '.'));
                return 0;
            }

            long myGroup = groups.ElementAt(0);
            var len = line.Length;
            if (string.IsNullOrEmpty(current) == false) len -= current.Length;
            for (int i = 0; i < line.Length; i++)
            {
                if (len <= 1 + i + myGroup)
                    return 0; // that's it, we cant do more

                // if a sum of all our groups made it not possible, and
                if (len <= i + groups.Sum() + groups.Count() - 1)
                {
                    //allPossible.Add(current.PadRight(line.Length, '.'));
                    return 0;
                }

                var str = (string.IsNullOrEmpty(current) ? "" : current + '.') + new string('.', i) + new string('#', (int)myGroup);
              
                GetPossiblePermutations(line, groups.TakeLast(groups.Count() - 1), str, allPossible);
            }
            return 0;
        }

        private static long ProcessLine(string line)
        {
            var groups = GetGroups(line);
            Log.WriteLine($"Groups for line {line}: {groups.ToReadable()}");

            // create all possible arrangements
            List<string> possibilities = new List<string>();

            GetPossiblePermutations(line, groups, null, possibilities);
            foreach(var possibility in possibilities)
            {
                Log.WriteLine(possibility);

            }










            int stringIndex = 0;
            while (true)
            {
                for (int i = 0; i < groups.Length; i++)
                {
                    var group = groups[i];

                }
            }




            foreach (var group in groups)
            {

            }



            return 0;
        }
        public static long Part1(string[] lines)
        {
            // test
            ProcessLine(lines[lines.Length - 1]);
            var sum = 0L;
            foreach (var line in lines)
            {
                //                sum += ProcessLine(line);
            }
            return sum;
        }
        public static long Part2(string[] lines)
        {
            return 0;
        }
    }
}