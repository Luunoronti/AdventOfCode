#include "pch.h"
#include "AoC_2024_02.h"

void AoC_2024_02::OnInitTestingTests() {  }
void AoC_2024_02::OnInitLiveTests() {  }


const bool AoC_2024_02::AnalyzeList(vector<int> List, const bool Allow2ndCheck) const
{
    bool increase = List[0] < List[1];
    bool badList = false;
    for(int i = 0; i < List.size() - 1; ++i)
    {
        int64_t a = List[i];
        int64_t b = List[i + 1];
        if(a == b || abs(a - b) > 3 || (increase && a > b) || (!increase && a < b))
        {
            if(!Allow2ndCheck)
                return false;
            badList = true;
            break;
        }
    }

    if(!badList)
        return true;

    vector<int> _2ndTest;
    for(int i = 0; i < List.size(); ++i)
    {
        _2ndTest.clear();
        for(int i2 = 0; i2 < List.size(); ++i2)
        {
            if(i2 == i) continue;
            _2ndTest.push_back(List[i2]);
        }
        if(AnalyzeList(_2ndTest, false))
        {
            return true;
        }
    }

    return false;
}


const int64_t AoC_2024_02::Step1()
{
    vector<vector<int>> Input;
    aoc::AoCStream(GetFileName()) >> Input;
    
    TIME_PART;

    long safeCount = 0;
    for(const auto& list : Input)
    {
        if(AnalyzeList(list, false))
        {
            safeCount++;
        }
    }
    return safeCount;
}

const int64_t AoC_2024_02::Step2()
{
    vector<vector<int>> Input;
    aoc::AoCStream(GetFileName()) >> Input;

    TIME_PART;

    long safeCount = 0;
    for(const auto& list : Input)
    {
        if(AnalyzeList(list, true))
        {
            safeCount++;
        }
    }
    return safeCount;

}
