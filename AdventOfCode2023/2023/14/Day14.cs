//#define SHOWTILTINGANIMATIONS
using System.Diagnostics;
using System.Text;
using Cache = System.Collections.Generic.Dictionary<int, (int hash, long weigth)>;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(136)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(64)] // if != 0, will report failure if expected answer != given answer
    class Day14
    {
        const byte RollableRock = 1;
        const byte StaticRock = 255;
        const byte EmptySpace = 0;

        private static (byte[] buffer, int width, int heigth) ProduceMapFromInput(string[] lines)
        {
            var width = lines[0].Length;
            var heigth = lines.Length;

            var buffer = new byte[width * heigth];
            var full = string.Join("", lines);
            for (int i = 0; i < width * heigth; i++)
            {
                var c = full[i];
                buffer[i] = c == '.' ? EmptySpace : (c == '#' ? StaticRock : RollableRock);
            }
            return (buffer, width, heigth);
        }

        private static void TiltNorth(byte[] map, int width, int heigth)
        {
#if SHOWTILTINGANIMATIONS
            Thread.Sleep(1000);
            Log.WriteLine("Tilting north...");
            var cx = Console.CursorLeft;
            var cy = Console.CursorTop;

            PrintMap(map, width, heigth);
            Thread.Sleep(1000);
#endif
            for (var x = 0; x < width; x++)
            {
                var targetRow = 0;
                for (var y = 1; y < heigth; y++)
                {
#if SHOWTILTINGANIMATIONS
                    Console.CursorLeft = cx;
                    Console.CursorTop = cy;
#endif
                    if (targetRow == y) continue;
                    if (map[targetRow * width + x] != EmptySpace)
                    {
                        targetRow++;
                        continue;
                    }
                    var c = map[y * width + x];
                    if (c == EmptySpace) continue;
                    if (c == StaticRock)
                    {
                        if (y + 1 == heigth)
                            break;
                        targetRow = y + 1;
                    }
                    else
                    {
                        map[targetRow * width + x] = c;
                        map[y * width + x] = EmptySpace;
                        targetRow++;
                    }
#if SHOWTILTINGANIMATIONS
                    PrintMap(map, width, heigth);
                    Thread.Sleep(1000);
#endif
                }
            }
        }
        private static void TiltSouth(byte[] map, int width, int heigth)
        {
#if SHOWTILTINGANIMATIONS
            Thread.Sleep(1000);
            Log.WriteLine("Tilting south...");
            var cx = Console.CursorLeft;
            var cy = Console.CursorTop;

            PrintMap(map, width, heigth);
            Thread.Sleep(1000);
#endif
            for (var x = 0; x < width; x++)
            {
                var targetRow = heigth - 1;
                for (var y = heigth - 1; y >= 0; y--)
                {
#if SHOWTILTINGANIMATIONS
                    Console.CursorLeft = cx;
                    Console.CursorTop = cy;
#endif
                    if (targetRow == y) continue;
                    if (map[targetRow * width + x] != EmptySpace)
                    {
                        targetRow--;
                        continue;
                    }
                    var c = map[y * width + x];
                    if (c == EmptySpace) continue;
                    if (c == StaticRock)
                    {
                        if (y - 1 < 0)
                            break;
                        targetRow = y - 1;
                    }
                    else
                    {
                        map[targetRow * width + x] = c;
                        map[y * width + x] = EmptySpace;
                        targetRow--;
                    }
#if SHOWTILTINGANIMATIONS
                    PrintMap(map, width, heigth);
                    Thread.Sleep(1000);
#endif
                }
            }
        }

        private static void TiltWest(byte[] map, int width, int heigth)
        {
#if SHOWTILTINGANIMATIONS
            Thread.Sleep(1000);
            Log.WriteLine("Tilting west...");
            var cx = Console.CursorLeft;
            var cy = Console.CursorTop;

            PrintMap(map, width, heigth);
            Thread.Sleep(1000);
#endif
            for (var y = 0; y < heigth; y++)
            {
                var targetColumn = 0;
                for (var x = 0; x < width; x++)
                {
#if SHOWTILTINGANIMATIONS
                    Console.CursorLeft = cx;
                    Console.CursorTop = cy;
#endif
                    if (targetColumn == x) continue;
                    if (map[y * width + targetColumn] != EmptySpace)
                    {
                        targetColumn++;
                        continue;
                    }

                    var c = map[y * width + x];
                    if (c == EmptySpace) continue;
                    if (c == StaticRock)
                    {
                        //if (y + 1 == heigth)
                        //    break;
                        targetColumn = x + 1;
                    }
                    else
                    {
                        map[y * width + targetColumn] = c;
                        map[y * width + x] = EmptySpace;
                        targetColumn++;
                    }
#if SHOWTILTINGANIMATIONS
                    PrintMap(map, width, heigth);
                    Thread.Sleep(1000);
#endif
                }
            }
        }
        private static void TiltEast(byte[] map, int width, int heigth)
        {
#if SHOWTILTINGANIMATIONS
            Thread.Sleep(1000);
            Log.WriteLine("Tilting west...");
            var cx = Console.CursorLeft;
            var cy = Console.CursorTop;

            PrintMap(map, width, heigth);
            Thread.Sleep(1000);
#endif
            for (var y = 0; y < heigth; y++)
            {
                var targetColumn = width - 1;
                for (var x = width - 1; x >= 0; x--)
                {
#if SHOWTILTINGANIMATIONS
                    Console.CursorLeft = cx;
                    Console.CursorTop = cy;
#endif
                    if (targetColumn == x) continue;
                    if (map[y * width + targetColumn] != EmptySpace)
                    {
                        targetColumn--;
                        continue;
                    }

                    var c = map[y * width + x];
                    if (c == EmptySpace) continue;
                    if (c == StaticRock)
                    {
                        if (x - 1 < 0)
                            break;
                        targetColumn = x - 1;
                    }
                    else
                    {
                        map[y * width + targetColumn] = c;
                        map[y * width + x] = EmptySpace;
                        targetColumn--;
                    }
#if SHOWTILTINGANIMATIONS
                    PrintMap(map, width, heigth);
                    Thread.Sleep(1000);
#endif
                }
            }
        }

        private static long ComputeWeight(byte[] map, int width, int heigth)
        {
            long sum = 0;
            int multiplier = heigth;
            for (int y = 0; y < heigth; y++)
            {
                var rowSum = 0;
                for (int x = 0; x < width; x++)
                {
                    if (map[y * width + x] == RollableRock)
                        rowSum++;
                }
                rowSum *= multiplier;
                multiplier--;
                sum += rowSum;
            }
            return sum;
        }

        private static void PrintMap(byte[] map, int width, int heigth)
        {
            if (Log.Enabled == false) return;
            var sb = new StringBuilder();
            for (var y = 0; y < heigth; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var c = map[y * width + x];
                    if (c == EmptySpace)
                    {
                        sb.Append($"{CC.Frm}*{CC.Clr}");
                        continue;
                    }
                    if (c == StaticRock)
                    {
                        sb.Append($"{CC.Att}#{CC.Clr}");
                        continue;
                    }

                    sb.Append($"{CC.Sys}O{CC.Clr}");
                }
                sb.AppendLine();
            }
            Log.WriteLine(sb.ToString());
        }
        
        public static int GetHashCodeForMap(byte[] map, int width, int heigth)
        {
            const int power = 16777619;
            unchecked
            {
                var result = (int)2166136261;
                for (int i = 0; i < map.Length; i++)
                {
                    result = (result ^ map[i]) * power;
                }
                return result;
            }
}

        public static long Part1(string[] lines)
        {
            // will time each step
            var sw1 = Stopwatch.StartNew();
            var (map, width, heigth) = ProduceMapFromInput(lines);
            sw1.Stop();
            Log.WriteLine($"{CC.Sys}{nameof(ProduceMapFromInput)}{CC.Clr} took {CC.Val}{sw1.Elapsed} ({sw1.ElapsedMilliseconds} ms){CC.Clr}");

            //PrintMap(map, width, heigth);

            // tilt up (north)
            var sw2 = Stopwatch.StartNew();
            TiltNorth(map, width, heigth);
            sw2.Stop();
            Log.WriteLine($"{CC.Sys}{nameof(TiltNorth)}{CC.Clr} took {CC.Val}{sw2.Elapsed} ({sw2.ElapsedMilliseconds} ms){CC.Clr}");

            // compute weigths
            //PrintMap(map, width, heigth);

            var sw3 = Stopwatch.StartNew();
            var answer = ComputeWeight(map, width, heigth);
            sw3.Stop();
            Log.WriteLine($"{CC.Sys}{nameof(ComputeWeight)}{CC.Clr} took {CC.Val}{sw3.Elapsed} ({sw3.ElapsedMilliseconds} ms){CC.Clr}");

            return answer;
        }


        private static (int hash, long weigth, bool isHit) TiltAll(int hash, Cache cache, byte[] map, int width, int heigth)
        {
            if (cache.TryGetValue(hash, out var ret))
            {
                //Log.WriteLine("Cache hit!");
                //platform.Print(Console.CursorLeft, Console.CursorTop);
                //Log.WriteLine();
                return (ret.hash, ret.weigth, true);
            }
            else
            {
                // perform all 4 swings
                TiltNorth(map, heigth, width);
                TiltWest(map, heigth, width);
                TiltSouth(map, heigth, width);
                TiltEast(map, heigth, width);

                //platform.Print(Console.CursorLeft, Console.CursorTop);
                //Log.WriteLine();

                var newHash = GetHashCodeForMap(map, width, heigth);
                var weigth = ComputeWeight(map, width, heigth);

                cache[hash] = (newHash, weigth);
                return (newHash, weigth, false);
            }
        }
       

        public static long Part2(string[] lines)
        {
            const long repetitions = 1_000_000_000;

            var sw1 = Stopwatch.StartNew();
            var (map, width, heigth) = ProduceMapFromInput(lines);
            sw1.Stop();
            Log.WriteLine($"{CC.Sys}{nameof(ProduceMapFromInput)}{CC.Clr} took {CC.Val}{sw1.Elapsed} ({sw1.ElapsedMilliseconds} ms){CC.Clr}");

            var cache = new Cache();
            var cachesList = new List<int>();

            var currentHash = GetHashCodeForMap(map, width, heigth);
            var currentWeight = 0L;
            int numberOfSteps = 0;
            for (var i = 0; i < repetitions; i++)
            {
                var ret = TiltAll(currentHash, cache, map, width, heigth);

                currentHash = ret.hash;
                currentWeight = ret.weigth;
                if (ret.isHit)
                {
                    var index = cachesList.IndexOf(ret.hash);
                    // we should compute our way out, so we don't have to
                    // iterate over a dictionary for X times. as we are in cache already, we know nothing
                    // will change in terms of map permutations, no new result will be produced.
                    // we just need to compute, which of our (already computed) results is the one
                    var oneIteration = cachesList.Count - index;
                    var remainingSteps = repetitions - numberOfSteps;

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

                
            }
            return currentWeight;//platform.CalculateTotawWeight();
        }






        /////////////////////
        /// EVRYTHING BELLOW IS OLD CODE (INCLUDING ENUMS AND CLASSES)
        /// SLOW BUT DOES YIELD POPER RESULTS
        /// BY SLOW I MEAN 2.5 SECONDS ON PART2
        /////////////////////


        public static long Part1_Old(string[] lines)
        {
            var platform = new Platform(lines);
            platform.MoveAll(MoveDirection.North);
            return platform.CalculateTotawWeight();
        }

        private static (int hash, long weigth, bool isHit) FullSwing(int hash, Platform platform, Cache cache)
        {
            if (cache.TryGetValue(hash, out var ret))
            {
                Log.WriteLine("Cache hit!");
                //platform.Print(Console.CursorLeft, Console.CursorTop);
                //Log.WriteLine();
                return (ret.hash, ret.weigth, true);
            }
            else
            {
                // perform all 4 swings
                platform.MoveAll(MoveDirection.North);
                platform.MoveAll(MoveDirection.West);
                platform.MoveAll(MoveDirection.South);
                platform.MoveAll(MoveDirection.East);

                //platform.Print(Console.CursorLeft, Console.CursorTop);
                //Log.WriteLine();

                var newHash = platform.GetHash();
                var weigth = platform.CalculateTotawWeight();

                cache[hash] = (newHash, weigth);
                return (newHash, weigth, false);
            }
        }

        public static long Part2_Old(string[] lines)
        {
            // this takes 2 seconds.
            // too long, if i was to operate on proper mem representation, and move
            // objects 
            const long repetitions = 1_000_000_000;

            // we will use our old platform implementation here.
            // it is somehow slow, but it works
            // if we are too slow, we will implement other approach
            var platform = new Platform(lines);

            var cache = new Cache();
            var cachesList = new List<int>();

            var currentHash = platform.GetHash();
            var currentWeight = 0L;
            int numberOfSteps = 0;
            for (var i = 0; i < repetitions; i++)
            {
                var ret = FullSwing(currentHash, platform, cache);
                currentHash = ret.hash;
                currentWeight = ret.weigth;
                if (ret.isHit)
                {
                    var index = cachesList.IndexOf(ret.hash);
                    // we should compute our way out, so we don't have to
                    // iterate over a dictionary for X times. as we are in cache already, we know nothing
                    // will change in terms of map permutations, no new result will be produced.
                    // we just need to compute, which of our (already computed) results is the one
                    var oneIteration = cachesList.Count - index;
                    var remainingSteps = repetitions - numberOfSteps;

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
            }
            return currentWeight;//platform.CalculateTotawWeight();
        }

        enum MoveDirection
        {
            North,
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

            public void MoveAllNorth() => MoveAll(MoveDirection.North);

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
                    case MoveDirection.North:
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
            public override string ToString() => IsMovable ? "O" : "#";
        }
    }
}