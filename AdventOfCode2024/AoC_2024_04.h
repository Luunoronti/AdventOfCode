#pragma once
#include "AoCBase.h"
class AoC_2024_04 : public AoCBase
{
    const virtual __forceinline int GetDay() const override { return 4; }
    // Inherited via AoCBase
    const long Step1() override;
    const long Step2() override;

    friend class AoCBase;
};

