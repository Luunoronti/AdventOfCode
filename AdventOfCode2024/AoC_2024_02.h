#pragma once
#include "AoCBase.h"

class AoC_2024_02 : public AoCBase
{
    const virtual __forceinline int GetDay() const override { return 2; }
    const long Step1() override;
    const long Step2() override;

    friend class AoCBase;


    const bool CheckAtIndices(const vector<long>& List, const int indexA, const int indexB, const bool increase) const;
    const bool AnalyzeList(vector<long> List, const int Start, const int End, int& ErrorIndex, const int OmmitIndex = -1) const;


};

