namespace AoC;

public static class Solver
{
    struct Range
    {
        public long Start;
        public long End;
    }
    private static bool IsFresh(long Id, Range[] Ranges, int RangeCount)
    {
        var Low = 0;
        var High = RangeCount - 1;

        while (Low <= High)
        {
            var Mid = (Low + High) >> 1;
            var r = Ranges[Mid];

            if (Id < r.Start)
                High = Mid - 1;
            else if (Id > r.End)
                Low = Mid + 1;
            else
                // r.Start <= id <= r.End
                return true;
        }
        return false;
    }
    private static Range[] ParseAndMergeRanges(string[] lines, out int MergedCount, out int IndexAfterRanges)
    {
        var LineCount = lines.Length;

        var Ranges = new Range[LineCount];
        var RangeCount = 0;
        var Index = 0;

        for (; Index < LineCount; Index++)
        {
            var Line = lines[Index];
            if (string.IsNullOrWhiteSpace(Line))
            {
                Index++;
                break;
            }

            var Span = Line.AsSpan().Trim();
            var DashPos = Span.IndexOf('-');
            var Start = long.Parse(Span[..DashPos]);
            var End = long.Parse(Span[(DashPos + 1)..]);
            Ranges[RangeCount++] = new Range { Start = Start, End = End };
        }

        IndexAfterRanges = Index;
        if (RangeCount == 0)
        {
            MergedCount = 0;
            return [];
        }

        Array.Sort(Ranges, 0, RangeCount, Comparer<Range>.Create((A, B) => A.Start.CompareTo(B.Start)));

        var Merged = new Range[RangeCount];
        MergedCount = 0;

        var Current = Ranges[0];

        for (var I = 1; I < RangeCount; I++)
        {
            var R = Ranges[I];

            if (R.Start <= Current.End)
            {
                if (R.End > Current.End)
                    Current.End = R.End;
            }
            else
            {
                Merged[MergedCount++] = Current;
                Current = R;
            }
        }
        Merged[MergedCount++] = Current;
        return Merged;
    }



    public static object? SolvePart1(string[] lines)
    {
        var Merged = ParseAndMergeRanges(lines, out var MergedCount, out var IndexAfterRanges);

        long FreshCount = 0;

        for (var I = IndexAfterRanges; I < lines.Length; I++)
        {
            var Line = lines[I];
            if (string.IsNullOrWhiteSpace(Line))
                continue;

            var Id = long.Parse(Line.AsSpan().Trim());
            if (IsFresh(Id, Merged, MergedCount))
                FreshCount++;
        }
        return FreshCount.ToString();
    }

    public static object? SolvePart2(string[] lines)
    {
        var Merged = ParseAndMergeRanges(lines, out var MergedCount, out _);
        long TotalFreshIds = 0;
        for (var I = 0; I < MergedCount; I++)
        {
            var R = Merged[I];
            TotalFreshIds += (R.End - R.Start + 1);
        }

        return TotalFreshIds.ToString();
    }
}
