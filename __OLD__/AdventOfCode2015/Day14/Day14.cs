
using AmaAocHelpers;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode0;

//[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
//[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
//[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
//[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
//[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
[ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
[ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
class Day14
{
    private const int RealRaceTime = 2503;
    class Deer
    {
        public string? Name;
        public int Speed;
        public int MaxRunTime;
        public int RequiredRestTime;
        public int Score = 0;

        public int runTime = 0;
        public int distance = 0;
        public int restTime = 0;
        public int points;
    }

    public static long Part1(string[] lines)
    {
        Dictionary<string, (int, int, int)> reindeers = new();
        foreach (var l in lines)
        {
            var l2 = l.Replace("can fly", " ").Replace("km/s for", " ").Replace("seconds, but then must rest for", " ").Replace(" seconds.", "");

            var sp = l2.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            reindeers[sp[0]] = (int.Parse(sp[1]), int.Parse(sp[2]), int.Parse(sp[3]));
        }

        var selectedDeer = "";
        var selDeerDistance = 0;
        foreach (var deer in reindeers)
        {
            var dist = GetDistance(RealRaceTime, deer.Value.Item1, deer.Value.Item2, deer.Value.Item3);
            if (dist > selDeerDistance)
            {
                selDeerDistance = dist;
                selectedDeer = deer.Key;

                Log.WriteLine($"{selectedDeer} is winning with distance {dist}");
            }
        }

        return selDeerDistance;
        static int GetDistance(int raceTime, int speed, int time, int restTime)
        {
            var dist = 0;
            while (raceTime > 0)
            {
                // first, run for x
                var rrt = time;
                if (rrt > raceTime)
                    rrt = raceTime;

                dist += speed * rrt;
                raceTime -= rrt;

                // then, wait n
                raceTime -= restTime;
            }
            return dist;
        }
    }

    public static long Part2(string[] lines)
    {
        List<Deer> deerList = new List<Deer>();
        foreach (var l in lines)
        {
            var l2 = l.Replace("can fly", " ").Replace("km/s for", " ").Replace("seconds, but then must rest for", " ").Replace(" seconds.", "");

            var sp = l2.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            deerList.Add(new Deer
            {
                Name = sp[0],
                Speed = int.Parse(sp[1]),
                MaxRunTime = int.Parse(sp[2]),
                RequiredRestTime = int.Parse(sp[3]),
            });
        }
        // at start, all deers run
        foreach (var d in deerList)
        {
            d.runTime = d.MaxRunTime;
        }

        for (int i = 0; i < RealRaceTime; i++)
        {
            foreach (var d in deerList)
            {
                if (d.runTime > 0)
                {
                    d.runTime--;
                    d.distance += d.Speed;
                    if (d.runTime == 0)
                    {
                        d.restTime = d.RequiredRestTime;
                    }
                    continue;
                }
                else
                {
                    d.restTime--;
                    if (d.restTime == 0)
                    {
                        d.runTime = d.MaxRunTime;
                    }
                }
            }

            // get all winning
            var max = deerList.Max(d => d.distance);
            foreach (var d in deerList)
            {
                if (d.distance == max)
                    d.points++;

            }
        }


        return deerList.Max(d => d.points);
    }
}
