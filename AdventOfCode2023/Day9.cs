using System.Reflection.Metadata;

namespace AdventOfCode2023
{
    class Day9
    {
        public static bool TestData => false;



        private static void ProcessLine_Part1(int index, string line)
        {
            var numbers = line.SplitAtAsArrayOfLongs(' ').ToList();
            if (numbers.Count == 0) return;

            // put an list of subtracts on stack
            var stack = new Stack<List<long>>();
            var currentList = numbers;
            stack.Push(currentList);
            while (currentList.Any(n => n != 0))
            {
                var innerL = new List<long>(currentList.Count - 1);
                for (int i = 0; i < currentList.Count - 1; i++)
                {
                    innerL.Add(currentList[i + 1] - currentList[i]);
                }
                stack.Push(innerL);
                currentList = innerL;
            }
            // for first list on stack, add 0
            stack.Peek().Add(0);
            // now, remove from stack and add sum of last in the list, and last from last list
            List<long>? lastList = null;
            while (stack.TryPop(out var list))
            {
                if (lastList == null)
                {
                    lastList = list;
                    continue;
                }
                list.Add(list.Last() + lastList.Last());
                lastList = list;
            }
            
            Interlocked.Add(ref _answer, lastList?.Last() ?? 0);
        }

        private static long _answer;
        public static long Part1(string[] lines)
        {
            _answer = 0;
            for (int i = 0; i < lines.Length; i++) 
                ProcessLine_Part1(i, lines[i]);
            return _answer;
        }

        private static void ProcessLine_Part2(int index, string line)
        {
            var numbers = line.SplitAtAsArrayOfLongs(' ').ToList();
            if (numbers.Count == 0) return;

            // put an list of subtracts on stack
            var stack = new Stack<List<long>>();
            var currentList = numbers;
            stack.Push(currentList);
            while (currentList.Any(n => n != 0))
            {
                var innerL = new List<long>(currentList.Count - 1);
                for (int i = 0; i < currentList.Count - 1; i++)
                {
                    innerL.Add(currentList[i + 1] - currentList[i]);
                }
                stack.Push(innerL);
                currentList = innerL;
            }
            // for first list on stack, add 0
            stack.Peek().Insert(0, 0);
            // now, remove from stack and add sum of last in the list, and last from last list
            List<long>? lastList = null;
            while (stack.TryPop(out var list))
            {
                if (lastList == null)
                {
                    lastList = list;
                    continue;
                }
                list.Insert(0, list.First() - lastList.First());
                lastList = list;
                //Console.WriteLine(string.Join(", ", list));
            }

            Interlocked.Add(ref _answer, lastList?.First() ?? 0);
        }
        public static long Part2(string[] lines)
        {
            _answer = 0;
            for (int i = 0; i < lines.Length; i++)
                ProcessLine_Part2(i, lines[i]);
            return _answer;
        }
    }
}