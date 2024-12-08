#pragma once
#include "AoCBase.h"
class AoC2024 : public AoCBase
{
    const virtual __forceinline int GetDay() const override = 0;
    virtual const int64_t Step1() = 0;
    virtual const int64_t Step2() = 0;

public:
    const virtual __forceinline int GetYear() const override { return 2024; }
};

