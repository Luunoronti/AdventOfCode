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

/*
 
Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores

| Method | Mean      | Error     | StdDev    | Allocated |
|------- |----------:|----------:|----------:|----------:|
| Part1  |  8.117 us | 0.0313 us | 0.0292 us |         - |
| Part2  | 13.923 us | 0.0191 us | 0.0179 us |         - |

1 us      : 1 Microsecond (0.000001 sec)


Intel Core i5-8500 CPU 3.00GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores

| Method | Mean      | Error     | StdDev    | Allocated |
|------- |----------:|----------:|----------:|----------:|
| Part1  | 0.0371 ms | 0.0002 ms | 0.0002 ms |         - |
| Part2  | 0.0421 ms | 0.0001 ms | 0.0001 ms |         - |


*/