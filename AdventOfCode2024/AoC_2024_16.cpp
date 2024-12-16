#include "pch.h"
#include "AoC_2024_16.h"
#include <iostream> 
#include <vector> 
#include <unordered_map> 
#include <queue> 
#include <limits> 
#include <algorithm>


const int64_t AoC_2024_16::Step1()
{
    aoc::maps::Map2d<char> maze;
    aoc::AoCStream() >> maze;

    TIME_PART;

    int sx, sy, ex, ey;
    if(!maze.find('S', sx, sy) || !maze.find('E', ex, ey))
        throw std::runtime_error("Unable to find start or end point");

    return 0;
}


const int64_t AoC_2024_16::Step2()
{
    aoc::maps::Map2d<char> maze;
    aoc::AoCStream() >> maze;

    TIME_PART;

    int sx, sy, ex, ey;
    if(!maze.find('S', sx, sy) || !maze.find('E', ex, ey))
        throw std::runtime_error("Unable to find start or end point");

    return 0;
}


