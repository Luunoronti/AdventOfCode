using TermGlass;

namespace Year2016;

class Day01
{
    Traveller traveller;
    string[] tokens;
    int instrIndex;
    int stepsLeftInInstr;

    /** For visualization */
    string Tooltip(int x, int y) => $"Dist from start: {traveller.StartLocation.ManhattanDistance(new(x, y))} \nDist from trav:  {traveller.Location.ManhattanDistance(new(x, y))}";
    string Status() => $"Steps: {traveller.TotalStepsTaken} | Distance: {traveller.Location.ManhattanDistance(new(0, 0))} | Instr: {instrIndex}/{tokens.Length} | Steps left: {stepsLeftInInstr}";
    void Draw(Frame frame, bool completed)
    {
        frame.Draw(traveller);
        if (completed)
            frame.Draw(4, 2, $"Done. Manhattan distance: {traveller.Location.ManhattanDistance(new(0, 0))}", Rgb.Black, Rgb.Yellow);
    }
    /** For visualization */

    // This is called every step from Vis
    bool Process(bool part2)
    {
        if (stepsLeftInInstr <= 0)
        {
            if (instrIndex >= tokens.Length) return false;

            var part = tokens[instrIndex++];
            traveller.CardinalDirection = part[0] switch
            {
                'R' => traveller.CardinalDirection.Right,
                'L' => traveller.CardinalDirection.Left,
                _ => traveller.CardinalDirection,
            };
            stepsLeftInInstr = int.TryParse(part[1..], out var s) ? s : 0;
        }

        if (stepsLeftInInstr > 0)
        {
            // Walk exactly one step
            var result = traveller.Walk(1, null, t => t.GetVisitedCount(t.Location) <= 1);
            stepsLeftInInstr--;

            // Stop early if revisiting for Part2 semantics
            if (part2 && result == WalkResult.Cancelled) return false;
        }
        return true;
    }

    // parts
    public string Part1(PartInput Input)
    {
        tokens = [.. Input.FullString.Split(',').Select(s => s.Trim())];
        instrIndex = 0;
        stepsLeftInInstr = 0;
        traveller = new Traveller(CardinalDirection.North) { };

        Visualizer.Run(new VizConfig { AutoPlay = false, CenterAtZero = true }, () => Process(false), Draw, Tooltip, Status);
        return traveller.Location.ManhattanDistance(new(0, 0)).ToString();
    }
    public string Part2(PartInput Input)
    {
        tokens = [.. Input.FullString.Split(',').Select(s => s.Trim())];
        instrIndex = 0;
        stepsLeftInInstr = 0;
        traveller = new Traveller(CardinalDirection.North) { StoreVisitedLocations = true };

        Visualizer.Run(new VizConfig { AutoPlay = false, CenterAtZero = true }, () => Process(true), Draw, Tooltip, Status);

        return traveller.Location.ManhattanDistance(new(0, 0)).ToString();
    }
}
