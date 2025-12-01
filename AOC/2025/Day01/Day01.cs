
namespace Year2025;

class Day01
{
    public int Solution(PartInput Input, bool Part2)
    {
        var position = 50;
        var movesBy0 = 0;
        foreach (var line in Input.Lines)
        {
            if (int.TryParse(line[1..], out var steps))
            {
                if (steps > 100)
                {
                    if (Part2)
                    {
                        movesBy0 += (steps / 100);
                    }

                    steps -= 100 * (steps / 100);
                }

                if (line[0] == 'L')
                {
                    position -= steps;
                    if (position == 0)
                    {
                        movesBy0++;
                    }
                    else if (position < 0)
                    {
                        if (Part2 && position + steps != 0)
                        {
                            movesBy0++;
                        }
                        position = 100 + position;
                    }
                }
                else if (line[0] == 'R')
                {
                    position += steps;
                    if (position == 100)
                    {
                        movesBy0++;
                        position = 0;
                    }
                    else if (position > 100)
                    {
                        if (Part2 && position - steps != 0)
                        {
                            movesBy0++;
                        }
                        position -= 100;
                    }
                }
            }
        }
        return movesBy0;
    }

    public string Part1(PartInput Input) => Solution(Input, false).ToString();
    public string Part2(PartInput Input) => Solution(Input, true).ToString();
}
