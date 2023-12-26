using System.Collections.Immutable;
using System.Drawing;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    [AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    [UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    [RequestsVisualizer]
    class Day23
    {
        enum Path : byte
        {
            Open,
            Closed,
            SlopeToXPos,
            SlopeToYPos,
            StartPos,
        }


        class PathPart
        {
            public Point startPos;
            public Point endPos;
            public int length;
            public Point startJointPoint;

            public int Index;
            public bool PathSearchFinished = false;
            public bool isEndPart;
            public bool isReverser;

            private int _search_x = -1;
            private int _search_y = -1;
            public bool startDirRightDownOnly = true;

            private Color visColor = BitmapContext.RandomizeColor(120, 120, 120, 20);

            public void Advance(Map2DSpan<Path> map, Map2DSpan<bool> closed, BitmapContext bmpC)
            {
                if (PathSearchFinished) return;

                closed.At(_search_x, _search_y, true);

                bmpC?.SetSelectedColor(visColor);

                bmpC?.FillRectangle(_search_x * 4 + 1, _search_y * 4 + 1, 2, 2);


                // if we've found the end
                if (_search_y == map.Height - 1)
                {
                    isEndPart = true;
                    closed.At(_search_x + 1, _search_y, true);
                    endPos = new Point(_search_x + 1, _search_y);
                    PathSearchFinished = true;

                    bmpC?.FillRectangle((_search_x+1) * 4 + 1, (_search_y + 1) * 4 + 1, 2, 2);
                    bmpC?.DrawRectangle((_search_x + 1) * 4 + 1, (_search_y + 1) * 4 + 1, 2, 2, Color.Green);
                    //virGr?.DrawRectangle(Pens.Green, new Rectangle((_search_x + 1) * 4, (_search_y + 1) * 4, 4, 4));
                    //length++;
                    return;
                }

                length++;

                if (_search_x == -1 && _search_y == -1)
                {
                    _search_x = startPos.X;
                    _search_y = startPos.Y;
                    closed.At(_search_x, _search_y, true);
                }
                else
                {
                    // if this spot is a slope, end here. also, add next point as slope end
                    var mp = map.At(_search_x, _search_y);
                    if (mp == Path.SlopeToXPos)
                    {
                        closed.At(_search_x + 1, _search_y, true);
                        endPos = new Point(_search_x + 1, _search_y);
                        length++;
                        PathSearchFinished = true;
                        bmpC?.DrawRectangle((_search_x + 1) * 4, (_search_y + 1) * 4, 4, 4, Color.Blue);
                        bmpC?.FillRectangle((_search_x + 1) * 4 + 1, _search_y * 4 + 1, 2, 2);

                        return;
                    }
                    if (mp == Path.SlopeToYPos)
                    {
                        closed.At(_search_x, _search_y + 1, true);
                        endPos = new Point(_search_x, _search_y + 1);
                        length++;
                        PathSearchFinished = true;
                        bmpC?.DrawRectangle((_search_x + 1) * 4, (_search_y + 1) * 4, 4, 4, Color.Yellow);
                        bmpC?.FillRectangle(_search_x * 4 + 1, (_search_y + 1) * 4 + 1, 2, 2);
                        return;
                    }
                }
                if (IsValidAt(_search_x + 1, _search_y, map, closed))
                    _search_x++;
                else if (IsValidAt(_search_x, _search_y + 1, map, closed))
                    _search_y++;
                else if (!startDirRightDownOnly && IsValidAt(_search_x - 1, _search_y, map, closed))
                    _search_x--;
                else if (!startDirRightDownOnly && IsValidAt(_search_x, _search_y - 1, map, closed))
                    _search_y--;
                else
                {
                    PathSearchFinished = true;
                }
                startDirRightDownOnly = false;

            }
            public static bool IsValidAt(int x, int y, Map2DSpan<Path> map, Map2DSpan<bool> closed)
            {
                var pm = map.At(x, y, out var oob);
                if (oob) return false;
                if (closed.At(x, y)) return false;
                if (pm != Path.Closed) return true;
                return false;
            }

            public override string ToString() => $"Id: {Index}, Rev: {isReverser}, Last: {isEndPart}";

        }

        private static List<PathPart> FindPathParts(Map2DSpan<Path> map, Point startPos, bool visualize)
        {
            var start = new PathPart() { startPos = startPos };

            var parts = new List<PathPart>() { start };
            var mapWidth = map.Width;
            var mapHeight = map.Height;

            using var bmpC = new BitmapContext(mapWidth, mapHeight, scalingFactor: 4);
            bmpC.SetSelectedColor(50, 50, 50);
            for (var i = 0; i < map.Length; i++) if (map.AtOffset(i) == Path.Closed) bmpC.SetPixel(i);
            bmpC.SelectedColor = Color.Cyan;
            bmpC.SetPixel(startPos.X, startPos.Y);
            bmpC.ScalingFactor = 1;


            Map2DSpan<bool> closed = new Map2DSpan<bool>(mapWidth, mapHeight);

            while (parts.Any(p => p.PathSearchFinished == false))
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    var part = parts[i];
                    if (part.PathSearchFinished) continue;

                    parts[i].Advance(map, closed, visualize ? bmpC : null);
                    // search ended. attempt to spawn new parts from this one
                    if (part.PathSearchFinished)
                    {
                        if (PathPart.IsValidAt(part.endPos.X + 1, part.endPos.Y, map, closed))
                            parts.Add(new PathPart { startPos = new Point(part.endPos.X + 1, part.endPos.Y), startJointPoint = part.endPos });

                        if (PathPart.IsValidAt(part.endPos.X, part.endPos.Y + 1, map, closed))
                            parts.Add(new PathPart { startPos = new Point(part.endPos.X, part.endPos.Y + 1), startJointPoint = part.endPos });
                    }
                }

                if (visualize)
                    Visualizer.SendBitmap(bmpC, additionalMessage: $"{parts.Count} are in progress");
            }

            parts.ForEach((p, i) => p.Index = i);

            return parts;
        }

        private static List<PathPart> CreateAndAddPathReversers(List<PathPart> original)
        {
            var ret = new List<PathPart>(original);
            foreach (var og in original)
            {
                if (og.Index == 0) continue; // we don't allow path 0 to be traversed back
                if (og.isEndPart) continue; // we do not allow to travel from last path
                ret.Add(new PathPart
                {
                    endPos = og.startJointPoint,
                    startPos = og.endPos,
                    isEndPart = false,
                    Index = og.Index,
                    isReverser = true,
                    length = og.length,
                    startJointPoint = og.endPos,
                });
            }

            return ret;
        }

        private static long Compute(StringSpan input, int width, int height, bool allowBackPaths, bool visualize)
        {
            // look for starting pos. that's the first index of '.', in the top row
            var startingPos = new Point(input.IndexOf('.'), 0);
            var map = new Map2DSpan<Path>(width, height, input, (c) => c switch
            {
                '.' => Path.Open,
                '#' => Path.Closed,
                '>' => Path.SlopeToXPos,
                'v' => Path.SlopeToYPos,
                _ => throw new InvalidDataException("Not a valid character")
            });

            var visited = new Map2DSpan<bool>(width, height);
            visited.AsSpan().Clear();

            var allParts = FindPathParts(map, startingPos, visualize);
            if (allowBackPaths)
            {
                allParts = CreateAndAddPathReversers(allParts);
            }
            var start = allParts.First();
            long maximumFound = 0;

            PriorityQueue<(PathPart part, List<int>), long> workPrio = new();

            Queue<(PathPart part, long sum, List<int>)> work = new();
            //work.Enqueue((start, start.length, new List<int> { start.Index }));
            workPrio.Enqueue((start, new List<int> { start.Index }), -start.length);

            //while (work.Count > 0)
            while (workPrio.TryDequeue(out var pe, out var _sum))
            {
                //var (part, sum, occupied) = work.Dequeue();
                var sum = -_sum;
                var (part, occupied) = pe;

                if (part.isEndPart)
                {
                    maximumFound = Math.Max(maximumFound, sum);
                    //Log.WriteLine($"Found path that ends properly, with steps sum of {sum}");
                    continue;
                }
                var possible = allParts.Where(p => p.startJointPoint == part.endPos && occupied.Contains(p.Index) == false).ToList();
                possible.Sort((a, b) => b.length - a.length);
                foreach (var p in possible)
                {
                    //work.Enqueue((p, sum + p.length, new List<int>(occupied) { p.Index }));
                    workPrio.Enqueue((p, new List<int>(occupied) { p.Index }), -(sum + p.length));
                }
            }
            return maximumFound;
        }


        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(StringSpan input, int width, int height)
        {
            return Compute(input, width, height, allowBackPaths: false, visualize: true);
        }

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int width, int height)
        {
            return Compute(input, width, height, allowBackPaths: true, visualize: false);
        }
    }
}