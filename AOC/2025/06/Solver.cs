namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    [ExpectedResult("test", 4277556)]
    [ExpectedResult("live", 4771265398012)]
    public static unsafe object? SolvePart1(string[] Lines)
    {
        // check input's shape (is it rectangular?)
        //var w = Lines[0].Length;
        //foreach (var l in Lines)
        //    if (w != l.Length)
        //    {
        //        int aa = 0;
        //    }

        // we assume rectangle input for numbers
        var height = Lines.Length;
        var width = Lines[0].Length;
        var numRows = height - 1; 

        // get ops
        Span<char> ops = stackalloc char[width >> 1];
        var totalOps = PrepareOps(Lines[height - 1].AsSpan(), ops);
        var currOp = 0;

        // keep current column for each line
        Span<int> columnPos = stackalloc int[height - 1];
        columnPos.Clear();

        // look for numbers and perform op
        var total = 0L;
        var partial = 0L;
        var num = 0L;
        for (currOp = 0; currOp < totalOps; currOp++)
        {
            partial = ops[currOp] == '*' ? 1L : 0L;
            for (var numR = 0; numR < numRows; numR++)
            {
                num = 0L;
                var line = Lines[numR].AsSpan();
                var cp = columnPos[numR];

                // look for any char first
                while (cp < width && line[cp++] == ' ') ;

                // get back by 1 (from cp++ above)
                cp--;

                // construct number
                while (cp < width && line[cp] != ' ') num = (num * 10) + (line[cp++] - '0');

                // store next empty column
                columnPos[numR] = cp;

                // perform op
                partial = ops[currOp] == '*' ? partial * num : partial + num;
            }

            total += partial;
        }

        return total;
    }

    [ExpectedResult("test", 3263827)]
    [ExpectedResult("live", 10695785245101)]
    public static unsafe object? SolvePart2(string[] Lines)
    {
        // we assume rectangle input
        var height = Lines.Length;
        var width = Lines[0].Length;
        var numRows = height - 1;

        // let's get ops
        Span<char> ops = stackalloc char[width >> 1];
        var totalOps = PrepareOps(Lines[height - 1].AsSpan(), ops);
        var currOp = 0;

        var currColumPos = 0;

        var total = 0L;
        var partial = ops[currOp] == '*' ? 1L : 0L;
        var num = 0L;

        while (currColumPos < width)
        {
            // read num from all lines (all but one, ops line)
            for (var r = 0; r < numRows; r++)
            {
                var c = Lines[r][currColumPos];
                if (char.IsDigit(c))
                    num = (num * 10) + (c - '0');
            }
            currColumPos++;

            if (num == 0)
            {
                // if partial is > 0, we just finished one column of numbers
                if (partial > 0)
                {
                    currOp++;
                    num = 0;
                    total += partial;
                    partial = ops[currOp] == '*' ? 1L : 0L;
                }
            }
            else
            {
                partial = ops[currOp] == '*' ? partial * num : partial + num;
                num = 0;
            }
        }
        // last result, as it may be ommited by the loop above
        // (if there was no "empty" column at the end of the file, the "if (num == 0)" would not run)
        total += partial;

        return total;
    }

    private static int PrepareOps(ReadOnlySpan<char> opLine, Span<char> ops)
    {
        ops.Clear();
        var opLinew = opLine.Length;
        var currOp = 0;
        for (var i = 0; i < opLinew; i++)
        {
            var o = opLine[i];
            if (o == '*' || o == '+')
            {
                ops[currOp++] = o;
            }
        }
        return currOp;
    }
}


/*

| Method | Mean     | Error    | StdDev   | Allocated |
|------- |---------:|---------:|---------:|----------:|
| Part1  | 57.83 us | 0.210 us | 0.197 us |      24 B |
| Part2  | 55.27 us | 0.446 us | 0.395 us |      24 B |

1 us      : 1 Microsecond (0.000001 sec)

Intel Core i5-8500 CPU 3.00GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX2

=============================================================================================

| Method | Mean     | Error    | StdDev   | Allocated |
|------- |---------:|---------:|---------:|----------:|
| Part1  | 23.39 us | 0.063 us | 0.059 us |      24 B |
| Part2  | 14.52 us | 0.095 us | 0.084 us |      24 B |

1 us      : 1 Microsecond (0.000001 sec)

Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX2

*/