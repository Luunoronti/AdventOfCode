#include "pch.h"
#include "AoC_2024_11.h"

using namespace aoc;
typedef std::unordered_map<int64_t, int64_t> Map;
typedef std::pair<int64_t, int64_t> MapPair;


int64_t largestValue = 0;
int64_t loops_total = 0;
std::unordered_map<int64_t, std::pair<int64_t, int64_t>> doubleDigitCache;

void AdvanceOneStep(Map* map, Map* target)
{
    for(const auto& pair : *map)
    {
        const auto& number = pair.first;
        const auto& count = pair.second;
        largestValue = max(largestValue, number);

        if(number == 0)
        {
            (*target)[1] += count;
            continue;
        }

        auto it = doubleDigitCache.find(number);
        if(it != doubleDigitCache.end())
        {
            (*target)[it->second.first] += count;
            (*target)[it->second.second] += count;
            continue;
        }

        int64_t numDigits = static_cast<int64_t>(log10(number)) + 1;
        if(!(numDigits % 2))
        {
            int divisor = static_cast<int64_t>(std::pow(10, numDigits / 2));
            const auto p = std::pair<int64_t, int64_t>(number / divisor, number % divisor);
            (*target)[p.first] += count;
            (*target)[p.second] += count;
            doubleDigitCache[number] = p;
        }
        else
        {
            (*target)[number * 2024] += count;
        }

        loops_total++;
    }
}
int64_t CountAll(const vector<int64_t>& list, int steps, Map& map, Map& map2)
{
    //doubleDigitCache.clear();

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
    auto sum = CountAll(list, 75, map, map2);
    //cout << loops_total << endl
    //    ;
    return sum;
};