#include "pch.h"

#include "AoC_2024_06.h"

#define SOBM_NORTH 0x01
#define SOBM_EAST 0x02
#define SOBM_SOUTH 0x04
#define SOBM_WEST 0x08

void AoC_2024_06::ClearState() // turned out I am using it once, so this should be a ctor
{
    Map.clear();
    Marks.clear();
    Step = { 0, -1 }; // starting north
}

const int64_t AoC_2024_06::Step1()
{
    ClearState();
    aoc::AoCStream() >> Map;
    Marks = aoc::maps::Map2d<uint8_t>(Map.Width, Map.Height);

    TIME_PART;
    FindStartLocation();
    MarkCurrentLocation();

    while(true)
    {
        const auto& stepResult = StepForward(nullptr);
        if(stepResult == OutOfMap)
        {
            break;
        }
        else if(stepResult == Obstacle)
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

const int64_t AoC_2024_06::Step2()
{
    TIME_PART;
    long sum = 0;

    for(int y = 0; y < Map.Height; ++y)
    {
        for(int x = 0; x < Map.Width; ++x)
        {
            if(!Marks.Get(x, y))
                continue;

            PutArtificialWallAt(x, y);
            if(CheckForCircularPath())
            {
                ++sum;
            }
            ClearArtificialWall(x, y);
        }
    }
    return sum;
}


void AoC_2024_06::TurnRight()
{
    if(Step.x == 1) { Step.x = 0; Step.y = 1; }
    else if(Step.y == 1) { Step.x = -1; Step.y = 0; }
    else if(Step.x == -1) { Step.x = 0; Step.y = -1; }
    else if(Step.y == -1) { Step.x = 1; Step.y = 0; }
}
void AoC_2024_06::MarkCurrentLocation()
{
    Marks.Set(Location, 1);
}
long AoC_2024_06::CountMarkedLocations()
{
    long sum = 0;
    for(const uint8_t m : Marks.Map)
        sum += m;
    return sum;
}
void AoC_2024_06::FindStartLocation()
{
    if(Map.find('^', Location))
    {
        StartLocation = Location;
        return;
    }
    cout << RED << BLINK << "Day 2024/06: Unable to find starting location" << RESET << endl;
}

void AoC_2024_06::PutArtificialWallAt(const int x, const int y)
{
    Map.Set(x, y, '#');
}
void AoC_2024_06::ClearArtificialWall(const int x, const int y)
{
    Map.Set(x, y, '.');
}

StepForwardResult AoC_2024_06::StepForward(aoc::maps::Map2d<uint8_t>* StepoverBuffer)
{
    Location += Step;

    if(!Map.WithinBounds(Location))
        return OutOfMap;


    if(StepoverBuffer)
    {
        const int step = StepoverBuffer->Get(Location);
        if((Step.y < 0 && step & SOBM_NORTH)
            || (Step.x > 0 && step & SOBM_EAST)
            || (Step.y > 0 && step & SOBM_SOUTH)
            || (Step.x < 0 && step & SOBM_WEST)
            )
            return Overstep;
    }

    const char& mark = Map.Get(Location);

    if(mark == '#')
    {
        // get back
        Location -= Step;
        return Obstacle;
    }
    return Ok;
}


bool AoC_2024_06::CheckForCircularPath()
{
    Location = StartLocation;
    Step = { 0, -1 };


    aoc::maps::Map2d<uint8_t> SteppedOverBuffer(Map.Width, Map.Height, true);
    SteppedOverBuffer.Set(Location, SOBM_NORTH);

    while(true)
    {
        const auto& stepResult = StepForward(&SteppedOverBuffer);

        if(stepResult == Overstep)
        {
            return true;
        }
        else if(stepResult == Obstacle)
        {
            TurnRight();
        }
        else if(stepResult == OutOfMap)
        {
            return false;
        }
        else
        {
            uint8_t mask = 0;
            if(Step.y < 0) mask = SOBM_NORTH;
            if(Step.x > 0) mask = SOBM_EAST;
            if(Step.y > 0) mask = SOBM_SOUTH;
            if(Step.x < 0) mask = SOBM_WEST;

            SteppedOverBuffer.Set(Location, SteppedOverBuffer.Get(Location) | mask);
        }
    }
}