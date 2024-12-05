#pragma once
#include "AoCBaseWithD3D.h"
class AoC_2024_05 : public AoCBaseWithD3D
{
    const virtual __forceinline int GetDay() const override { return 5; }

    virtual void OnPaint(HDC hdc) override;

    // Inherited via AoCBaseWithD3D
    const long Step1() override;
    const long Step2() override;

    friend class AoCBase;


    static const int duration = 10000; // Duration of the animation in milliseconds 
    static const int interval = 30; // Update interval in milliseconds

    RECT rect{ 50, 50, 150, 150 };
    int speed{ 5 };
};

