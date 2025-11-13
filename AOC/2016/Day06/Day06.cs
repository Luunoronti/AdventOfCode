
namespace Year2016;

class Day06
{
    private string GetAnswer(PartInput Input, bool sortAscending)
    {
        char[] result = new char[Input.LineWidth];
        for (int i = 0; i < Input.LineWidth; i++)
        {
            // Process each column
            var columnChars = new Dictionary<char, int>();
            foreach (var line in Input.Lines)
            {
                char c = line[i];
                columnChars[c] = columnChars.GetValueOrDefault(c, 0) + 1;
            }
            char mostCommonChar = (sortAscending ? columnChars.OrderBy(kv => kv.Value) : columnChars.OrderByDescending(kv => kv.Value)).First().Key;
            result[i] = mostCommonChar;
        }
        return new string(result);
    }
    public string Part1(PartInput Input)
    {
        return GetAnswer(Input, false);
    }
    public string Part2(PartInput Input)
    {
        return GetAnswer(Input, true);
    }
}
