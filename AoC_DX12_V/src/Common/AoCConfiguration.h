#pragma once
#include <stdint.h>
#include <string>
#include <vector>
#include <hlsl++.h>


struct JsonColor
{
    int r{ 0 };
    int g{ 0 };
    int b{ 0 };

    float3 ToColor()
    {
        return float3((float)r / 255, (float)g / 255, (float)b / 255);
    }
    static JsonColor FromColor(const float3& color)
    {
        JsonColor c;
        c.r = (int)(color.x * 255);
        c.g = (int)(color.y * 255);
        c.b = (int)(color.z * 255);
        return c;
    }
};

struct AoCBaseExecutionConfigurationKnownErrors
{
    std::vector<std::string> General;
    std::vector<int64_t> TooBig;
    std::vector<int64_t> TooSmall;
};


struct AoCBaseExecutionConfigurationEntry
{
    int64_t ExpectedResult;
    std::string ExpectedResultStr;
    AoCBaseExecutionConfigurationKnownErrors KnownErrorResults;
    bool EnableDebugOutput{ false };
    bool EnableVisualization{ false };
    inline bool IsNotKnown() const { return ExpectedResultStr.size() == 0 && ExpectedResult == 0; }
};

struct AoCBaseExecutionConfiguration
{
    int Year{ 0 };
    int8_t Day{ 0 };
    std::string Name;
    bool OkToRunInRelease{ false };
    bool OkToRunInDebug{ false };
    bool EnableDebugOutput{ false };
    bool EnableVisualization{ false };

    std::string GetNameWithDate() { return std::to_string(Year) + "/" + std::to_string(Day) + " (" + Name + ")"; }
    
    AoCBaseExecutionConfigurationEntry Part1Test;
    AoCBaseExecutionConfigurationEntry Part2Test;
    AoCBaseExecutionConfigurationEntry Part1Live;
    AoCBaseExecutionConfigurationEntry Part2Live;
};



class AoCConfiguration
{
private:
    static bool ProgramConfigurationLoaded;
    static std::vector<AoCBaseExecutionConfiguration> DaysDatabase;

    static void ReadDaysDatabaseIfNotDoneAlready();
public:
    static AoCBaseExecutionConfiguration GetResultJsonEntry(int Year, int Day);
};