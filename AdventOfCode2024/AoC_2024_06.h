#pragma once
#include "AoCBase.h"

enum StepForwardResult
{
    Ok,
    Obstacle,
    OutOfMap,
    Overstep,
};


class AoC_2024_06 : public AoCBase
{
    // Inherited via AoCBase
    const int GetDay() const override { return 6; }
    const int64_t Step1() override;
    const int64_t Step2() override;

    friend class AoCBase;

    void ClearState();

    __forceinline const int BufPos(const int& x, const int& y) const 
    {
        return x + y * Width;
    };

    StepForwardResult StepForward(vector<uint8_t>* StepoverBuffer);
    void TurnRight();
    void MarkCurrentLocation();
    long CountMarkedLocations();
    void FindStartLocation();

    void PutArtificialWallAt(const int x, const int y);
    void ClearArtificialWall(const int x, const int y);

    bool CheckForCircularPath();


private:
    int Width;
    int Height;
    string Map;
    vector<uint8_t> Marks;

    int StartLocationX;
    int StartLocationY;

    int LocationX;
    int LocationY;

    int StepX;
    int StepY;
public:

};

