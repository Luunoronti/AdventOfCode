#pragma once
#include "AoC2024.h"
class AoC_2024_24 : public AoC2024
{
public:
    const virtual __forceinline int GetDay() const override { return 24; }
    const int64_t Step1() override;
    const int64_t Step2() override;

};

