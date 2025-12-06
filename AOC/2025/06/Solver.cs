namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    [ExpectedResult("test", 4277556)]
    [ExpectedResult("live", 4771265398012)]
    public static object? SolvePart1(string[] Lines)
    {
        return SolvePart1_C(Lines);
        //return SolvePart1_F(Lines);
    }


    private static object? SolvePart1_C(string[] Lines)
    {
        var grid = Lines.Select(L => L.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList()).ToList();
        var height = grid.Count;
        var width = grid[0].Count;

        var transposed = new List<List<string>>();
        for (var c = 0; c < width; c++)
        {
            var l = new List<string>();
            transposed.Add(l);
            for (var r = 0; r < height; r++)
            {
                l.Add(grid[r][c]);
            }
        }

        var total = 0L;

        foreach (var t in transposed)
        {
            var mulOp = t.Last() == "*";
            var partial = mulOp ? 1L : 0L;
            for (var c = 0; c < t.Count - 1; c++)
            {
                if (mulOp)
                    partial *= long.Parse(t[c]);
                else
                    partial += long.Parse(t[c]);
            }
            total += partial;
        }
        return total;
    }
    private static object? SolvePart1_F(string[] Lines)
    {
        if (Lines.Length == 0) return 0L;
        var height = Lines.Length;
        var width = 0;
        for (var I = 0; I < height; I++)
        {
            var line = Lines[I];
            if (line.Length > width)
                width = line.Length;
        }

        var blankcolumns = new bool[width];
        for (var column = 0; column < width; column++)
        {
            var isBlank = true;
            for (var r = 0; r < height; r++)
            {
                var line = Lines[r];
                var ch = column < line.Length ? line[column] : ' ';
                if (ch != ' ')
                {
                    isBlank = false;
                    break;
                }
            }
            blankcolumns[column] = isBlank;
        }
        var total = 0L;
        var cIndex = 0;
        while (cIndex < width)
        {
            while (cIndex < width && blankcolumns[cIndex]) cIndex++;
            if (cIndex >= width) break;
            var start = cIndex;
            while (cIndex < width && !blankcolumns[cIndex]) cIndex++;
            var end = cIndex;
            var op = '+';
            var bottomL = Lines[height - 1];
            for (var c = start; c < end; c++)
            {
                if (c >= bottomL.Length) continue;
                var ch = bottomL[c];
                if (ch == '+' || ch == '*')
                {
                    op = ch;
                    break;
                }
            }
            var partial = op == '+' ? 0L : 1L;
            for (var r = 0; r < height - 1; r++)
            {
                var line = Lines[r];
                var c = start;
                while (c < end && (c >= line.Length || line[c] < '0' || line[c] > '9')) c++;
                if (c >= end || c >= line.Length) continue;
                var num = 0L;
                while (c < end && c < line.Length)
                {
                    var ch = line[c];
                    if (ch < '0' || ch > '9') break;
                    num = num * 10L + (ch - '0');
                    c++;
                }
                if (op == '+') partial += num; else partial *= num;
            }
            total += partial;
        }
        return total;
    }






    [ExpectedResult("test", 3263827)]
    [ExpectedResult("live", 10695785245101)]
    public static object? SolvePart2(string[] Lines)
    {
        return SolvePart2_C(Lines);
        //return SolvePart2_F(Lines);
    }

    private static object? SolvePart2_C(string[] Lines)
    {
        var height = Lines.Length;
        var width = Lines.Max(L => L.Length);
        var padded = Lines.Select(L => L.PadRight(width)).ToArray();
        var bottom = height - 1;
        List<bool> emptyFlags = [];
        for(var c = 0; c<width; c++)
        {
            emptyFlags.Add(padded.All(r => r[c] == ' '));
        }

        var total = 0L;
        for (var column = 0; column < width;)
        {
            while (column < width && emptyFlags[column]) column++;
            if (column >= width) break;
            var start = column;
            while (column < width && !emptyFlags[column]) column++;
            var end = column;

            var op = padded[height - 1].Skip(start).Take(end - start).First(c => c == '+' || c == '*');

            var partial = op == '*' ? 1L : 0L;
            for (var c = end - 1; c >= start; c--)
            {
                var num = 0L;
                var valid = false;
                for (var R = 0; R < height - 1; R++)
                {
                    var Ch = padded[R][c];
                    if (Ch < '0' || Ch > '9') continue;
                    valid = true;
                    num = num * 10L + (Ch - '0');
                }
                if (!valid) continue;
                if (op == '*') partial *= num; else partial += num;
            }
            total += partial;
        }
        return total;
    }

    private static object? SolvePart2_F(string[] Lines)
    {
        var height = Lines.Length;
        if (height == 0) return 0L;
        var width = 0;
        for (var I = 0; I < height; I++)
        {
            var Line = Lines[I];
            if (Line.Length > width) width = Line.Length;
        }
        var blankcolumns = new bool[width];
        for (var c = 0; c < width; c++)
        {
            var isBlank = true;
            for (var r = 0; r < height; r++)
            {
                var line = Lines[r];
                if (c < line.Length && line[c] != ' ')
                {
                    isBlank = false;
                    break;
                }
            }
            blankcolumns[c] = isBlank;
        }
        var total = 0L;
        var cIndex = 0;
        while (cIndex < width)
        {
            while (cIndex < width && blankcolumns[cIndex]) cIndex++;
            if (cIndex >= width) break;
            var start = cIndex;
            while (cIndex < width && !blankcolumns[cIndex]) cIndex++;
            var end = cIndex;
            var bottomL = Lines[height - 1];
            var op = '+';
            for (var c = start; c < end; c++)
            {
                if (c >= bottomL.Length) continue;
                var ch = bottomL[c];
                if (ch == '+' || ch == '*')
                {
                    op = ch;
                    break;
                }
            }
            var partial = op == '+' ? 0L : 1L;
            for (var c = end - 1; c >= start; c--)
            {
                var num = 0L;
                var valid = false;
                for (var r = 0; r < height - 1; r++)
                {
                    var line = Lines[r];
                    if (c >= line.Length) continue;
                    var ch = line[c];
                    if (ch < '0' || ch > '9') continue;
                    valid = true;
                    num = num * 10L + (ch - '0');
                }
                if (!valid) continue;
                if (op == '+') partial += num; else partial *= num;
            }
            total += partial;
        }
        return total;
    }

}
