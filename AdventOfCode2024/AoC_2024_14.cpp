#include "pch.h"
#include "AoC_2024_14.h"

#define Q1 0
#define Q2 1
#define Q3 2
#define Q4 3

struct FRobot
{
    int sx;
    int sy;
    int x;
    int y;
    int vx;
    int vy;

    void Simulate(int seconds)
    {
        x = sx + vx * seconds;
        y = sy + vy * seconds;
    }
    void Wrap(int areaX, int areaY)
    {
        x = (x) % areaX;
        if(x < 0) x = areaX + x;

        y = (y) % areaY;
        if(y < 0) y = areaY + y;
    }
    void SelectQuadrant(int halfAreaX, int halfAreaY, int64_t* pQuadrants)
    {
        if(x < halfAreaX)
        {
            if(y < halfAreaY)
                (*(pQuadrants + Q1))++;
            else if(y > halfAreaY)
                (*(pQuadrants + Q4))++;
        }
        else if(x > halfAreaX)
        {
            if(y < halfAreaY)
                (*(pQuadrants + Q2))++;
            else if(y > halfAreaY)
                (*(pQuadrants + Q3))++;
        }
    }

};
void ParseLine(const char* line, FRobot* pRobot)
{
    /*
        format:
        p=15,97 v=2,6
        [..]
    */
    const char* p = line;
    p += 2; // skip 'p+'
    pRobot->sx = strtol(p, (char**)&p, 10);
    p += 1; // skip ','
    pRobot->sy = strtol(p, (char**)&p, 10);
    p += 3; // skip ' v='
    pRobot->vx = strtol(p, (char**)&p, 10);
    p += 1; // skip ','
    pRobot->vy = strtol(p, (char**)&p, 10);
}

const int64_t AoC_2024_14::Step1()
{
    TIME_PART;

    FILE* file;
    fopen_s(&file, GetFileName().c_str(), "r");
    if(file == NULL)
    {
        perror("Failed to open file");
        return 0;
    }

    int64_t quadrants[4]{ 0 };
    char line[128];
    FRobot robot;
    int areaX = IsTest() ? 11 : 101;
    int areaY = IsTest() ? 7 : 103;

    int halfAreaX = areaX / 2;
    int halfAreaY = areaY / 2;

    while(fgets(line, sizeof(line), file))
    {
        ParseLine(line, &robot);
        robot.Simulate(100);
        robot.Wrap(areaX, areaY);
        robot.SelectQuadrant(halfAreaX, halfAreaY, quadrants);
    }
    fclose(file);

    return quadrants[Q1] * quadrants[Q2] * quadrants[Q3] * quadrants[Q4];
};


/*
 If robots are to form an image,
 no two units can be at the same spot.
 Before I coded second part of this test, 
 I submitted the answer and that was the right one.
 This was just a guess. 
 If this we've failed, we would just see what's going on 
 in the map, and check for continuous lines
 that are in the center. Or maybe not in the center...
*/
const int64_t AoC_2024_14::Step2()
{
    TIME_PART;

    FILE* file;
    fopen_s(&file, GetFileName().c_str(), "r");
    if(file == NULL)
    {
        perror("Failed to open file");
        return 0;
    }
    char line[128];
    std::vector<FRobot> allRobots;
    FRobot robot;
    while(fgets(line, sizeof(line), file))
    {
        ParseLine(line, &robot);
        allRobots.push_back(robot);
    }
    fclose(file);

    int areaX = IsTest() ? 11 : 101;
    int areaY = IsTest() ? 7 : 103;

    int halfAreaX = areaX / 2;
    int halfAreaY = areaY / 2;

    aoc::maps::Map2d<int8_t> map(areaX, areaY);

    int second = 1;
    while(1)
    {
        bool found = true;
        for(auto& r : allRobots)
        {
            r.Simulate(second);
            r.Wrap(areaX, areaY);
            const auto l = std::pair<int, int>{ r.x, r.y };
            if(map.Get(r.x, r.y))
            {
                found = false;
                break; // two robots overlap
            }
            map.Set(r.x, r.y, 1);
        }
        if(found)
            return second;

        ++second;
        map.clear();
    }
    return 0;
};