namespace AdventOfCode2023
{
    public static class StringExtentions
    {
        private static readonly Dictionary<(string, char, bool, bool), string[]> _splitCache = new();
        private static readonly Dictionary<(string, char, int, bool, bool), int> _splitCacheInts = new();

        private static readonly Dictionary<(string, char, bool, bool), long[]> _splitCacheArrayOfLongs = new();
        public static long[] SplitAsArrayOfLongs(this string str, char splitChar, bool removeEmpty = true, bool trim = true)
        {
            var co = (str, splitChar, removeEmpty, trim);
            if (_splitCacheArrayOfLongs.TryGetValue(co, out var ret)) return ret;

            var opt = StringSplitOptions.None;
            if (removeEmpty) opt |= StringSplitOptions.RemoveEmptyEntries;
            if (trim) opt |= StringSplitOptions.TrimEntries;

            var split = str.Split(splitChar, opt);
            try
            {
                ret = split.Select(s => long.Parse(s)).ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Split to long[] failed. Split char: {splitChar}, String: {str}. Error: {ex.Message}");
            }
            _splitCacheArrayOfLongs[co] = ret;
            return ret;
        }

        public static int SplitAtAsInt(this string str, char splitChar, int index, bool removeEmpty = true, bool trim = true)
        {
            var co = (str, splitChar, index, removeEmpty, trim);
            if (_splitCacheInts.TryGetValue(co, out var ret)) return ret;
            if (int.TryParse(str.SplitAtAsString(splitChar, index, removeEmpty, trim), out ret) == false)
            {
                throw new InvalidDataException($"Split int failed. Split char: {splitChar}, requested index: {index}, String: {str}");
            }
            _splitCacheInts[co] = ret;
            return ret;
        }
        public static string SplitAtAsString(this string str, char splitChar, int index, bool removeEmpty = true, bool trim = true)
        {
            var co = (str, splitChar, removeEmpty, trim);
            if (_splitCache.TryGetValue(co, out var ret))
            {
                if (ret.Length <= index) throw new InvalidDataException($"Split array is too small. Split char: {splitChar}, requested index: {index}, split size: {ret.Length}, String: {str}");
                return ret[index];
            }

            var opt = StringSplitOptions.None;
            if (removeEmpty) opt |= StringSplitOptions.RemoveEmptyEntries;
            if (trim) opt |= StringSplitOptions.TrimEntries;

            var sp = str.Split(new char[] { splitChar }, opt) ?? throw new NullReferenceException($"Split array is null. Split char: {splitChar}, String: {str}");
            if (sp.Length <= index) throw new InvalidDataException($"Split array is too small. Split char: {splitChar}, requested index: {index}, split size: {sp.Length}, String: {str}");

            _splitCache[co] = sp;

            return sp[index];
        }
    }
}