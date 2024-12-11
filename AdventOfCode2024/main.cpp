#include "pch.h"
#include "AoC_2024_Includes.h"

int main()
{
    // we must set locale
    std::locale::global(std::locale("pl_PL.UTF-8"));
#include "AoC_2024_Run.h"
    AoCBase::PrintExecutionReport();
    return 0;
}


