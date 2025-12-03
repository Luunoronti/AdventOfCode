
namespace Year2025;

class Day03
{
    public string Part1(PartInput Input)
    {
        return Input.Lines.Select(line =>
        {
            var allDigits = line.ToCharArray().Select(c => (int)(c - '0')).ToList();
            var m1 = allDigits[..^1].Max();
            var max2 = allDigits[(allDigits.IndexOf(m1) + 1)..].Max();
            return (long)m1 * 10 + max2;
        }).Sum().ToString();
    }
    public string Part2(PartInput Input)
    {
        return Input.Lines.Select(line =>
        {
            var allDigits = line.Select(c => c - '0').ToList();
            var startIndex = 0;
            return Enumerable.Range(0, 12).Aggregate(0L, (lineValue, step) =>
            {
                var end = allDigits.Count - (12 - step);
                var sr = allDigits[startIndex..(end + 1)];
                var max = sr.Max();
                var maxIndex = sr.IndexOf(max) + startIndex;
                startIndex = maxIndex + 1;
                return lineValue * 10 + max;
            });
        }).Sum().ToString();
    }

}
