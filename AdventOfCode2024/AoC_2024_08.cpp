#include "AoC_2024_08.h"

__forceinline bool WithinBounds(int x, int y, int width, int height)
{
    return !(x < 0 || y < 0 || x >= width || y >= height);
}
__forceinline const char GetAt(int x, int y, int width, int height, string& Map)
{
    if(!WithinBounds(x, y, width, height))
        return '.';
    return Map[x + y * width];
}
__forceinline void SetAt(int x, int y, int width, int height, vector<uint8_t>& AntinodeMap)
{
    if(!WithinBounds(x, y, width, height))
        return;
    AntinodeMap[x + y * width] = 1;
}

struct AntennaLocation
{
    int x;
    int y;

    AntennaLocation(int x, int y)
        : x(x), y(y)
    {
    }
};

typedef unordered_map<char, vector<AntennaLocation>> AntennaMap;
const int64_t AoC_2024_08::Step1()
{
    int width = 0, height = 0;

    auto string = ReadStringFromFile(1, height, width);
    AntennaMap antennaMap;

    // scan for all antennas
    for(int y = 0; y < height; ++y)
    {
        for(int x = 0; x < width; ++x)
        {
            auto currentChar = GetAt(x, y, width, height, string);
            if(currentChar == '.') continue;
            antennaMap[currentChar].push_back(AntennaLocation(x, y));
        }
    }

    // the simplest way to hold antinodes is bitmap
    auto AntinodeMap = vector<uint8_t>(string.size());

    for(const auto& antennaType : antennaMap)
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
                SetAt(_1stAntennaPos.x + (dx * 2), _1stAntennaPos.y + (dy * 2), width, height, AntinodeMap);

                // reverse
                SetAt(_1stAntennaPos.x - dx, _1stAntennaPos.y - dy, width, height, AntinodeMap);
            }
        }
    }
    return std::accumulate(AntinodeMap.begin(), AntinodeMap.end(), 0);
}



const int64_t AoC_2024_08::Step2()
{
    int width = 0, height = 0;

    auto string = ReadStringFromFile(2, height, width);
    AntennaMap antennaMap;

    // scan for all antennas
    for(int y = 0; y < height; ++y)
    {
        for(int x = 0; x < width; ++x)
        {
            auto currentChar = GetAt(x, y, width, height, string);
            if(currentChar == '.') continue;
            antennaMap[currentChar].push_back(AntennaLocation(x, y));
        }
    }

    // the simplest way to hold antinodes is bitmap
    auto AntinodeMap = vector<uint8_t>(string.size());

    for(const auto& antennaType : antennaMap)
    {
        auto locations = antennaType.second;
        for(int i = 0; i < locations.size(); ++i)
        {
            auto _1stAntennaPos = locations[i];
            for(int i2 = i + 1; i2 < antennaType.second.size(); ++i2)
            {
                auto _2ndAntennaPos = locations[i2];

                // we put antinodes on both antenna locations (second will override itself later, but ok)
                SetAt(_1stAntennaPos.x, _1stAntennaPos.y, width, height, AntinodeMap);
                SetAt(_2ndAntennaPos.x, _2ndAntennaPos.y, width, height, AntinodeMap);

                // get distance
                int dx = _2ndAntennaPos.x - _1stAntennaPos.x;
                int dy = _2ndAntennaPos.y - _1stAntennaPos.y;

                // extend from first antenna as long as both coordinates are within bounds
                int ex = _1stAntennaPos.x + dx;
                int ey = _1stAntennaPos.y + dy;
                while(WithinBounds(ex, ey, width, height))
                {
                   SetAt(ex, ey, width, height, AntinodeMap);
                   ex += dx;
                   ey += dy;
                }

                // reverse as long as both coordinates are within bounds
                ex = _1stAntennaPos.x - dx;
                ey = _1stAntennaPos.y - dy;
                while(WithinBounds(ex, ey, width, height))
                {
                   SetAt(ex, ey, width, height, AntinodeMap);
                   ex -= dx;
                   ey -= dy;
                }
            }
        }
    }

    return std::accumulate(AntinodeMap.begin(), AntinodeMap.end(), 0);
}
