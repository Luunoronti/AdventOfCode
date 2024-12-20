#pragma once
#include "AoC2024.h"
class AoC_2024_12 : public AoC2024
{
public:
    const virtual __forceinline int GetDay() const override { return 12; }
    const int64_t Step1() override;
    const int64_t Step2() override;

    int64_t part2Sum{ 0 };
};

