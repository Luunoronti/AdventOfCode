#pragma once
#include "AoCBase.h"



enum State 
{ 
    Idle, 
    M, 
    Mu, 
    Mul, 
    OpenParen, 
    X1, 
    X2, 
    X3, 
    Comma, 
    Y1, 
    Y2, 
    Y3, 
    D,
    DO,
    DON,
    DON_,
    DON_T,
    DoOpenParen, 
    DontOpenParen, 
};



class AoC_2024_03 : public AoCBase
{
    // Inherited via AoCBase
    const int GetDay() const override;
    const int GetExpectedResultStep1() const override;
    const int GetExpectedResultStep1Test() const override;
    const int GetExpectedResultStep2() const override;
    const int GetExpectedResultStep2Test() const override;
    const long Step1() override;
    const long Step2() override;

    friend class AoCBase;

    const long Process(const string& Line, const bool AllowModifiers);
};

