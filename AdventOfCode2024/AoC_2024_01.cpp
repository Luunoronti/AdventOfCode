#include "AoC_2024_01.h"

const int64_t AoC_2024_01::Step1()
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

const int64_t AoC_2024_01::Step2()
{
    const auto& Columns = ReadVerticalVectorsFromFile(2);
    return std::accumulate(Columns[0].begin(), Columns[0].end(), 0L, [&](long acc, const long& left) { return (long)acc + (long)left * (long)std::count(Columns[1].begin(), Columns[1].end(), left); });
}
