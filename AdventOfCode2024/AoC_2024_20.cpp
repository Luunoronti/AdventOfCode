#include "pch.h"
#include "AoC_2024_20.h"

using namespace aoc;
using namespace aoc::maps;

inline int GetLengthSave(int totalLength, int cheatStart, int cheatEnd)
{
    if(cheatStart == -1 || cheatEnd == -1)
        return 0;
    return totalLength - (cheatStart + totalLength - cheatEnd);
}
const int64_t AoC_2024_20::Step1()
{
    TIME_PART;
    Map2d<char> input;
    AoCStream() >> input;

    int sx, sy;
    input.find('S', sx, sy);

    // prepare integer map
    Map2d<int> map = input.ToMap<int>([](const char& in) {return in == '#' ? -1 : 0; });

    int fillIndex = 0;
    map.fill({ sx, sy }, [](int v, int, int) { return v != -1; }, [&fillIndex](int, int, int) {return ++fillIndex; });

    //map.print(
    //    [](int, int, int v) { return v == -1 ? ' ' : 'o'; },
    //    [&fillIndex](int, int, int v) { return mutil::Vector3(0, 0.3f + (0.7f * ((float)v / (float)fillIndex)), 0); },
    //    [](int, int, int v) { return v == -1 ? mutil::Vector3(0.4f, 0.0f, 0.1f) : mutil::Vector3(0, 0, 0); }
    //);

    int totalCheatsOver100 = 0;
    auto countFunc = [&fillIndex, &totalCheatsOver100](Map2d<int>& map, int startVal, int x, int y)
        {
            if((GetLengthSave(fillIndex, startVal, map.Get(x, y, -1)) - 2) >= 100)
                totalCheatsOver100++;
        };
    map.for_each([&countFunc](Map2d<int>& map, int x, int y, int val)
        {
            if(val == -1) return;

            int sv = val;

            countFunc(map, val, x + 2, y);
            countFunc(map, val, x - 2, y);
            countFunc(map, val, x, y + 2);
            countFunc(map, val, x, y - 2);
        });


    return IsTest() ? 1 : totalCheatsOver100;
};
const int64_t AoC_2024_20::Step2()
{
    TIME_PART;
    Map2d<char> input;
    AoCStream() >> input;

    int sx, sy;
    input.find('S', sx, sy);

    // prepare integer map
    Map2d<int> map = input.ToMap<int>([](const char& in) {return in == '#' ? -1 : 0; });

    int fillIndex = 0;
    map.fill({ sx, sy }, [](int v, int, int) { return v != -1; }, [&fillIndex](int, int, int) {return ++fillIndex; });

    int totalCheatsOver100 = 0;
    auto countFunc = [&fillIndex, &totalCheatsOver100](Map2d<int>& map, int startVal, int x, int y, int len)
        {
            if(len <= 20 && (GetLengthSave(fillIndex, startVal, map.Get(x, y, -1)) - len) >= 100)
                totalCheatsOver100++;
        };
    map.for_each([&countFunc](Map2d<int>& map, int x, int y, int val)
        {
            if(val == -1) return;
            int sv = val;
            for(int y2 = 0; y2 < map.Height; y2++)
                for(int x2 = 0; x2 < map.Width; x2++)
                    countFunc(map, val, x2, y2, std::abs(x - x2) + std::abs(y - y2));
        });
    return IsTest() ? 1 : totalCheatsOver100;
};