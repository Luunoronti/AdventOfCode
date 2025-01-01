#include "pch.h"
#include "TimeKeeper.h"


void TimeKeeper::Frame()
{
    frameCount++;
    LARGE_INTEGER currentTime;
    QueryPerformanceCounter(&currentTime);
    double elapsedTime = static_cast<double>(currentTime.QuadPart - startTime.QuadPart) / frequency;
    frameTime = static_cast<double>(currentTime.QuadPart - frameQPFTime.QuadPart) / frequency;
    frameQPFTime = currentTime;
    if(elapsedTime >= 0.5)
    {
        fps = frameCount / elapsedTime;
        frameCount = 0;
        startTime = frameQPFTime;
    }
}
