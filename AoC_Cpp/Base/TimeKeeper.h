#pragma once

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>


class TimeKeeper
{
public:
    TimeKeeper() :
        frameCount(0), fps(0.0)
    {
        LARGE_INTEGER f;
        QueryPerformanceFrequency(&f);
        frequency = f.QuadPart;
        QueryPerformanceCounter(&startTime);
    }
    void Frame();
    double GetFPS() const { return fps; }
    double GetFrameTime() const { return frameTime; }
private:
    int frameCount;
    double fps;
    double frameTimeAcc{ 0 };
    double frameTime{ 0 };
    LONGLONG frequency;
    LARGE_INTEGER startTime;
    LARGE_INTEGER frameQPFTime{ 0 };
};