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
#pragma region VISUALIZATION
    if(CurrentStepConfiguration.EnableVisualization)
    {
        printf("Visualization enabled. This day will never end now :)");
        Sleep(1000);
    }
#pragma endregion


    TIME_PART;  // performance timer starts here


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

#pragma region VISUALIZATION
    COORD bufferSize = { areaX, areaY + 1 };
    COORD bufferCoord = { 0, 0 };
    SMALL_RECT writeRegion = { 0, 0, areaX - 1, areaY };
    CHAR_INFO* consoleBuffer{ nullptr };
    if(CurrentStepConfiguration.EnableVisualization)
    {
        consoleBuffer = new CHAR_INFO[areaX * (areaY + 1)];
    }
#pragma endregion


    int second = 1;
    while(1)
    {
#pragma region VISUALIZATION
        // Fill the console buffer with blank spaces
        if(consoleBuffer)
        {
            ReadFromConsoleBuffer(consoleBuffer, bufferSize, bufferCoord, writeRegion);
            // Fill the buffer with blank spaces
            for(int i = 0; i < areaY * areaX; ++i)
            {
                consoleBuffer[i].Char.UnicodeChar = ' ';
                consoleBuffer[i].Attributes = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE; // White text
            }
        }
#pragma endregion

        bool found = true;
        for(auto& r : allRobots)
        {
            r.Simulate(second);
            r.Wrap(areaX, areaY);
            const auto l = std::pair<int, int>{ r.x, r.y };

#pragma region VISUALIZATION
            if(consoleBuffer)
            {
                WCHAR c = consoleBuffer[r.x + r.y * areaX].Char.UnicodeChar;
                consoleBuffer[r.x + r.y * areaX].Attributes = c == ' ' ? FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE : FOREGROUND_RED | FOREGROUND_INTENSITY;
                consoleBuffer[r.x + r.y * areaX].Char.UnicodeChar = L'\u2588';
                //consoleBuffer[r.x + r.y * areaX].Char.UnicodeChar = c == ' ' ? '1' : c + 1;
            }
#pragma endregion

            if(map.Get(r.x, r.y))
            {
                found = false;
                if(!consoleBuffer) // do not break out if visualization is enabled
                    break; // two robots overlap
            }
            map.Set(r.x, r.y, 1);

        }
        
#pragma region VISUALIZATION
        if(consoleBuffer)
        {
            char numBuff[32];
            ZeroMemory(&numBuff[0], 32);
            sprintf_s(numBuff, 32, "%d", second);
            for(int i = 0; i < 32; ++i)
            {
                if(numBuff[i] != 0)
                {
                    //consoleBuffer[areaY * (areaY - 2) + i].Char.AsciiChar = numBuff[i];
                }
                
            }

            WriteToConsoleBuffer(consoleBuffer, bufferSize, bufferCoord, writeRegion);
        }
#pragma endregion

        if(found)
        {
            if(!consoleBuffer)
                return second;
            else
            {
#pragma region VISUALIZATION
                Sleep(10000);
#pragma endregion
            }
        }

        ++second;
        map.clear();
    }
    return 0;
};

