#pragma once
#include "AoCBase.h"
class AoC_2024_08 : public AoCBase
{
    // Inherited via AoCBase
    virtual void OnInitTestingTests() override;
    virtual void OnInitLiveTests() override;
    const int GetDay() const override { return 8; };
    const int64_t Step1() override;
    const int64_t Step2() override;

    friend class AoCBase;

    int Width{ 0 };
    int Height{ 0 };
    string Input;
};

