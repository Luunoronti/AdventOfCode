#include "pch.h"
#include "AoC_2024_22.h"

#include <conio.h> // for kbhit and getch


typedef std::unordered_map<mutil::IntVector4, int, aoc::IntVector4Hash, aoc::IntVector4Equal> PriceChangeMap;

class Monkey
{
public:
    Monkey(uint64_t startingSecret)
        : secret(startingSecret) {}

    void ProcessSecretAndPrices(int repetitions)
    {
        int lastPrice = 0;
        mutil::IntVector4 changeVector{ 0 };

        for(int i = 0; i < repetitions; i++)
        {
            // price is a ones-digit of the secret
            int price = secret % 10;

            // this is our change history over last 4 iterations
            // we shift it 'left' and store current change
            changeVector[3] = changeVector[2];
            changeVector[2] = changeVector[1];
            changeVector[1] = changeVector[0];
            changeVector[0] = price - lastPrice;

            // remember last price for next iteration
            lastPrice = price;

            // if we are over 3 iterations, we start to store prices
            // if there is a price stored for given change vector already, don't override it.
            if(i >= 4 && firstPriceChanges.find(changeVector) == firstPriceChanges.end())
                firstPriceChanges[changeVector] = price;

            // process secret as per spec
            secret = prune(mix(secret, secret * 64));
            secret = prune(mix(secret, secret / 32));
            secret = prune(mix(secret, secret * 2048));
        }
    }
    __forceinline static uint64_t mix(uint64_t secretNumber, uint64_t value) { return value ^ secretNumber; }
    __forceinline static uint64_t prune(uint64_t secretNumber) { return secretNumber % 16777216; }

    uint64_t secret;
    PriceChangeMap firstPriceChanges;
};

vector<Monkey> monkeys;

const int64_t AoC_2024_22::Compute() const
{
    monkeys.clear();
    vector<int> input;
    aoc::AoCStream() >> input;

    for(const auto& secret : input)
        monkeys.push_back(Monkey(secret));

    int64_t sum = 0;
    concurrency::parallel_for_each(monkeys.begin(), monkeys.end(), [&sum](Monkey& monkey)
        {
            monkey.ProcessSecretAndPrices(2000);
            InterlockedAdd64(&sum, (int64_t)monkey.secret);
        });
    return sum;
}
const int64_t AoC_2024_22::Step1()
{
    TIME_PART;
    return Compute();
};
const int64_t AoC_2024_22::Step2()
{
    TIME_PART;

    // test input does change from P1 to P2, live input does not
    if(IsTest())
        Compute();

    // construct the map with all possible price change vectors
    PriceChangeMap sumPricesPerChangeVector;
    for(const auto& m : monkeys)
        for(const auto& c : m.firstPriceChanges)
            sumPricesPerChangeVector[c.first] += c.second;

    // now, get the highest possible price
    int64_t highestPrice = 0;
    for(const auto& c : sumPricesPerChangeVector)
        highestPrice = max(highestPrice, c.second);

    return highestPrice;
};