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

void AoC_2024_08::OnInitTestingTests() { Input = ReadStringFromFile(1, Height, Width); }
void AoC_2024_08::OnInitLiveTests() { Input = ReadStringFromFile(1, Height, Width); }

typedef unordered_map<char, vector<AntennaLocation>> AntennaMap;
const int64_t AoC_2024_08::Step1()
{

    dout << "This is first debug message " << RED+BLINK << " with some blinking" << RESET << endl;

    AntennaMap antennaMap;

    // scan for all antennas
    for(int y = 0; y < Height; ++y)
    {
        for(int x = 0; x < Width; ++x)
        {
            auto currentChar = GetAt(x, y, Width, Height, Input);
            if(currentChar == '.') continue;
            antennaMap[currentChar].push_back(AntennaLocation(x, y));
        }
    }

    // the simplest way to hold antinodes is bitmap
    auto AntinodeMap = vector<uint8_t>(Input.size());

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
                SetAt(_1stAntennaPos.x + (dx * 2), _1stAntennaPos.y + (dy * 2), Width, Height, AntinodeMap);

                // reverse
                SetAt(_1stAntennaPos.x - dx, _1stAntennaPos.y - dy, Width, Height, AntinodeMap);
            }
        }
    }
    return std::accumulate(AntinodeMap.begin(), AntinodeMap.end(), 0);
}



const int64_t AoC_2024_08::Step2()
{
    AntennaMap antennaMap;
    // scan for all antennas
    for(int y = 0; y < Height; ++y)
    {
        for(int x = 0; x < Width; ++x)
        {
            auto currentChar = GetAt(x, y, Width, Height, Input);
            if(currentChar == '.') continue;
            antennaMap[currentChar].push_back(AntennaLocation(x, y));
        }
    }

    // the simplest way to hold antinodes is bitmap
    auto AntinodeMap = vector<uint8_t>(Input.size());

    for(const auto& antennaType : antennaMap)
    {
        auto locations = antennaType.second;

        for(int i = 0; i < locations.size(); ++i)
        {
            auto _1stAntennaPos = locations[i];

            // we put antinode on antenna location itself
            SetAt(_1stAntennaPos.x, _1stAntennaPos.y, Width, Height, AntinodeMap);

            for(int i2 = i + 1; i2 < antennaType.second.size(); ++i2)
            {
                auto _2ndAntennaPos = locations[i2];

                // get distance
                int dx = _2ndAntennaPos.x - _1stAntennaPos.x;
                int dy = _2ndAntennaPos.y - _1stAntennaPos.y;

                // extend from first antenna as long as both coordinates are within bounds
                int ex = _1stAntennaPos.x + dx;
                int ey = _1stAntennaPos.y + dy;
                while(WithinBounds(ex, ey, Width, Height))
                {
                    SetAt(ex, ey, Width, Height, AntinodeMap);
                    ex += dx;
                    ey += dy;
                }

                // reverse as long as both coordinates are within bounds
                ex = _1stAntennaPos.x - dx;
                ey = _1stAntennaPos.y - dy;
                while(WithinBounds(ex, ey, Width, Height))
                {
                    SetAt(ex, ey, Width, Height, AntinodeMap);
                    ex -= dx;
                    ey -= dy;
                }
            }
        }
    }

    return std::accumulate(AntinodeMap.begin(), AntinodeMap.end(), 0);
}
