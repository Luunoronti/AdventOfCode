#pragma once
#include "AoC2024.h"
class AoC_2024_08 : public AoC2024
{
    // Inherited via AoCBase
public:
    const int GetDay() const override { return 8; };
    const int64_t Step1() override;
    const int64_t Step2() override;
};

