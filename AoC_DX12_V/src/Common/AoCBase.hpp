#pragma once
#include <stdint.h>
#include <string>

struct AoCExecutionContext
{
    class AoCBaseExecutionConfiguration* DayConfig;
    class AoCBaseExecutionConfigurationEntry* PartConfig;
    class AoCVisualizer* Visualizer;
};

class AoCBase
{
public:
public:
    virtual const int64_t Step1() { return 0; };
    virtual const int64_t Step2() { return 0; };
    const virtual __forceinline int GetYear() const { return 0; };
    const virtual __forceinline int GetDay() const = 0;
    const __forceinline int GetStep() const { return Step; };
    const bool IsTest() const;
    void SetTest(const bool IsTest);


protected:
    std::string Input;
    AoCExecutionContext* Context;

private:
    bool IsUnderTest{ false };
    int Step{ 0 };
};

