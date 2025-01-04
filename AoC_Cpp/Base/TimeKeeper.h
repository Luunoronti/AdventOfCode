#pragma once

class TimeKeeper
{
public:
    TimeKeeper() : frameCount(0), fps(0.0), frameTime(0.0)
    {
    }
    void AddFrameTime(float frameTime);
    double GetFPS() const { return fps; }
    double GetFrameTime() const { return frameTime; }
    double GetTimeSinceStart() const { return frameTimeAcc; }
private:
    int frameCount;
    double fps;
    double frameTimeAcc{ 0 };
    double frameTime{ 0 };
};
