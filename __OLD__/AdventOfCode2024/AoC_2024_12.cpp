#include "pch.h"
#include "AoC_2024_12.h"

using namespace aoc;
using namespace aoc::maps;

#define USE_RECURSION

typedef std::unordered_map<int, int64_t> RegionInfo;
typedef std::unordered_map<int, std::unordered_map<int, std::vector<std::pair<int, int>>>> Segments;


aoc::maps::Map2d<int> filledMap;
Segments regionVerticalForwardSegments;
Segments regionVerticalBackSegments;
Segments regionHorizontalForwardSegments;
Segments regionHorizontalBackSegments;
std::unordered_map<int, int> regionSidesCount;
std::unordered_map<int, char> idToChar;
RegionInfo RegionsAreas;
RegionInfo RegionsFenceCount;

void floodFill(const aoc::maps::Map2d<char>& input, const char regionChar, int x, int y, int id);
void floodFillRecurse(const aoc::maps::Map2d<char>& input, const char regionChar, int x, int y, int id);
void idRegions(const aoc::maps::Map2d<char>& input);
void raymarch();
std::pair<int64_t, int64_t> countCosts();

const int64_t AoC_2024_12::Step1()
{
    TIME_PART;

    regionVerticalForwardSegments.clear();
    regionVerticalBackSegments.clear();
    regionHorizontalForwardSegments.clear();
    regionHorizontalBackSegments.clear();
    regionSidesCount.clear();
    RegionsFenceCount.clear();
    RegionsAreas.clear();

    // load input
    aoc::maps::Map2d<char> input;
    aoc::aocs >> input;

    // flood fill. it will also save region areas.
    filledMap = aoc::maps::Map2d<int>(input.Width, input.Height, true);
    idRegions(input);

    // raymarch to find fences
    raymarch();

    // count all regions
    const auto& result = countCosts();

    // profit :)
    part2Sum = result.second;
    return result.first;
}

const int64_t AoC_2024_12::Step2()
{
    TIME_PART;
    return part2Sum;
};

void floodFill(const aoc::maps::Map2d<char>& input, const char regionChar, int x, int y, int id)
{
    std::deque<std::pair<int, int>> queue;
    queue.push_back({ x, y });

    int64_t area{ 0 };

    while(!queue.empty())
    {
        auto [cx, cy] = queue.back();
        queue.pop_back();

        if(!input.WithinBounds(cx, cy)) continue;
        if(filledMap.Get(cx, cy) != 0 || input.Get(cx, cy) != regionChar) continue;

        filledMap.Set(cx, cy, id);
        ++area;

        queue.push_back({ cx + 1, cy });
        queue.push_back({ cx - 1, cy });
        queue.push_back({ cx, cy + 1 });
        queue.push_back({ cx, cy - 1 });
    }
    RegionsAreas[id] = area;
}
void floodFillRecurse(const aoc::maps::Map2d<char>& input, const char regionChar, int x, int y, int id)
{
    if(!input.WithinBounds(x, y)) return;
    if(filledMap.Get(x, y) != 0) return;
    if(input.Get(x, y) != regionChar) return;

    filledMap.Set(x, y, id);
    ++RegionsAreas[id];

    floodFillRecurse(input, regionChar, x + 1, y, id);
    floodFillRecurse(input, regionChar, x - 1, y, id);
    floodFillRecurse(input, regionChar, x, y + 1, id);
    floodFillRecurse(input, regionChar, x, y - 1, id);
}
void idRegions(const aoc::maps::Map2d<char>& input)
{
    int id = 0;
    for(int y = 0; y < input.Height; ++y)
    {
        for(int x = 0; x < input.Width; ++x)
        {
            if(filledMap.Get(x, y) == 0)
            {
                ++id;
                idToChar[id] = input.Get(x, y);
#ifdef USE_RECURSION
                floodFillRecurse(input, input.Get(x, y), x, y, id);
#else
                floodFill(input, filledMap, input.Get(x, y), x, y, id);
#endif
            }
        }
    }
}

void connectOrCreateVerticalSegment(int id, int x, int y1, int y2, bool forward)
{
    auto& segments = (forward ? regionVerticalForwardSegments : regionVerticalBackSegments)[id];
    auto& forX = segments[x];
    for(int i = 0; i < forX.size(); ++i)
    {
        auto& l = forX[i];
        if(l.second == y1)
        {
            forX[i] = { l.first, y2 };
            return;
        }
    }
    forX.push_back({ y1, y2 });
    regionSidesCount[id]++;
}
void connectOrCreateHorizontalSegment(int id, int x1, int x2, int y, bool forward)
{
    auto& segments = (forward ? regionHorizontalForwardSegments : regionHorizontalBackSegments)[id];
    auto& forY = segments[y];
    for(int i = 0; i < forY.size(); ++i)
    {
        auto& l = forY[i];
        if(l.second == x1)
        {
            forY[i] = { l.first, x2 };
            return;
        }
    }
    forY.push_back({ x1, x2 });
    regionSidesCount[id]++;
}
void raymarch()
{
    for(int y = 0; y < filledMap.Height; ++y)
    {
        const auto firstId = filledMap.Get(0, y);
        RegionsFenceCount[firstId]++;
        connectOrCreateVerticalSegment(firstId, -1, y, y + 1, true);

        for(int x = 0; x < filledMap.Width - 1; ++x)
        {
            const int id1 = filledMap.Get(x, y);
            const int id2 = filledMap.Get(x + 1, y);

            if(id1 != id2)
            {
                RegionsFenceCount[id1]++;
                RegionsFenceCount[id2]++;
                connectOrCreateVerticalSegment(id1, x, y, y + 1, false);
                connectOrCreateVerticalSegment(id2, x, y, y + 1, true);
            }
        }

        const auto lastId = filledMap.Get(filledMap.Width - 1, y);
        RegionsFenceCount[lastId]++;
        connectOrCreateVerticalSegment(lastId, filledMap.Width - 1, y, y + 1, false);
    }
    for(int x = 0; x < filledMap.Width; ++x)
    {
        const auto firstId = filledMap.Get(x, 0);
        RegionsFenceCount[firstId]++;
        connectOrCreateHorizontalSegment(firstId, x, x + 1, -1, true);

        for(int y = 0; y < filledMap.Height - 1; ++y)
        {
            const int id1 = filledMap.Get(x, y);
            const int id2 = filledMap.Get(x, y + 1);

            if(id1 != id2)
            {
                RegionsFenceCount[id1]++;
                RegionsFenceCount[id2]++;

                connectOrCreateHorizontalSegment(id1, x, x + 1, y, false);
                connectOrCreateHorizontalSegment(id2, x, x + 1, y, true);
            }
        }
        const auto lastId = filledMap.Get(x, filledMap.Height - 1);
        RegionsFenceCount[lastId]++;
        connectOrCreateHorizontalSegment(lastId, x, x + 1, filledMap.Height - 1, false);
    }

}

std::pair<int64_t, int64_t> countCosts()
{
    int64_t sum = 0;
    for(const auto& p1 : RegionsFenceCount)
    {
        auto id = p1.first;
        sum += RegionsAreas[id] * p1.second;
    }

    // also count for part 2
    int64_t sum2 = 0;
    for(const auto& p1 : RegionsFenceCount)
    {
        auto id = p1.first;
        auto sc = regionSidesCount[id];
        sum2 += RegionsAreas[id] * sc;
    }
    return { sum, sum2 };
}
