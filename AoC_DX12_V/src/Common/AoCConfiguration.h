#pragma once
#include <stdint.h>
#include <string>
#include <vector>
#include <hlsl++.h>
#include "base.h"

struct AOCLIBRARY_API JsonColor
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
public:
    const AOCLIBRARY_API void GetGeneral(std::vector<std::string>& general) const;
    const AOCLIBRARY_API void GetTooBig(std::vector<int64_t>& general) const;
    const AOCLIBRARY_API void GetTooSmall(std::vector<int64_t>& general) const;
public:
    std::vector<std::string> General;
    std::vector<int64_t> TooBig;
    std::vector<int64_t> TooSmall;
};


struct AoCBaseExecutionConfigurationEntry
{
    const AOCLIBRARY_API int64_t GetExpectedResult() const;
    const AOCLIBRARY_API std::string GetExpectedResultStr() const;
    const AOCLIBRARY_API AoCBaseExecutionConfigurationKnownErrors* GetKnownErrorResults() const;

    int64_t ExpectedResult;
    std::string ExpectedResultStr;
    AoCBaseExecutionConfigurationKnownErrors KnownErrorResults;
    bool EnableDebugOutput{ false };
    bool EnableVisualization{ false };
    inline bool IsNotKnown() const { return ExpectedResultStr.size() == 0 && ExpectedResult == 0; }
};

struct AoCBaseExecutionConfiguration
{
    const AOCLIBRARY_API int GetYear() const;
    const AOCLIBRARY_API int GetDay() const;
    const AOCLIBRARY_API bool GetOkToRunInRelease() const;
    const AOCLIBRARY_API bool GetOkToRunInDebug() const;
    const AOCLIBRARY_API bool GetEnableDebugOutput() const;
    const AOCLIBRARY_API bool GetEnableVisualization() const;
    const AOCLIBRARY_API std::string GetName() const;
    
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
    bool ProgramConfigurationLoaded{ false };
    std::vector<AoCBaseExecutionConfiguration> DaysDatabase;
    void ReadDaysDatabaseIfNotDoneAlready();
public:
    AoCBaseExecutionConfiguration AOCLIBRARY_API GetResultJsonEntry(int Year, int Day);
};