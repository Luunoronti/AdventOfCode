#include "pch.h"
#include "AoC_2024_10.h"

using namespace std;
using namespace aoc;
using namespace aoc::maps;


int64_t EndOfPathFound(const single_digit_map& map, const Location2di& location)
{
    int8_t val = map.Get(location);
    cout << (int)val << " ";
    if(val == 9) 
        return 1;

    std::vector<Location2di> nLocs;
    map.get_neighbors(location, Cardinal, 1, [](const Location2di& loc, const Location2di& nloc, int8_t _val, int8_t _n_val, Directions _dir)
        {
            return _val + 1 == _n_val;
        }) >> nLocs;

    int64_t sum = 0;
    for(const auto& loc : nLocs)
    {
        sum += EndOfPathFound(map, loc);
    }
    return sum;
}


const int64_t AoC_2024_10::Step1()
{
    single_digit_map Map;
    vector<Location2di> Locations;
    AoCStream(GetFileName()) >> Map;

    TIME_PART;

    Map.select_value(0) >> Locations;
    
    // let's use recursion to look for all possible paths
    // will think of non recursive method later
    int64_t sum = 0;
    for(const auto& loc : Locations)
    {
        sum += EndOfPathFound(Map, loc);
    }
    return sum;

};
const int64_t AoC_2024_10::Step2()
{
    single_digit_map Map;
    AoCStream(GetFileName()) >> Map;

    TIME_PART;
    return 0;
};