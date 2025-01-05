#include "AoCExecutor.h"
#include "AoCConfiguration.h"
#include <fstream>
#include <sstream>
#include <iostream>
#include <thread>
#include <chrono>
#include <filesystem>

namespace fs = std::filesystem;

void AoCExecutor::ExecuteDayParts(AoCConfiguration* ConfigurationService, AoCBase* instance)
{
    // request configuration for this instance
    auto configuration = ConfigurationService->GetResultJsonEntry(instance->GetYear(), instance->GetDay());

#if _DEBUG
    if(!configuration.OkToRunInDebug) return;
#else
    if(!configuration.OkToRunInRelease) return;
#endif
    // create context
    AoCExecutionContext context;
    context.DayConfig = &configuration;

    TODO("add visualizations");
    TODO("add Tracy notification about days/steps");

    std::cout << "\033]0;" << "Advent of Code Year " << std::to_string(instance->GetYear()) << "\007";

    std::cout << "Executing day " << instance->GetYear() << " " << instance->GetDay() << std::endl;

    instance->SetTest(true);
    LoadInput(instance->GetYear(), instance->GetDay(), instance->IsTest(), 1, &context);
    context.PartConfig = &configuration.Part1Test;
    TODO("add timing");
    instance->Part1(&context);

    LoadInput(instance->GetYear(), instance->GetDay(), instance->IsTest(), 2, &context);
    context.PartConfig = &configuration.Part2Test;
    TODO("add timing");
    instance->Part2(&context);


    instance->SetTest(false);
    LoadInput(instance->GetYear(), instance->GetDay(), instance->IsTest(), 1, &context);
    context.PartConfig = &configuration.Part1Live;
    TODO("add timing");
    instance->Part1(&context);

    LoadInput(instance->GetYear(), instance->GetDay(), instance->IsTest(), 2, &context);
    context.PartConfig = &configuration.Part2Live;
    TODO("add timing");
    instance->Part2(&context);

}

void AoCExecutor::LoadInput(int year, int day, bool isTest, int part, AoCExecutionContext* context)
{
    // construct file name
    std::string folder_p = isTest ? "test" : "live";
    std::string folder = ".\\" + folder_p;
    std::string FileName = folder + "\\" + std::to_string(year) + "_" + std::to_string(day) + "_" + std::to_string(part) + ".txt";

    if(!fs::exists(folder))
    { 
        fs::create_directories(folder);
    }

    // if file does not exists, create/download
    struct stat buffer;
    if(stat(FileName.c_str(), &buffer))
    {
        if(isTest) CreateTestFile(FileName);
        else DownloadLiveFile(FileName, year, day);
    }

    std::ifstream file(FileName);
    if(!file.is_open())
    {
        std::cout << "Error opening file " << FileName << std::endl;
        context->Input = "";
        return;
    }
    std::stringstream sstream;
    sstream << file.rdbuf();
    file.close();
    context->Input = sstream.str();
}

void AoCExecutor::CreateTestFile(const std::string& FileName)
{
    struct stat buffer;
    std::string command = "code \"" + FileName + "\"";
    int result = system(command.c_str());
    if(result == 0)
    {
        std::cout << "VS Code opened successfully on file " << FileName << std::endl;
        while(stat(FileName.c_str(), &buffer) != 0)
        {
            std::this_thread::sleep_for(std::chrono::milliseconds(1));
        }
        return;
    }
    else std::cerr << "Failed to open VS Code. Attempting to revert to Notepad" << std::endl;

    command = "notepad \"" + FileName + "\"";
    result = system(command.c_str());
    if(result == 0)
    {
        std::cout << "Notepad opened successfully on file " << FileName << std::endl;
        return;
    }
    else std::cerr << "Failed to open Notepad." << std::endl;

}

void AoCExecutor::DownloadLiveFile(const std::string& FileName, int year, int day)
{}
