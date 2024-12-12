#include "pch.h"
#include "AoC_2024_12.h"

using namespace aoc;
using namespace aoc::maps;

#define USE_RECURSION

typedef std::unordered_map<int, int64_t> RegionInfo;
typedef std::unordered_map<int, std::pair<int, int>> RegionLocations;
typedef std::unordered_map<int, std::unordered_map<int, vector<int>>> RegionFenceLocations;

RegionFenceLocations HorizontalFencesLocations;
RegionFenceLocations VerticalFencesLocations;


void floodFill(const aoc::maps::Map2d<char>& input, aoc::maps::Map2d<int>& filledMap, const char regionChar, int x, int y, int id, RegionInfo& RegionsAreas);
void floodFillRecurse(const aoc::maps::Map2d<char>& input, aoc::maps::Map2d<int>& filledMap, const char regionChar, int x, int y, int id, RegionInfo& RegionsAreas);
void labelRegions(const aoc::maps::Map2d<char>& input, aoc::maps::Map2d<int>& filledMap, RegionInfo& RegionsAreas, RegionLocations& RegionStartingLocations);
void countFencesWithRayMarch(aoc::maps::Map2d<int>& filledMap, RegionInfo& RegionsFenceCount);

void countCorners(aoc::maps::Map2d<int>& filledMap);

aoc::maps::Map2d<int> filledMap;

const int64_t AoC_2024_12::Step1()
{
    TIME_PART;

    // load input
    aoc::maps::Map2d<char> input;
    aoc::aocs >> input;

    // flood fill (and also, store region starting locations)
    filledMap = aoc::maps::Map2d<int>(input.Width, input.Height, true);
    RegionInfo RegionsAreas;
    RegionLocations RegionStartLocations;
    labelRegions(input, filledMap, RegionsAreas, RegionStartLocations);

    // raymarch to find fences
    RegionInfo RegionsFenceCount;
    countFencesWithRayMarch(filledMap, RegionsFenceCount);

    // count all regions
    int64_t sum = 0;
    for(const auto& p1 : RegionsFenceCount)
    {
        auto id = p1.first;
        sum += RegionsAreas[id] * p1.second;
    }

    // profit :)
    return sum;
}

std::unordered_map<int, int> cornersFound;

const int64_t AoC_2024_12::Step2()
{
    TIME_PART;

    return 0;
};

void floodFill(const aoc::maps::Map2d<char>& input, aoc::maps::Map2d<int>& filledMap, const char regionChar, int x, int y, int id, RegionInfo& RegionsAreas)
{
    std::deque<std::pair<int, int>> queue;
    queue.push_back({ x, y });

    int64_t area{ 0 };

    while(!queue.empty())
    {
        auto [cx, cy] = queue.front();
        queue.pop_front();

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

void floodFillRecurse(const aoc::maps::Map2d<char>& input, aoc::maps::Map2d<int>& filledMap, const char regionChar, int x, int y, int id, RegionInfo& RegionsAreas)
{
    if(!input.WithinBounds(x, y)) return;
    if(filledMap.Get(x, y) != 0) return;
    if(input.Get(x, y) != regionChar) return;

    filledMap.Set(x, y, id);
    ++RegionsAreas[id];

    floodFillRecurse(input, filledMap, regionChar, x + 1, y, id, RegionsAreas);
    floodFillRecurse(input, filledMap, regionChar, x - 1, y, id, RegionsAreas);
    floodFillRecurse(input, filledMap, regionChar, x, y + 1, id, RegionsAreas);
    floodFillRecurse(input, filledMap, regionChar, x, y - 1, id, RegionsAreas);
}

void labelRegions(const aoc::maps::Map2d<char>& input, aoc::maps::Map2d<int>& filledMap, RegionInfo& RegionsAreas, RegionLocations& RegionStartingLocations)
{
    int id = 0;
    RegionStartingLocations.clear();

    for(int y = 0; y < input.Height; ++y)
    {
        for(int x = 0; x < input.Width; ++x)
        {
            if(filledMap.Get(x, y) == 0)
            {
                ++id;
                RegionStartingLocations[id] = { x, y };
#ifdef USE_RECURSION
                floodFillRecurse(input, filledMap, input.Get(x, y), x, y, id++, RegionsAreas);
#else
                floodFill(input, filledMap, input.Get(x, y), x, y, id++, RegionsAreas);
#endif
            }
        }
    }
}

void countFencesWithRayMarch(aoc::maps::Map2d<int>& filledMap, RegionInfo& RegionsFenceCount)
{
    for(int y = 0; y < filledMap.Height; ++y)
    {
        const auto firstId = filledMap.Get(0, y);
        RegionsFenceCount[firstId]++;
        VerticalFencesLocations[firstId][y].push_back(0);

        for(int x = 0; x < filledMap.Width - 1; ++x)
        {
            const int id1 = filledMap.Get(x, y);
            const int id2 = filledMap.Get(x + 1, y);

            if(id1 != id2)
            {
                RegionsFenceCount[id1]++;
                RegionsFenceCount[id2]++;

                VerticalFencesLocations[id1][y].push_back(x);
                VerticalFencesLocations[id2][y].push_back(x);
            }
        }

        const auto lastId = filledMap.Get(filledMap.Width - 1, y);
        RegionsFenceCount[lastId]++;
        VerticalFencesLocations[lastId][y].push_back(filledMap.Width - 1);
    }
    for(int x = 0; x < filledMap.Width; ++x)
    {
        const auto firstId = filledMap.Get(x, 0);
        RegionsFenceCount[firstId]++;
        HorizontalFencesLocations[firstId][x].push_back(0);

        for(int y = 0; y < filledMap.Height - 1; ++y)
        {
            const int id1 = filledMap.Get(x, y);
            const int id2 = filledMap.Get(x, y + 1);

            if(id1 != id2)
            {
                RegionsFenceCount[id1]++;
                RegionsFenceCount[id2]++;

                HorizontalFencesLocations[id1][x].push_back(y);
                HorizontalFencesLocations[id2][x].push_back(y);
            }
        }
        const auto lastId = filledMap.Get(x, filledMap.Height - 1);
        RegionsFenceCount[lastId]++;
        HorizontalFencesLocations[lastId][filledMap.Height - 1].push_back(filledMap.Width - 1);
    }

}


/*
// test
    /*for(const auto& p1 : RegionsFenceCount)
    {
        auto id = p1.first;
        testMap[id] = { RegionsAreas[id], p1.second };
    }


    for(int y = 0; y < filledMap.Height; ++y)
    {
        for(int x = 0; x < filledMap.Width; ++x)
        {
            std::cout << std::setw(3) << std::setfill(' ') << filledMap.Get(x, y) << " ";
        }
        std::cout << std::endl;
    }
    std::cout << std::endl;



for(const auto& p1 : RegionsFenceCount)
    {
        auto id = p1.first;
        testMap[id] = { RegionsAreas[id], p1.second };
    }



    for(int y = 0; y < input.Height; ++y)
    {
        for(int x = 0; x < input.Width; ++x)
        {
            std::cout << input.Get(x, y) << " ";
        }
        std::cout << std::endl;
    }

    std::cout << std::endl;
    std::cout << std::endl;

    for(int y = 0; y < filledMap.Height; ++y)
    {
        for(int x = 0; x < filledMap.Width; ++x)
        {
            std::cout << std::setw(2) << std::setfill(' ') << filledMap.Get(x, y) << " ";
        }
        std::cout << std::endl;
    }

*/