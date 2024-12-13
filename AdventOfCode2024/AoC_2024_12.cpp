#include "pch.h"
#include "AoC_2024_12.h"

using namespace aoc;
using namespace aoc::maps;

#define USE_RECURSION

typedef std::unordered_map<int, int64_t> RegionInfo;
typedef std::unordered_map<int, std::pair<int, int>> RegionLocations;
typedef std::unordered_map<int, std::unordered_map<int, vector<int>>> RegionFenceLocations;

struct FLine
{
    int x1, y1, x2, y2, used;

    FLine(int x1, int y1, int x2, int y2)
        : x1(x1), y1(y1), x2(x2), y2(y2), used(0)
    {
    }
};


RegionFenceLocations HorizontalFencesLocations;
RegionFenceLocations VerticalFencesLocations;
std::unordered_map<int, char> idToChar;
std::unordered_map<int, vector<FLine>> regionRawLines;


void floodFill(const aoc::maps::Map2d<char>& input, const char regionChar, int x, int y, int id, RegionInfo& RegionsAreas);
void floodFillRecurse(const aoc::maps::Map2d<char>& input, const char regionChar, int x, int y, int id, RegionInfo& RegionsAreas);
void idRegions(const aoc::maps::Map2d<char>& input, RegionInfo& RegionsAreas, RegionLocations& RegionStartingLocations);
void countFencesWithRayMarch(RegionInfo& RegionsFenceCount);

void buildShapeAndCountCorners();

aoc::maps::Map2d<int> filledMap;

const int64_t AoC_2024_12::Step1()
{
    TIME_PART;

    // load input
    aoc::maps::Map2d<char> input;
    aoc::aocs >> input;

    // flood fill (and store region starting locations)
    filledMap = aoc::maps::Map2d<int>(input.Width, input.Height, true);
    RegionInfo RegionsAreas;
    RegionLocations RegionStartLocations;
    idRegions(input, RegionsAreas, RegionStartLocations);

    // raymarch to find fences
    RegionInfo RegionsFenceCount;
    countFencesWithRayMarch(RegionsFenceCount);

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
    buildShapeAndCountCorners();
    return 0;
};

void floodFill(const aoc::maps::Map2d<char>& input, const char regionChar, int x, int y, int id, RegionInfo& RegionsAreas)
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

void floodFillRecurse(const aoc::maps::Map2d<char>& input, const char regionChar, int x, int y, int id, RegionInfo& RegionsAreas)
{
    if(!input.WithinBounds(x, y)) return;
    if(filledMap.Get(x, y) != 0) return;
    if(input.Get(x, y) != regionChar) return;

    filledMap.Set(x, y, id);
    ++RegionsAreas[id];

    floodFillRecurse(input, regionChar, x + 1, y, id, RegionsAreas);
    floodFillRecurse(input, regionChar, x - 1, y, id, RegionsAreas);
    floodFillRecurse(input, regionChar, x, y + 1, id, RegionsAreas);
    floodFillRecurse(input, regionChar, x, y - 1, id, RegionsAreas);
}

void idRegions(const aoc::maps::Map2d<char>& input, RegionInfo& RegionsAreas, RegionLocations& RegionStartingLocations)
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
                idToChar[id] = input.Get(x, y);
                RegionStartingLocations[id] = { x, y };
#ifdef USE_RECURSION
                floodFillRecurse(input, input.Get(x, y), x, y, id, RegionsAreas);
#else
                floodFill(input, filledMap, input.Get(x, y), x, y, id, RegionsAreas);
#endif
            }
        }
    }
}

void countFencesWithRayMarch(RegionInfo& RegionsFenceCount)
{
    for(int y = 0; y < filledMap.Height; ++y)
    {
        const auto firstId = filledMap.Get(0, y);
        RegionsFenceCount[firstId]++;
        regionRawLines[firstId].push_back(FLine(0, y, 0, y + 1));

        for(int x = 0; x < filledMap.Width - 1; ++x)
        {
            const int id1 = filledMap.Get(x, y);
            const int id2 = filledMap.Get(x + 1, y);

            if(id1 != id2)
            {
                RegionsFenceCount[id1]++;
                RegionsFenceCount[id2]++;

                regionRawLines[id1].push_back(FLine(x, y, x, y + 1));
                regionRawLines[id2].push_back(FLine(x, y, x, y + 1));
            }
        }

        const auto lastId = filledMap.Get(filledMap.Width - 1, y);
        RegionsFenceCount[lastId]++;
        regionRawLines[lastId].push_back(FLine(filledMap.Width - 1, y, filledMap.Width - 1, y + 1));
    }
    for(int x = 0; x < filledMap.Width; ++x)
    {
        const auto firstId = filledMap.Get(x, 0);
        RegionsFenceCount[firstId]++;
        regionRawLines[firstId].push_back(FLine(x, 0, x + 1, 0));


        for(int y = 0; y < filledMap.Height - 1; ++y)
        {
            const int id1 = filledMap.Get(x, y);
            const int id2 = filledMap.Get(x, y + 1);

            if(id1 != id2)
            {
                RegionsFenceCount[id1]++;
                RegionsFenceCount[id2]++;

                regionRawLines[id1].push_back(FLine(x, y, x + 1, y));
                regionRawLines[id2].push_back(FLine(x, y, x + 1, y));
            }
        }
        const auto lastId = filledMap.Get(x, filledMap.Height - 1);
        RegionsFenceCount[lastId]++;
        regionRawLines[lastId].push_back(FLine(x, filledMap.Height - 1, x + 1, filledMap.Height - 1));
    }

}


bool linesShareAPoint(const FLine& line1, const FLine& l2)
{
    return false;
}
bool detectDirectionChange(const FLine& line1, const FLine& l2)
{
    return false;
}
FLine findAnyUnusedLine(const vector<FLine>& lines)
{
    for(const auto& line : lines)
    {
        if(!line.used)return line;
    }
    return FLine(0, 0, 0, 0);
}
FLine findNextLine(const vector<FLine>& lines, FLine lastLine)
{
    for(const auto& line : lines)
    {
        if(!line.used)return line;
    }
    return FLine(0, 0, 0, 0);
}
bool hasAnyUnusedLine(const vector<FLine>& lines)
{
    for(const auto& line : lines)
    {
        if(!line.used) return true;
    }
    return false;
}
void buildShapeAndCountCorners()
{
    auto lines = regionRawLines[9];
    return;

    int dirChanges = 0;
    while(hasAnyUnusedLine(lines))
    {
        auto l1 = findAnyUnusedLine(lines);

        while(true)
        {
            auto l2 = findNextLine(lines, l1);
            if(detectDirectionChange(l1, l2))
                dirChanges++;
        }
    }

}

