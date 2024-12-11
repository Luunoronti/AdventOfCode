#pragma once
#include "AoC2024.h"
class AoC_2024_10 : public AoC2024
{
public:
    const virtual __forceinline int GetDay() const override { return 10; }
    const int64_t Step1() override;
    const int64_t Step2() override;

    const int64_t Step1_Internal();

    int64_t part2_Test{ 0 };
    int64_t part2_Live{ 0 };
};

