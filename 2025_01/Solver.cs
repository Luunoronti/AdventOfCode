namespace AoC;

public static class Solver
{
    readonly struct Step
    {
        public readonly char Direction;
        public readonly int FullTurns;  // Distance / 100
        public readonly int Remainder;  // Distance % 100

        public Step(char direction, int distance)
        {
            Direction = direction;
            FullTurns = Math.DivRem(distance, 100, out int rem);
            Remainder = rem;
        }
    }

    static Step[] Steps = [];

    private static void EnsureSteps(string[] Lines)
    {
        var lines = Lines;
        if (Steps.Length == lines.Length) return;

        Steps = new Step[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            ReadOnlySpan<char> span = lines[i].AsSpan();
            int distance = int.Parse(span.Slice(1));
            Steps[i] = new Step(span[0], distance);
        }
    }


    public static int Solution(string[] Lines, bool part2)
    {
        EnsureSteps(Lines);

        int position = 50;
        int movesBy0 = 0;

        foreach (ref readonly Step step in Steps.AsSpan())
        {
            // Part 2: full 100-step rotations always cross 0 fullTurns times.
            if (part2 && step.FullTurns != 0)
                movesBy0 += step.FullTurns;

            int steps = step.Remainder;
            if (steps == 0)
                continue; // nothing to do position-wise

            if (step.Direction == 'L')
            {
                int prevPos = position;
                position -= steps;

                if (position == 0)
                {
                    movesBy0++;
                }
                else if (position < 0)
                {
                    // Your original: if (Part2 && position + steps != 0)
                    // position + steps == prevPos, so:
                    if (part2 && prevPos != 0)
                        movesBy0++;

                    position += 100; // wrap
                }
            }
            else // 'R'
            {
                int prevPos = position;
                position += steps;

                if (position == 100)
                {
                    movesBy0++;
                    position = 0;
                }
                else if (position > 100)
                {
                    // original: if (Part2 && position - steps != 0)
                    // position - steps == prevPos:
                    if (part2 && prevPos != 0)
                        movesBy0++;

                    position -= 100; // wrap
                }
            }
        }

        return movesBy0;
    }

    [ExpectedResult("test", 3)]
    [ExpectedResult("live", 1048)]
    public static long SolvePart1(string[] Lines)  => Solution(Lines, false);

    [ExpectedResult("test", 6)]
    [ExpectedResult("live", 6498)]
    public static long SolvePart2(string[] Lines) => Solution(Lines, true);
}
