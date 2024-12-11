#pragma once
#include "AoC2024.h"


class AoC_2024_11 : public AoC2024
{
    typedef std::unordered_map<int64_t, int64_t> Map;
    typedef std::pair<int64_t, int64_t> MapPair;

public:
    const virtual __forceinline int GetDay() const override { return 11; }
    const int64_t Step1() override;
    const int64_t Step2() override;

    void CountAll(const vector<int64_t>& list, int steps, int stepsForPart1, Map& map, Map& map2);
    int64_t AdvanceOneStep(Map* map, Map* target);

    // static __forceinline const int64_t CountSum(Map* map);

    int64_t secondPart_Test{ 0 };
    int64_t secondPart_Live{ 0 };

    int64_t firstPart_Test{ 0 };
    int64_t firstPart_Live{ 0 };
};

