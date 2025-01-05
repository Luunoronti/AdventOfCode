#pragma once
#include "base.h"
#include <stdint.h>
#include <string>

struct AoCExecutionContext
{
    struct AoCBaseExecutionConfiguration* DayConfig;
    struct AoCBaseExecutionConfigurationEntry* PartConfig;
    class AoCVisualizer* Visualizer;
    std::string Input;
};

class AOCLIBRARY_API AoCBase
{
    friend class AoCExecutor;
public:
    virtual const std::string Part1(const AoCExecutionContext* Context) { return ""; };
    virtual const std::string Part2(const AoCExecutionContext* Context) { return ""; };
    const virtual __forceinline int GetYear() const { return 0; };
    const virtual __forceinline int GetDay() const = 0;
    const __forceinline int GetStep() const { return Step; };
    const bool IsTest() const { return IsUnderTest; }
    void SetTest(const bool IsTest) { IsUnderTest = IsTest; }


protected:

    AoCExecutionContext* Context;

private:
    bool IsUnderTest{ false };
    int Step{ 0 };
};

