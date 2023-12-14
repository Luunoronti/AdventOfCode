namespace AdventOfCode2023
{
    //[Force]                   // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]         // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]       // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]       // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]       // if uncommented, will use test data in both debugging session and non-debugging session
    class Day14
    {
        public static string TestFile => "2023\\14\\test.txt";
        public static string LiveFile => "2023\\14\\live.txt";

        public static long Part1(string[] lines)
        {
            var platform = new Platform(lines);
            platform.MoveAllNorht();
            return platform.CalculateTotawWeight();
        }
        public static long Part2(string[] lines)
        {
            var platform = new Platform2(lines);
            platform.DoSpins(1_000_000_000);

            var target = 1_000_000_000;
            for (int i = 0; i < target; i++)
            {
                platform.DoASpint();
                //Console.WriteLine();
                //platform.Print(Console.CursorLeft, Console.CursorTop);
            }

            return platform.CalculateTotawWeight();
        }


        // new approach, with a byte map that we will work with
        // in unsafe mode, to speed up
        class Platform2
        {
            public Platform2(string[] lines)
            {
                var linesLen = lines.Length;
                var lines0Len = lines[0].Length;
                mapWidth = lines0Len;
                mapHeight = linesLen;
                _map = new byte[mapWidth * mapHeight];

                for (int y = 0; y < linesLen; y++)
                {
                    for (int x = 0; x < lines0Len; x++)
                    {
                        var c = lines[y][x];
                        if (c == '.') continue;

                        _map[y * mapWidth + x] = (byte)(c == 'O' ? 1 : 2); // 1 means movable rock, 2 means non movable 
                    }
                }
            }
            private byte[] _map;
            private int mapWidth;
            private int mapHeight;


            private unsafe void MoveNorth(byte* p)
            {
                // note: we can't move first row to the north
                // so we stat at second row
                for (int i = mapWidth; i < mapWidth * mapHeight; i++)
                {
                    var r = *(p + i);
                    if (r == 0 || r == 2) continue;
                    //var x = i % mapWidth;
                    var y = i;// / mapHeight;

                    while (y >= 0 && (*p + y) == 0)
                        y -= mapWidth;

                    // clear at our pos
                    p[i] = 0;

                    // set at new pos
                    p[y] = r;
}
            }
            public unsafe void DoSpins(long count)
            {
                fixed (byte* p = _map)
                {
                    for (var i = 0; i < count; i++)
                    {
                        MoveNorth(p);

                        if (i % 1_000_000 == 0)
                        {
                            var per = (double)i / (double)count;
                            Console.WriteLine(per * 100);
                        }
                    }
                }
            }
            public long DoASpint()
            {
                return 0;
            }
            public long CalculateTotawWeight()
            {
                return 0;
            }
        }




        // old, OO approach to a map
        enum MoveDirection
        {
            Noth,
            South,
            East,
            West
        }
        class Platform
        {
            public Platform(string[] lines)
            {
                var linesLen = lines.Length;
                var lines0Len = lines[0].Length;

                Map = new Rock[linesLen * lines0Len];
                MapWidth = lines0Len;
                MapHeight = linesLen;

                for (int y = 0; y < linesLen; y++)
                {
                    for (int x = 0; x < lines0Len; x++)
                    {
                        var c = lines[y][x];
                        if (c == '.') continue;
                        var rock = new Rock(this, x, y, c == 'O');
                        _allRocks.Add(rock);
                        SetAt(x, y, rock);
                    }
                }
            }

            public int MapWidth { get; }
            public int MapHeight { get; }
            public Rock[] Map { get; }
            private List<Rock> _allRocks = new();

            public Rock GetAt(int x, int y) => Map[y * MapWidth + x];
            public void SetAt(int x, int y, Rock rock) => Map[y * MapWidth + x] = rock;

            public void Print(int x, int y)
            {
                Console.CursorTop = y;
                Console.CursorLeft = x;

                for (int my = 0; my < MapHeight; my++)
                {
                    for (int mx = 0; mx < MapWidth; mx++)
                    {
                        var r = GetAt(mx, my);

                        Console.Write(r == null ? '.' : r);
                    }
                    Console.WriteLine();
                }
            }
            public long MoveAll(MoveDirection direction)
            {
                var moved = 0;
                while (true)
                {
                    var anyMoved = false;
                    foreach (var rock in _allRocks)
                    {
                        if (rock.MoveIfPossible(direction))
                        {
                            anyMoved = true;
                            moved++;
                        }
                    }
                    if (!anyMoved)
                        break;
                }
                return moved;
            }

            public void MoveAllNorht() => MoveAll(MoveDirection.Noth);
            public long DoASpint()
            {
                return MoveAll(MoveDirection.Noth)
                + MoveAll(MoveDirection.West)
                + MoveAll(MoveDirection.South)
                + MoveAll(MoveDirection.East);
            }

            public long CalculateTotawWeight() => _allRocks.Sum(rock => rock.Weight);
        }
        class Rock
        {
            private Platform _platform;
            public Rock(Platform platform, int x, int y, bool isMovable)
            {
                _platform = platform;
                IsMovable = isMovable;
                X = x;
                Y = y;
            }

            public bool IsMovable { get; }
            public int X { get; private set; }
            public int Y { get; private set; }

            public int Weight => IsMovable ? _platform.MapHeight - Y : 0;

            private bool MoveTo(int newX, int newY)
            {
                if (!IsMovable) return false; // can't move at all

                var rockAtDest = _platform.GetAt(newX, newY);
                if (rockAtDest != null) return false; // can't move, there is a rock there

                // update Map
                _platform.SetAt(newX, newY, this);
                _platform.SetAt(X, Y, null);
                // we can move
                X = newX;
                Y = newY;
                return true;
            }
            // move one step in direction
            // returns true if moved, false otherwise
            public bool MoveIfPossible(MoveDirection direction)
            {
                if (!IsMovable) return false; // can't move at all

                switch (direction)
                {
                    case MoveDirection.Noth:
                        if (Y == 0) return false; // can't move beyond the platform
                        return MoveTo(X, Y - 1);

                    case MoveDirection.South:
                        if (Y == _platform.MapHeight - 1) return false; // can't move beyond the platform
                        return MoveTo(X, Y + 1);

                    case MoveDirection.East:
                        if (X == _platform.MapWidth - 1) return false; // can't move beyond the platform
                        return MoveTo(X + 1, Y);

                    case MoveDirection.West:
                        if (X == 0) return false; // can't move beyond the platform
                        return MoveTo(X - 1, Y);

                    default:
                        Log.WriteLine($"{CC.Err}Other move directions not implemented yet{CC.Clr}");
                        break;
                }

                return false;
            }

            public override string ToString()
            {
                return IsMovable ? "O" : "#";
            }
        }
    }
}