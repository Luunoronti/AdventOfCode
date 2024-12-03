#pragma once
#include "AoCBase.h"

class AoC_2024_02 : public AoCBase
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


    const bool CheckAtIndices(const vector<long>& List, const int indexA, const int indexB, const bool increase) const;
    const bool AnalyzeList(vector<long> List, const int Start, const int End, int& ErrorIndex, const int OmmitIndex = -1) const;


};

