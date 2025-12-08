using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AoC;

[DefaultInput("live")]
public static partial class Solver
{
    public struct Point
    {
        public int X;
        public int Y;
        public int Z;

        public Point(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point(Point other)
        {
            X = other.X;
            Y = other.Y;
            Z = other.Z;
        }

        public long DistanceSquared(Point Other)
        {
            long dx = X - Other.X;
            long dy = Y - Other.Y;
            long dz = Z - Other.Z;
            return dx * dx + dy * dy + dz * dz;
        }

    }
    internal struct Pair : IComparable<Pair>
    {
        public int A;
        public int B;
        public long DistanceSquared;

        public Pair(int indexA, int indexB, long distanceSquared)
        {
            A = indexA;
            B = indexB;
            DistanceSquared = distanceSquared;
        }

        public Pair(Pair other)
        {
            A = other.A;
            B = other.B;
            DistanceSquared = other.DistanceSquared;
        }

        public override string ToString() => $"{DistanceSquared}";

        public int CompareTo(Pair other)
        {
            if (DistanceSquared < other.DistanceSquared) return -1;
            if (DistanceSquared > other.DistanceSquared) return 1;
            if (A < other.A) return -1;
            if (A > other.A) return 1;
            if (B < other.B) return -1;
            if (B > other.B) return 1;
            return 0;
        }

        public static bool operator <(Pair left, Pair right)
        {
            if (left.DistanceSquared < right.DistanceSquared) return true;
            if (left.DistanceSquared > right.DistanceSquared) return false;
            if (left.A < right.A) return true;
            if (left.A > right.A) return false;
            return left.B < right.B;
        }

        public static bool operator >(Pair left, Pair right)
        {
            if (left.DistanceSquared > right.DistanceSquared) return true;
            if (left.DistanceSquared < right.DistanceSquared) return false;
            if (left.A > right.A) return true;
            if (left.A < right.A) return false;
            return left.B > right.B;
        }
    }

