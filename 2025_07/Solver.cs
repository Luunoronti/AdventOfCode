namespace AoC;

using System;
using System.Runtime.InteropServices;

[DefaultInput("live")]
public static class Solver
{
    const uint GenericRead = 0x80000000;
    const uint FileShareRead = 0x00000001;
    const uint OpenExisting = 3;
    const uint FileAttributeNormal = 0x80;
    static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern IntPtr CreateFileW(string FileName, uint DesiredAccess, uint ShareMode, IntPtr SecurityAttributes, uint CreationDisposition, uint FlagsAndAttributes, IntPtr TemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern unsafe bool ReadFile(IntPtr FileHandle, byte* Buffer, int NumberOfBytesToRead, out int NumberOfBytesRead, IntPtr Overlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool CloseHandle(IntPtr Handle);


    public static (int Width, int FullWidth, int Height) GetRectangularBufferDimensions(ReadOnlySpan<byte> Data)
    {
        if (Data.Length == 0) return (0, 0, 0);
        var NewLineIndex = Data.IndexOf((byte)'\n');
        if (NewLineIndex < 0) return (Data.Length, Data.Length, 1);
        var HasCarriageReturn = NewLineIndex > 0 && Data[NewLineIndex - 1] == (byte)'\r';
        var Width = HasCarriageReturn ? NewLineIndex - 1 : NewLineIndex;
        var FullWidth = Width + (HasCarriageReturn ? 2 : 1);
        var Length = Data.Length;
        if (Length % FullWidth == 0)
        {
            var Height = Length / FullWidth;
            return (Width, FullWidth, Height);
        }
        var Numerator = Length - Width;
        if (Numerator <= 0 || Numerator % FullWidth != 0) throw new InvalidOperationException("Input is not rectangular");
        var HeightWithLastShort = Numerator / FullWidth + 1;
        return (Width, FullWidth, HeightWithLastShort);
    }

    const byte StartLocMarker = (byte)'S';

    [ExpectedResult("test", 21)]
    [ExpectedResult("live", 1703)]
    public static unsafe long SolvePart1(string FilePath)
    {
        var Handle = CreateFileW(FilePath, GenericRead, FileShareRead, IntPtr.Zero, OpenExisting, FileAttributeNormal, IntPtr.Zero);
        if (Handle == InvalidHandleValue) throw new InvalidOperationException("CreateFile failed");
        ReadOnlySpan<byte> Buffer = stackalloc byte[64 * 1024];
        var TotalRead = 0;

        // this routine is simple, because we can fit the whole file into the buffer
        // should this be to big for stack, we would have to process the file line by line
        // or chunk by chunk
        fixed (byte* Pointer = Buffer)
        {
            while (true)
            {
                if (!ReadFile(Handle, Pointer, Buffer.Length, out var Read, IntPtr.Zero)) throw new InvalidOperationException("ReadFile failed");
                if (Read == 0) break;
                TotalRead += Read;
            }
        }
        if (!CloseHandle(Handle)) throw new InvalidOperationException("CloseHandle failed");

        if (TotalRead == 0) throw new InvalidOperationException("Input file is empty");

        (var width, var fullWidth, var height) = GetRectangularBufferDimensions(Buffer[..TotalRead]);

        var startColumn = 0;
        while (startColumn < height && Buffer[startColumn++] != StartLocMarker) ;
        startColumn--;

        Span<byte> active = stackalloc byte[width];
        Span<byte> next = stackalloc byte[width];
        var splits = 0L;

        active.Clear();
        next.Clear();

        active[startColumn] = 1;

        for (var row = 2; row < height; row += 2)
        {
            next.Clear();
            var line = Buffer[(row * fullWidth)..(width + (row * fullWidth))];

            for (var col = 0; col < width; col++)
            {
                // there is no beam in this cell, so we have nothing to do
                if (active[col] == 0)
                    continue;

                // no splitter here, so we just let the beam travel
                if (line[col] != '^')
                {
                    next[col] = 1;
                    continue;
                }

                // there is a split. record it
                splits++;

                // and spawn two new beams, to the left and right
                if (col > 0) next[col - 1] = 1;
                if (col + 1 < width) next[col + 1] = 1;
            }
            var temp = active;
            active = next;
            next = temp;
        }
        return splits;
    }

    [ExpectedResult("test", 40)]
    [ExpectedResult("live", 171692855075500)]
    public static unsafe long SolvePart2(string FilePath)
    {
        var Handle = CreateFileW(FilePath, GenericRead, FileShareRead, IntPtr.Zero, OpenExisting, FileAttributeNormal, IntPtr.Zero);
        if (Handle == InvalidHandleValue) throw new InvalidOperationException("CreateFile failed");
        ReadOnlySpan<byte> Buffer = stackalloc byte[64 * 1024];
        var TotalRead = 0;

        // this routine is simple, because we can fit the whole file into the buffer
        // should this be to big for stack, we would have to process the file line by line
        // or chunk by chunk
        fixed (byte* Pointer = Buffer)
        {
            while (true)
            {
                if (!ReadFile(Handle, Pointer, Buffer.Length, out var Read, IntPtr.Zero)) throw new InvalidOperationException("ReadFile failed");
                if (Read == 0) break;
                TotalRead += Read;
            }
        }
        if (!CloseHandle(Handle)) throw new InvalidOperationException("CloseHandle failed");

        if (TotalRead == 0) throw new InvalidOperationException("Input file is empty");

        (var width, var fullWidth, var height) = GetRectangularBufferDimensions(Buffer[..TotalRead]);

        var startColumn = 0;
        while (startColumn < height && Buffer[startColumn++] != StartLocMarker) ;
        startColumn--;

        Span<long> active = stackalloc long[width];
        Span<long> next = stackalloc long[width];
        var completedTimelines = 0L;

        active.Clear();
        next.Clear();
        active[startColumn] = 1;

        for (var row = 2; row < height; row += 2)
        {
            next.Clear();
            var line = Buffer[(row * fullWidth)..(width + (row * fullWidth))];

            for (var col = 0; col < width; col++)
            {
                var count = active[col];
                // there is no beam in this cell, so we have nothing to do
                if (count == 0)
                    continue;

                // no splitter here, so we just let the beam travel
                if (line[col] != '^')
                {
                    next[col] += count;
                    continue;
                }

                if (col > 0) next[col - 1] += count;
                else completedTimelines += count;

                if (col + 1 < width) next[col + 1] += count;
                else completedTimelines += count;
            }
            var temp = active;
            active = next;
            next = temp;
        }
        for (var col = 0; col < width; col++)
            completedTimelines += active[col];

        return completedTimelines;


        //var height = Lines.Length;
        //var width = Lines[0].Length;

        //var startColumn = 0;

        //var line = Lines[0].AsSpan();

        //while (startColumn < height && line[startColumn++] != 'S') ;
        //startColumn--;

        //Span<long> active = stackalloc long[width];
        //Span<long> next = stackalloc long[width];

        //var completedTimelines = 0L;

        //active.Clear();
        //active[startColumn] = 1;

        //for (var row = 1; row < height; row++)
        //{
        //    next.Clear();
        //    line = Lines[row].AsSpan();

        //    for (var col = 0; col < width; col++)
        //    {
        //        var count = active[col];
        //        if (count == 0) continue;
        //        if (line[col] != '^')
        //        {
        //            next[col] += count;
        //            continue;
        //        }
        //        if (col > 0) next[col - 1] += count;
        //        else completedTimelines += count;

        //        if (col + 1 < width) next[col + 1] += count;
        //        else completedTimelines += count;
        //    }

        //    var temp = active;
        //    active = next;
        //    next = temp;
        //}

        //for (var col = 0; col < width; col++)
        //    completedTimelines += active[col];

        //return completedTimelines;

        // return 0;
    }
}

/*
 

*/