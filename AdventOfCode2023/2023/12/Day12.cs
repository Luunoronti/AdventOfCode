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


        private static List<string> GetPossiblePermutations(string line, IEnumerable<long> groups)
        {
            var list = new List<string>();
            int counter = 0;
            foreach (var group in groups)
            {
                if(list.Count > 0)
                {
                    var list2 = new List<string>(list);
                    list.Clear();
                    foreach (var l in list2)
                    {

                        for (int i = 0; i < line.Length - group + 1; i++)
                        {
                            var str = l + $".{"".PadLeft(i, '.')}{"".PadRight((int)group, '#')}";
                            if (str.Length > line.Length) continue;
                            list.Add(str);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(counter);
                    for (int i = 0; i < line.Length - group + 1; i++)
                    {
                        list.Add($"{"".PadLeft(i, '.')}{"".PadRight((int)group, '#')}");
                    }
                }
            }

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].PadRight(line.Length, '.');
            }
            return list;
        }


        private static List<string> FilterOutPermutations(string line, List<string> permutations)
        {
            var ret = new List<string>();

            foreach (var permutation in permutations)
            {
                bool failed = false;
                for (int i = 0; i < line.Length; i++)
                {
                    var c = line[i];

                    if (c == '#')
                    {
                        if (permutation[i] != '#')
                        {
                            failed = true;
                            break;
                        }
                    }
                    if (c == '.')
                    {
                        if (permutation[i] != '.')
                        {
                            failed = true;
                            break;
                        }
                    }
                }
                if (!failed)
                    ret.Add(permutation);
            }

            return ret;
        }


        private static long ProcessLine(string line, int copyAmount = 1)
        {
            var groups = GetGroups(line);
            if (copyAmount > 1)
            {
                var gr = new List<long>();
                for (int i = 0; i < copyAmount; i++)
                    gr.AddRange(groups);

                groups = gr.ToArray();
            }
            // create all possible arrangements
            var mapLine = line.Split(' ')[0];

            if (copyAmount > 1)
            {
                var ln = new List<string>();
                for (int i = 0; i < copyAmount; i++)
                    ln.Add(mapLine);

                mapLine = string.Join("?", ln);
            }



            var possibilities = GetPossiblePermutations(mapLine, groups);

            var filtered = FilterOutPermutations(mapLine, possibilities);

            var sum = filtered.Count;
            Log.WriteLine($"Possible: {possibilities.Count}, filtered: {sum} for line {line}");

            return sum;
        }
        public static long Part1(string[] lines)
        {
            ProcessLine(lines[1]);
            var sum = 0L;
            for (int i = 0; i < lines.Length; i++)
            {
                string? line = lines[i];
                sum += ProcessLine(line);
            }
            return sum;
        }
        public static long Part2(string[] lines)
        {
            var sum = 0L;
            for (int i = 0; i < lines.Length; i++)
            {
                string? line = lines[i];
                sum += ProcessLine(line, 5);
            }
            return sum;
        }
    }
}