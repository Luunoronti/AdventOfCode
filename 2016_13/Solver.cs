using System.Numerics;
using System.Runtime.CompilerServices;

namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    //[ExpectedResult("test", 11)]
    [ExpectedResult("live", 86)]
    public static unsafe long SolvePart1(string FilePath)
    {
        var test = FindShortestPath(10, new Vector2(7, 4));
        var live = FindShortestPath(1364, new Vector2(31, 39));

        return live;// test;
    }

    //[ExpectedResult("test", 151)]
    [ExpectedResult("live", 127)]
    public static unsafe long SolvePart2(string FilePath)
    {
        var test = CountReachable(10, 50);
        var live = CountReachable(1364, 50);

        return live;// test;
    }


    private static unsafe long FindShortestPath(int favorite, Vector2 target)
    {
        // create a directions array.
        var directions = stackalloc ByteVec2d[4];
        *(directions) = new ByteVec2d(1, 0);
        *(directions + 1) = new ByteVec2d(-1, 0);
        *(directions + 2) = new ByteVec2d(0, 1);
        *(directions + 3) = new ByteVec2d(0, -1);

        // create queue and visited map for BSF
        var maxW = 128;  // this limit is ok 
        var maxH = 128;
        var maxCells = maxW * maxH;
        Span<ulong> visited = stackalloc ulong[(maxCells + 63) >> 6]; // 1 bit per single cell
        SpanQueue<Node> bsfQ = new(stackalloc Node[maxCells]);

        // initialize
        var startX = 1;
        var startY = 1;
        if (!IsOpen(startX, startY, favorite)) return -1;
        var startIdx = startY * maxW + startX;
        SetVisited(visited, startIdx);
        bsfQ.Enqueue(new Node { x = (short)startY, y = (short)startY, d = 0 });

        // process BSF
        while (bsfQ.Count > 0)
        {
            var n = bsfQ.Dequeue();

            var x = n.x;
            var y = n.y;
            var d = n.d;

            if (x == target.X && y == target.Y) return d;

            for (var i = 0; i < 4; i++)
            {
                var candX = (int)(x + directions[i].x);
                var candY = (int)(y + directions[i].y);

                // check map bounds
                if ((uint)candX >= (uint)maxW || (uint)candY >= (uint)maxH) continue;

                if (!IsOpen(candX, candY, favorite)) continue;

                var idx = candY * maxW + candX;
                if (GetVisited(visited, idx)) continue;

                SetVisited(visited, idx);
                bsfQ.Enqueue(new Node { x = (short)candX, y = (short)candY, d = (short)(d + 1) });
            }
        }
        return -1;
    }
    private static unsafe long CountReachable(int favorite, int maxSteps)
    {
        // create a directions array.
        var directions = stackalloc ByteVec2d[4];
        *(directions) = new ByteVec2d(1, 0);
        *(directions + 1) = new ByteVec2d(-1, 0);
        *(directions + 2) = new ByteVec2d(0, 1);
        *(directions + 3) = new ByteVec2d(0, -1);

        // For part 2 we only care about positions within <= maxSteps from (1,1),
        // so 64x64 is safely enough (1 + 50 fits, plus a bit of margin).
        var maxW = 64;
        var maxH = 64;
        var maxCells = maxW * maxH;
        Span<ulong> visited = stackalloc ulong[(maxCells + 63) >> 6]; // 1 bit per single cell
        SpanQueue<Node> bsfQ = new(stackalloc Node[maxCells]);

        // initialize
        var reachableCount = 1; // Count start cell

        var startX = 1;
        var startY = 1;
        if (!IsOpen(startX, startY, favorite)) return -1;
        var startIdx = startY * maxW + startX;
        SetVisited(visited, startIdx);
        bsfQ.Enqueue(new Node { x = (short)startY, y = (short)startY, d = 0 });

        // process BSF
        while (bsfQ.Count > 0)
        {
            var n = bsfQ.Dequeue();

            var x = n.x;
            var y = n.y;
            var d = n.d;

            if (d == maxSteps) continue;

            for (var i = 0; i < 4; i++)
            {
                var candX = (int)(x + directions[i].x);
                var candY = (int)(y + directions[i].y);

                // check map bounds
                if ((uint)candX >= (uint)maxW || (uint)candY >= (uint)maxH) continue;

                if (!IsOpen(candX, candY, favorite)) continue;

                var idx = candY * maxW + candX;
                if (GetVisited(visited, idx)) continue;

                reachableCount++;
                SetVisited(visited, idx);
                bsfQ.Enqueue(new Node { x = (short)candX, y = (short)candY, d = (short)(d + 1) });
            }
        }
        return reachableCount;
    }

    private struct ByteVec2d
    {
        public sbyte x;
        public sbyte y;

        public ByteVec2d(sbyte x, sbyte y)
        {
            this.x = x;
            this.y = y;
        }
    }
    private struct Node
    {
        public short x;
        public short y;
        public short d;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsOpen(int x, int y, int favorite)
    {
        var v = (ulong)((long)x * x + 3L * x + 2L * x * y + y + (long)y * y + favorite);
        return (BitOperations.PopCount(v) & 1) == 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool GetVisited(Span<ulong> grid, int idx)
    {
        var word = idx >> 6;
        var mask = 1UL << (idx & 63);
        return (grid[word] & mask) != 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetVisited(Span<ulong> grid, int idx)
    {
        var word = idx >> 6;
        var mask = 1UL << (idx & 63);
        grid[word] |= mask;
    }
}
