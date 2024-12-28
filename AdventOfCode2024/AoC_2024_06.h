#pragma once
#include "AoC2024.h"

enum StepForwardResult
{
    Ok,
    Obstacle,
    OutOfMap,
    Overstep,
};


class AoC_2024_06 : public AoC2024
{
    // Inherited via AoCBase
public:
    const int GetDay() const override { return 6; }
    const int64_t Step1() override;
    const int64_t Step2() override;

private:
    void ClearState();

    StepForwardResult StepForward(aoc::maps::Map2d<uint8_t>* StepoverBuffer);
    void TurnRight();
    void MarkCurrentLocation();
    long CountMarkedLocations();
    void FindStartLocation();

    void PutArtificialWallAt(const int x, const int y);
    void ClearArtificialWall(const int x, const int y);

    bool CheckForCircularPath();


private:
    aoc::maps::Map2d<char> Map;
    aoc::maps::Map2d<uint8_t> Marks;

    mutil::IntVector2 StartLocation;
    mutil::IntVector2 Location;
    mutil::IntVector2 Step;
public:

};

