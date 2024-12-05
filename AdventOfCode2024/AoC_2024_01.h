#pragma once

#include "AoCBase.h"


class AoC_2024_01 : public AoCBase
{
    const virtual __forceinline int GetDay() const override { return 1; }
    const long Step1() override;
    const long Step2() override;

    friend class AoCBase;
};

