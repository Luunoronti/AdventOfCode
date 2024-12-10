
Console.WriteLine(test.Solve1(File.ReadAllText(@"C:\Work\AoC\AdventOfCode\AdventOfCode2024\Live\2024_9_1.txt")));
Console.WriteLine(test.Solve2(File.ReadAllText(@"C:\Work\AoC\AdventOfCode\AdventOfCode2024\Live\2024_9_1.txt")));

class test
{
    public static long Solve1(string input)
    {
        int[] nums = input.ToCharArray().Select(c => c - '0').ToArray();
        long result = 0L;
        int k = 0;
        int i = 0;
        int j = nums.Length - 1;
        while (true)
        {
            if (i % 2 == 0)
            {
                result += UpdateFileCheckSum(nums[i], i / 2, ref k);
                nums[i] = 0;
                if (i >= j)
                    break;
                i += 1;
            }
            else
            {
                if (i >= j)
                {
                    result += (nums[j] + 1) * nums[j] / 2;
                    break;
                }
                int m = Math.Min(nums[i], nums[j]);
                result += UpdateFileCheckSum(m, j / 2, ref k);
                nums[i] -= m;
                nums[j] -= m;
                if (nums[i] == 0)
                    i += 1;
                if (nums[j] == 0)
                    j -= 2;
            }
        }
        return result;
    }
    public static long Solve2(string input)
    {
        int[] nums = input.ToCharArray().Select(c => c - '0').ToArray();
        int[] numsCopy = nums.ToArray();
        long result = 0L;
        int k = 0;
        int i = 0;
        while (i < nums.Length)
        {
            if (i % 2 == 0)
            {
                int v = i / 2;
                if (nums[i] == 0)
                    k += numsCopy[i];
                result += UpdateFileCheckSum(nums[i], i / 2, ref k);
                nums[i] = 0;
                i += 1;
            }
            else
            {
                bool found = false;
                int j = -1;
                var loopTo = i;
                for (j = nums.Length - 1; j >= loopTo; j -= 2)
                {
                    if (nums[j] > 0 && nums[j] <= nums[i])
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    result += UpdateFileCheckSum(nums[j], j / 2, ref k);
                    nums[i] -= nums[j];
                    nums[j] = 0;
                    if (nums[i] == 0)
                        i += 1;
                }
                else
                {
                    k += nums[i];
                    i += 1;
                }
            }
        }
        return result;
    }
    private static long UpdateFileCheckSum(int cur, long value, ref int k)
    {
        long ret = (cur - 1 + 2 * k) * cur / 2 * value;
        k += cur;
        return ret;
    }

}