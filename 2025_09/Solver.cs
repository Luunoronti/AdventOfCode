using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace AoC;

[DefaultInput("live")]
public static partial class Solver
{
    internal record struct Point(int X, int Y);
    internal readonly record struct PointWithPotential(Point Point, long BestArea);

    internal readonly struct VerticalEdge
    {
        public readonly int X;
        public readonly int Y1;
        public readonly int Y2;
        public VerticalEdge(int x, int y1, int y2)
        {
            X = x; Y1 = y1; Y2 = y2;
        }
    }
    internal readonly struct HorizontalEdge
    {
        public readonly int Y;
        public readonly int X1;
        public readonly int X2;
        public HorizontalEdge(int y, int x1, int x2)
        {
            Y = y; X1 = x1; X2 = x2;
        }
    }

    [ExpectedResult("test", 50)]
    [ExpectedResult("live", 4763509452)]
    public static unsafe long SolvePart1(string FilePath)
    {
        var UsedStackMemory = 0;

        var FileSize = FileIO.GetFileSize(FilePath);
        ReadOnlySpan<byte> Buffer = stackalloc byte[(int)FileSize];
        UsedStackMemory += Buffer.Length;
        FileIO.ReadToBuffer(FilePath, Buffer);

        // how many boxes do we have?
        var Count = FileIO.GetLinesCount(Buffer);

        Span<Point> points = stackalloc Point[Count];
        UsedStackMemory += points.Length * Unsafe.SizeOf<Point>();

        GetPointsFromFile(Buffer, points);

        var maxArea = 0L;
        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                ref readonly var p1 = ref points[i];
                ref readonly var p2 = ref points[j];
                var dx = Math.Abs(p1.X - p2.X) + 1;
                var dy = Math.Abs(p1.Y - p2.Y) + 1;
                var area = (long)dx * dy;
                if (area > maxArea) maxArea = area;
            }
        }

        // 9736 bytes used
        //Console.WriteLine($"P1 UsedStackMemory: {UsedStackMemory}");
        return maxArea;
    }

    [ExpectedResult("test", 24)]
    [ExpectedResult("live", 1516897893)]
    public static unsafe long SolvePart2(string FilePath)
    {
        var UsedStackMemory = 0;

        var FileSize = FileIO.GetFileSize(FilePath);
        ReadOnlySpan<byte> Buffer = stackalloc byte[(int)FileSize];
        UsedStackMemory += Buffer.Length;
        FileIO.ReadToBuffer(FilePath, Buffer);

        // how many boxes do we have?
        var Count = FileIO.GetLinesCount(Buffer);

        Span<Point> Points = stackalloc Point[Count];
        UsedStackMemory += Points.Length * Unsafe.SizeOf<Point>();

        GetPointsFromFile(Buffer, Points);

        // to save on memory, we count all edges.
        // we could just allocate (Count) edges and call it a day, but every byte counts :)
        // and the number of edges should be equal, good place to check for scan bugs
        var VerticalEdgesCount = 0;
        var HorizontalEdgesCount = 0;
        for (var i = 0; i < Count; i++)
        {
            ref readonly var a = ref Points[i];
            ref readonly var b = ref Points[(i + 1) % Count];
            if (a.X == b.X)
            {
                VerticalEdgesCount++;
            }
            else
            {
                HorizontalEdgesCount++;
            }
        }

        // allocate buffers for edges
        Span<int> HorizontalY = stackalloc int[VerticalEdgesCount];
        Span<int> HorizontalX1 = stackalloc int[VerticalEdgesCount];
        Span<int> HorizontalX2 = stackalloc int[VerticalEdgesCount];
        UsedStackMemory += VerticalEdgesCount * 3 * Unsafe.SizeOf<int>();

        Span<int> VerticalX = stackalloc int[HorizontalEdgesCount];
        Span<int> VerticalY1 = stackalloc int[HorizontalEdgesCount];
        Span<int> VerticalY2 = stackalloc int[HorizontalEdgesCount];
        UsedStackMemory += HorizontalEdgesCount * 3 * Unsafe.SizeOf<int>();

        // fill up with data
        // we will also use this loop to establish AABB of the scene
        var MinX = int.MaxValue;
        var MinY = int.MaxValue;
        var MaxX = int.MinValue;
        var MaxY = int.MinValue;

        VerticalEdgesCount = 0;
        HorizontalEdgesCount = 0;
        for (var i = 0; i < Count; i++)
        {
            ref readonly var a = ref Points[i];
            ref readonly var b = ref Points[(i + 1) % Count];

            if (a.X > MaxX) MaxX = a.X;
            if (a.X < MinX) MinX = a.X;
            if (a.Y > MaxY) MaxY = a.Y;
            if (a.Y < MinY) MinY = a.Y;

            if (a.X == b.X)
            {
                var y1 = a.Y < b.Y ? a.Y : b.Y;
                var y2 = a.Y > b.Y ? a.Y : b.Y;
                VerticalX[VerticalEdgesCount] = a.X;
                VerticalY1[VerticalEdgesCount] = y1;
                VerticalY2[VerticalEdgesCount] = y2;
                VerticalEdgesCount++;
            }
            else
            {
                var x1 = a.X < b.X ? a.X : b.X;
                var x2 = a.X > b.X ? a.X : b.X;
                HorizontalY[HorizontalEdgesCount] = a.Y;
                HorizontalX1[HorizontalEdgesCount] = x1;
                HorizontalX2[HorizontalEdgesCount] = x2;
                HorizontalEdgesCount++;
            }
        }

        SortVerticalSoa(VerticalX, VerticalY1, VerticalY2, VerticalEdgesCount);
        SortHorizontalSoa(HorizontalY, HorizontalX1, HorizontalX2, HorizontalEdgesCount);

        // prepare heuristics buffers
        // heuristics are to check
        Span<long> BestArea = stackalloc long[Count];
        Span<int> Order = stackalloc int[Count];
        UsedStackMemory += Count * Unsafe.SizeOf<long>();
        UsedStackMemory += Count * Unsafe.SizeOf<int>();

        for (var i = 0; i < Count; i++)
        {
            ref readonly var p = ref Points[i];
            var Dx1 = p.X - MinX;
            if (Dx1 < 0) Dx1 = -Dx1;
            var Dx2 = p.X - MaxX;
            if (Dx2 < 0) Dx2 = -Dx2;
            var BestDx = Dx1 > Dx2 ? Dx1 : Dx2;

            var Dy1 = p.Y - MinY;
            if (Dy1 < 0) Dy1 = -Dy1;
            var Dy2 = p.Y - MaxY;
            if (Dy2 < 0) Dy2 = -Dy2;
            var BestDy = Dy1 > Dy2 ? Dy1 : Dy2;

            BestArea[i] = (long)(BestDx + 1) * (BestDy + 1);
            Order[i] = i;
        }
        SortOrderByBestAreaDescending(Order, BestArea);

        var maxArea = 0L;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        long Area(int x1, int y1, int x2, int y2) => (long)(x2 - x1 + 1) * (y2 - y1 + 1);

        maxArea = 0;
        var count = Points.Length;
        //for (var i = 0; i < Points.Length - 1; i++)
        for (var ii = 0; ii < count - 1; ii++)
        {
            var i = Order[ii];
            if (BestArea[i] <= maxArea) break;

            ref readonly var a = ref Points[i];
            for (var jj = ii + 1; jj < count; jj++)
            //for (var j = i + 1; j < Points.Length; j++)
            {
                var j = Order[jj];
                if (BestArea[j] <= maxArea) break;
                ref readonly var b = ref Points[j];

                // if same coordinate in X or Y, are is 0
                if (a.X == b.X || a.Y == b.Y) continue;

                // contruct a rectangle from two points (max/max)
                var X1 = a.X < b.X ? a.X : b.X;
                var X2 = a.X > b.X ? a.X : b.X;
                var Y1 = a.Y < b.Y ? a.Y : b.Y;
                var Y2 = a.Y > b.Y ? a.Y : b.Y;

                // compute area first. if it's smaller than max, there is no point in checking edge crossing
                var area = Area(X1, Y1, X2, Y2);
                if (area <= maxArea) continue;

                if (!EdgeCrossesRectInteriorAvx(VerticalX, VerticalY1, VerticalY2, HorizontalY, HorizontalX1, HorizontalX2, X1, Y1, X2, Y2))
                    maxArea = area;
            }
        }
        // 21640 bytes used in total
        //Console.WriteLine($"P2 UsedStackMemory: {UsedStackMemory}");
        return maxArea;
    }

    private static void SortVerticalSoa(Span<int> VerticalX, Span<int> VerticalY1, Span<int> VerticalY2, int Count)
    {
        for (var i = 1; i < Count; i++)
        {
            var x = VerticalX[i];
            var y1 = VerticalY1[i];
            var y2 = VerticalY2[i];
            var j = i - 1;
            while (j >= 0 && VerticalX[j] > x)
            {
                VerticalX[j + 1] = VerticalX[j];
                VerticalY1[j + 1] = VerticalY1[j];
                VerticalY2[j + 1] = VerticalY2[j];
                j--;
            }
            VerticalX[j + 1] = x;
            VerticalY1[j + 1] = y1;
            VerticalY2[j + 1] = y2;
        }
    }

    private static void SortHorizontalSoa(Span<int> HorizontalY, Span<int> HorizontalX1, Span<int> HorizontalX2, int Count)
    {
        for (var i = 1; i < Count; i++)
        {
            var y = HorizontalY[i];
            var x1 = HorizontalX1[i];
            var x2 = HorizontalX2[i];
            var j = i - 1;
            while (j >= 0 && HorizontalY[j] > y)
            {
                HorizontalY[j + 1] = HorizontalY[j];
                HorizontalX1[j + 1] = HorizontalX1[j];
                HorizontalX2[j + 1] = HorizontalX2[j];
                j--;
            }
            HorizontalY[j + 1] = y;
            HorizontalX1[j + 1] = x1;
            HorizontalX2[j + 1] = x2;
        }
    }

    private static void SortOrderByBestAreaDescending(Span<int> Order, ReadOnlySpan<long> BestArea)
    {
        for (var i = 1; i < Order.Length; i++)
        {
            var index = Order[i];
            var area = BestArea[index];
            var j = i - 1;
            while (j >= 0 && BestArea[Order[j]] < area)
            {
                Order[j + 1] = Order[j];
                j--;
            }
            Order[j + 1] = index;
        }
    }

    private static unsafe bool EdgeCrossesRectInteriorAvx(ReadOnlySpan<int> VerticalX, ReadOnlySpan<int> VerticalY1, ReadOnlySpan<int> VerticalY2,
        ReadOnlySpan<int> HorizontalY, ReadOnlySpan<int> HorizontalX1, ReadOnlySpan<int> HorizontalX2,
        int X1, int Y1, int X2, int Y2)
    {
        var verticalC = VerticalX.Length;
        var horizonalC = HorizontalY.Length;

        if (verticalC > 0)
        {
            var sx = X1 + 1;

            var l = 0;
            var h = verticalC;
            while (l < h)
            {
                var m = (l + h) >> 1;
                if (VerticalX[m] < sx) l = m + 1; else h = m;
            }

            var index = l;
            if (Avx2.IsSupported && verticalC - index >= 8)
            {
                fixed (int* ptrX = VerticalX)
                fixed (int* ptrY1 = VerticalY1)
                fixed (int* ptrY2 = VerticalY2)
                {
                    var xv1 = Vector256.Create(X1);
                    var xv2 = Vector256.Create(X2);
                    var yv1 = Vector256.Create(Y1);
                    var yv2 = Vector256.Create(Y2);
                    var i = index;
                    var limit = verticalC - 8;
                    for (; i <= limit; i += 8)
                    {
                        var vX = Avx.LoadVector256(ptrX + i);
                        var vy1 = Avx.LoadVector256(ptrY1 + i);
                        var vy2 = Avx.LoadVector256(ptrY2 + i);
                        var g1 = Avx2.CompareGreaterThan(vX, xv1);
                        var g2 = Avx2.CompareGreaterThan(xv2, vX);
                        var g3 = Avx2.CompareGreaterThan(yv2, vy1);
                        var g4 = Avx2.CompareGreaterThan(vy2, yv1);
                        var g5 = Avx2.And(g1, g2);
                        var g6 = Avx2.And(g3, g4);
                        var g7 = Avx2.And(g5, g6);
                        if (Avx2.MoveMask(g7.AsByte()) != 0) return true;
                    }
                    for (; i < verticalC; i++)
                    {
                        var x = VerticalX[i];
                        if (x <= X1 || x >= X2) continue;
                        if (VerticalY1[i] < Y2 && VerticalY2[i] > Y1) return true;
                    }
                }
            }
            else
            {
                for (var i = index; i < verticalC; i++)
                {
                    var x = VerticalX[i];
                    if (x <= X1 || x >= X2) continue;
                    if (VerticalY1[i] < Y2 && VerticalY2[i] > Y1) return true;
                }
            }
        }

        if (horizonalC > 0)
        {
            var sy = Y1 + 1;
            var lo = 0;
            var hi = horizonalC;
            while (lo < hi)
            {
                var m = (lo + hi) >> 1;
                if (HorizontalY[m] < sy) lo = m + 1; else hi = m;
            }

            var indexY = lo;
            if (Avx2.IsSupported && horizonalC - indexY >= 8)
            {
                fixed (int* ptrY = HorizontalY)
                fixed (int* ptrX1 = HorizontalX1)
                fixed (int* ptrX2 = HorizontalX2)
                {
                    var yv1 = Vector256.Create(Y1);
                    var yv2 = Vector256.Create(Y2);
                    var xv1 = Vector256.Create(X1);
                    var xv2 = Vector256.Create(X2);
                    var i = indexY;
                    var limit = horizonalC - 8;
                    for (; i <= limit; i += 8)
                    {
                        var vY = Avx.LoadVector256(ptrY + i);
                        var vX1 = Avx.LoadVector256(ptrX1 + i);
                        var vX2 = Avx.LoadVector256(ptrX2 + i);

                        var g1 = Avx2.CompareGreaterThan(vY, yv1);    // Y > Y1
                        var g2 = Avx2.CompareGreaterThan(yv2, vY);    // Y2 > Y
                        var g3 = Avx2.CompareGreaterThan(xv2, vX1);   // X2 > X1Edge
                        var g4 = Avx2.CompareGreaterThan(vX2, xv1);   // X2Edge > X1Rect

                        var g5 = Avx2.And(g1, g2);
                        var g6 = Avx2.And(g3, g4);
                        var g7 = Avx2.And(g5, g6);

                        if (Avx2.MoveMask(g7.AsByte()) != 0) return true;
                    }

                    for (; i < horizonalC; i++)
                    {
                        var y = HorizontalY[i];
                        if (y <= Y1 || y >= Y2) continue;
                        if (HorizontalX1[i] < X2 && HorizontalX2[i] > X1) return true;
                    }
                }
            }
            else
            {
                for (var i = indexY; i < horizonalC; i++)
                {
                    var y = HorizontalY[i];
                    if (y <= Y1 || y >= Y2) continue;
                    if (HorizontalX1[i] < X2 && HorizontalX2[i] > X1) return true;
                }
            }


            //var sy = Y1 + 1;
            //var lo = 0;
            //var hi = horizonalC;
            //while (lo < hi)
            //{
            //    var m = (lo + hi) >> 1;
            //    if (HorizontalEdges[m].Y < sy) lo = m + 1; else hi = m;
            //}
            //for (var i = lo; i < horizonalC; i++)
            //{
            //    var e = HorizontalEdges[i];
            //    if (e.Y >= Y2) break;
            //    if (e.X1 < X2 && e.X2 > X1) return true;
            //}
        }

        return false;
    }


    ////////////////////////////////////////////// 
    /// FILE OP
    ////////////////////////////////////////////// 
    private static void GetPointsFromFile(ReadOnlySpan<byte> Buffer, Span<Point> Points)
    {
        var num = 0;
        var c = 0;
        var box = 0;
        while (c < Buffer.Length)
        {
            num = 0;
            // read first num
            while (c < Buffer.Length && Buffer[c] != ',') num = num * 10 + (Buffer[c++] - '0');
            Points[box].X = num;
            c++;

            num = 0;
            // read second num
            while (c < Buffer.Length && Buffer[c] != '\n' && Buffer[c] != '\r') num = num * 10 + (Buffer[c++] - '0');
            Points[box].Y = num;
            while (c < Buffer.Length && (Buffer[c] < '0' || Buffer[c] > '9')) c++;

            box++;
        }

    }
}
