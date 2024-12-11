#include "pch.h"
#include "AoC_2024_10.h"
#include <cstdio>  // for C file I/O

#define USE_RECURSIVE
#define FIND_START_LOCATIONS_INSIDE_LOOPS
#define USE_PARALELL

using namespace std;
using namespace aoc;
using namespace aoc::maps;

int64_t ProcessPath(const single_digit_map& map, const Location2di& location, set<Location2di>* trailheads)
{
#ifndef USE_RECURSIVE
    std::stack<Location2di>stack;

    stack.push(location);
    int64_t sum = 0;

    while(!stack.empty())
    {
        Location2di loc = stack.top();
        stack.pop();
        int w = map.Width;
        int h = map.Height;
        int x = loc.x;
        int y = loc.y;

        int8_t c = map.Map[x + y * w];
        if(c == 9)
        {
            trailheads->emplace(loc);
            sum += 1;
            continue;
        }

        if(x - 1 >= 0 && map.Map[(x - 1) + y * w] == c + 1)
        {
            stack.push(Location2di(x - 1, y));
        }
        if(x + 1 < w && map.Map[(x + 1) + y * w] == c + 1)
        {
            stack.push(Location2di(x + 1, y));
        }
        if(y - 1 >= 0 && map.Map[x + (y - 1) * w] == c + 1)
        {
            stack.push(Location2di(x, y - 1));
        }
        if(y + 1 < h && map.Map[x + (y + 1) * w] == c + 1)
        {
            stack.push(Location2di(x, y + 1));
        }
    }

    return sum;
#else

    int w = map.Width;
    int h = map.Height;

    int x = location.x;
    int y = location.y;

    int8_t c = map.Map[x + y * w];
    if(c == 9)
    {
        trailheads->emplace(location);
        return 1;
    }

    int64_t sum = 0;
    Location2di loc2(x - 1, y);
    if(loc2.x >= 0 && map.Map[loc2.x + loc2.y * w] == c + 1)
        sum += ProcessPath(map, loc2, trailheads);

    loc2.x = x + 1;
    if(loc2.x < w && map.Map[loc2.x + loc2.y * w] == c + 1)
        sum += ProcessPath(map, loc2, trailheads);

    loc2.x = x;
    loc2.y = y - 1;
    if(loc2.y >= 0 && map.Map[loc2.x + loc2.y * w] == c + 1)
        sum += ProcessPath(map, loc2, trailheads);

    loc2.y = y + 1;
    if(loc2.y < h && map.Map[loc2.x + loc2.y * w] == c + 1)
        sum += ProcessPath(map, loc2, trailheads);

    return sum;
#endif

}

#ifdef FIND_START_LOCATIONS_INSIDE_LOOPS
int64_t FindStartsAndProcessPath(const single_digit_map& map, const int row, int64_t& trailSize)
{
    //cout << "Processing row " << row << endl;
    int x = 0, w = map.Width, yw = row * w;
    int64_t sum = 0;
    int64_t trail_sum = 0;
    for(; x < w; ++x)
    {
        if(map.Map[x + yw] == 0)
        {
            set<Location2di> trailheads;
            sum += ProcessPath(map, Location2di(x, row), &trailheads);
            trail_sum += trailheads.size();
        }
    }
    trailSize = trail_sum;
    return sum;
}
#endif


const int64_t AoC_2024_10::Step1_Internal()
{
    //if(!IsTest())return 0;

    string fileName_wu = GetFileName();

    single_digit_map Map_wu;
    AoCStream(fileName_wu) >> Map_wu;

    int locIndex_wu = 0;
    int64_t sum2_wu = 0;
    int64_t sum_wu = 0;

    TIME_PART;

#ifndef FIND_START_LOCATIONS_INSIDE_LOOPS
    vector<Location2di> startingLocations_wu;
    Map_wu.select_value(0) >> startingLocations_wu;
    parallel_for_each(startingLocations_wu.begin(), startingLocations_wu.end(), [&sum_wu, &sum2_wu, &Map_wu, &locIndex_wu](const Location2di& location)
        {
            set<Location2di> trailheads;
            InterlockedAdd64(&sum2_wu, ProcessPath(Map_wu, location, &trailheads));
            InterlockedAdd64(&sum_wu, trailheads.size());
        });
#else

#ifdef USE_PARALELL
    parallel_for(0, Map_wu.Height, 1, [&sum_wu, &sum2_wu, &Map_wu, &locIndex_wu](size_t i)
        {
            int64_t sum_trail;
            InterlockedAdd64(&sum2_wu, FindStartsAndProcessPath(Map_wu, (int)i, sum_trail));
            InterlockedAdd64(&sum_wu, sum_trail);
        });
#else
    for(int y = 0; y < Map_wu.Height; ++y)
    {
        set<Location2di> trailheads;
        int64_t sum_trail;
        InterlockedAdd64(&sum2_wu, FindStartsAndProcessPath(Map_wu, (int)y, sum_trail));
        InterlockedAdd64(&sum_wu, sum_trail);
    };
#endif
#endif

    if(IsTest())
        part2_Test = sum2_wu;
    else
        part2_Live = sum2_wu;

    return sum_wu;


}
const int64_t AoC_2024_10::Step1()
{
    Step1_Internal();  // first loop will warm up the caches
    return Step1_Internal();
};
const int64_t AoC_2024_10::Step2()
{
    if(IsTest()) return part2_Test;
    else return part2_Live;
};





/*
OLD CODE


#include "pch.h"
#include "AoC_2024_10.h"

using namespace std;
using namespace aoc;
using namespace aoc::maps;

int64_t ProcessPath(const single_digit_map& map, const Location2di& location, int locIndex, unordered_map<int, set<Location2di>>* trailheads)
{
    if(map.Get(location) == 9)
    {
        if(trailheads) (*trailheads)[locIndex].emplace(location);
        return 1;
    }
    std::vector<Location2di> neighborLocations;
    map.get_neighbors(location, Cardinal, 1, [](const Location2di& loc, const Location2di& n_loc, int8_t _val, int8_t _n_val, Directions _dir)
        {
            return _val + 1 == _n_val;
        }) >> neighborLocations;

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
        ProcessPath(Map, loc, locIndex++, &trailheads);
    }

    int sum = 0;
    for(const auto& pair : trailheads)
    {
        sum += pair.second.size();
    }
    return sum;

};
const int64_t AoC_2024_10::Step2()
{
    single_digit_map Map;
    AoCStream(GetFileName()) >> Map;

    TIME_PART;

    vector<Location2di> startingLocations;
    Map.select_value(0) >> startingLocations;


    int locIndex = 0;
    int64_t sum = 0;
    for(const auto& loc : startingLocations)
    {
        sum += ProcessPath(Map, loc, locIndex++, nullptr);
    }
    return sum;
};




*/

// old sum loops that work but are slow

// ProcessPath()
/*

/*
    return std::accumulate(neighborLocations.begin(), neighborLocations.end(), int64_t(0), [locIndex, map, &trailheads](int64_t acc, const Location2di& location)
        {
            return acc + ProcessPath(map, location, locIndex, trailheads);
        });
*/
// Step1()
    // return std::accumulate(trailheads.begin(), trailheads.end(), int64_t(0), [](int64_t acc, const auto& pair) { return acc + pair.second.size(); });

// Step2()
    /* 
    int locIndex = 0;
    return std::accumulate(startingLocations.begin(), startingLocations.end(), int64_t(0), [&locIndex, Map, &trailheads](int64_t acc, const Location2di& location)
        {
            return acc + ProcessPath(Map, location, locIndex++, trailheads);
        });
    */