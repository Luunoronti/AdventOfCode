using System.Diagnostics;
using TermGlass;

namespace Year2025;

class Day04
{
    public string Part1(PartInput Input)
    {
        var map = new Map<char>(Input.Lines, static (c) => c);
        var adjencedBuffer = new char[8];
        var count = map.Count((x, y, c) =>
        {
            if (c != '@') return false;
            map.GetAdjenced(x, y, adjencedBuffer);
            return adjencedBuffer.Count(c => c == '@') < 4;
        });
        return count.ToString();
    }
    public string Part2(PartInput Input)
    {
        if (DayRunner.PartRunner.Current.PartConfig.ShowVisualisation)
            return Part2WithVis(Input);

        Console.WriteLine("b");
        var map = new Map<char>(Input.Lines, static (c) => c);
        var secondMap = new Map<char>(map.SizeX, map.SizeY);
        var adjencedBuffer = new char[8];

        var accessibleCount = 0;
        var totalCount = 0L;
        var srcMap = map;
        var dstMap = secondMap;
        do
        {
            srcMap.CopyTo(dstMap);

            accessibleCount = srcMap.Count((x, y, c) =>
            {
                if (c != '@') return false;
                srcMap.GetAdjenced(x, y, adjencedBuffer);
                var hasAccess = adjencedBuffer.Count(c => c == '@') < 4;
                if (hasAccess)
                {
                    dstMap.Set(x, y, '.');
                }
                return hasAccess;
            });

            // swap maps
            (srcMap, dstMap) = (dstMap, srcMap);

            totalCount += accessibleCount;
        } while (accessibleCount > 0);
        return totalCount.ToString();
    }



    public string Part2_2(PartInput Input)
    {
        var map = new Map<char>(Input.Lines, static (c) => c);
        var adjencedBuffer = new char[8];

        var accessibleCount = 0;
        var totalCount = 0L;

        var wasOccupied = '1';
        var otherFlag = '2';

        var sweeps = 0;
        do
        {
            accessibleCount = map.Count((x, y, c) =>
            {
                if (c == wasOccupied || c == otherFlag) { map.SetUnsafe(x, y, '.'); return false; }
                if (c != '@') return false;

                map.GetAdjenced(x, y, adjencedBuffer);
                var hasAccess = adjencedBuffer.Count(c => c == '@' || c == wasOccupied) < 4;
                if (hasAccess)
                {
                    map.SetUnsafe(x, y, wasOccupied);
                }
                return hasAccess;
            });

            (wasOccupied, otherFlag) = (otherFlag, wasOccupied);
            sweeps++;
            totalCount += accessibleCount;
        } while (accessibleCount > 0);

        return totalCount.ToString();
    }



    public string Part2WithVis(PartInput Input)
    {
        var map = new Map<char>(Input.Lines, static (c) => c);
        var secondMap = new Map<char>(map.SizeX, map.SizeY);
        var adjencedBuffer = new char[8];

        var accessibleCount = 0;
        var totalCount = 0L;
        var srcMap = map;
        var dstMap = secondMap;

        Visualiser.Run(new VisConfig
        {
            AutoPlay = true,
            CenterAtZero = true,
            AutoStepPerSecond = 2
        },
        process: () =>
        {
            srcMap.CopyTo(dstMap);
            accessibleCount = srcMap.Count((x, y, c) =>
            {
                if (c != '@') return false;
                srcMap.GetAdjenced(x, y, adjencedBuffer);
                var hasAccess = adjencedBuffer.Count(c => c == '@') < 4;
                if (hasAccess)
                {
                    dstMap.Set(x, y, '.');
                }
                return hasAccess;
            });
            (srcMap, dstMap) = (dstMap, srcMap);
            totalCount += accessibleCount;
            return accessibleCount != 0;
        },
        draw: (Frame, Completed) => Frame.Draw(dstMap, (x, y) => srcMap[x, y] == dstMap[x, y] ? Rgb.White : Rgb.Red, (x, y) => Rgb.Black),
        info: (wx, wy) =>
        {
            if (wx < 0 || wy < 0 || wx >= dstMap.SizeX || wy >= dstMap.SizeY)
                return "";

            dstMap.GetAdjenced(wx, wy, adjencedBuffer);
            return $"{adjencedBuffer.Count(c => c == '@')} occupied adjenced cells";
        },
        status: () => $"Last sweep: {accessibleCount}, total: {totalCount}"
        );

        return totalCount.ToString();
    }
}
