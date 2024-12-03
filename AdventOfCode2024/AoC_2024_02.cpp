#include "AoC_2024_02.h"

const int AoC_2024_02::GetDay() const
{
    return 2;
}

const int AoC_2024_02::GetExpectedResultStep1() const
{
    return 472;
}

const int AoC_2024_02::GetExpectedResultStep1Test() const
{
    return 2;
}

const int AoC_2024_02::GetExpectedResultStep2() const
{
    return 0;
}

const int AoC_2024_02::GetExpectedResultStep2Test() const
{
    return 4;
}

const bool AoC_2024_02::AnalyzeList(vector<long> List, const int Start, const int End, int& ErrorIndex, int OmitIndex) const
{
    ErrorIndex = -1;

    if(List.size() <= 1) return false;
    bool increase = List[Start] < List[Start+1];

    for(int i = Start; i < End - 1; ++i)
    {
        int ti1 = i;
        int ti2 = i + 1;

        if(OmitIndex == i) ti1 = i - 1;
        if(OmitIndex == i + 1) ti2 = i + 2;
        if(ti1 < 0) continue;
        if(ti2 >= List.size()) continue;

        if(!CheckAtIndices(List, ti1, ti2, increase))
        {
            ErrorIndex = ti1;
            return false;
        }
    }
    return true;
}
const bool AoC_2024_02::CheckAtIndices(const vector<long>& List, const int indexA, const int indexB, const bool increase) const
{
    if(indexA < 0 || indexB < 0 || indexA >= List.size() || indexB >= List.size())
        return false;

    if(increase)
    {
        if(List[indexA] >= List[indexB] || (List[indexA] < List[indexB] - 3))
            return false;
    }
    else
    {
        if(List[indexA] <= List[indexB] || (List[indexA] > List[indexB] + 3))
            return false;
    }
    return true;
}



const long AoC_2024_02::Step1()
{
    const auto& List = ReadLongVectorsFromFile(1);

    long safeCount = 0;
    int IndexA = 0;
    for(const auto& Line : List)
    {
        if(AnalyzeList(Line, 0, Line.size(), IndexA))
            safeCount++;
    }
    return safeCount;
}

const long AoC_2024_02::Step2()
{
    const auto& List = ReadLongVectorsFromFile(2);
    long safeCount = 0;

    int ErrorIndex = 0;
    int NoUse = 0;
    for(const auto& Line : List)
    {
        if(AnalyzeList(Line, 0, Line.size(), ErrorIndex))
        {
            safeCount++;
        }
        else
        {
            // we must analyze same list, but without checking at  
            // IndexA, IndexB, or IndexA-1

            if(AnalyzeList(Line, 0, Line.size(), NoUse, ErrorIndex))
            {
                safeCount++;
            }
            else if(AnalyzeList(Line, 0, Line.size(), NoUse, ErrorIndex + 1))
            {
                safeCount++;
            }
            else if(ErrorIndex == 1 && AnalyzeList(Line, 1, Line.size(), NoUse))
            {
                safeCount++;
            }
        }
    }
    return safeCount;
}
