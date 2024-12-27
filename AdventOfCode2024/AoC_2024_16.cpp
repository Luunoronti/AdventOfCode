#include "pch.h"
#include "AoC_2024_16.h"
#include <iostream> 
#include <vector> 
#include <unordered_map> 
#include <queue> 
#include <limits> 
#include <algorithm>

struct State
{
    mutil::IntVector2 loc;
    int dir, cost;
    bool operator>(const State& other) const { return cost > other.cost; }
};
const vector<mutil::IntVector2> directions = { {0, 1}, {1, 0}, {0, -1}, {-1, 0} };

int bfs_shortest_path(const aoc::maps::Map2d<char>& maze, const mutil::IntVector2& start, const mutil::IntVector2& end)
{
    //queue<State> q;
    priority_queue<State, vector<State>, greater<State>> pq;
    vector<vector<vector<bool>>> visited(maze.Height, vector<vector<bool>>(maze.Width, vector<bool>(4, false)));

    // Start facing right (direction 1)
    pq.push({ start, 1, 0 });
    visited[start.x][start.y][1] = true;

    while(!pq.empty())
    {
        State current = pq.top();
        pq.pop();

        // Check if we reached the end
        if(current.loc == end)
        {
            return current.cost;
        }

        auto d = current.loc + directions[current.dir];

        if(maze.Get(d, '#') != '#' && !visited[d.x][d.y][current.dir])
        {
            visited[d.x][d.y][current.dir] = true;
            pq.push({ d, current.dir, current.cost + 1 });
        }

        // Try turning left and right
        for(int turn : {-1, 1})
        {
            int new_dir = (current.dir + turn + 4) % 4;
            if(!visited[current.loc.x][current.loc.y][new_dir])
            {
                visited[current.loc.x][current.loc.y][new_dir] = true;
                pq.push({ current.loc, new_dir, current.cost + 1000 });
            }
        }
    }

    return -1; // If no path is found
}


const int64_t AoC_2024_16::Step1()
{
    aoc::maps::Map2d<char> maze;
    aoc::AoCStream() >> maze;

    TIME_PART;

    int sx, sy, ex, ey;
    if(!maze.find('S', sx, sy) || !maze.find('E', ex, ey))
        throw std::runtime_error("Unable to find start or end point");


    return bfs_shortest_path(maze, { sx, sy }, { ex, ey });
}


const int64_t AoC_2024_16::Step2()
{
    aoc::maps::Map2d<char> maze;
    aoc::AoCStream() >> maze;

    TIME_PART;

    // int sx, sy, ex, ey;
    //if(!maze.find('S', sx, sy) || !maze.find('E', ex, ey))
    //    throw std::runtime_error("Unable to find start or end point");

    return 0;
}

void AoC_2024_16::Tick(double timeDelta)
{

    ::Sleep(100);
    this->RepeatTick();
}


/*
int main() {
    vector<string> grid = {
        "#################",
        "#...#...#...#..E#",
        "#.#.#.#.#.#.#.#.#",
        "#.#.#.#...#...#.#",
        "#.#.#.#.###.#.#.#",
        "#...#.#.#.....#.#",
        "#.#.#.#.#.#####.#",
        "#.#...#.#.#.....#",
        "#.#.#####.#.###.#",
        "#.#.#.......#...#",
        "#.#.###.#####.###",
        "#.#.#...#.....#.#",
        "#.#.#.#####.###.#",
        "#.#.#.........#.#",
        "#.#.#.#########.#",
        "#S#.............#",
        "#################"
    };

    pair<int, int> start = { 15, 1 };
    pair<int, int> end = { 1, 15 };

    int shortest_path_cost = bfs_shortest_path(grid, start, end);
    cout << "Shortest path cost: " << shortest_path_cost << endl;

    return 0;
}
*/