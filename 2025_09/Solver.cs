using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    internal record struct Point(int X, int Y);

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

        // allocate buffer
        Span<Point> Points = stackalloc Point[Count];
        UsedStackMemory += Points.Length * Unsafe.SizeOf<Point>();

        // read from the file buffer (simple parsing)
        GetPointsFromFile(Buffer, Points);

        // to save on memory, we allocate just as much memory as we need.
        // we could just allocate (Count) edges and call it a day, but every byte counts :)
        // to be a valid input, the number of horizontal edges must be equal to the number of vertical edges
        // and sum must be equal to Points.Length.
        // if this fails, we would get runtime memory exception so t would tell us soon enough :)
        var edgeCount = Points.Length >> 1;
        
        // allocate buffers for edges
        // we keep them SOA (struct over array) for fast and simple Avx load
        // which basically means we don't do array of structs, but arrays of simple types
        Span<int> HorizontalY = stackalloc int[edgeCount];
        Span<int> HorizontalX1 = stackalloc int[edgeCount];
        Span<int> HorizontalX2 = stackalloc int[edgeCount];
        UsedStackMemory += edgeCount * 3 * Unsafe.SizeOf<int>();

        Span<int> VerticalX = stackalloc int[edgeCount];
        Span<int> VerticalY1 = stackalloc int[edgeCount];
        Span<int> VerticalY2 = stackalloc int[edgeCount];
        UsedStackMemory += edgeCount * 3 * Unsafe.SizeOf<int>();

        // fill up with data
        var VerticalEdgesCount = 0;
        var HorizontalEdgesCount = 0;
        for (var i = 0; i < Count; i++)
        {
            ref readonly var a = ref Points[i];
            ref readonly var b = ref Points[(i + 1) % Count];
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

        // also, store 4 longest horizontal and vertical edges
        // end intersect them first


        var maxArea = 0L;
        for (var i = 0; i < Points.Length - 1; i++)
        {
            ref readonly var a = ref Points[i];
            for (var j = i + 1; j < Points.Length; j++)
            {
                ref readonly var b = ref Points[j];

                // if same coordinate in X or Y, area is 0
                if (a.X == b.X || a.Y == b.Y) continue;

                // contruct a rectangle from two points (max/max)
                var X1 = a.X < b.X ? a.X : b.X;
                var X2 = a.X > b.X ? a.X : b.X;
                var Y1 = a.Y < b.Y ? a.Y : b.Y;
                var Y2 = a.Y > b.Y ? a.Y : b.Y;

                // compute area first. if it's smaller than max, there is no point in checking edge crossing
                var area = (long)(X2 - X1 + 1) * (Y2 - Y1 + 1);
                if (area <= maxArea) continue;

                if (!EdgeCrossesRectInteriorAvx(VerticalX, VerticalY1, VerticalY2, HorizontalY, HorizontalX1, HorizontalX2, X1, Y1, X2, Y2))
                    maxArea = area;
            }
        }
        // 15688 bytes used in total
        // Console.WriteLine($"P2 UsedStackMemory: {UsedStackMemory}");
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

    private static unsafe bool EdgeCrossesRectInteriorAvx(ReadOnlySpan<int> VerticalX, ReadOnlySpan<int> VerticalY1, ReadOnlySpan<int> VerticalY2,
        ReadOnlySpan<int> HorizontalY, ReadOnlySpan<int> HorizontalX1, ReadOnlySpan<int> HorizontalX2,
        int X1, int Y1, int X2, int Y2)
    {
        var verticalC = VerticalX.Length;
        var horizonalC = HorizontalY.Length;

        if (verticalC > 0 && Y1 != Y2 ) // vertical edge
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

                        // this is equivalent of 
                        //                                X1 < X && X < X2 && Y1E < Y2 && Y2E > Y1
                        // but done on 8 edges at once
                        // where:
                        //    X: VerticalX[point]  (8 points in one pass)
                        //  Y1E: VerticalY1[point]
                        //  Y2E: VerticalY1[point]
                        //   X1: rectangle X1      (all 8 int are of the same value)
                        //   Y1: rectangle Y1
                        //   X2: rectangle X2
                        //   Y2: rectangle Y2

                        var g1 = Avx2.CompareGreaterThan(vX, xv1);    // X > X1
                        var g2 = Avx2.CompareGreaterThan(xv2, vX);    // X2 > X
                        var g3 = Avx2.CompareGreaterThan(yv2, vy1);   // Y2R > Y1E
                        var g4 = Avx2.CompareGreaterThan(vy2, yv1);   // Y2E > Y1R

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

        if (horizonalC > 0 && X1 != X2) // horizontal edge
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
