using Cache = System.Collections.Generic.Dictionary<int, (int hash, long weigth)>;

namespace AdventOfCode2023
{
    //[Force]                   // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]         // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]       // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]       // if uncommented and under a debug session, will use live data (problem data) instead of test data
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

        private static (string map, int hash, long weight) SwingGetHashAndComputeWeight(string input, int mapWidth, int mapHeight, int direction)
        {
            return (input, input.GetHashCode(), 0);
        }

        private static (int hash, long weigth, bool isHit) Swing(int hash, MoveDirection direction, Platform platform, Cache cache)
        {
            if ( cache.TryGetValue(hash, out var ret))
            {
                //Log.WriteLine("Cache hit!");
                //platform.Print(Console.CursorLeft, Console.CursorTop);
                //Console.WriteLine();
                return (ret.hash, ret.weigth, true);
            }
            else
            {
                // perform all 4 swings
                platform.MoveAll(MoveDirection.Noth);
                platform.MoveAll(MoveDirection.West);
                platform.MoveAll(MoveDirection.South);
                platform.MoveAll(MoveDirection.East);


                //platform.Print(Console.CursorLeft, Console.CursorTop);
                //Console.WriteLine();

                var newHash = platform.GetHash();
                var weigth = platform.CalculateTotawWeight();

                cache[hash] = (newHash, weigth);
                return (newHash, weigth, false);
            }
        }

        public static long Part2(string[] lines)
        {
            // we will use our old platform implementation here.
            // it is somehow slow, but it works
            // if we are too slow, we will implement other approach
            var platform = new Platform(lines);

            var cache = new Cache();
            var cachesList = new List<int>();

            var currentHash = platform.GetHash();
            var currentWeight = 0L;
            int numberOfSteps = 0;
            for (var i = 0; i < 1_000_000_000; i++)
            {
                var ret = Swing(currentHash, MoveDirection.Noth, platform, cache);
                currentHash = ret.hash;
                currentWeight = ret.weigth;
                if (ret.isHit)
                {
                    var index = cachesList.IndexOf(ret.hash);
                    // we should compute our way out, so we don't have to
                    // iterate over a dictionary for X times
                    var oneIteration = cachesList.Count - index;
                    var remainingSteps = 1_000_000_000 - numberOfSteps;

                    var fullIterations = remainingSteps / oneIteration;
                    var remaining = (remainingSteps - (fullIterations * oneIteration) - 1);

                    // step remaining times
                    for (int r = 0; r < remaining; r++)
                    {
                        var (hash, weigth) = cache[currentHash];
                        currentHash = hash;
                        currentWeight = weigth;
                    }
                    return currentWeight;
                }
                else
                {
                    // add this to a list
                    cachesList.Add(currentHash);
                }
                currentHash = ret.hash;
                currentWeight = ret.weigth;
                numberOfSteps++;

                if(numberOfSteps % 1_000_000 == 0)
                {
                    var pr = (double)numberOfSteps / (double)1_000_000_000;

                    Console.WriteLine($"{numberOfSteps} - {pr*100}");
                }
            }


            return currentWeight;//platform.CalculateTotawWeight();
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
           
            public long CalculateTotawWeight() => _allRocks.Sum(rock => rock.Weight);

            public string GetStringRepr() => string.Join("", Map.Select(m => m == null ? '.' : (m.IsMovable ? 'O' : '#')));
            public int GetHash() => GetStringRepr().GetHashCode();
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