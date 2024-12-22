#include "pch.h"
#include "AoC_2024_22.h"

typedef std::unordered_map<mutil::IntVector4, int, aoc::IntVector4Hash, aoc::IntVector4Equal> PriceChangeMap;

class Monkey
{
public:
    Monkey(uint64_t startingSecret)
        : secret(startingSecret), initialSecret(startingSecret)
    {
    }

    void CountSecretAndPrices(int repetitions)
    {
        int lastPrice = 0;
        int lastChange0 = 0;
        int lastChange1 = 0;
        int lastChange2 = 0;
        int lastChange3 = 0;

        for(int i = 0; i < repetitions; i++)
        {
            int price = secret % 10;

            if(i > 0)
            {
                lastChange3 = lastChange2;
                lastChange2 = lastChange1;
                lastChange1 = lastChange0;
                lastChange0 = price - lastPrice;
            }
            lastPrice = price;

            if(i >= 4)
            {
                mutil::IntVector4 changeVector(lastChange0, lastChange1, lastChange2, lastChange3);
                if(firstChangesWithPrices.find(changeVector) == firstChangesWithPrices.end())
                    firstChangesWithPrices[changeVector] = price;
            }
            secret = prune(mix(secret, secret * 64));
            secret = prune(mix(secret, secret / 32));
            secret = prune(mix(secret, secret * 2048));
        }
    }
    __forceinline static uint64_t mix(uint64_t secretNumber, uint64_t value)
    {
        return value ^ secretNumber;
    }
    __forceinline static uint64_t prune(uint64_t secretNumber)
    {
        return secretNumber % 16777216;
    }

    __forceinline const void FillPriceChangeVectorMap(PriceChangeMap& map) const
    {
        for(const auto& c : firstChangesWithPrices)
            map[c.first] = 1;
    }
    __forceinline const int GetPriceForChangeVector(const mutil::IntVector4& ChangeVector) const
    {
        auto it = firstChangesWithPrices.find(ChangeVector);
        if(it != firstChangesWithPrices.end())
            return it->second;
        return 0;
    }
    uint64_t secret;
    uint64_t initialSecret;
    PriceChangeMap firstChangesWithPrices;
};

vector<Monkey> monkeys;
const int64_t AoC_2024_22::Step1()
{
    monkeys.clear();

    vector<int> input;
    aoc::AoCStream() >> input;

    TIME_PART;
    int64_t sum = 0;

    for(const auto& secret : input) 
        monkeys.push_back(Monkey(secret));

    concurrency::parallel_for_each(monkeys.begin(), monkeys.end(), [&sum](Monkey& monkey)
        {
            monkey.CountSecretAndPrices(2000);
            InterlockedAdd64(&sum, (int64_t)monkey.secret);
        });
    return sum;
};
const int64_t AoC_2024_22::Step2()
{
    TIME_PART;

    // test input does change from P1 to P2, but live input does not
    if(IsTest())
    {
        monkeys.clear();
        vector<int> input;
        aoc::AoCStream() >> input;

        for(const auto& secret : input) 
            monkeys.push_back(Monkey(secret));
        concurrency::parallel_for_each(monkeys.begin(), monkeys.end(), [](Monkey& monkey)
            {
                monkey.CountSecretAndPrices(2000);
            });
    }

    // construct the map with all possible price change vectors
    // the value doesn't matter
    PriceChangeMap allChanges;
    for(const auto& m : monkeys)
        m.FillPriceChangeVectorMap(allChanges);

    // now, estimate the highest possible price
    int64_t highestPrice = 0;

    CRITICAL_SECTION cs;
    ::InitializeCriticalSection(&cs);

    concurrency::parallel_for_each(allChanges.begin(), allChanges.end(), [&](const auto& c)
        {
            int priceSum = 0;
            for(auto& m : monkeys)
            {
                priceSum += m.GetPriceForChangeVector(c.first);
            }

            ::EnterCriticalSection(&cs);
            if(highestPrice < priceSum)
            {
                highestPrice = priceSum;
            }
            ::LeaveCriticalSection(&cs);
        });

    ::DeleteCriticalSection(&cs);
    return highestPrice;
};






