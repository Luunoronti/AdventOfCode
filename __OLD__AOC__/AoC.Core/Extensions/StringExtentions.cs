namespace AdventOfCode.Extensions;

public static class StringExtentions
{
    public static string[] SplitTrim(this string @string, char separator) => @string.Split(separator).Select(s => s.Trim()).ToArray();
}
