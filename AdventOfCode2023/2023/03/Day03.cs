namespace AdventOfCode2023
{
    class Day03
    {
        public static string TestFile => "2023\\03\\test.txt";
        public static string LiveFile => "2023\\03\\live.txt";
        struct Day3Str1
        {
            public int x;
            public int y;
            public char c;
            public override readonly string ToString() => $"{c} - {x}, {y}";
        }
        struct Day3Str2
        {
            public int x1;
            public int y1;
            public int x2;
            public int y2;
            public int value;

            public override readonly string ToString() => $"({x1}, {y1}), ({x2}, {y2}), ({value})";

        }

        private static IEnumerable<Day3Str2> EnumerateNumbers(string[] lines)
        {
            List<Day3Str2> ret = new();

            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                var numStr = "";
                for (int x = 0; x < line.Length; x++)
                {
                    var c = line[x];
                    if (char.IsDigit(c))
                    {
                        numStr = "";
                        int x2 = 0;
                        for (x2 = x; x2 <= line.Length; x2++)
                        {
                            if (x2 == line.Length)
                            {
                                ret.Add(new Day3Str2
                                {
                                    y1 = y,
                                    y2 = y,
                                    x1 = x,
                                    x2 = x2 - 1,
                                    value = int.Parse(numStr)
                                });
                                break;
                            }
                            var c2 = line[x2];
                            if (!char.IsDigit(c2))
                            {
                                ret.Add(new Day3Str2
                                {
                                    y1 = y,
                                    y2 = y,
                                    x1 = x,
                                    x2 = x2 - 1,
                                    value = int.Parse(numStr)
                                });
                                break;
                            }
                            numStr += c2;
                        }

                        x = x2 - 1;
                    }
                }
            }

            return ret;
        }

        private static IEnumerable<Day3Str1> EnumerateSymbols(string[] lines)
        {
            List<Day3Str1> ret = new();
            for (int y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    var c = line[x];
                    if (c == '.') continue;
                    if (char.IsDigit(c)) continue;
                    ret.Add(new Day3Str1 { x = x, y = y, c = c });
                }
            }
            return ret;
        }

        public static bool TestData => true;
        
        public static long Part1(string[] lines)
        {
            var symbols = EnumerateSymbols(lines);
            var numbers = EnumerateNumbers(lines);

            var sum = 0L;
            foreach (var number in numbers)
            {
                foreach (var s in symbols)
                {
                    if (s.x < number.x1 - 1) continue;
                    if (s.x > number.x2 + 1) continue;
                    if (s.y < number.y1 - 1) continue;
                    if (s.y > number.y2 + 1) continue;
                    sum += number.value;
                }
            }
            return sum;
        }
        public static long Part2(string[] lines)
        {
            var symbols = EnumerateSymbols(lines);
            var numbers = EnumerateNumbers(lines);
            var gearRationSum = 0L;

            foreach (var s in symbols.Where(sym => sym.c == '*'))
            {
                List<Day3Str2> lst = new();
                foreach (var number in numbers)
                {
                    if (s.x < number.x1 - 1) continue;
                    if (s.x > number.x2 + 1) continue;
                    if (s.y < number.y1 - 1) continue;
                    if (s.y > number.y2 + 1) continue;
                    lst.Add(number);
                }
                if (lst.Count == 2)
                    gearRationSum += lst[0].value * lst[1].value;
            }
            return gearRationSum;
        }
    }
}