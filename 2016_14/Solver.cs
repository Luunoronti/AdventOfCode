using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    [ExpectedResult("live", 35186)]
    public static long SolvePart1(string FilePath) => FindKeyIndex("jlmsuwbz"u8, 0);

    [ExpectedResult("live", 22429)]
    public static long SolvePart2(string FilePath) => FindKeyIndex("jlmsuwbz"u8, 2016);


    private static int FindKeyIndex(ReadOnlySpan<byte> salt, int extraRounds)
    {
        const int Window = 1000;
        const int RingSize = Window + 1;

        Span<byte> tripRing = stackalloc byte[RingSize];
        Span<ushort> quintRing = stackalloc ushort[RingSize];
        Span<ushort> quintCounts = stackalloc ushort[16];

        tripRing.Fill(0xFF);

        for (var i = 0; i <= Window; i++) ComputeTripAndQuints(salt, i, extraRounds, out tripRing[i], out quintRing[i]);

        for (var i = 1; i <= Window; i++)
            for (var m = 0; m < 16; m++)
                if (((quintRing[i] >> m) & 1) != 0)
                    quintCounts[m]++;

        var keys = 0;

        for (var i = 0; ; i++)
        {
            var trip = tripRing[i % RingSize];
            if (trip != 0xFF && quintCounts[trip] != 0) { keys++; if (keys == 64) return i; }

            var removeSlot = (i + 1) % RingSize;

            for (var m = 0; m < 16; m++)
                if (((quintRing[removeSlot] >> m) & 1) != 0)
                    quintCounts[m]--;

            var newIndex = i + Window + 1;
            var newSlot = newIndex % RingSize;
            ComputeTripAndQuints(salt, newIndex, extraRounds, out tripRing[newSlot], out quintRing[newSlot]);

            for (var m = 0; m < 16; m++)
                if (((quintRing[newSlot] >> m) & 1) != 0)
                    quintCounts[m]++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ComputeTripAndQuints(ReadOnlySpan<byte> salt, int index, int extraRounds, out byte tripNibble, out ushort quintMask)
    {
        ReadOnlySpan<byte> toLowerSpan = "0123456789abcdef"u8;

        Span<byte> input = stackalloc byte[64]; // salt + max 10 digits
        salt.CopyTo(input);
        var len = salt.Length + WriteDec(index, input.Slice(salt.Length));

        Span<byte> digest = stackalloc byte[16];
        MD5.HashData(input.Slice(0, len), digest);

        if (extraRounds != 0)
        {
            Span<byte> hex = stackalloc byte[32];
            for (var r = 0; r < extraRounds; r++)
            {
                for (var i = 0; i < 16; i++)
                {
                    var b = digest[i];
                    hex[(i << 1) + 0] = toLowerSpan[b >> 4];
                    hex[(i << 1) + 1] = toLowerSpan[b & 15];
                }

                MD5.HashData(hex, digest);
            }
        }

        ScanDigestNibbles(digest, out tripNibble, out quintMask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int WriteDec(int value, Span<byte> dst)
    {
        Span<byte> tmp = stackalloc byte[10];
        var n = 0;
        var v = value;
        do 
        { 
            tmp[n++] = (byte)('0' + (v % 10)); 
            v /= 10; 
        } while (v != 0);

        for (var i = 0; i < n; i++) 
            dst[i] = tmp[n - 1 - i];
        return n;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ScanDigestNibbles(ReadOnlySpan<byte> digest, out byte tripNibble, out ushort quintMask)
    {
        tripNibble = 0xFF;
        quintMask = 0;

        var prev = (byte)0xFF;
        var run = 0;

        for (var i = 0; i < 16; i++)
        {
            var b = digest[i];
            var hi = (byte)(b >> 4);
            var lo = (byte)(b & 15);

            if (hi == prev) run++; else { prev = hi; run = 1; }
            if (run == 3 && tripNibble == 0xFF) tripNibble = hi;
            if (run == 5) quintMask |= (ushort)(1 << hi);

            if (lo == prev) run++; else { prev = lo; run = 1; }
            if (run == 3 && tripNibble == 0xFF) tripNibble = lo;
            if (run == 5) quintMask |= (ushort)(1 << lo);
        }
    }
}
