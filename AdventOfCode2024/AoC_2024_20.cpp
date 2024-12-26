#include "pch.h"
#include "AoC_2024_20.h"

const int64_t AoC_2024_20::Step1()
{
    //if(!IsTest())return 0;

    TIME_PART;
    aoc::maps::Map2d<char> input;
    aoc::AoCStream() >> input;

    int sx, sy, ex, ey;
    input.find('S', sx, sy);
    input.find('E', ex, ey);

    // prepare integer map
    aoc::maps::Map2d<int> map = input.ToMap<int>([](const char& in) {return in == '#' ? -1 : 0; });

    int fillIndex = 0;
    map.fill({ sx, sy }, [](int v, int, int) {return v != -1; }, [&fillIndex](int, int, int) {return ++fillIndex; });
    map.print([](int, int, int v)
        {            
            if(v == -1)return '#';
            return '2';
        },

        [&fillIndex](int, int, int v)
        {
            if(v == -1)
            {
                return mutil::Vector3(0.6f, 0, 0);
            }

            return mutil::Vector3(0, 0.3f + (0.7f * ((float)v / (float)fillIndex)), 0);
        });
    return 0;
};
const int64_t AoC_2024_20::Step2()
{
    TIME_PART;
    return 0;
};