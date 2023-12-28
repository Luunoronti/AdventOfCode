namespace AmaAocHelpers;

public static class OtherExtensions
{
    public static string ToReadable(this IEnumerable<int>? array, string delimiter = ", ") => array == null ? "<null>" : $"{string.Join(delimiter, array)}";
    public static string ToReadable(this IEnumerable<long>? array, string delimiter = ", ") => array == null ? "<null>" : $"{string.Join(delimiter, array)}";

    public static List<T> SortRet<T>(this List<T> list, IComparer<T> comparer)
    {
        list.Sort(comparer);
        return list;
    }

    public static long MultiplyByAll(this IEnumerable<long> enumerable, long start)
    {
        var st = start;
        foreach (var e in enumerable)
            st *= e;
        return st;
    }

    public static T GetLastAndRemove<T>(this List<T> list)
    {
        if (list.Count == 0) return default!;
        var t = list.Last();
        list.RemoveAt(list.Count - 1);
        return t;
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var e in enumerable)
            action?.Invoke(e);
        return enumerable;
    }
    public static IEnumerable<T2> ForEach<T, T2>(this IEnumerable<T> enumerable, Func<T, T2> action)
    {
        foreach (var e in enumerable)
            yield return action.Invoke(e);
    }
    public static IEnumerable<T> SingleAction<T>(this IEnumerable<T> enumerable, Action action)
    {
        action?.Invoke();
        return enumerable;
    }
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
    {
        var count = enumerable.Count();
        for (int i = 0; i < count; i++)
            action?.Invoke(enumerable.ElementAt(i), i);
        return enumerable;
    }


    public static T As<T>(this ReadOnlySpan<char> chars, IFormatProvider? formatProvider = null) where T : ISpanParsable<T> => T.Parse(chars, formatProvider);



    public static T At<T>(this Span<T> span, int x, int y, int width, out bool outOfRange)
    {
        var pos = y * width + x;
        if (pos < 0 || pos >= span.Length)
        {
            outOfRange = true;
            return default!;
        }
        outOfRange = false;
        return span[pos];
    }
    public static T At<T>(this Span<T> span, int x, int y, int width)
    {
        var pos = y * width + x;
        if (pos < 0 || pos >= span.Length) return default!;
        return span[pos];
    }
    public static T At<T>(this Span<T> span, int x, int y, int width, T value, out bool outOfRange)
    {
        var pos = y * width + x;
        if (pos < 0 || pos >= span.Length)
        {
            outOfRange = true;
            return default!;
        }
        outOfRange = false;
        return span[pos] = value;
    }
    public static T At<T>(this Span<T> span, int x, int y, int width, T value)
    {
        var pos = y * width + x;
        if (pos < 0 || pos >= span.Length) return default!;
        return span[pos] = value;
    }

    public static T At<T>(this ReadOnlySpan<T> span, int x, int y, int width, out bool outOfRange)
    {
        var pos = y * width + x;
        if (pos < 0 || pos >= span.Length)
        {
            outOfRange = true;
            return default!;
        }
        outOfRange = false;
        return span[pos];
    }
    public static T At<T>(this ReadOnlySpan<T> span, int x, int y, int width)
    {
        var pos = y * width + x;
        if (pos < 0 || pos >= span.Length) return default!;
        return span[pos];
    }







    public static char GetAt(this ReadOnlySpan<char> span, int x, int y, int width, int height, out bool outOfBounds)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) { outOfBounds = true; return (char)0; }
        outOfBounds = false;
        return span[y * width + x];
    }
    public static byte GetAt(this Span<byte> span, int x, int y, int width, int height, out bool outOfBounds)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) { outOfBounds = true; return 0; }
        outOfBounds = false;
        return span[y * width + x];
    }
    public static void SetAt(this Span<byte> span, byte value, int x, int y, int width, int height, out bool outOfBounds)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) { outOfBounds = true; return; }
        outOfBounds = false;
        span[y * width + x] = value;
    }
    public static int GetAt(this Span<int> span, int x, int y, int width, int height, out bool outOfBounds)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) { outOfBounds = true; return 0; }
        outOfBounds = false;
        return span[y * width + x];
    }
    public static void SetAt(this Span<int> span, int value, int x, int y, int width, int height, out bool outOfBounds)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) { outOfBounds = true; return; }
        outOfBounds = false;
        span[y * width + x] = value;
    }
    public static void SetAt(this Span<int> span, byte a, byte b, byte c, byte d, int x, int y, int width, int height, out bool outOfBounds)
    {
        SetAt(span, (a) | (b << 8) | (c << 16) | (d << 24), x, y, width, height, out outOfBounds);
    }
    public static (byte a, byte b, byte c, byte d) GetBytesAt(this Span<int> span, int x, int y, int width, int height, out bool outOfBounds)
    {
        var v = GetAt(span, x, y, width, height, out outOfBounds);
        if (outOfBounds) return (0, 0, 0, 0);
        return ((byte)(v & 0xff), (byte)((v >> 8) & 0xff), (byte)((v >> 16) & 0xff), (byte)((v >> 24) & 0xff));
    }

}
