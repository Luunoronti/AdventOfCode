#pragma once
#include "AoCBase.h"



enum State 
{ 
    Idle, 
    M, 
    Mu, 
    Mul, 
    MulOpenParen,

    MulFirstNumber,
    MulSecondNumber,

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
    Skip,
};



class AoC_2024_03 : public AoCBase
{
    const virtual __forceinline int GetDay() const override { return 3; }
    // Inherited via AoCBase
    const int64_t Step1() override;
    const int64_t Step2() override;

    friend class AoCBase;

    const long Process(const string& Line, const bool AllowModifiers);
    const long Process2(const string& Line, const bool AllowModifiers);

    const long Process_NOMACROS(const string& Line, const bool AllowModifiers);
    const long Process_MACROS(const string& Line, const bool AllowModifiers);
};

