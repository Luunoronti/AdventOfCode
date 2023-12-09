namespace AdventOfCode2023
{
    class Day09
    {
        public static string TestFile => "2023\\09\\test.txt";
        public static string LiveFile => "2023\\09\\live.txt";
        public static bool TestData => true;

        private static long _answer1;
        private static long _answer2;

        private static void ProcessLine(string line)
        {
            var currentList = line.SplitAsArrayOfLongs(' ').ToList();
            if (currentList.Count == 0) return;
            var stack = new Stack<List<long>>();
            stack.Push(currentList);
            while (currentList.Any(n => n != 0))
            {
                var innerL = new List<long>(currentList.Count - 1);
                for (int i = 0; i < currentList.Count - 1; i++)
                    innerL.Add(currentList[i + 1] - currentList[i]);
                stack.Push(innerL);
                currentList = innerL;
            }
            stack.Peek().Add(0);
            stack.Peek().Insert(0, 0);
            List<long>? lastList = null;
            while (stack.TryPop(out var list))
            {
                if (lastList != null)
                {
                    list.Add(list.Last() + lastList.Last());
                    list.Insert(0, list.First() - lastList.First());
                }
                lastList = list;
                Log.WriteLine(string.Join(", ", list));
            }
            Log.WriteLine("====");
            Interlocked.Add(ref _answer1, lastList?.Last() ?? 0);
            Interlocked.Add(ref _answer2, lastList?.First() ?? 0);
        }

        public static long Part1(string[] lines)
        {
            lines.ForEach(line => ProcessLine(line));
            return _answer1;
        }
        public static long Part2(string[] _) => _answer2;
    }
}