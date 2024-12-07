#pragma once
#include "AoCBase.h"
class AoC_2024_07 : public AoCBase
{
    // Inherited via AoCBase
    const int GetDay() const override { return 7; };
    const int64_t Step1() override;
    const int64_t Step2() override;

    const int64_t TestSingleLine(const string& Line, bool AllowThird) const;
    void ParseInputLine(const string& Line, int64_t& ExpectedResult, vector<int>& Operands) const;
    const bool TestForValidResultOnOperators(const int64_t& ExpectedResult, const vector<int> Operands, int Operators, bool AllowThird) const;


    friend class AoCBase;
};

