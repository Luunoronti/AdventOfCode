#include "AoC_2024_01.h"

const int AoC_2024_01::GetExpectedResultStep1() const
{
    return 1873376;
}

const int AoC_2024_01::GetExpectedResultStep1Test() const
{
    return 11;
}

const int AoC_2024_01::GetExpectedResultStep2() const
{
    return 18997088;
}

const int AoC_2024_01::GetExpectedResultStep2Test() const
{
    return 31;
}

const long AoC_2024_01::Step1()
{
    const auto& Columns = ReadVerticalVectorsFromFile(1);

    long sum = 0;

    auto leftList = Columns[0];
    auto rightList = Columns[1];

    while(leftList.size() > 0)
    {
        const long leftMin = GetMinimum(leftList);
        const long rightMin = GetMinimum(rightList);

        sum += std::abs(leftMin - rightMin);

        // I do not like erase. It's linear, but still, it's an operation on memory
        leftList.erase(std::find(leftList.begin(), leftList.end(), leftMin));
        rightList.erase(std::find(rightList.begin(), rightList.end(), rightMin));
    }

    return sum;
}

const long AoC_2024_01::Step2()
{
    const auto& Columns = ReadVerticalVectorsFromFile(2);
    return std::accumulate(Columns[0].begin(), Columns[0].end(), 0L, [&](long acc, const long& left) { return acc + left * std::count(Columns[1].begin(), Columns[1].end(), left); });
}


