#pragma once
#include "AoCBase.h"
class AoC_2024_04 : public AoCBase
{
    const virtual __forceinline int GetDay() const override { return 4; }
    // Inherited via AoCBase
    const int64_t Step1() override;
    const int64_t Step2() override;

    friend class AoCBase;
};

