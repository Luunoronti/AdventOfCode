#pragma once
#include "AoCBaseWithD3D.h"
class AoC_2024_32 : public AoCBaseWithWindow
{
    // this is a test day, not for actual problem solving

public:

    const virtual __forceinline int GetDay() const override { return 32; }

    virtual void OnPaint(HDC hdc) override;

    // Inherited via AoCBaseWithD3D
    const int64_t Step1() override;
    const int64_t Step2() override;



    static const int duration = 10000; // Duration of the animation in milliseconds 
    static const int interval = 30; // Update interval in milliseconds

    RECT rect{ 50, 50, 150, 150 };
    int speed{ 5 };
};

