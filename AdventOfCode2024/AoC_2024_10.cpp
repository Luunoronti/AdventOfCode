#include "pch.h"
#include "AoC_2024_10.h"

using namespace aoc;

const int64_t AoC_2024_10::Step1()
{
    long sum = 0;
    maps::single_digit_map Map;
    std::vector<Location2di> Locations;
    AoCStream(GetFileName()) >> Map;

    TIME_PART;

    Map.select_value(0) >> Locations;


    std::vector<Location2di> SndLocations;
    Map.get_neighbors(Location2di(4, 0), maps::Directions::Cardinal, 1, [](const Location2di& loc, const Location2di& nloc, int8_t _val, int8_t _n_val, maps::Directions _dir)
        {
            return _val + 1 == _n_val;
        }) >> SndLocations;



    return 0;
};
const int64_t AoC_2024_10::Step2()
{
    aoc::maps::single_digit_map Map;
    aoc::AoCStream(GetFileName()) >> Map;

    TIME_PART;
    return 0;
};