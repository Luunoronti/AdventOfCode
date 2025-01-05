
#include <hlsl++.h>

#include "stdafx.h"
#include <Tracy.hpp>
#include <iostream>

#include <AoCConfiguration.h>
#include <AoCExecutor.h>
#include "..\\2024\\Days.h"
//#include "..\\2023\\Days.h"


void RunDays()
{
    std::cout << "Reading configuration" << std::endl;
    AoCConfiguration configuration;
    std::cout << "Running days..." << std::endl;

    //RUN_YEAR_2023(AoCExecutor::ExecuteDay, &configuration)
    RUN_YEAR_2024(AoCExecutor::ExecuteDay, &configuration)

    std::cout << "Tests complete." << std::endl;
}
