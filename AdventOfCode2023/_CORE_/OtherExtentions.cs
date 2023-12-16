namespace AdventOfCode2023
{
    public static class OtherExtentions
    {
        public static string ToReadable(this IEnumerable<int>? array, string delimeter = ", ") => array == null ? "<null>" : $"{string.Join(delimeter, array)}";
        public static string ToReadable(this IEnumerable<long>? array, string delimeter = ", ") => array == null ? "<null>" : $"{string.Join(delimeter, array)}";

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


        public static char GetAt(this ReadOnlySpan<char> span, int x, int y, int width, int heigth, out bool outOfBounds)
        {
            if (x < 0 || y < 0 || x >= width || y >= heigth) { outOfBounds = true; return (char)0; }
            outOfBounds = false;
            return span[y * width + x];
        }
        public static byte GetAt(this Span<byte> span, int x, int y, int width, int heigth, out bool outOfBounds)
        {
            if (x < 0 || y < 0 || x >= width || y >= heigth) { outOfBounds = true; return 0; }
            outOfBounds = false;
            return span[y * width + x];
        }
        public static void SetAt(this Span<byte> span, byte value, int x, int y, int width, int heigth, out bool outOfBounds)
        {
            if (x < 0 || y < 0 || x >= width || y >= heigth) { outOfBounds = true; return; }
            outOfBounds = false;
            span[y * width + x] = value;
        }

    }
}