    [ExpectedResult("test", 40)]
    [ExpectedResult("live", 42315)]
    public static unsafe long SolvePart1(string FilePath)
    {
        int PairLimit = 1000; // for live
        //int PairLimit = 10; // for test

        var UsedStackMemory = 0;

        var Handle = CreateFileW(FilePath, GenericRead, FileShareRead, IntPtr.Zero, OpenExisting, FileAttributeNormal, IntPtr.Zero);
        if (Handle == InvalidHandleValue) throw new InvalidOperationException("CreateFile failed");
        if (!GetFileSizeEx(Handle, out var FileSizeLong)) throw new InvalidOperationException("GetFileSizeEx failed");
        ReadOnlySpan<byte> FileBuffer = stackalloc byte[(int)FileSizeLong];
        UsedStackMemory += FileBuffer.Length;
        var TotalRead = 0;

        fixed (byte* Pointer = FileBuffer)
        {
            var ptr = Pointer;
            while (true)
            {
                if (!ReadFile(Handle, ptr, FileBuffer.Length - TotalRead, out var Read, IntPtr.Zero)) throw new InvalidOperationException("ReadFile failed");
                if (Read == 0) break;
                TotalRead += Read;
                ptr += Read;
            }
        }
        if (!CloseHandle(Handle)) throw new InvalidOperationException("CloseHandle failed");

        if (TotalRead == 0) throw new InvalidOperationException("Input file is empty");

        // how many boxes do we have?
        var boxesCount = GetLinesCount(FileBuffer);

        // allocate boxes
        Span<Point> points = stackalloc Point[boxesCount];
        UsedStackMemory += points.Length * Unsafe.SizeOf<Point>();

        // read them
        ReadBoxesFromFile(FileBuffer, points);

        // how many pairs will we have?
        var Count = points.Length;
        if (Count == 0) return 0;
        var MaxPairs = (long)Count * (Count - 1) / 2;
        if (MaxPairs < PairLimit) PairLimit = (int)MaxPairs;

        // create pairs and store 1k (or less) of closes ones 
        var BestCount = 0;
        Span<Pair> BestPairs = stackalloc Pair[PairLimit];
        UsedStackMemory += PairLimit * Unsafe.SizeOf<Pair>();

        // build pairs. we do not store all of them
        // just what we need (which is 1k pairs for live)
        for (var i = 0; i < Count; i++)
        {
            for (var j = i + 1; j < Count; j++)
            {
                ref readonly var pi = ref points[i];
                ref readonly var pj = ref points[j];
                var Dx = pi.X - pj.X;
                var Dy = pi.Y - pj.Y;
                var Dz = pi.Z - pj.Z;
                var NewPair = new Pair(i, j, (long)Dx * Dx + (long)Dy * Dy + (long)Dz * Dz);

                if (BestCount < PairLimit)
                {
                    // less than max allowed pairs in buffer, just add new
                    BestPairs[BestCount] = NewPair;
                    BestCount++;
                    // if buffer is full, sort it
                    if (BestCount == PairLimit)
                        MemoryExtensions.Sort(BestPairs);
                }
                else
                {
                    // buffer is full. we must insert new pair at the proper location in the buffer

                    // if new pair is larger (greater distance) than largest stored pair, do nothing
                    if (!(NewPair < BestPairs[PairLimit - 1])) continue;

                    // we must move all pairs larger than new one one place down
                    var InsertIndex = PairLimit - 1;
                    while (InsertIndex > 0 && NewPair < BestPairs[InsertIndex - 1])
                    {
                        BestPairs[InsertIndex] = BestPairs[InsertIndex - 1];
                        InsertIndex--;
                    }
                    // and insert the pair itself
                    BestPairs[InsertIndex] = NewPair;
                }
            }
        }

        var SortedPairs = BestPairs[..BestCount];


        Span<int> Parents = stackalloc int[Count];
        Span<int> Sizes = stackalloc int[Count];
        UsedStackMemory += 2 * Count * Unsafe.SizeOf<int>();

        for (var I = 0; I < Count; I++)
        {
            Parents[I] = I;
            Sizes[I] = 1;
        }
        for (var I = 0; I < SortedPairs.Length; I++)
        {
            var rootA = SortedPairs[I].A;
            while (Parents[rootA] != rootA) rootA = Parents[rootA];

            var rootB = SortedPairs[I].B;
            while (Parents[rootB] != rootB) rootB = Parents[rootB];

            if (rootA == rootB) continue;

            if (Sizes[rootA] < Sizes[rootB])
            {
                var t = rootA;
                rootA = rootB;
                rootB = t;
            }

            Parents[rootB] = rootA;
            Sizes[rootA] += Sizes[rootB];
        }

        Span<int> RootSizes = stackalloc int[Count];
        UsedStackMemory += Count * Unsafe.SizeOf<int>();

        var RootCount = 0;
        for (var I = 0; I < Count; I++)
        {
            if (Parents[I] == I)
            {
                RootSizes[RootCount] = Sizes[I];
                RootCount++;
            }
        }

        var RootSlice = RootSizes.Slice(0, RootCount);
        MemoryExtensions.Sort(RootSlice);

        var Last = RootCount - 1;
        var Result = (long)RootSlice[Last] * RootSlice[Last - 1] * RootSlice[Last - 2];

        //Console.WriteLine($"Used {UsedStackMemory} bytes of memory");
        return Result;
    }

