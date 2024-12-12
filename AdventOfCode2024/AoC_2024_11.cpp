#include "pch.h"
#include "AoC_2024_11.h"

using namespace aoc;

AoC_2024_11::DoubleNumberCache doubleNumberCache;
AoC_2024_11::SingleNumberCache singleNumberCache;
AoC_2024_11::SingleNumberCache pow2Cache;


float percentage(int a, int b)
{
    return (float)(100. * ((float)b / a));
}
void printValues(int hits, int misses, string nameA, string nameB, size_t cacheSize)
{
    dout << nameA << ": " << hits << ", "
        << nameB << ": " << misses
        << " (" << toStringWithPrecision(percentage(hits, misses), 2) << "%)" 
        << ", cache size: " << cacheSize
        << endl;
}
const int64_t AoC_2024_11::Step1()
{
    TIME_PART;
    vector<int64_t> list;
    aocs >> list;
    Map map, map2;
    CountAll(list, 75, 25, map, map2);


    return IsTest() ? firstPart_Test : firstPart_Live;
};
const int64_t AoC_2024_11::Step2()
{
    dout << endl << (IsTest() ? "TEST ITERATION" : "LIVE ITERATION") << endl;
    printValues(cacheHits, cacheMisses, "hits", "misses", doubleNumberCache.size() + singleNumberCache.size());
    printValues(dblNumCacheHits, dblNumCacheMisses, "dbl num hits", "misses", doubleNumberCache.size());
    printValues(sngNumCacheHits, sngNumCacheMisses, "sng num hits", "misses", singleNumberCache.size());
    printValues(powCacheHits, powCacheMisses, "pow hits", "misses", pow2Cache.size());

    TIME_PART;
    return IsTest() ? secondPart_Test : secondPart_Live;
};

void AoC_2024_11::CountAll(const vector<int64_t>& list, int steps, int stepsForPart1, Map& map, Map& map2)
{
    Map* mapSrc{ nullptr };
    Map* mapcTrg{ nullptr };
    int64_t stepSum = 0;

    for(size_t i = 0; i < list.size(); ++i) map[list[i]]++;
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
}

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
            dblNumCacheHits++;
            continue;
        }


        auto it2 = singleNumberCache.find(number);
        if(it2 != singleNumberCache.end())
        {
            (*target)[it2->second] += count;
            sum += count;
            cacheHits++;
            sngNumCacheHits++;
            continue;
        }

        cacheMisses++;

        int64_t numDigits = static_cast<int64_t>(log10(number)) + 1;
        if(!(numDigits & 1))
        {
            dblNumCacheMisses++;

            int64_t divisor = 0;
            auto it2 = pow2Cache.find(numDigits);
            if(it2 != pow2Cache.end())
            {
                divisor = it2->second;
                powCacheHits++;
            }
            else
            {
                divisor = static_cast<int64_t>(std::pow(10, numDigits / 2));
                pow2Cache[numDigits] = divisor;
                powCacheMisses++;
            }

            const auto p = std::pair<int64_t, int64_t>(number / divisor, number % divisor);
            (*target)[p.first] += count;
            (*target)[p.second] += count;
            doubleNumberCache[number] = p;
            sum += count * 2;

            continue;
        }
        sngNumCacheMisses++;

        int64_t num2024 = number * 2024;
        (*target)[num2024] += count;
        singleNumberCache[number] = num2024;
        sum += count;
    }
    return sum;
}



int cacheHits = 0;
int cacheMisses = 0;

int dblNumCacheHits = 0;
int sngNumCacheHits = 0;
int powCacheHits = 0;

int dblNumCacheMisses = 0;
int sngNumCacheMisses = 0;
int powCacheMisses = 0;