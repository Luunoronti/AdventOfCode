
namespace Year2016;

class Day03
{
    private bool IsValidTriangle(int s0, int s1, int s2)
    {
        return s0 < s1 + s2 && s1 < s0 + s2 && s2 < s0 + s1;
    }
    public string Part1(PartInput Input)
    {
        return
           Input.Lines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries))
           .Select(l => new { s0 = int.Parse(l[0]), s1 = int.Parse(l[1]), s2 = int.Parse(l[2]) })
           .Where(s => IsValidTriangle(s.s0, s.s1, s.s2))
           .Count()
           .ToString();
        ;
    }
    public string Part2(PartInput Input)
    {
        var parts = Input.Lines.Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries))
           .Select(l => new int[] { int.Parse(l[0]), int.Parse(l[1]), int.Parse(l[2]) })
           .ToArray();

        var valid = 0;
        for (int i = 0; i < parts.Length; i += 3)
        {
            for(int j = 0; j < 3; j++)
            {
                if (IsValidTriangle(parts[i + 0][j], parts[i + 1][j], parts[i + 2][j]))
                    valid++;
            }
        }

        return valid.ToString();
    }
}
