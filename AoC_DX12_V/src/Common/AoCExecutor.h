#pragma once
#include <stdint.h>
#include <string>

class AoCExecutor
{
public:
    template<typename T>
    static void ExecuteSteps()
    {
        static_assert(std::is_base_of<AoCBase, T>::value, "T must derive from AoCBase");
        T instance;
        ExecuteStep(instance);
    }
    static void ExecuteStep(class AoCBase& instance);
};

