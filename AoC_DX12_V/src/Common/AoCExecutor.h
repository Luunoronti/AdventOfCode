#pragma once
#include <stdint.h>
#include <string>
#include "base.h"
#include "AoCBase.h"

class AOCLIBRARY_API AoCExecutor
{
public:
    template<typename T>
    static void ExecuteDay(class AoCConfiguration* ConfigurationService)
    {
        static_assert(std::is_base_of<AoCBase, T>::value, "T must derive from AoCBase");
        T instance;
        ExecuteDayParts(ConfigurationService, (AoCBase*)&instance);
    }
    static void ExecuteDayParts(class AoCConfiguration* ConfigurationService, AoCBase* instance);
private:
    static void LoadInput(int year, int day, bool isTest, int part, AoCExecutionContext* context);
    static void CreateTestFile(const std::string& FileName);
    static void DownloadLiveFile(const std::string& FileName, int year, int day);
};

