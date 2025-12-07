
namespace Year2025;

class Day01
{
    struct Step
    {
        public char Direction;
        public int Distance;
    }
    static List<Step> Steps = [];

    public int Solution(PartInput Input, bool Part2)
    {
        if(Input.Lines.Length != Steps.Count)
        {
            Steps = [.. Input.Lines.Select(l => new Step { Direction = l[0], Distance = int.Parse(l[1..]) })];
        }
        var position = 50;
        var movesBy0 = 0;
        foreach (var step in Steps)
        {
            var steps = step.Distance;
            var direction = step.Direction;
            if (Part2)
            {
                movesBy0 += (steps / 100);
            }
            steps -= 100 * (steps / 100);

            if (direction == 'L')
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
            else if (direction == 'R')
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
        return movesBy0;
    }

    public string Part1(PartInput Input) => Solution(Input, false).ToString();
    public string Part2(PartInput Input) => Solution(Input, true).ToString();
}
