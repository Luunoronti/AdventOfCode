using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.HighPerformance;

namespace AoC;

[DefaultInput("live")]
public static partial class Solver
{
    internal record struct Point(int X, int Y);
    internal readonly record struct PointWithPotential(Point Point, long BestArea);

    [ExpectedResult("test", 50)]
    [ExpectedResult("live", 4763509452)]
    public static unsafe long SolvePart1(string FilePath)
    {
        var UsedStackMemory = 0;

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

        // how many boxes do we have?
        var Count = GetLinesCount(Buffer);

        Span<Point> Points = stackalloc Point[Count];
        UsedStackMemory += Points.Length * Unsafe.SizeOf<Point>();

        GetPointsFromFile(Buffer, Points);

        var maxArea = 0L;
        for (var i = 0; i < Points.Length - 1; i++)
        {
            var a = Points[i];
            for (var J = i + 1; J < Points.Length; J++)
            {
                var b = Points[J];

                // if same coordinate in X or Y, are is 0
                if (a.X == b.X || a.Y == b.Y) continue;

                // contruct a rectangle from two points (max/max)
                var X1 = a.X < b.X ? a.X : b.X;
                var X2 = a.X > b.X ? a.X : b.X;
                var Y1 = a.Y < b.Y ? a.Y : b.Y;
                var Y2 = a.Y > b.Y ? a.Y : b.Y;
                var C = new Point(X1, Y1);
                var D = new Point(X1, Y2);
                var E = new Point(X2, Y1);
                var F = new Point(X2, Y2);

                // check if edges of the rect are inside polygon (it does not cross any edges)
                if (EdgeCrossesRectInterior(Points, X1, Y1, X2, Y2)) continue;

                // compute area
                var Area = (long)(X2 - X1 + 1) * (Y2 - Y1 + 1);
                if (Area > maxArea) maxArea = Area;
            }
        }
        // 9736 bytes used in total
        //Console.WriteLine($"P2 UsedStackMemory: {UsedStackMemory}");
        return maxArea;
    }

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
