using System.Collections;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Linq;
using System.Diagnostics;

namespace AoC_NET;

internal class Program
{
    // this is a test of AoC2024/06
    // to check for the speed of code
    // we only do live inputs, no tests
    static void Main(string[] args)
    {
        var test = new AoC2024_06();

        var ts1 = Stopwatch.GetTimestamp();
        var step1 = test.Step1();
        var ts2 = Stopwatch.GetTimestamp();
        var step2 = test.Step2();
        var ts3 = Stopwatch.GetTimestamp();

        Console.WriteLine($"Step 1: {step1}. Time: {Stopwatch.GetElapsedTime(ts1, ts2).TotalMilliseconds} ms");
        Console.WriteLine($"Step 2: {step2}. Time: {Stopwatch.GetElapsedTime(ts2, ts3).TotalMilliseconds} ms");
    }
}

class AoC2024_06
{
    enum StepForwardResult
    {
        Ok,
        Obstacle,
        OutOfMap,
        Overstep,
    };

    const int SOBM_NORTH = 0x01;
    const int SOBM_EAST = 0x02;
    const int SOBM_SOUTH = 0x04;
    const int SOBM_WEST = 0x08;

    int Width;
    int Height;
    List<char> Map;
    byte[] Marks;
    int StartLocationX;
    int StartLocationY;
    int LocationX;
    int LocationY;
    int StepX;
    int StepY = -1;

     int BufPos(int x, int y)
    {
        return x + y * Width;
    }

    public long Step1()
    {
        var filePath = @"C:\Work\AoC\AdventOfCode\AdventOfCode2024\Live\2024_6_1.txt";
        var lines = File.ReadAllLines(filePath);
        Map = string.Join("", lines).ToList();
        Height = lines.Length;
        Width = lines[0].Length;

        Marks = new byte[Map.Count];

        FindStartLocation();
        MarkCurrentLocation();

        while (true)
        {
            var stepResult = StepForward(null);
            if (stepResult == StepForwardResult.OutOfMap)
            {
                break;
            }
            else if (stepResult == StepForwardResult.Obstacle)
            {
                TurnRight();
            }
            else
            {
                MarkCurrentLocation();
            }
        }
        return CountMarkedLocations();

    }

    public long Step2()
    {
        long sum = 0;

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                if (0 == Marks[BufPos(x, y)])
                    continue;

                PutArtificialWallAt(x, y);
                if (CheckForCircularPath())
                {
                    ++sum;
                }
                ClearArtificialWall(x, y);
            }
        }
        return sum;
    }



    void MarkCurrentLocation()
    {
        Marks[BufPos(LocationX, LocationY)] = 1;
    }

    void FindStartLocation()
    {
        for (var y = 0; y < Height; ++y)
        {
            for (var x = 0; x < Width; ++x)
            {
                if (Map[BufPos(x, y)] == '^')
                {
                    StartLocationX = LocationX = x;
                    StartLocationY = LocationY = y;
                    return;
                }
            }
        }
    }

    long CountMarkedLocations()
    {
        long sum = 0;
        foreach (var m in Marks)
            sum += m;
        return sum;
    }

    void TurnRight()
    {
        if (StepX == 1) { StepX = 0; StepY = 1; }
        else if (StepY == 1) { StepX = -1; StepY = 0; }
        else if (StepX == -1) { StepX = 0; StepY = -1; }
        else if (StepY == -1) { StepX = 1; StepY = 0; }
    }

    void PutArtificialWallAt(int x, int y)
    {
        Map[BufPos(x, y)] = '#';
    }
    void ClearArtificialWall(int x, int y)
    {
        Map[BufPos(x, y)] = '.';
    }


    StepForwardResult StepForward(byte[]? StepoverBuffer)
    {
        LocationX += StepX;
        LocationY += StepY;

        if (LocationX < 0 || LocationY < 0 || LocationX >= Width || LocationY >= Height)
            return StepForwardResult.OutOfMap;


        if (StepoverBuffer != null)
        {
            int step = StepoverBuffer[BufPos(LocationX, LocationY)];
            if ((StepY < 0 && (step & SOBM_NORTH) == SOBM_NORTH)
                || (StepX > 0 && (step & SOBM_EAST) == SOBM_EAST)
                || (StepY > 0 && (step & SOBM_SOUTH) == SOBM_SOUTH)
                || (StepX < 0 && (step & SOBM_WEST) == SOBM_WEST)
                )
                return StepForwardResult.Overstep;
        }

        var mark = Map[BufPos(LocationX, LocationY)];

        if (mark == '#')
        {
            // get back
            LocationX -= StepX;
            LocationY -= StepY;

            return StepForwardResult.Obstacle;
        }
        return StepForwardResult.Ok;
    }

    bool CheckForCircularPath()
    {
        LocationX = StartLocationX;
        LocationY = StartLocationY;
        StepX = 0;
        StepY = -1;

        byte[] SteppedOverBuffer = new byte[Map.Count];
        SteppedOverBuffer[BufPos(LocationX, LocationY)] = SOBM_NORTH;

        while (true)
        {
            var stepResult = StepForward(SteppedOverBuffer);

            if (stepResult == StepForwardResult.Overstep)
            {
                return true;
            }
            else if (stepResult == StepForwardResult.Obstacle)
            {
                TurnRight();
            }
            else if (stepResult == StepForwardResult.OutOfMap)
            {
                return false;
            }
            else
            {
                byte mask = 0;
                if (StepY < 0) mask = SOBM_NORTH;
                if (StepX > 0) mask = SOBM_EAST;
                if (StepY > 0) mask = SOBM_SOUTH;
                if (StepX < 0) mask = SOBM_WEST;

                SteppedOverBuffer[BufPos(LocationX, LocationY)] |= mask;
            }
        }
    }



}