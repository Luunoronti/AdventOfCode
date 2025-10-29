using System.Linq;
using TermGlass;

namespace Year2016;


class Day02
{
    Map<char> keypad;
    Traveller traveller;
    Queue<char> instructions;
    List<char> combination;

    bool Process(bool part2)
    {
        if (instructions.TryDequeue(out var instr))
        {
            if (instr == 'P')
            {
                combination.Add(keypad[traveller.Location]);
            }
            else
            {
                traveller.CardinalDirection = CardinalDirection.FromCharacter(instr);
                traveller.Walk(1, CanStepPred: (t) =>
                {
                    var trs = t.Location + t.CardinalDirection.Direction;
                    if (trs.X < 0 || trs.Y < 0 || trs.X >= keypad.SizeX || trs.Y >= keypad.SizeY) return StepPred.Stop;
                    if (part2 && keypad[trs] == ' ') return StepPred.Stop;
                    return StepPred.Continue;
                });
            }
            return true;
        }
        return false;
    }
    void Draw(Frame frame, bool completed)
    {
        frame.Draw(keypad, Rgb.White, Rgb.Black);
        frame.Draw(traveller, false, false);
    }
    string Status() => $"Combination: {string.Join(',', combination)}, instructions left: {instructions.Count}";

    public string Part1(PartInput Input)
    {
        traveller = new Traveller(new Location(1, 1), CardinalDirection.North);
        instructions = [];
        combination = [];
        keypad = new(3, 3, new char[3, 3]
        {
        { '1', '2', '3'},
        { '4', '5', '6'},
        { '7', '8', '9'},
        });
        // prepare instructions for traveller: NSWE are cardinal directions, P is a key press
        foreach (var c in Input.Span)
            instructions.Enqueue(c switch { 'U' => 'N', 'D' => 'S', 'L' => 'W', 'R' => 'E', '\n' => 'P', _ => '.' });

        Visualizer.Run(new VizConfig { AutoPlay = true, CenterAtZero = true, AutoStepPerSecond = 1400 }, () => Process(false), Draw, null, Status);
        return string.Join("", combination);
    }

   

    public string Part2(PartInput Input)
    {
        traveller = new Traveller(new Location(0, 2), CardinalDirection.North);
        instructions = [];
        combination = [];

        keypad = new(5, 5, new char[5, 5]
        {
        { ' ', ' ', '1', ' ', ' '},
        { ' ', '2', '3', '4', ' '},
        { '5', '6', '7', '8', '9'},
        { ' ', 'A', 'B', 'C', ' '},
        { ' ', ' ', 'D', ' ', ' '},
        });

        // prepare instructions for traveller: NSWE are cardinal directions, P is a key press
        foreach (var c in Input.Span)
            instructions.Enqueue(c switch { 'U' => 'N', 'D' => 'S', 'L' => 'W', 'R' => 'E', '\n' => 'P', _ => '.' });

        Visualizer.Run(new VizConfig { AutoPlay = true, CenterAtZero = true, AutoStepPerSecond = 1400 }, () => Process(true), Draw, null, Status);

        return string.Join("", combination);
    }
}
