namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    [ExpectedResult("test", 0)]
    public static unsafe long SolvePart1(string FilePath)
    {
        var lines = File.ReadAllLines(FilePath);

        var numOfFits = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (line[1] == ':')
            {
                // shape, skip
                i += 4;
                continue;
            }

            // region
            var region = line.Split(':');
            var dim = region[0].Split('x');
            var presentsReq = region[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            var w = int.Parse(dim[0]);
            var h = int.Parse(dim[1]); 

            // ...
            w /= 3;
            h /= 3;

            var area = w * h;
            var total = presentsReq.Sum(int.Parse);

            if (area >= total)
                numOfFits++;
        }

        return numOfFits;
    }

    [ExpectedResult("test", 0)]
    public static unsafe long SolvePart2(string FilePath)
    {
        // TODO: implement Part 2
        return 0;
    }
}
