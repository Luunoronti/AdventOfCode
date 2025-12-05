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
        //return Part2ByNick(Input);
        //if (DayRunner.PartRunner.Current.IsTestRun)
        //      return Part2Nums(Input);

        //        return "";
        //if (DayRunner.PartRunner.Current.PartConfig.ShowVisualisation)
        //    return Part2ByNickWithVis(Input);

        return Part2_2(Input).ToString();

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



    public string Part2Nums(PartInput Input)
    {
        const int MAX = 4;
        int removed = 0;
        //var inputMap = new Map<char>(Input.Lines, static (c) => c);
        var sizeX = Input.Lines[0].Length;
        var sizeY = Input.Lines.Length;

        unsafe
        {
            // fill it up from input
            int* stackMap = stackalloc int[sizeX * sizeY];
            int* ogMap = stackalloc int[sizeX * sizeY];

            int GetValue(int x, int y) => (x >= 0 && y >= 0 && x < sizeX && y < sizeY) ? stackMap[sizeX * y + x] : 0;
            void SetValue(int x, int y, int value) { if (x >= 0 && y >= 0 && x < sizeX && y < sizeY) stackMap[sizeX * y + x] = value; }

            int GetValueOg(int x, int y) => (x >= 0 && y >= 0 && x < sizeX && y < sizeY) ? ogMap[sizeX * y + x] : 0;
            void SetValueOg(int x, int y, int value) { if (x >= 0 && y >= 0 && x < sizeX && y < sizeY) ogMap[sizeX * y + x] = value; }

            void Print()
            {
                Console.WriteLine("-----------------------------");
                for (var y = 0; y < sizeY; y++)
                {
                    for (var x = 0; x < sizeX; x++)
                    {
                        Console.Write(GetValue(x, y));
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("-----------------------------");
            }


            void TryDec1IfOver(int x, int y)
            {
                if (GetValue(x, y) >= MAX) { TryDec1(x, y); }
            }
            void TryDec1(int x, int y)
            {
                var v = GetValue(x, y);
                if (v > 0)// return;
                    SetValue(x, y, v - 1);
                if (v >= MAX) return;

                TryDec1IfOver(x + 0, y - 1);
                TryDec1IfOver(x + 0, y + 1);
                TryDec1IfOver(x - 1, y + 0);
                TryDec1IfOver(x + 1, y + 0);

                TryDec1IfOver(x + 1, y - 1);
                TryDec1IfOver(x + 1, y + 1);
                TryDec1IfOver(x - 1, y - 1);
                TryDec1IfOver(x - 1, y + 1);
            }


            for (var y = 0; y < sizeY; y++)
            {
                var line = Input.Lines[y];
                for (var x = 0; x < sizeX; x++)
                {
                    SetValueOg(x, y, line[x] == '@' ? 1 : 0);
                    SetValue(x, y, line[x] == '@' ? 1 : 0);
                }
            }
            // fill with field counts
            Queue<(int x, int y)> toRemove = [];
            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    if (GetValue(x, y) == 0) continue;

                    int count = 0;
                    if (GetValue(x + 0, y - 1) > 0) count++;
                    if (GetValue(x + 0, y + 1) > 0) count++;
                    if (GetValue(x - 1, y + 0) > 0) count++;
                    if (GetValue(x + 1, y + 0) > 0) count++;

                    if (GetValue(x + 1, y - 1) > 0) count++;
                    if (GetValue(x + 1, y + 1) > 0) count++;
                    if (GetValue(x - 1, y - 1) > 0) count++;
                    if (GetValue(x - 1, y + 1) > 0) count++;

                    SetValue(x, y, count);
                }
            }

            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    if (GetValue(x, y) < MAX && GetValueOg(x, y) > 0)
                    {
                        removed++;
                    }
                }
            }
            for (var y = 0; y < sizeY; y++)
            {
                for (var x = 0; x < sizeX; x++)
                {
                    TryDec1(x, y);
                }
            }
        }
        return removed.ToString();
    }

    static int[] dirX = { -1, 0, 1, -1, 1, -1, 0, 1 };
    static int[] dirY = { -1, -1, -1, 0, 0, 1, 1, 1 };

    int Part2_2(PartInput Input)
    {
        const byte MAX = 4;

        int sizeY = Input.Lines.Length;
        int sizeX = Input.Lines[0].Length;
        int size = sizeY * sizeX;

        var rolls = new bool[size];
        var counts = new byte[size];

        Queue<int> queue = new Queue<int>(size);

        for (int y = 0; y < sizeY; y++)
        {
            string line = Input.Lines[y];
            int row = y * sizeX;
            for (int x = 0; x < sizeX; x++)
            {
                if (line[x] == '@')
                    rolls[row + x] = true;
            }
        }


        for (int y = 0; y < sizeY; y++)
        {
            int row = y * sizeX;
            for (int x = 0; x < sizeX; x++)
            {
                int idx = row + x;
                if (!rolls[idx]) continue;

                byte c = 0;
                for (int d = 0; d < 8; d++)
                {
                    int nx = x + dirX[d];
                    int ny = y + dirY[d];

                    if ((uint)nx >= (uint)sizeX || (uint)ny >= (uint)sizeY) continue;

                    if (rolls[ny * sizeX + nx])
                        c++;
                }
                counts[idx] = c;
            }
        }

        for (int i = 0; i < size; i++)
        {
            if (rolls[i] && counts[i] < MAX)
                queue.Enqueue(i);
        }

        int removed = 0;

        while (queue.TryDequeue(out var idx))
        {
            if (!rolls[idx])
                continue; 
            if (counts[idx] >= MAX)
                continue;

            rolls[idx] = false;
            removed++;

            int y = idx / sizeX;
            int x = idx - y * sizeX;

            for (int d = 0; d < 8; d++)
            {
                int nx = x + dirX[d];
                int ny = y + dirY[d];

                if ((uint)nx >= (uint)sizeX || (uint)ny >= (uint)sizeY)
                    continue;

                int ni = ny * sizeX + nx;
                if (!rolls[ni])
                    continue;

                if (counts[ni] > 0)
                    counts[ni]--;

                if (counts[ni] < MAX)
                    queue.Enqueue(ni);
            }
        }

        return removed;
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


    ////////////// 
    enum Direction
    {
        Left,
        Right,
        Up,
        Down,
        UpLeft,
        DownLeft,
        UpRight,
        DownRight,
    }
    struct GridLocation
    {
        public int X;
        public int Y;
        public int Row => Y;
        public int Column => X;

        public GridLocation(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    Dictionary<Direction, (int y, int x)> DirectionalMove = new() {
            { Direction.Up, (-1, 0) },
            { Direction.Right, (0, 1) },
            { Direction.Down, (1, 0) },
            { Direction.Left, (0, -1) },
            { Direction.UpLeft, (-1, -1) },
            { Direction.UpRight, (-1, 1) },
            { Direction.DownLeft, (1, -1) },
            { Direction.DownRight, (1, 1) },
        };

    int GetSurroundingCount(HashSet<GridLocation> paperRolls, int x, int y, int exitAfter)
    {
        int count = 0;
        foreach (var dir in DirectionalMove)
        {
            if (paperRolls.Contains(new GridLocation(x + dir.Value.x, y + dir.Value.y)))
            {
                if (++count >= exitAfter) return count;
            }
        }
        return count;
    }

    public string Part2ByNickWithVis(PartInput Input)
    {
        HashSet<GridLocation> paperRolls = [];
        HashSet<GridLocation> ogPaperRolls = [];
        int MaxSurroundingRolls = 4;

        for (int y = 0; y < Input.Lines.Length; y++)
        {
            var line = Input.Lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '@')
                {
                    paperRolls.Add(new GridLocation(x, y));
                    ogPaperRolls.Add(new GridLocation(x, y));
                }
            }
        }

        long p2 = 0L;
        Dictionary<GridLocation, int> counts = [];
        foreach (var roll in paperRolls)
        {
            counts.Add(roll, GetSurroundingCount(paperRolls, roll.X, roll.Y, 10));
        }

        Rgb validColor = new Rgb(210, 210, 210);
        Rgb removedColor = new Rgb(40, 60, 60);


        Visualiser.Run(new VisConfig
        {
            AutoPlay = true,
            CenterAtZero = true,
            AutoStepPerSecond = 20
        },
        process: () =>
        {
            var toRemove = new HashSet<GridLocation>();
            int removed = 0;

            GridLocation? startLocation = null;

            foreach (var kv in counts)
            {
                if (kv.Value < MaxSurroundingRolls)
                {
                    startLocation = kv.Key;
                    break;
                }
            }

            if (startLocation == null)
                return false;

            var stack = new Stack<GridLocation>();
            stack.Push(startLocation.Value);

            while (stack.Count > 0)
            {
                var location = stack.Pop();

                if (toRemove.Contains(location))
                    continue;

                toRemove.Add(location);
                removed++;

                foreach (var neighbour in DirectionalMove)
                {
                    var nLocation = new GridLocation(location.X + neighbour.Value.x, location.Y + neighbour.Value.y);

                    if (toRemove.Contains(nLocation))
                        continue;

                    if (counts.TryGetValue(nLocation, out int ncount))
                    {
                        if (ncount <= MaxSurroundingRolls)
                        {
                            stack.Push(nLocation);
                        }
                        else
                        {
                            counts[nLocation] = ncount - 1;
                        }
                    }
                }
                // const int MaxRemovedPerCall = 3;
                // if (removed >= MaxRemovedPerCall)
                //     break;
            }

            foreach (var loc in toRemove)
                counts.Remove(loc);

            p2 += removed;
            return removed > 0;
        },
        draw: (Frame, Completed) =>
        {

            foreach (var l in ogPaperRolls)
            {
                if (counts.TryGetValue(l, out int ncount))
                {
                    Frame.Draw(l.X, l.Y, (char)('0' + ncount), validColor, Rgb.Black);
                }
                else
                {
                    Frame.Draw(l.X, l.Y, '0', removedColor, Rgb.Black);
                }
            }

            //foreach (var v in counts)
            //{
            //    Frame.Draw(v.Key.X, v.Key.Y, (char)('0' + v.Value), Rgb.White, Rgb.Black);
            //}
        },
        info: (wx, wy) =>
        {
            //if (wx < 0 || wy < 0 || wx >= dstMap.SizeX || wy >= dstMap.SizeY)
            //    return "";

            //dstMap.GetAdjenced(wx, wy, adjencedBuffer);
            //return $"{adjencedBuffer.Count(c => c == '@')} occupied adjenced cells";
            return "";
        },
        status: () => $"Total: {p2}"//$"Last sweep: {accessibleCount}, total: {totalCount}"
        );


        return p2.ToString();
    }

}
