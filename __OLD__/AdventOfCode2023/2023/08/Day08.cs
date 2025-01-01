namespace AdventOfCode2023
{
    //[Force]
    [AlwaysEnableLog]
    //[DisableLogInDebug]
    [UseLiveDataInDeug]
    //[AlwaysUseTestData]
    class Day08
    {
        public static string TestFile { get; set; } = "2023\\08\\test.txt";
        public static string LiveFile => "2023\\08\\live.txt";

        public static long Part1(string[] lines)
        {
            var instructions = lines[0].Trim().Select(c => c == 'L' ? 0 : 1).ToArray();
            var map = lines[2..]
                .Select(l => l.Replace("(", "").Replace(")", "").Replace(" ", ""))
                .Select(l => l.Split(new char[] { '=', ',' }))
                .ToDictionary(s => s[0], s => new string[] { s[1], s[2] });
            var current = "AAA";
            var stepIndex = 0;
            var steps = 0;
            while (current != "ZZZ")
            {
                current = map[current][instructions[stepIndex++]];
                if (stepIndex >= instructions.Length) stepIndex = 0;
                steps++;
            }
            Log.WriteLine($"Path length: {CC.Val}{steps}{CC.Clr}");
            TestFile = "2023\\08\\test2.txt";
            return steps;
        }
        public static long Part2(string[] lines)
        {
            var instructions = lines[0].Trim().Select(c => c == 'L' ? 0 : 1).ToArray();

            var map = lines[2..]
                .Select(l => l.Replace("(", "").Replace(")", "").Replace(" ", ""))
                .Select(l => l.Split(new char[] { '=', ',' }))
                .ToDictionary(s => s[0], s => new string[] { s[1], s[2] });

            return map.Select(d => d.Key)
                .Where(k => k.EndsWith('A'))
                .ForEach(c =>
                {
                    var current = c;
                    var stepIndex = 0;
                    var steps = 0L;
                    while (!current.EndsWith('Z'))
                    {
                        current = map[current][instructions[stepIndex++]];
                        if (stepIndex >= instructions.Length) stepIndex = 0;
                        steps++;
                    }
                    Log.WriteLine($"Path length: {CC.Val}{steps}{CC.Clr}");
                    return steps;
                }).Select(s => s / instructions.Length).MultiplyByAll(1)
                * instructions.Length
                ;
        }
    }
}