using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using CommunityToolkit.HighPerformance;

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

        ////////////////////////////////////////////// 
        /// FILE OP
        ////////////////////////////////////////////// 
        var Handle = CreateFileW(FilePath, GenericRead, FileShareRead, IntPtr.Zero, OpenExisting, FileAttributeNormal, IntPtr.Zero);
        if (Handle == InvalidHandleValue) throw new InvalidOperationException("CreateFile failed");
        if (!GetFileSizeEx(Handle, out var FileSizeLong)) throw new InvalidOperationException("GetFileSizeEx failed");
        ReadOnlySpan<byte> Buffer = stackalloc byte[(int)FileSizeLong];
        UsedStackMemory += Buffer.Length;
        var TotalRead = 0;

        fixed (byte* Pointer = Buffer)
        {
            var ptr = Pointer;
            while (true)
            {
                if (!ReadFile(Handle, ptr, Buffer.Length - TotalRead, out var Read, IntPtr.Zero)) throw new InvalidOperationException("ReadFile failed");
                if (Read == 0) break;
                TotalRead += Read;
                ptr += Read;
            }
        }
        if (!CloseHandle(Handle)) throw new InvalidOperationException("CloseHandle failed");
        if (TotalRead == 0) throw new InvalidOperationException("Input file is empty");
        ////////////////////////////////////////////// 
        /// FILE OP
        ////////////////////////////////////////////// 


        // how many boxes do we have?
        var Count = GetLinesCount(Buffer);

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

        ////////////////////////////////////////////// 
        /// FILE OP
        ////////////////////////////////////////////// 
        var Handle = CreateFileW(FilePath, GenericRead, FileShareRead, IntPtr.Zero, OpenExisting, FileAttributeNormal, IntPtr.Zero);
        if (Handle == InvalidHandleValue) throw new InvalidOperationException("CreateFile failed");
        if (!GetFileSizeEx(Handle, out var FileSizeLong)) throw new InvalidOperationException("GetFileSizeEx failed");
        ReadOnlySpan<byte> Buffer = stackalloc byte[(int)FileSizeLong];
        UsedStackMemory += Buffer.Length;
        var TotalRead = 0;

        fixed (byte* Pointer = Buffer)
        {
            var ptr = Pointer;
            while (true)
            {
                if (!ReadFile(Handle, ptr, Buffer.Length - TotalRead, out var Read, IntPtr.Zero)) throw new InvalidOperationException("ReadFile failed");
                if (Read == 0) break;
                TotalRead += Read;
                ptr += Read;
            }
        }
        if (!CloseHandle(Handle)) throw new InvalidOperationException("CloseHandle failed");

        if (TotalRead == 0) throw new InvalidOperationException("Input file is empty");
        ////////////////////////////////////////////// 
        /// FILE OP
        ////////////////////////////////////////////// 


        // how many boxes do we have?
        var Count = GetLinesCount(Buffer);

        Span<Point> Points = stackalloc Point[Count];
        UsedStackMemory += Points.Length * Unsafe.SizeOf<Point>();

        GetPointsFromFile(Buffer, Points);

        // to save on memory, we count all edges.
        // we could just allocate Count edges and call it a day, but every byte counts :)
        // and the number of edges should be equal, but it's almost noop anyway
        var VerticalEdgesCount = 0;
        var HorizontalEdgesCount = 0;
        for (var i = 0; i < Count; i++)
        {
            var a = Points[i];
            var b = Points[(i + 1) % Count];
            if (a.X == b.X)
            {
                VerticalEdgesCount++;
            }
            else
            {
                HorizontalEdgesCount++;
            }
        }

        Span<HorizontalEdge> HorizontalEdges = stackalloc HorizontalEdge[HorizontalEdgesCount];
        UsedStackMemory += HorizontalEdgesCount * Unsafe.SizeOf<HorizontalEdge>();

        //Span<VerticalEdge> VerticalEdges = stackalloc VerticalEdge[VerticalEdgesCount];
        //UsedStackMemory += VerticalEdgesCount * Unsafe.SizeOf<VerticalEdge>();
        Span<int> VerticalX = stackalloc int[Count];
        Span<int> VerticalY1 = stackalloc int[Count];
        Span<int> VerticalY2 = stackalloc int[Count];
        UsedStackMemory += VerticalEdgesCount * 3 * Unsafe.SizeOf<int>();


        VerticalEdgesCount = 0;
        HorizontalEdgesCount = 0;
        for (var i = 0; i < Count; i++)
        {
            var a = Points[i];
            var b = Points[(i + 1) % Count];
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
                HorizontalEdges[HorizontalEdgesCount++] = new HorizontalEdge(a.Y, x1, x2);
            }
        }

        SortVerticalSoa(VerticalX, VerticalY1, VerticalY2, VerticalEdgesCount);
        //SortVerticalEdgesByX(VerticalEdges[..VerticalEdgesCount]);
        SortHorizontalEdgesByY(HorizontalEdges[..HorizontalEdgesCount]);

        // compute AABB of the whole scene
        var MinX = int.MaxValue;
        var MinY = int.MaxValue;
        var MaxX = int.MinValue;
        var MaxY = int.MinValue;

        for (var i = 0; i < Points.Length - 1; i++)
        {
            ref readonly var p = ref Points[i];
            if (p.X > MaxX) MaxX = p.X;
            if (p.X < MinX) MinX = p.X;
            if (p.Y > MaxY) MaxY = p.Y;
            if (p.Y < MinY) MinY = p.Y;
        }

        // prepare heuristics buffers
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

        for (var i = 0; i < Points.Length - 1; i++)
        {
            ref readonly var a = ref Points[i];

            // heuristics
            if (BestArea[Order[i]] <= maxArea) break;

            for (var j = i + 1; j < Points.Length; j++)
            {
                if (BestArea[Order[j]] <= maxArea) break;

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

                var C = new Point(X1, Y1);
                var D = new Point(X1, Y2);
                var E = new Point(X2, Y1);
                var F = new Point(X2, Y2);

                // check if edges of the rect are inside polygon (it does not cross any edges)
                //if (EdgeCrossesRectInterior(Points, X1, Y1, X2, Y2)) continue;
                //if (!EdgeCrossesRectInteriorSorted(VerticalEdges, HorizontalEdges, X1, Y1, X2, Y2))
                if (!EdgeCrossesRectInteriorAvx(VerticalX, VerticalY1, VerticalY2, HorizontalEdges, X1, Y1, X2, Y2))
                
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


    private static void SortVerticalEdgesByX(Span<VerticalEdge> Edges)
    {
        for (var i = 1; i < Edges.Length; i++)
        {
            var Key = Edges[i];
            var j = i - 1;
            while (j >= 0 && Edges[j].X > Key.X)
            {
                Edges[j + 1] = Edges[j];
                j--;
            }
            Edges[j + 1] = Key;
        }
    }
    private static void SortHorizontalEdgesByY(Span<HorizontalEdge> Edges)
    {
        for (var i = 1; i < Edges.Length; i++)
        {
            var Key = Edges[i];
            var j = i - 1;
            while (j >= 0 && Edges[j].Y > Key.Y)
            {
                Edges[j + 1] = Edges[j];
                j--;
            }
            Edges[j + 1] = Key;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool EdgeCrossesRectInterior(ReadOnlySpan<Point> poly, int X1, int Y1, int X2, int Y2)
    {
        for (var i = 0; i < poly.Length; i++)
        {
            var a = poly[i];
            var b = poly[(i + 1) % poly.Length];

            // edge is vertical
            if (a.X == b.X)
            {
                var x = a.X;
                if (x <= X1 || x >= X2) continue;
                var minY = a.Y < b.Y ? a.Y : b.Y;
                var maxY = a.Y > b.Y ? a.Y : b.Y;
                if (minY < Y2 && maxY > Y1) return true;
            }
            else // edge is horizontal
            {
                var y = a.Y;
                if (y <= Y1 || y >= Y2) continue;
                var minX = a.X < b.X ? a.X : b.X;
                var maxX = a.X > b.X ? a.X : b.X;
                if (minX < X2 && maxX > X1) return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool EdgeCrossesRectInteriorSorted(ReadOnlySpan<VerticalEdge> VerticalEdges, ReadOnlySpan<HorizontalEdge> HorizontalEdges, int X1, int Y1, int X2, int Y2)
    {
        if (VerticalEdges.Length > 0)
        {
            var sx = X1 + 1;
            var l = 0;
            var h = VerticalEdges.Length;
            while (l < h)
            {
                var mid = (l + h) >> 1;
                if (VerticalEdges[mid].X < sx) l = mid + 1; else h = mid;
            }

            for (var i = l; i < VerticalEdges.Length; i++)
            {
                var e = VerticalEdges[i];
                if (e.X >= X2) break;
                if (e.Y1 < Y2 && e.Y2 > Y1) return true;
            }
        }

        if (HorizontalEdges.Length > 0)
        {
            var sy = Y1 + 1;
            var l = 0;
            var h = HorizontalEdges.Length;
            while (l < h)
            {
                var mid = (l + h) >> 1;
                if (HorizontalEdges[mid].Y < sy) l = mid + 1; else h = mid;
            }

            for (var i = l; i < HorizontalEdges.Length; i++)
            {
                var E = HorizontalEdges[i];
                if (E.Y >= Y2) break;
                if (E.X1 < X2 && E.X2 > X1) return true;
            }
        }

        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LowerBoundVertical(ReadOnlySpan<int> VerticalX, int Count, int X)
    {
        var l = 0;
        var h = Count;
        while (l < h)
        {
            var m = (l + h) >> 1;
            if (VerticalX[m] < X) l = m + 1; else h = m;
        }
        return l;
    }

    private static unsafe bool EdgeCrossesRectInteriorAvx(ReadOnlySpan<int> VerticalX, ReadOnlySpan<int> VerticalY1, ReadOnlySpan<int> VerticalY2, ReadOnlySpan<HorizontalEdge> HorizontalEdges, int X1, int Y1, int X2, int Y2)
    {
        var verticalC = VerticalX.Length;
        var horizonalC = HorizontalEdges.Length;

        if (verticalC > 0)
        {
            var StartX = X1 + 1;
            var Index = LowerBoundVertical(VerticalX, verticalC, StartX);
            if (Avx2.IsSupported && verticalC - Index >= 8)
            {
                fixed (int* PtrX = VerticalX)
                fixed (int* PtrY1 = VerticalY1)
                fixed (int* PtrY2 = VerticalY2)
                {
                    var X1Vec = Vector256.Create(X1);
                    var X2Vec = Vector256.Create(X2);
                    var Y1Vec = Vector256.Create(Y1);
                    var Y2Vec = Vector256.Create(Y2);
                    var I = Index;
                    var Limit = verticalC - 8;
                    for (; I <= Limit; I += 8)
                    {
                        var VX = Avx.LoadVector256(PtrX + I);
                        var VY1 = Avx.LoadVector256(PtrY1 + I);
                        var VY2 = Avx.LoadVector256(PtrY2 + I);
                        var C1 = Avx2.CompareGreaterThan(VX, X1Vec);
                        var C2 = Avx2.CompareGreaterThan(X2Vec, VX);
                        var C3 = Avx2.CompareGreaterThan(Y2Vec, VY1);
                        var C4 = Avx2.CompareGreaterThan(VY2, Y1Vec);
                        var M1 = Avx2.And(C1, C2);
                        var M2 = Avx2.And(C3, C4);
                        var M = Avx2.And(M1, M2);
                        if (Avx2.MoveMask(M.AsByte()) != 0) return true;
                    }
                    for (; I < verticalC; I++)
                    {
                        var X = VerticalX[I];
                        if (X <= X1 || X >= X2) continue;
                        var EdgeY1 = VerticalY1[I];
                        var EdgeY2 = VerticalY2[I];
                        if (EdgeY1 < Y2 && EdgeY2 > Y1) return true;
                    }
                }
            }
            else
            {
                for (var I = Index; I < verticalC; I++)
                {
                    var X = VerticalX[I];
                    if (X <= X1 || X >= X2) continue;
                    var EdgeY1 = VerticalY1[I];
                    var EdgeY2 = VerticalY2[I];
                    if (EdgeY1 < Y2 && EdgeY2 > Y1) return true;
                }
            }
        }

        if (horizonalC > 0)
        {
            var StartY = Y1 + 1;
            var Lo = 0;
            var Hi = horizonalC;
            while (Lo < Hi)
            {
                var Mid = (Lo + Hi) >> 1;
                if (HorizontalEdges[Mid].Y < StartY) Lo = Mid + 1; else Hi = Mid;
            }
            for (var I = Lo; I < horizonalC; I++)
            {
                var E = HorizontalEdges[I];
                if (E.Y >= Y2) break;
                if (E.X1 < X2 && E.X2 > X1) return true;
            }
        }

        return false;
    }





    private static void SortOrderByBestAreaDescending(Span<int> Order, Span<long> BestArea)
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
    private static int GetLinesCount(ReadOnlySpan<byte> Buffer)
    {
        var boxesCount = 0;
        for (var c = 0; c < Buffer.Length; c++)
            if (Buffer[c] == '\n') boxesCount++;
        if (Buffer[^1] == '\n' || Buffer[^1] == '\r')
            return boxesCount;
        return boxesCount + 1;
    }

    private const uint GenericRead = 0x80000000;
    private const uint FileShareRead = 0x00000001;
    private const uint OpenExisting = 3;
    private const uint FileAttributeNormal = 0x80;
    private static readonly IntPtr InvalidHandleValue = new(-1);

    [LibraryImport("kernel32.dll", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    private static partial IntPtr CreateFileW(string FileName, uint DesiredAccess, uint ShareMode, IntPtr SecurityAttributes, uint CreationDisposition, uint FlagsAndAttributes, IntPtr TemplateFile);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static unsafe partial bool ReadFile(IntPtr FileHandle, byte* Buffer, int NumberOfBytesToRead, out int NumberOfBytesRead, IntPtr Overlapped);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static unsafe partial bool GetFileSizeEx(IntPtr FileHandle, out long FileSizeLong);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(IntPtr Handle);
}
