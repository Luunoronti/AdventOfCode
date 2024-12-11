#include "pch.h"
#include "AoC_2024_11.h"

using namespace aoc;

AoC_2024_11::DoubleNumberCache doubleNumberCache;
AoC_2024_11::SingleNumberCache singleNumberCache;

int cacheHits = 0;
int cacheMisses = 0;
int64_t AoC_2024_11::AdvanceOneStep(Map* map, Map* target)
{
    int64_t sum = 0;
    for(const auto& pair : *map)
    {
        const auto& number = pair.first;
        const auto& count = pair.second;
        if(number == 0)
        {
            (*target)[1] += count;
            sum += count;
            continue;
        }

        auto it = doubleNumberCache.find(number);
        if(it != doubleNumberCache.end())
        {
            (*target)[it->second.first] += count;
            (*target)[it->second.second] += count;
            sum += count * 2;
            cacheHits++;
            continue;
        }

        auto it2 = singleNumberCache.find(number);
        if(it2 != singleNumberCache.end())
        {
            (*target)[it2->second] += count;
            sum += count;
            cacheHits++;
            continue;
        }

        cacheMisses++;

        int64_t numDigits = static_cast<int64_t>(log10(number)) + 1;
        if(!(numDigits & 1))
        {
            int64_t divisor = static_cast<int64_t>(std::pow(10, numDigits / 2));
            const auto p = std::pair<int64_t, int64_t>(number / divisor, number % divisor);
            (*target)[p.first] += count;
            (*target)[p.second] += count;
            doubleNumberCache[number] = p;
            sum += count * 2;

            continue;
        }
        int64_t num2024 = number * 2024;
        (*target)[num2024] += count;
        singleNumberCache[number] = num2024;
        sum += count;
    }
    return sum;
}
void AoC_2024_11::CountAll(const vector<int64_t>& list, int steps, int stepsForPart1, Map& map, Map& map2)
{
    Map* mapSrc{ nullptr };
    Map* mapcTrg{ nullptr };
    int64_t stepSum = 0;

    for(size_t i = 0; i < list.size(); ++i) map[list[i]] = 1;
    for(int step = 0; step < steps; ++step)
    {
        mapSrc = step & 0x01 ? &map2 : &map;
        mapcTrg = step & 0x01 ? &map : &map2;
        mapcTrg->clear();
        stepSum = AdvanceOneStep(mapSrc, mapcTrg);
        if(step + 1 == stepsForPart1)
        {
            if(IsTest()) firstPart_Test = stepSum;
            else firstPart_Live = stepSum;
        }
    }
    if(IsTest()) secondPart_Test = stepSum;
    else secondPart_Live = stepSum;

    // cout << "Cache hits: "<< cacheHits << ", misses: "<< cacheMisses <<  endl;
}

const int64_t AoC_2024_11::Step1()
{
    TIME_PART;
    vector<int64_t> list;
    AoCStream(GetFileName()) >> list;
    Map map, map2;
    CountAll(list, 75, 25, map, map2);
    return IsTest() ? firstPart_Test : firstPart_Live;
};
const int64_t AoC_2024_11::Step2()
{
    TIME_PART;
    return IsTest() ? secondPart_Test : secondPart_Live;
};