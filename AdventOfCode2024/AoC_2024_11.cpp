#include "pch.h"
#include "AoC_2024_11.h"

using namespace aoc;


int64_t CountAll(vector<int64_t> list, int steps)
{
    std::unordered_map<int64_t, int> map;
    std::unordered_map<int64_t, int> map2;
    std::unordered_map<int64_t, int>* mapc;

    for(size_t i = 0; i < list.size(); ++i)
    {
        map[list[i]] = 1;
    }

    mapc = &map;

    for(int step = 0; step < steps; ++step)
    {
        mapc = mapc == &map ? &map2 : &map;

        for(const auto& pair : *mapc)
        {
            const auto& number = pair.first;
            const auto& count = pair.second;

            if(number == 0)
            {
                // replace with 1
            }
            // determine the number of digits
            int64_t numDigits = static_cast<int64_t>(log10(number)) + 1;
            if(!(numDigits % 2))
            {
                // split in half
            }

            // else, just mul by 2024
        }
    }

    return 0;
}


const int64_t AoC_2024_11::Step1()
{
    vector<int64_t> list;
    AoCStream(GetFileName()) >> list;
    TIME_PART;

    CountAll(list, 25);


    return 0;
};
const int64_t AoC_2024_11::Step2()
{
    TIME_PART;
    return 0;
};