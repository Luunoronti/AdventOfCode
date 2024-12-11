#include "pch.h"
#include "AoC_2024_11.h"

using namespace aoc;
typedef std::unordered_map<int64_t, int64_t> Map;
typedef std::pair<int64_t, int64_t> MapPair;

void AdvanceOneStep(Map* map, Map* target)
{
    for(const auto& pair : *map)
    {
        const auto& number = pair.first;
        const auto& count = pair.second;

        int64_t numDigits = static_cast<int64_t>(log10(number)) + 1;
        if(!(numDigits % 2))
        {
            int divisor = static_cast<int64_t>(std::pow(10, numDigits / 2));
            (*target)[number / divisor] += count;
            (*target)[number % divisor] += count;
        }
        else
        {
            const uint64_t newValue = number == 0 ? 1 : number * 2024;
            (*target)[newValue] += count;
        }
    }
}
int64_t CountAll(const vector<int64_t>& list, int steps, Map& map, Map& map2)
{
    Map* mapSrc{ nullptr };
    Map* mapcTrg{ nullptr };

    for(size_t i = 0; i < list.size(); ++i) map[list[i]] = 1;
    for(int step = 0; step < steps; ++step)
    {
        mapSrc = step & 0x01 ? &map2 : &map;
        mapcTrg = step & 0x01 ? &map : &map2;
        mapcTrg->clear();
        AdvanceOneStep(mapSrc, mapcTrg);
    }
    int64_t sum = 0;
    if(mapcTrg)
        for(const auto& pair : *mapcTrg)
            sum += pair.second;
    return sum;
}
const int64_t AoC_2024_11::Step1()
{
    TIME_PART;
    vector<int64_t> list;
    AoCStream(GetFileName()) >> list;
    Map map, map2;
    return CountAll(list, 25, map, map2);
};
const int64_t AoC_2024_11::Step2()
{
    TIME_PART;
    vector<int64_t> list;
    AoCStream(GetFileName()) >> list;
    Map map, map2;
    return CountAll(list, 75, map, map2);
};