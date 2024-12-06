#include "AoC_2024_06.h"


void AoC_2024_06::ClearState()
{
    Map.clear();
    Marks.clear();
    Width = 0;
    Height = 0;
    StepX = 0;
    StepY = -1; // starting north
}
StepForwardResult AoC_2024_06::StepForward()
{
    LocationX += StepX;
    LocationY += StepY;

    if(LocationX < 0 || LocationY < 0 || LocationX >= Width || LocationY >= Height)
        return OutOfMap;

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
bool AoC_2024_06::CheckForCircularPath()
{
    LocationX = StartLocationX;
    LocationY = StartLocationY;
    StepX = 0;
    StepY = -1;

    SteppedOver = vector<uint8_t>(Map.size());
    SteppedOver[BufPos(LocationX, LocationY)] = 1;
    while(true)
    {
        const auto& stepResult = StepForward();

        if(SteppedOver[BufPos(LocationX, LocationY)])
            return true;

        if(stepResult == OutOfMap)
        {
            return false;
        }
        else if(stepResult == Obstacle)
        {
            TurnRight();
            SteppedOver[BufPos(LocationX, LocationY)] = 0;
        }
        else
        {
            SteppedOver[BufPos(LocationX, LocationY)] = 1;
        }
    }
}
void AoC_2024_06::PrintCurrentMapForStep2(const int awpx, const int awpy)
{
    for(int y = 0; y < Height; ++y)
    {
        for(int x = 0; x < Width; ++x)
        {
            const auto& m = Map[BufPos(x, y)];
            const auto& s = SteppedOver[BufPos(x, y)];
            const auto& l = Marks[BufPos(x, y)];

            const bool awp = (x == awpx && y == awpy);

            cout << (l ? GREEN : "") << (awp ? RED : "") << (s ? '*' : (awp ? 'G' : m)) << RESET;
        }
        cout << endl;
    }
    cout << endl;
    cout << endl;
}
const long AoC_2024_06::Step1()
{
    ClearState();
    Map = ReadStringFromFile(1, Height, Width);
    Marks = vector<uint8_t>(Map.size());

    FindStartLocation();
    MarkCurrentLocation();

    while(true)
    {
        const auto& stepResult = StepForward();
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

const long AoC_2024_06::Step2()
{
    // we use data from last step, do not clear, do not read

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
                if(IsTest())
                    PrintCurrentMapForStep2(x, y);

                ++sum;
            }

            ClearArtificialWall(x, y);
        }
    }

    return sum;
}
