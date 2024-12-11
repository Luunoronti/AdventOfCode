#include "pch.h"
#include "AoC_2024_11.h"

using namespace aoc;


std::unordered_map<int64_t, std::pair<int64_t, int64_t>> doubleDigitCache;

int64_t totalLogs = 0;
int64_t totalPows = 0;
int64_t maxLogsPerRun = 0;
int64_t AoC_2024_11::AdvanceOneStep(Map* map, Map* target)
{
    int64_t sum = 0;
    int64_t logsThisRun = 0;
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

        auto it = doubleDigitCache.find(number);
        if(it != doubleDigitCache.end())
        {
            (*target)[it->second.first] += count;
            (*target)[it->second.second] += count;
            sum += count * 2;
            continue;
        }

        // add this number to an array for avx2 parallel processing
        // we will have as many vecotrs as ve have cores
        // and each vector will be processed 
        totalLogs++;
        logsThisRun++;
        int64_t numDigits = static_cast<int64_t>(log10(number)) + 1;
        if(!(numDigits % 2))
        {
            totalPows++;
            int64_t divisor = static_cast<int64_t>(std::pow(10, numDigits / 2));
            const auto p = std::pair<int64_t, int64_t>(number / divisor, number % divisor);
            (*target)[p.first] += count;
            (*target)[p.second] += count;
            doubleDigitCache[number] = p;
            sum += count * 2;
            
            continue;
        }

        (*target)[number * 2024] += count;
        sum += count;
    }
    maxLogsPerRun = max(maxLogsPerRun, logsThisRun);
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
}
const int64_t AoC_2024_11::Step1()
{
    TIME_PART;
    vector<int64_t> list;
    AoCStream(GetFileName()) >> list;
    Map map, map2;
    CountAll(list, 75, 25, map, map2);

    cout << "Total logs: " << totalLogs << ", total pows: " << totalPows << ", max logs per run: " << maxLogsPerRun << endl;
    return IsTest() ? firstPart_Test : firstPart_Live;
};
const int64_t AoC_2024_11::Step2()
{
    TIME_PART;
    return IsTest() ? secondPart_Test : secondPart_Live;
};