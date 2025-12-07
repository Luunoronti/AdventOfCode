namespace AoC;
/*
| Method | Mean     | Error    | StdDev   | Allocated |
|------- |---------:|---------:|---------:|----------:|
| Part1  | 19.36 us | 0.076 us | 0.071 us |      24 B |
| Part2  | 20.04 us | 0.098 us | 0.092 us |      24 B |
*/
[DefaultInput("live")]
public static class Solver
{
    [ExpectedResult("test", 209)]
    [ExpectedResult("live", 55538)]
    public static object? SolvePart1(string[] Lines)
    {
        var Sum = 0L;
        for (var Index = 0; Index < Lines.Length; Index++)
        {
            var Line = Lines[Index];
            Sum += GetCalibrationValuePart1(Line);
        }
        return Sum;
    }

    [ExpectedResult("test", 281)]
    [ExpectedResult("live", 54875)]
    public static object? SolvePart2(string[] Lines)
    {
        var Sum = 0L;
        for (var Index = 0; Index < Lines.Length; Index++)
        {
            var Line = Lines[Index];
            Sum += GetCalibrationValuePart2(Line);
        }
        return Sum;
    }

    static int GetCalibrationValuePart1(string Line)
    {
        var First = -1;
        var Last = 0;
        for (var Index = 0; Index < Line.Length; Index++)
        {
            var Character = Line[Index];
            if (Character >= '0' && Character <= '9')
            {
                var Digit = Character - '0';
                if (First < 0) First = Digit;
                Last = Digit;
            }
        }
        if (First < 0) return 0;
        return First * 10 + Last;
    }

    static int GetCalibrationValuePart2(string Line)
    {
        var First = -1;
        var Last = 0;
        for (var Index = 0; Index < Line.Length; Index++)
        {
            var Character = Line[Index];
            var Digit = -1;
            if (Character >= '0' && Character <= '9') Digit = Character - '0';
            else Digit = TryGetWordDigit(Line, Index);
            if (Digit >= 0)
            {
                if (First < 0) First = Digit;
                Last = Digit;
            }
        }
        if (First < 0) return 0;
        return First * 10 + Last;
    }

    static int TryGetWordDigit(string Line, int Index)
    {
        var Remaining = Line.Length - Index;
        if (Remaining <= 1) return -1;
        var Character = Line[Index];
        switch (Character)
        {
            case 'z':
                if (Remaining >= 4 && Line[Index + 1] == 'e' && Line[Index + 2] == 'r' && Line[Index + 3] == 'o') return 0;
                break;
            case 'o':
                if (Remaining >= 3 && Line[Index + 1] == 'n' && Line[Index + 2] == 'e') return 1;
                break;
            case 't':
                if (Remaining >= 3 && Line[Index + 1] == 'w' && Line[Index + 2] == 'o') return 2;
                if (Remaining >= 5 && Line[Index + 1] == 'h' && Line[Index + 2] == 'r' && Line[Index + 3] == 'e' && Line[Index + 4] == 'e') return 3;
                break;
            case 'f':
                if (Remaining >= 4 && Line[Index + 1] == 'o' && Line[Index + 2] == 'u' && Line[Index + 3] == 'r') return 4;
                if (Remaining >= 4 && Line[Index + 1] == 'i' && Line[Index + 2] == 'v' && Line[Index + 3] == 'e') return 5;
                break;
            case 's':
                if (Remaining >= 3 && Line[Index + 1] == 'i' && Line[Index + 2] == 'x') return 6;
                if (Remaining >= 5 && Line[Index + 1] == 'e' && Line[Index + 2] == 'v' && Line[Index + 3] == 'e' && Line[Index + 4] == 'n') return 7;
                break;
            case 'e':
                if (Remaining >= 5 && Line[Index + 1] == 'i' && Line[Index + 2] == 'g' && Line[Index + 3] == 'h' && Line[Index + 4] == 't') return 8;
                break;
            case 'n':
                if (Remaining >= 4 && Line[Index + 1] == 'i' && Line[Index + 2] == 'n' && Line[Index + 3] == 'e') return 9;
                break;
        }
        return -1;
    }
}
