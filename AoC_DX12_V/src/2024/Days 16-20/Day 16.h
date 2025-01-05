#pragma once
#include <AoCBase.h>

class Year2024Day16 : public AoCBase
{
public:
    const std::string Part1(const AoCExecutionContext* Context) override;
    const std::string Part2(const AoCExecutionContext* Context) override;
    const __forceinline int GetYear() const override { return 2024; };
    const __forceinline int GetDay() const override { return 16; }

};