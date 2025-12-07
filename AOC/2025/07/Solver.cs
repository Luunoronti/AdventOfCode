namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    [ExpectedResult("test", 21)]
    [ExpectedResult("live", 1703)]
    public static long SolvePart1(string[] Lines)
    {
        var height = Lines.Length;
        var width = Lines[0].Length;

        var startColumn = 0;

        var line = Lines[0].AsSpan();

        while (startColumn < height && line[startColumn++] != 'S') ;
        startColumn--;

        Span<byte> active = stackalloc byte[width];
        Span<byte> next = stackalloc byte[width];
        var splits = 0L;

        active.Clear();
        active[startColumn] = 1;

        for (var row = 1; row < height; row++)
        {
            next.Clear();
            line = Lines[row].AsSpan();
            for (var col = 0; col < width; col++)
            {
                if (active[col] == 0) continue;
                if (line[col] != '^')
                {
                    next[col] = 1;
                    continue;
                }
                splits++;

                if (col > 0) next[col - 1] = 1;
                if (col + 1 < width) next[col + 1] = 1;
            }
            var temp = active;
            active = next;
            next = temp;
        }
        return splits;
    }

    [ExpectedResult("test", 40)]
    [ExpectedResult("live", 171692855075500)]
    public static long SolvePart2(string[] Lines)
    {
        var height = Lines.Length;
        var width = Lines[0].Length;

        var startColumn = 0;

        var line = Lines[0].AsSpan();

        while (startColumn < height && line[startColumn++] != 'S') ;
        startColumn--;

        Span<long> active = stackalloc long[width];
        Span<long> next = stackalloc long[width];

        var completedTimelines = 0L;

        active.Clear();
        active[startColumn] = 1;

        for (var row = 1; row < height; row++)
        {
            next.Clear();
            line = Lines[row].AsSpan();

            for (var col = 0; col < width; col++)
            {
                var count = active[col];
                if (count == 0) continue;
                if (line[col] != '^')
                {
                    next[col] += count;
                    continue;
                }
                if (col > 0) next[col - 1] += count;
                else completedTimelines += count;

                if(col + 1 < width) next[col + 1] += count;
                else completedTimelines += count;
            }

            var temp = active;
            active = next;
            next = temp;
        }

        for (var col = 0; col < width; col++)
            completedTimelines += active[col];

        return completedTimelines;
    }
}
