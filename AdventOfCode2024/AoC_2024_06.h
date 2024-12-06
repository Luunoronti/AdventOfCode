#pragma once
#include "AoCBase.h"

enum StepForwardResult
{
    Ok,
    Obstacle,
    OutOfMap
};


class AoC_2024_06 : public AoCBase
{
    // Inherited via AoCBase
    const int GetDay() const override { return 6; }
    const long Step1() override;
    const long Step2() override;

    friend class AoCBase;

    void ClearState();

    __forceinline const int BufPos(const int& x, const int& y) const 
    {
        return x + y * Width;
    };

    StepForwardResult StepForward();
    void TurnRight();
    void MarkCurrentLocation();
    long CountMarkedLocations();
    void FindStartLocation();

    void PutArtificialWallAt(const int x, const int y);
    void ClearArtificialWall(const int x, const int y);

    bool CheckForCircularPath();


    void PrintCurrentMapForStep2(const int awpx, const int awpy);

private:
    int Width;
    int Height;
    string Map;
    vector<uint8_t> Marks;
    vector<uint8_t> SteppedOver;

    int StartLocationX;
    int StartLocationY;

    int LocationX;
    int LocationY;

    int StepX;
    int StepY;
};

