#include "pch.h"
#include "AoC_2024_18.h"

using namespace mutil;
using namespace std;
using namespace aoc;
using namespace aoc::maps;

const int64_t AoC_2024_18::Step1()
{
    TIME_PART;

    vector<IntVector2> Locations;
    AoCStream() >> Locations;

    int width = IsTest() ? 7 : 71;
    int heigth = IsTest() ? 7 : 71;
    int size = IsTest() ? 12 : 1024;
    size = min(size, (int)Locations.size());

    Map2d<uint8_t> map(width, heigth);

    for(int l = 0; l < size; l++)
    {
        map.Set(Locations[l], 1);
    }
    return map.GetLengthOfShortestPath({ 0, 0 }, { width - 1, heigth - 1 }, [](const auto& loc, auto v) { return v == 0; });
};

const int64_t AoC_2024_18::Step2()
{
    TIME_PART;

    vector<IntVector2> Locations;
    AoCStream() >> Locations;

    int width = IsTest() ? 7 : 71;
    int heigth = IsTest() ? 7 : 71;
    int size = IsTest() ? 12 : 1024;
    size = min(size, (int)Locations.size());

    Map2d<uint8_t> map(width, heigth);

    for(int l = 0; l < size; l++)
    {
        map.Set(Locations[l], 1);
    }

    auto isValid = [](const auto& loc, const auto& v) { return v == 0; };

    for(int l = size; l < Locations.size(); l++)
    {
        const auto& loc = Locations[l];
        map.Set(loc, 1);

        if(map.GetLengthOfShortestPath({ 0,0 }, { width - 1, heigth - 1 }, isValid) == -1)
        {
            printf("%d,%d\n", loc.x, loc.y);
            return 1;
        }
    }
    return 0;
};
