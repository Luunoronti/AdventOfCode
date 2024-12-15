#include "AoCBase.h"
#pragma once
#include "AoCBase.h"
class AoC2024 : public AoCBase
{
    const virtual __forceinline int GetDay() const override = 0;

public:
    const virtual __forceinline int GetYear() const override { return 2024; }
};

