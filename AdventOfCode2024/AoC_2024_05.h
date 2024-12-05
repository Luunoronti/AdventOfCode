#pragma once
#include "AoCBaseWithD3D.h"
class AoC_2024_05 : public AoCBaseWithD3D
{
    const virtual __forceinline int GetDay() const override { return 5; }
    // Inherited via AoCBaseWithD3D
    const long Step1() override;
    const long Step2() override;

    friend class AoCBase;
};

