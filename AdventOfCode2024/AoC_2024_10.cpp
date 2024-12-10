#include "pch.h"
#include "AoC_2024_10.h"

using namespace std;
using namespace aoc;
using namespace aoc::maps;

int64_t ProcessPath(const single_digit_map& map, const Location2di& location, int locIndex, unordered_map<int, set<Location2di>>& trailheads)
{
    if(map.Get(location) == 9)
    {
        trailheads[locIndex].emplace(location);
        return 1;
    }
    std::vector<Location2di> neighborLocations;
    map.get_neighbors(location, Cardinal, 1, [](const Location2di& loc, const Location2di& n_loc, int8_t _val, int8_t _n_val, Directions _dir)
        {
            return _val + 1 == _n_val;
        }) >> neighborLocations;

    /*
    /** this accumulate works and yields good result. BUT, it causes my program to use 2x time than a loop bellow *
    return std::accumulate(neighborLocations.begin(), neighborLocations.end(), int64_t(0), [locIndex, map, &trailheads](int64_t acc, const Location2di& location)
        {
            return acc + ProcessPath(map, location, locIndex, trailheads);
        });
     */

    int64_t sum = 0;
    for(const auto& loc : neighborLocations)
    {
        sum += ProcessPath(map, loc, locIndex, trailheads);
    }
    return sum;

}

const int64_t AoC_2024_10::Step1()
{
    single_digit_map Map;
    AoCStream(GetFileName()) >> Map;

    TIME_PART;

    vector<Location2di> startingLocations;
    unordered_map<int, set<Location2di>> trailheads;

    Map.select_value(0) >> startingLocations;

    int locIndex = 0;
    for(const auto& loc : startingLocations)
    {
        ProcessPath(Map, loc, locIndex++, trailheads);
    }

    return std::accumulate(trailheads.begin(), trailheads.end(), int64_t(0), [](int64_t acc, const auto& pair) { return acc + pair.second.size(); });
};
const int64_t AoC_2024_10::Step2()
{
    single_digit_map Map;
    AoCStream(GetFileName()) >> Map;

    TIME_PART;

    vector<Location2di> startingLocations;
    Map.select_value(0) >> startingLocations;
    unordered_map<int, set<Location2di>> trailheads;

    /* same as with accumulate vs. for() above
    int locIndex = 0;
    return std::accumulate(startingLocations.begin(), startingLocations.end(), int64_t(0), [&locIndex, Map, &trailheads](int64_t acc, const Location2di& location)
        {
            return acc + ProcessPath(Map, location, locIndex++, trailheads);
        });
    */

    int locIndex = 0;
    int64_t sum = 0;
    for(const auto& loc : startingLocations)
    {
        sum += ProcessPath(Map, loc, locIndex++, trailheads);
    }
    return sum;
};
