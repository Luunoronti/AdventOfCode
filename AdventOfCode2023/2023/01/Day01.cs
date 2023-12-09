namespace AdventOfCode2023
{
    class Day01
    {
        public static string TestFile => "2023\\01\\test.txt";
        public static string LiveFile => "2023\\01\\live.txt";
        public static bool TestData => true;
        public static long Part1(string[] lines)
        {
            return lines.Select(l => GetSum(l.ToLower().Trim())).Sum();
            static long GetSum(string line)
            {
                var list = line.Where(l => l >= '0' && l <= '9').ToList();
                if (list.Count == 0) return 0;
                return int.Parse($"{list[0]}{list[^1]}");
            }

        }
        static bool GetWordedDigit(string line, int index, string digit)
        {
            if (index + digit.Length > line.Length) return false;

            for (int i = index; i < line.Length; i++)
            {
                if ((i - index) == digit.Length) break;
                char c = line[i];
                var c2 = digit[i - index];
                if (c != c2) return false;
            }
            return true;
        }

        public static long Part2(string[] lines)
        {
            return lines.Select(l => GetSum(l.ToLower().Trim())).Sum();
            static long GetSum(string line)
            {
                List<char> list = new();
                for (int i = 0; i < line.Length; i++)
                {
                    var c = line[i];
                    if (GetWordedDigit(line, i, "one")) { list.Add('1'); }
                    else if (GetWordedDigit(line, i, "two")) { list.Add('2'); }
                    else if (GetWordedDigit(line, i, "three")) { list.Add('3'); }
                    else if (GetWordedDigit(line, i, "four")) { list.Add('4'); }
                    else if (GetWordedDigit(line, i, "five")) { list.Add('5'); }
                    else if (GetWordedDigit(line, i, "six")) { list.Add('6'); }
                    else if (GetWordedDigit(line, i, "seven")) { list.Add('7'); }
                    else if (GetWordedDigit(line, i, "eight")) { list.Add('8'); }
                    else if (GetWordedDigit(line, i, "nine")) { list.Add('9'); }
                    else if (c >= '0' && c <= '9') list.Add(c);
                }
                if (list.Count == 0) return 0;

                var num = $"{list[0]}{list[^1]}";
                return int.Parse(num);
            }
        }
      
    }
}