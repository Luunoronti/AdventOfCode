#include "pch.h"
#include "TimeKeeper.h"


void TimeKeeper::AddFrameTime(float frameTime)
{
    frameCount++;
    this->frameTime = frameTime;
    frameTimeAcc += frameTime;
    fps = 1 / frameTime;
}
