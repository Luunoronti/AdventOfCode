#define DRAWMAPENABLED
#define SHOW_HIT_MISS_ON_MAP

using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2023
{

    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    [AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(102)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(94)] // if != 0, will report failure if expected answer != given answer
    class Day17
    {
        private const int DrawMapDelay = 5;

        enum Direction : byte
        {
            Xnegative = 0,
            Ynegative = 1,
            Xpositive = 2,
            Ypositive = 3
        }


        struct Node
        {
            public Node(int x, int y, Direction direction, int dirMoves)
            {
                this.x = x;
                this.y = y;
                this.direction = direction;
                this.dirMoves = dirMoves;
            }
            public int x;
            public int y;
            public Direction direction;
            public int dirMoves;
        }
        static int TraverseAma(StringSpan input, int width, int height, int minSteps, int maxSteps)
        {
            Map2DSpan<int> inputHeats = new(width, height, input, (c) => (c - '0'));

            // two 4-dimensional arrays, one for heats and one for visit flag
            var heatsMap = new int[width * height * 4 * maxSteps]; // 4 is number of possible directions
            var visitMap = new bool[width * height * 4 * maxSteps];

            var heats = heatsMap.AsSpan();
            var visit = visitMap.AsSpan();

            //TODO: for now let's do a PQ, but maybe I can think of something better (more GC friendly) later
            var queue = new PriorityQueue<Node, long>();

            // we try to add nodes on both directions here



            // start with two possible directions, right and down
            queue.Enqueue(new Node(0, 0, Direction.Xpositive, 0), 0);
            queue.Enqueue(new Node(0, 0, Direction.Ypositive, 0), 0);


            while (queue.TryDequeue(out var node, out var priority))
            {
                if (visit[MakeOffset(node, width, maxSteps)])
                    continue;  // this node (with this configuration) has already been visited

                var heat = heats[MakeOffset(node, width, maxSteps)];

                // mark this node as visited
                visit[MakeOffset(node, width, maxSteps)] = true;

                // move "forward" for max number of allowed steps
                for (int steps = 0; steps < maxSteps; steps++)
                {
                    var ny = node.y + steps * (-2 + (int)node.direction);
                    var nx = node.x + steps * (-1 + (int)node.direction);

                    var newNode = new Node(nx, ny, node.direction, steps);
                    TryAddNode(newNode, heat, queue, inputHeats, heats, visit, width, height, maxSteps);
                }
                
            }
            return 0;


            // this would be the method declaration if we were to follow DRY.
            // I'd argue this is not as nice, the amount of parameters is kinda huge
            // and we need all of them. some just cause captures, some can't be members or globals (spans)
        }


        private static void TryAddNode(Node node, int heat, PriorityQueue<Node, long> queue, Map2DSpan<int> inputHeats, Span<int> heats, Span<bool> visit, int width, int heigth, int maxSteps)
        {
            if (node.x < 0 || node.x >= width || node.y < 0 || node.y >= heigth)
                return;

            var offset = MakeOffset(node, width, maxSteps);

            if (visit[offset])
                return; // this node has already been visited, we do not need to queue and waste time

            heat += inputHeats.At(node.x, node.y);
            heats[offset] = heat;
            queue.Enqueue(node, heat);
        }


        private static int MakeOffset(Node node, int width, int maxSteps) => (node.y * width * 4 * maxSteps) + (node.x * 4 * maxSteps) + ((int)node.direction * maxSteps) + node.dirMoves;
        private static int MakeOffset(int x, int y, Direction direction, int steps, int width, int maxSteps) => (y * width * 4 * maxSteps) + (x * 4 * maxSteps) + ((int)direction * maxSteps) + steps;





#if DRAWMAPENABLED
#if SHOW_HIT_MISS_ON_MAP
        private static (int r, int g, int b) StepStarvationClr = (0, 255, 255);
        private static string StepStarvationClrStr = CC.BgRGB(StepStarvationClr.r, StepStarvationClr.g, StepStarvationClr.b);
        private static (int r, int g, int b) HeatTooHighClr = (255, 10, 10);
        private static string HeatTooHighClrStr = CC.BgRGB(HeatTooHighClr.r, HeatTooHighClr.g, HeatTooHighClr.b);
        private static (int r, int g, int b) CurrCellClr = (255, 255, 255);
        private static string CurrCellClrStr = CC.BgRGB(CurrCellClr.r, CurrCellClr.g, CurrCellClr.b);
        private static (int r, int g, int b) CellQueuedClr = (10, 255, 10);
        private static string CellQueuedClrStr = CC.BgRGB(CellQueuedClr.r, CellQueuedClr.g, CellQueuedClr.b);
#else
        private static (int r, int g, int b) PositiveXClr = (240, 20, 30);
        private static string PositiveXClrStr = CC.BgRGB(PositiveXClr.r, PositiveXClr.g, PositiveXClr.b);

        private static (int r, int g, int b) PositiveYClr = (240, 240, 30);
        private static string PositiveYClrStr = CC.BgRGB(PositiveYClr.r, PositiveYClr.g, PositiveYClr.b);

        private static (int r, int g, int b) NegativeXClr = (30, 30, 240);
        private static string NegativeXClrStr = CC.BgRGB(NegativeXClr.r, NegativeXClr.g, NegativeXClr.b);

        private static (int r, int g, int b) NegativeYClr = (20, 240, 240);
        private static string NegativeYClrStr = CC.BgRGB(NegativeYClr.r, NegativeYClr.g, NegativeYClr.b);


#endif

#endif
        [RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(StringSpan input, int lineWidth, int count) => Traverse(input, lineWidth, count, 1, 3);

        [RemoveSpacesFromInput]
        [RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int lineWidth, int count) => Traverse(input, lineWidth, count, 4, 10);

        static int Traverse(StringSpan input, int width, int height, int minSteps, int maxSteps)
        {
#if DRAWMAPENABLED
            Log.WriteLine("Press any key to continue...");
            Console.ReadLine();
            Console.Clear();
#if SHOW_HIT_MISS_ON_MAP
            Log.WriteLine($"{CurrCellClrStr} {CC.Clr} - Current cell");
            Log.WriteLine($"{StepStarvationClrStr} {CC.Clr} - Step starvation");
            Log.WriteLine($"{HeatTooHighClrStr} {CC.Clr} - Heat too high (power greater than cell)");
            Log.WriteLine($"{CellQueuedClrStr} {CC.Clr} - Cell queued");
#else
            Log.WriteLine($"{PositiveXClrStr} {CC.Clr} - Positive X (Right - East)");
            Log.WriteLine($"{PositiveYClrStr} {CC.Clr} - Positive Y (Down - South)");
            Log.WriteLine($"{NegativeXClrStr} {CC.Clr} - Negative X (Left - West");
            Log.WriteLine($"{NegativeYClrStr} {CC.Clr} - Negative Y (Up - North)");
#endif
            Log.WriteLine();

            var mapDrawer = Log.CreateRectangularMapContext(width, height);
            mapDrawer.Init(DrawMapDelay);
            mapDrawer.SetBackgroundPostProcess((@in, _) => (r: (byte)Math.Max(0, @in.r - 5), g: (byte)Math.Max(0, @in.g - 5), b: (byte)Math.Max(0, @in.b - 5)));
            mapDrawer.SetContent(input, replaceDots: true);
            mapDrawer.FillForegroundColor(30, 30, 30);
            mapDrawer.FillBackgroundColor(0, 0, 0);
#endif

            Map2DSpan<int> map = new(width, height, input, (c) => (c - '0'));
            PriorityQueue<(int y, int x, Direction direction, int directionMoves), int> queue = new();
            PriorityQueue<(int y, int x, Direction direction, int directionMoves), int> queue2 = new();
            Dictionary<(Direction, int), int>[][] visited = new Dictionary<(Direction, int), int>[map.Height][];
            for (var y = 0; y < map.Height; y++)
            {
                visited[y] = new Dictionary<(Direction, int), int>[map.Width];
                for (var x = 0; x < map.Width; x++)
                    visited[y][x] = new Dictionary<(Direction, int), int>();
            }

            queue.Enqueue((0, 0, Direction.Xpositive, 0), 0);
            queue.Enqueue((0, 0, Direction.Ypositive, 0), 0);

            while (queue.Count > 0)
            {
                var (y, x, direction, directionMoves) = queue.Dequeue();

#if DRAWMAPENABLED && SHOW_HIT_MISS_ON_MAP
                mapDrawer?.SetBackgroundColor(x, y, (byte)CurrCellClr.r, (byte)CurrCellClr.g, (byte)CurrCellClr.b);
#endif

#if DRAWMAPENABLED && !SHOW_HIT_MISS_ON_MAP

                var clr = direction switch
                {
                    Direction.Xpositive => PositiveXClr,
                    Direction.Xnegative => NegativeXClr,
                    Direction.Ynegative => NegativeYClr,
                    Direction.Ypositive => PositiveYClr,
                };
                mapDrawer.SetBackgroundColor(x, y, (byte)clr.r, (byte)clr.g, (byte)clr.b);
#endif
                var heat = visited[y][x].GetValueOrDefault((direction, directionMoves));

                if (directionMoves < maxSteps)
                    Move(y, x, direction, heat, directionMoves, mapDrawer);

                if (directionMoves >= minSteps)
                {
                    var left = direction switch { Direction.Ynegative => Direction.Xnegative, Direction.Xnegative => Direction.Ypositive, Direction.Ypositive => Direction.Xpositive, Direction.Xpositive => Direction.Ynegative, };
                    var right = direction switch { Direction.Ynegative => Direction.Xpositive, Direction.Xpositive => Direction.Ypositive, Direction.Ypositive => Direction.Xnegative, Direction.Xnegative => Direction.Ynegative, };

                    Move(y, x, left, heat, 0, mapDrawer);
                    Move(y, x, right, heat, 0, mapDrawer);
                }

#if DRAWMAPENABLED
                mapDrawer.DrawAndWait();
#endif

            }

            var maxY = map.Height - 1;
            var maxX = map.Width - 1;

#if DRAWMAPENABLED
            for (int i = 0; i < 60; i++) mapDrawer.DrawAndWait();
            mapDrawer.Close();
#endif
            return visited[maxY][maxX].Min(x => x.Value);

            void Move(int y, int x, Direction direction, int heat, int directionMoves, Log.MapContext mapDrawer = null)
            {
                var dy = direction switch
                {
                    Direction.Ynegative => -1,
                    Direction.Ypositive => 1,
                    _ => 0
                };

                var dx = direction switch
                {
                    Direction.Xpositive => 1,
                    Direction.Xnegative => -1,
                    _ => 0
                };

                for (var i = 1; i <= maxSteps; i++)
                {
                    var newY = y + i * dy;
                    var newX = x + i * dx;
                    var newDirectionMoves = directionMoves + i;

                    if (newY < 0 || newY >= map.Height || newX < 0 || newX >= map.Width || newDirectionMoves > maxSteps)
                    {
                        return;
                    }

                    heat += map.At(newX, newY);// map[newY][newX];

                    if (i < minSteps)
                    {
#if DRAWMAPENABLED && SHOW_HIT_MISS_ON_MAP
                        mapDrawer?.SetBackgroundColor(newX, newY, (byte)StepStarvationClr.r, (byte)StepStarvationClr.g, (byte)StepStarvationClr.b);
#endif
                        continue;
                    }

                    var vlist = visited[newY][newX];

                    if (vlist.TryGetValue((direction, newDirectionMoves), out var visitedHeat))
                    {
                        if (visitedHeat <= heat)
                        {
#if DRAWMAPENABLED && SHOW_HIT_MISS_ON_MAP
                            mapDrawer?.SetBackgroundColor(newX, newY, (byte)HeatTooHighClr.r, (byte)HeatTooHighClr.g, (byte)HeatTooHighClr.b);
#endif                            
                            return;
                        }
                    }

#if DRAWMAPENABLED && SHOW_HIT_MISS_ON_MAP
                    mapDrawer?.SetBackgroundColor(newX, newY, (byte)CellQueuedClr.r, (byte)CellQueuedClr.g, (byte)CellQueuedClr.b);
#endif
                    queue.Enqueue((newY, newX, direction, newDirectionMoves), heat);
                    vlist[(direction, newDirectionMoves)] = heat;
                }
            }


        }
    }
}