namespace AdventOfCode2023
{
    public static class OtherExtentions
    {
        public static string ToReadable(this int[]? array) => array == null ? "<null>" : $"{string.Join(", ", array)}";
        public static List<T> SortRet<T>(this List<T> list, IComparer<T> comparer)
        {
            list.Sort(comparer);
            return list;
        }
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var e in enumerable)
                action?.Invoke(e);
            return enumerable;
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
    }
}