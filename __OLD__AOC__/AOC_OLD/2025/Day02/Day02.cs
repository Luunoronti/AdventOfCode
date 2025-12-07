
using Windows.Media.Capture;

namespace Year2025;

class Day02
{
    public string Part1Linq(PartInput Input)
    {
        var invalidIds =
           Input.FullString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Split('-'))
                .Select(parts => (Start: long.Parse(parts[0]), End: long.Parse(parts[1])))
                .SelectMany(range => Enumerable.Range(range.Start.ToString().Length, range.End.ToString().Length - range.Start.ToString().Length + 1)
                    .Where(d => d % 2 == 0)
                    .Select(d => (pow: (long)Math.Pow(10, d / 2), hMin: (long)Math.Pow(10, d / 2 - 1), hMax: (long)Math.Pow(10, d / 2) - 1))
                    .Select(d => (I: d, hStart: Math.Max(d.hMin, (long)Math.Ceiling((double)range.Start / (d.pow + 1))), hEnd: Math.Min(d.hMax, (long)Math.Floor((double)range.End / (d.pow + 1)))))
                    .SelectMany(d => Enumerable.Range((int)d.hStart, (int)(d.hEnd - d.hStart + 1))
                        .Select(H => (long)H * d.I.pow + H)
                        .Where(candidate => candidate >= range.Start && candidate <= range.End)
                    )
                )
                .ToList();

        return invalidIds.Sum().ToString();
    }
    public string Part1(PartInput Input)
    {
        return Part1Linq(Input);

        List<long> invalidIds = [];
        var ranges = Input.FullString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var r in ranges)
        {
            var parts = r.Split('-');
            var start = long.Parse(parts[0]);
            var end = long.Parse(parts[1]);

            var minDigits = start.ToString().Length;
            var maxDigits = end.ToString().Length;

            for (var d = minDigits; d <= maxDigits; d++)
            {
                if (d % 2 != 0) continue;

                var half = d / 2;
                var pow = (long)Math.Pow(10, half);
                var hMin = (long)Math.Pow(10, half - 1);
                var hMax = (long)Math.Pow(10, half) - 1;

                var hStart = Math.Max(hMin, (long)Math.Ceiling((double)start / (pow + 1)));
                var hEnd = Math.Min(hMax, (long)Math.Floor((double)end / (pow + 1)));

                for (var H = hStart; H <= hEnd; H++)
                {
                    var candidate = H * pow + H;
                    if (candidate >= start && candidate <= end)
                    {
                        invalidIds.Add(candidate);
                    }
                }
            }
        }

        return invalidIds.Sum().ToString();
    }




    public string Part2(PartInput Input)
    {
        List<long> invalidIds = [];

        var ranges = Input.FullString.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var r in ranges)
        {
            var parts = r.Split('-');
            var start = long.Parse(parts[0]);
            var end = long.Parse(parts[1]);

            var minDigits = start.ToString().Length;
            var maxDigits = end.ToString().Length;

            // Loop over possible digit lengths
            for (var d = minDigits; d <= maxDigits; d++)
            {
                // Loop over possible block sizes
                for (var block = 1; block <= d / 2; block++)
                {
                    if (d % block != 0) continue;

                    var min = (long)Math.Pow(10, block - 1);
                    var max = (long)Math.Pow(10, block) - 1;

                    for (var H = min; H <= max; H++)
                    {
                        var pattern = H.ToString();
                        var candidateStr = string.Concat(System.Linq.Enumerable.Repeat(pattern, d / block));
                        var candidate = long.Parse(candidateStr);
                        if (candidate >= start && candidate <= end)
                        {
                            if(invalidIds.Contains(candidate) == false)
                                invalidIds.Add(candidate);
                        }
                    }
                }
            }
        }
        return invalidIds.Sum().ToString();
    }
}
