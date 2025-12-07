using System;

namespace AoC;

public static class Solver
{
    private static readonly long[] Pow10 = BuildPow10();

    [ExpectedResult("test", 1227775554)]
    [ExpectedResult("live", 44487518055)]
    public static long SolvePart1(string[] Lines)
    {
        ReadOnlySpan<char> Line = Lines[0].AsSpan();
        long Sum = 0;

        int Pos = 0;
        while (Pos < Line.Length)
        {
            // Skip commas and whitespace
            while (Pos < Line.Length && (Line[Pos] == ',' || char.IsWhiteSpace(Line[Pos])))
                Pos++;

            if (Pos >= Line.Length)
                break;

            int RangeStart = Pos;
            while (Pos < Line.Length && Line[Pos] != ',')
                Pos++;

            ReadOnlySpan<char> RangeSpan = Line.Slice(RangeStart, Pos - RangeStart).Trim();
            if (RangeSpan.Length == 0)
                continue;

            int DashIndex = RangeSpan.IndexOf('-');
            long Start = long.Parse(RangeSpan.Slice(0, DashIndex));
            long End = long.Parse(RangeSpan.Slice(DashIndex + 1));

            AddInvalidIdsInRange(Start, End, ref Sum);
        }

        return Sum;
    }

    [ExpectedResult("test", 4174379265)]
    [ExpectedResult("live", 53481866137)]
    public static long SolvePart2(string[] Lines)
    {
        ReadOnlySpan<char> Line = Lines[0].AsSpan();
        var InvalidIds = new HashSet<long>();

        int Pos = 0;
        while (Pos < Line.Length)
        {
            // Skip commas and whitespace
            while (Pos < Line.Length && (Line[Pos] == ',' || char.IsWhiteSpace(Line[Pos])))
                Pos++;

            if (Pos >= Line.Length)
                break;

            int RangeStart = Pos;
            while (Pos < Line.Length && Line[Pos] != ',')
                Pos++;

            ReadOnlySpan<char> RangeSpan = Line.Slice(RangeStart, Pos - RangeStart).Trim();
            if (RangeSpan.Length == 0)
                continue;

            int DashIndex = RangeSpan.IndexOf('-');
            long Start = long.Parse(RangeSpan.Slice(0, DashIndex));
            long End = long.Parse(RangeSpan.Slice(DashIndex + 1));

            AddInvalidIdsInRange(Start, End, InvalidIds);
        }

        long Sum = 0;
        foreach (long Value in InvalidIds)
            Sum += Value;

        return Sum;
    }


    private static void AddInvalidIdsInRange(long Start, long End, HashSet<long> InvalidIds)
    {
        if (Start > End)
            return;

        int MinDigits = CountDigits(Start);
        int MaxDigits = CountDigits(End);
        if (MaxDigits > 18) MaxDigits = 18; // long can only hold up to 18-19 digits safely

        for (int D = MinDigits; D <= MaxDigits; D++)
        {
            // Block size B: from 1 to D/2, and D must be a multiple of B
            for (int Block = 1; Block <= D / 2; Block++)
            {
                if (D % Block != 0)
                    continue;

                int Repeats = D / Block;

                long PowBlock = Pow10[Block];         // 10^Block
                long HMin = Pow10[Block - 1];     // smallest B-digit number
                long HMax = Pow10[Block] - 1;     // largest B-digit number

                // Factor = 1 + powB + powB^2 + ... + powB^(Repeats-1)
                long Factor = 0;
                long Power = 1; // powB^0
                for (int r = 0; r < Repeats; r++)
                {
                    Factor += Power;
                    Power *= PowBlock;
                }

                // H must satisfy:
                //   Start <= H * Factor <= End
                // => ceil(Start / Factor) <= H <= floor(End / Factor)
                long HStart = CeilDiv(Start, Factor);
                long HEnd = End / Factor;

                if (HStart < HMin) HStart = HMin;
                if (HEnd > HMax) HEnd = HMax;

                if (HStart > HEnd)
                    continue;

                for (long H = HStart; H <= HEnd; H++)
                {
                    long Candidate = H * Factor;
                    InvalidIds.Add(Candidate);
                }
            }
        }
    }

    private static void AddInvalidIdsInRange(long Start, long End, ref long Sum)
    {
        int MinDigits = CountDigits(Start);
        int MaxDigits = CountDigits(End);

        for (int D = MinDigits; D <= MaxDigits; D++)
        {
            if ((D & 1) != 0)
                continue;

            int Half = D >> 1;

            long Pow = Pow10[Half];
            long HMin = Pow10[Half - 1];
            long HMax = Pow - 1;

            long Factor = Pow + 1;

            long HStart = CeilDiv(Start, Factor);
            long HEnd = End / Factor;

            if (HStart < HMin) HStart = HMin;
            if (HEnd > HMax) HEnd = HMax;

            if (HStart > HEnd)
                continue;

            for (long H = HStart; H <= HEnd; H++)
            {
                Sum += H * Factor;
            }
        }
    }

    private static long[] BuildPow10()
    {
        var Result = new long[19];
        long Value = 1;
        for (var i = 0; i < Result.Length; i++)
        {
            Result[i] = Value;
            Value *= 10;
        }
        return Result;
    }

    private static int CountDigits(long Value)
    {
        if (Value < 0) Value = -Value;
        var Digits = 1;
        while (Value >= 10)
        {
            Value /= 10;
            Digits++;
        }
        return Digits;
    }
    private static long CeilDiv(long Numerator, long Denominator)
    {
        return (Numerator + Denominator - 1) / Denominator;
    }
}