    [ExpectedResult("test", 25272)]
    [ExpectedResult("live", 8079278220)]
    public static unsafe long SolvePart2(string FilePath)
    {
        var UsedStackMemory = 0;

        var Handle = CreateFileW(FilePath, GenericRead, FileShareRead, IntPtr.Zero, OpenExisting, FileAttributeNormal, IntPtr.Zero);
        if (Handle == InvalidHandleValue) throw new InvalidOperationException("CreateFile failed");
        if (!GetFileSizeEx(Handle, out var FileSizeLong)) throw new InvalidOperationException("GetFileSizeEx failed");
        ReadOnlySpan<byte> FileBuffer = stackalloc byte[(int)FileSizeLong];
        UsedStackMemory += FileBuffer.Length;
        var TotalRead = 0;

        fixed (byte* Pointer = FileBuffer)
        {
            var ptr = Pointer;
            while (true)
            {
                if (!ReadFile(Handle, ptr, FileBuffer.Length - TotalRead, out var Read, IntPtr.Zero)) throw new InvalidOperationException("ReadFile failed");
                if (Read == 0) break;
                TotalRead += Read;
                ptr += Read;
            }
        }
        if (!CloseHandle(Handle)) throw new InvalidOperationException("CloseHandle failed");

        if (TotalRead == 0) throw new InvalidOperationException("Input file is empty");

        // how many boxes do we have?
        var boxesCount = GetLinesCount(FileBuffer);

        // allocate boxes
        Span<Point> points = stackalloc Point[boxesCount];
        UsedStackMemory += points.Length * Unsafe.SizeOf<Point>();

        // read them
        ReadBoxesFromFile(FileBuffer, points);

        var Count = points.Length;
        if (Count < 2) return 0;

        // MST data
        Span<bool> IsInMst = stackalloc bool[Count];
        Span<long> BestDistance = stackalloc long[Count];
        Span<int> BestMstNode = stackalloc int[Count];

        UsedStackMemory += Count * (Unsafe.SizeOf<bool>() + Unsafe.SizeOf<int>() + Unsafe.SizeOf<long>());

        for (var I = 0; I < Count; I++)
        {
            IsInMst[I] = false;
            BestDistance[I] = long.MaxValue;
            BestMstNode[I] = -1;
        }
        // seed
        BestDistance[0] = 0;

        var HeaviestSeenEdgeDistance = -1L;
        var HeaviestA = 0;
        var HeaviestB = 0;

        // build MST
        for (var Step = 0; Step < Count; Step++)
        {
            // get next best node
            var bestN = -1;
            var MinDistance = long.MaxValue;
            for (var i = 0; i < Count; i++)
            {
                if (!IsInMst[i] && BestDistance[i] < MinDistance)
                {
                    MinDistance = BestDistance[i];
                    bestN = i;
                }
            }

            // update heaviest seen distance
            IsInMst[bestN] = true;
            var node = BestMstNode[bestN];
            if (node != -1)
            {
                var Distance = BestDistance[bestN];
                if (Distance > HeaviestSeenEdgeDistance)
                {
                    HeaviestSeenEdgeDistance = Distance;
                    HeaviestA = node;
                    HeaviestB = bestN;
                }
            }

            // update distances for nodes not in MST
            ref readonly var PointU = ref points[bestN];
            for (var v = 0; v < Count; v++)
            {
                if (IsInMst[v]) continue;
                ref readonly var PointV = ref points[v];
                var Dx = PointU.X - PointV.X;
                var Dy = PointU.Y - PointV.Y;
                var Dz = PointU.Z - PointV.Z;
                var dist = (long)Dx * Dx + (long)Dy * Dy + (long)Dz * Dz;
                if (dist < BestDistance[v])
                {
                    BestDistance[v] = dist;
                    BestMstNode[v] = bestN;
                }
            }
        }

        return (long)points[HeaviestA].X * points[HeaviestB].X;
    }















    private static void ReadBoxesFromFile(ReadOnlySpan<byte> Buffer, Span<Point> Boxes)
    {
        var num = 0;
        var c = 0;
        var box = 0;
        while (c < Buffer.Length)
        {
            num = 0;
            // read first num
            while (Buffer[c] != ',') num = num * 10 + (Buffer[c++] - '0');
            Boxes[box].X = num;
            c++;

            num = 0;
            // read second num
            while (Buffer[c] != ',') num = num * 10 + (Buffer[c++] - '0');
            Boxes[box].Y = num;
            c++;

            num = 0;
            // read third num
            while (c < Buffer.Length && Buffer[c] != '\n' && Buffer[c] != '\r') num = num * 10 + (Buffer[c++] - '0');
            Boxes[box].Z = num;
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



