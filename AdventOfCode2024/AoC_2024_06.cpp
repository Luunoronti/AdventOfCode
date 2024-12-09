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
    Width = 0;
    Height = 0;
    StepX = 0;
    StepY = -1; // starting north
}

const int64_t AoC_2024_06::Step1()
{
    ClearState();
    Map = ReadStringFromFile(1, Height, Width);
    Marks = vector<uint8_t>(Map.size());

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

    for(int y = 0; y < Height; ++y)
    {
        for(int x = 0; x < Width; ++x)
        {
            if(!Marks[BufPos(x, y)])
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
    if(StepX == 1) { StepX = 0; StepY = 1; }
    else if(StepY == 1) { StepX = -1; StepY = 0; }
    else if(StepX == -1) { StepX = 0; StepY = -1; }
    else if(StepY == -1) { StepX = 1; StepY = 0; }
}
void AoC_2024_06::MarkCurrentLocation()
{
    Marks[BufPos(LocationX, LocationY)] = 1;
}
long AoC_2024_06::CountMarkedLocations()
{
    long sum = 0;
    for(const uint8_t m : Marks)
        sum += m;
    return sum;
}
void AoC_2024_06::FindStartLocation()
{
    for(int y = 0; y < Height; ++y)
    {
        for(int x = 0; x < Width; ++x)
        {
            if(Map[BufPos(x, y)] == '^')
            {
                StartLocationX = LocationX = x;
                StartLocationY = LocationY = y;
                return;
            }
        }
    }
    cout << RED << BLINK << "Unable to find starting location" << RESET << endl;
}

void AoC_2024_06::PutArtificialWallAt(const int x, const int y)
{
    Map[BufPos(x, y)] = '#';
}
void AoC_2024_06::ClearArtificialWall(const int x, const int y)
{
    Map[BufPos(x, y)] = '.';
}

StepForwardResult AoC_2024_06::StepForward(vector<uint8_t>* StepoverBuffer)
{
    LocationX += StepX;
    LocationY += StepY;

    if(LocationX < 0 || LocationY < 0 || LocationX >= Width || LocationY >= Height)
        return OutOfMap;


    if(StepoverBuffer)
    {
        const int step = (*StepoverBuffer)[BufPos(LocationX, LocationY)];
        if((StepY < 0 && step & SOBM_NORTH)
            || (StepX > 0 && step & SOBM_EAST)
            || (StepY > 0 && step & SOBM_SOUTH)
            || (StepX < 0 && step & SOBM_WEST)
            )
            return Overstep;
    }

    const char& mark = Map[BufPos(LocationX, LocationY)];

    if(mark == '#')
    {
        // get back
        LocationX -= StepX;
        LocationY -= StepY;

        return Obstacle;
    }
    return Ok;
}


bool AoC_2024_06::CheckForCircularPath()
{
    LocationX = StartLocationX;
    LocationY = StartLocationY;
    StepX = 0;
    StepY = -1;

    vector<uint8_t>SteppedOverBuffer(Map.size());
    SteppedOverBuffer[BufPos(LocationX, LocationY)] = SOBM_NORTH;

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
            if(StepY < 0) mask = SOBM_NORTH;
            if(StepX > 0) mask = SOBM_EAST;
            if(StepY > 0) mask = SOBM_SOUTH;
            if(StepX < 0) mask = SOBM_WEST;

            SteppedOverBuffer[BufPos(LocationX, LocationY)] |= mask;
        }
    }
}