#include "pch.h"
#include "AoC_2024_08.h"
using namespace aoc;
using namespace aoc::maps;

typedef aoc::maps::sparse_map<char, Location2di> AntennaMap;
const int64_t AoC_2024_08::Step1()
{
    AntennaMap antennaMap;
    aocs >> omit_char('.') >> antennaMap;
    
    TIME_PART;

    // the simplest way to hold antinodes is bitmap
    auto AntinodeMap = Map2d<int8_t>(antennaMap.Width, antennaMap.Height);

    for(const auto& antennaType : antennaMap.Map)
    {
        auto locations = antennaType.second;
        for(int i = 0; i < locations.size(); ++i)
        {
            auto _1stAntennaPos = locations[i];
            for(int i2 = i + 1; i2 < antennaType.second.size(); ++i2)
            {
                auto _2ndAntennaPos = locations[i2];

                // get distance
                int dx = _2ndAntennaPos.x - _1stAntennaPos.x;
                int dy = _2ndAntennaPos.y - _1stAntennaPos.y;

                // extend
                AntinodeMap.Set(_1stAntennaPos.x + (dx * 2), _1stAntennaPos.y + (dy * 2), 1);

                // reverse
                AntinodeMap.Set(_1stAntennaPos.x - dx, _1stAntennaPos.y - dy, 1);
            }
        }
    }
    return AntinodeMap.accumulate(0);
}

const int64_t AoC_2024_08::Step2()
{
    AntennaMap antennaMap;
    aocs >> omit_char('.') >> antennaMap;

    TIME_PART;

    // the simplest way to hold antinodes is bitmap
    auto AntinodeMap = Map2d<int8_t>(antennaMap.Width, antennaMap.Height);

    for(const auto& antennaType : antennaMap.Map)
    {
        auto locations = antennaType.second;
        TIME("Inner loop timer");
        for(int i = 0; i < locations.size(); ++i)
        {
            auto _1stAntennaPos = locations[i];

            // we put antinode on antenna location itself
            AntinodeMap.Set(_1stAntennaPos.x, _1stAntennaPos.y, 1);

            for(int i2 = i + 1; i2 < antennaType.second.size(); ++i2)
            {
                auto _2ndAntennaPos = locations[i2];

                // get distance
                int dx = _2ndAntennaPos.x - _1stAntennaPos.x;
                int dy = _2ndAntennaPos.y - _1stAntennaPos.y;

                // extend from first antenna as long as both coordinates are within bounds
                int ex = _1stAntennaPos.x + dx;
                int ey = _1stAntennaPos.y + dy;
                while(AntinodeMap.WithinBounds(ex, ey))
                {
                    AntinodeMap.Set(ex, ey, 1);
                    ex += dx;
                    ey += dy;
                }

                // reverse as long as both coordinates are within bounds
                ex = _1stAntennaPos.x - dx;
                ey = _1stAntennaPos.y - dy;
                while(AntinodeMap.WithinBounds(ex, ey))
                {
                    AntinodeMap.Set(ex, ey, 1);
                    ex -= dx;
                    ey -= dy;
                }
            }
        }
    }
    return AntinodeMap.accumulate(0);
}
