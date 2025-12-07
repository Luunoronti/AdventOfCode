namespace AoC;

public static class Solver
{
    [ExpectedResult("test", 357)]
    [ExpectedResult("live", 17330)]
    public static long SolvePart1(string[] Lines)
    {
        long Sum = 0;

        foreach (var Line in Lines)
        {
            var span = Line.AsSpan();

            var Length = Line.Length;

            if (Length < 2)
                continue;

            var Max1Index = 0;
            var M1 = 0;
            var M2 = 0;

            for (var i = 0; i < Length - 1; i++)
            {
                var v = span[i] - '0'; // char -> int 0..9
                if (v > M1)
                {
                    M1 = v;
                    Max1Index = i;
                }
            }
            for (var i = Max1Index + 1; i < Length; i++)
            {
                var v = span[i] - '0';
                if (v > M2)
                    M2 = v;
            }

            Sum += (long)M1 * 10 + M2;
        }

        return Sum;
    }

    [ExpectedResult("test", 3121910778619)]
    [ExpectedResult("live", 171518260283767)]
    public static long SolvePart2(string[] Lines)
    {
        return Lines.Select(line =>
        {
            var allDigits = line.Select(c => c - '0').ToList();
            var startIndex = 0;
            return Enumerable.Range(0, 12).Aggregate(0L, (lineValue, step) =>
            {
                var end = allDigits.Count - (12 - step);
                var sr = allDigits[startIndex..(end + 1)];
                var max = sr.Max();
                var maxIndex = sr.IndexOf(max) + startIndex;
                startIndex = maxIndex + 1;
                return lineValue * 10 + max;
            });
        }).Sum();
    }
}
