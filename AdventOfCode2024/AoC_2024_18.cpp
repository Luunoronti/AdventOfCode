#include "pch.h"
#include "AoC_2024_18.h"

#include <queue>

using namespace mutil;


void ReadLocations(const char* fileName, vector<mutil::IntVector2>& Locations);
int64_t FindShortestPathLen(const aoc::maps::Map2d<uint8_t>& Grid, mutil::IntVector2 Start, mutil::IntVector2 End);
const int64_t AoC_2024_18::Step1()
{
    TIME_PART;

    vector<mutil::IntVector2> Locations;
    ReadLocations(GetFileName().c_str(), Locations);

    int Width = IsTest() ? 7 : 71;
    int Heigth = IsTest() ? 7 : 71;

    aoc::maps::Map2d<uint8_t> map(Width, Heigth);

    int size = IsTest() ? 12 : 1024;
    size = min(size, map.Map.size());

    for(int l = 0; l < size; l++)
    {
        map.Set( Locations[l], 1);
    }
    return FindShortestPathLen(map, { 0,0 }, { Width - 1, Heigth - 1 });
};



const int64_t AoC_2024_18::Step2()
{
    TIME_PART;

    vector<mutil::IntVector2> Locations;
    ReadLocations(GetFileName().c_str(), Locations);

    int Width = IsTest() ? 7 : 71;
    int Heigth = IsTest() ? 7 : 71;

    aoc::maps::Map2d<uint8_t> map(Width, Heigth);

    int size = IsTest() ? 12 : 1024;
    size = min(size, Locations.size());

    for(int l = 0; l < size; l++)
    {
        const auto& loc = Locations[l];
        map.Set(loc.x, loc.y, 1);
    }

    
    for(int l = size; l < Locations.size(); l++)
    {
        const auto& loc = Locations[l];
        map.Set(loc.x, loc.y, 1);

        if(FindShortestPathLen(map, { 0,0 }, { Width - 1, Heigth - 1 }) == -1)
        {
            cout << loc.x << "," << loc.y << endl;
            return 1;
        }
    }
    return 0;
};



void ReadLocations(const char* fileName, vector<IntVector2>& Locations)
{
    FILE* file;
    fopen_s(&file, fileName, "r");
    if(file == NULL)
    {
        throw std::runtime_error("Failed to open file");
    }

    char line[64];

    while(fgets(line, sizeof(line), file))
    {
        const char* p = &line[0];
        if(*p == '\n')continue;
        
        int x = strtol(p, (char**)&p, 10);
        p++;
        int y = strtol(p, (char**)&p, 10);
        
        Locations.push_back(mutil::IntVector2(x, y));
    }
    fclose(file);
}


int64_t FindShortestPathLen(const aoc::maps::Map2d<uint8_t>& Grid, IntVector2 Start, IntVector2 End)
{
    if(Grid.Get(Start, 1) == 1 || Grid.Get(End, 1) == 1)
        return -1;

    std::vector<IntVector2> directions = { {0, 1}, {1, 0}, {0, -1}, {-1, 0} }; // Right, Down, Left, Up
    std::queue<IntVector2> queue;
    aoc::maps::Map2d<int8_t> visited(Grid.Width, Grid.Height, true);
    queue.push(Start);
    visited.Set(Start, 1);
    int64_t steps = 0;
    while(!queue.empty())
    {
        int qSize = queue.size();
        for(int i = 0; i < qSize; ++i)
        {
            IntVector2 curr = queue.front();
            queue.pop();

            if(curr == End)
            {
                return steps;
            }
            for(const auto& dir : directions)
            {
                IntVector2 _new = curr + dir;
                if(!Grid.Get(_new, 1) && !visited.Get(_new, 1))
                {
                    queue.push(_new);
                    visited.Set(_new, 1);
                }
            }
        }
        ++steps;
    }
    return -1;
}
