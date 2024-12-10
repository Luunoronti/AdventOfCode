#include "pch.h"
#include "AoC_2024_10.h"

const int64_t AoC_2024_10::Step1()
{
    long sum = 0;
    aoc::maps::single_digit_map Map;
    std::vector<aoc::_Location2d<uint8_t>> Locations;
    aoc::AoCStream(GetFileName()) >> Map;

    Map.select_value(0) >> Locations;


    std::vector<aoc::Location2du8> SndLocations;
    Map.higher_by_one_neighbors(aoc::Location2du8(4, 0), aoc::maps::Directions::Cardinal) >> SndLocations;




    TIME_PART;
    return 0;
};
const int64_t AoC_2024_10::Step2()
{
    aoc::maps::single_digit_map Map;
    aoc::AoCStream(GetFileName()) >> Map;

    TIME_PART;
    return 0;
};