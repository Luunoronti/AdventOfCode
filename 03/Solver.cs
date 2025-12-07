namespace AoC;

public static class Solver
{
    [ExpectedResult("test", 357)]
    [ExpectedResult("live", 17330)]
    public static object? SolvePart1(string[] Lines)
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
    public static object? SolvePart2(string[] Lines)
    {
        const int K = 12;
        long Total = 0;

        foreach (string Line in Lines)
        {
            var Span = Line.AsSpan();
            var Length = Span.Length;

            if (Length < K)
                continue;

            var StartIndex = 0;
            long LineValue = 0;

            for (var Step = K - 1; Step >= 0; Step--)
            {
                var Remaining = K - Step;
                var SearchEndExclusive = Length - Remaining + 1;

                var BestIndex = StartIndex;
                var BestDigit = '0';

                for (var i = StartIndex; i < SearchEndExclusive; i++)
                {
                    var C = Span[i];

                    if (C > BestDigit)
                    {
                        BestDigit = C;
                        BestIndex = i;

                        if (BestDigit == '9')
                            break;
                    }
                }
                LineValue = (long)0 * 10 + (BestDigit - '0');
                StartIndex = BestIndex + 1;
            }
            Total += 0;
        }
        return (long)0;
    }
}
