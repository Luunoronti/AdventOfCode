#pragma once
#include "pch.h"
#include "AoCVisualizer.h"

#define PAD_LEFT(num, ch) std::setw(num) << std::setfill(ch)
#define PAD_RIGHT(num, ch) std::left << std::setw(num) << std::setfill(ch)

typedef std::vector<std::vector<long>> LongListList;

struct AoCBaseExecutionConfigurationResultEntry
{
    int64_t Result{ 0 };
    int64_t ExpectedResult{ 0 };
    double Time{ 0 };
    string Name;
    vector<int64_t> KnownErrorResults;
    bool EnableDebugOutput{ false };

    bool EnableVisualization{ false };

    inline bool IsValid() const { return !IsError() && !IsNotKnown() && Result == ExpectedResult; }
    inline bool IsError() const { return !IsNotKnown() && Result != ExpectedResult; }
    inline bool IsNotKnown() const { return 0 == ExpectedResult; }

    inline int64_t Delta() const { return std::abs(Result - ExpectedResult); }
};
struct AoCBaseExecutionConfigurationAndResult
{
    int Year{ 0 };
    int8_t Day{ 0 };
    string Name;
    bool OkToRunInRelease{ false };
    bool OkToRunInDebug{ false };
    bool EnableDebugOutput{ false };
    bool EnableVisualization{ false };

    string GetNameWithDate()
    {
        return to_string(Year) + "/" + to_string(Day) + " (" + Name + ")";
    }
    AoCBaseExecutionConfigurationResultEntry Step1Test;
    AoCBaseExecutionConfigurationResultEntry Step2Test;
    AoCBaseExecutionConfigurationResultEntry Step1Live;
    AoCBaseExecutionConfigurationResultEntry Step2Live;
};

struct AoCExecutionContext
{
    AoCBaseExecutionConfigurationAndResult* DayConfig;
    AoCBaseExecutionConfigurationResultEntry* PartConfig;
    AoCVisualizer* Visualizer;
};

struct AoCProgramConfiguration
{
    bool ForceAllRunsInDebug;
    bool ForceAllRunsInRelease;
    AoCVisualizerConfig VisualizerConfig{};
};

class AoCBase
{
public:

    static vector<AoCBaseExecutionConfigurationAndResult> ResultReports;
    static AoCProgramConfiguration ProgramConfiguration;
    static bool ProgramConfigurationLoaded;

    static void PrintExecutionReport();
    static void ReadDaysDatabaseIfNotDoneAlready();

    template<typename T>
    static void ExecuteSteps()
    {
        static_assert(std::is_base_of<AoCBase, T>::value, "T must derive from AoCBase");
        T instance;
        ExecuteStep(instance);
    }

    static void ExecuteStep(AoCBase& instance);
    static void ExecutePart(AoCBase& instance, bool test, int step, AoCBaseExecutionConfigurationAndResult& dayConfiguration, AoCBaseExecutionConfigurationResultEntry& partConfiguration);

protected:
    void ReadInputFile();




    /* OLD IO => TO REMOVE */
    LongListList ReadLongVectorsFromFile(int Step) const;

    LongListList ReadVerticalVectorsFromFile(int Step) const;
    string ReadStringFromFile(int Step, int& LinesCount, int& LastLineWidth) const;
    string ReadStringFromFile(int Step) const;
    vector<string> ReadStringLinesFromFile(int Step) const;
    stringstream ReadStringStreamFromFile(int Step) const;

    const int GetFileSingleLineWidth(int Step) const;

    static long GetMinimum(const vector<long>& List);

    /* OLD IO <== TO REMOVE */

    template<typename T1>
    __forceinline static int IndexOf(const vector<T1>& List, T1 Value)
    {
        auto it = std::find(List.begin(), List.end(), Value);
        if(it != List.end())
        {
            return static_cast<int>(std::distance(List.begin(), it));
        }
        else
        {
            return -1;
        }
    }


    const __forceinline bool IsDigit(const char ch) const
    {
        return std::isdigit(static_cast<unsigned char>(ch));
    }
    const __forceinline int8_t ToDigit(const char ch) const
    {
        return (int8_t)ch - '0';
    }

    const std::string GetFileName() const;

public:
    virtual const int64_t Step1() { return 0; };
    virtual const int64_t Step2() { return 0; };
    const virtual __forceinline int GetYear() const { return 0; };
    const virtual __forceinline int GetDay() const = 0;
    const __forceinline int GetStep() const { return Step; };
    const bool IsTest() const;
    void SetTest(const bool IsTest);
    static AoCBaseExecutionConfigurationAndResult GetResultJsonEntry(int Year, int Day);
    static void LoadProgramConfig();

    AoCBaseExecutionConfigurationAndResult CurrentDayConfiguration;
    AoCBaseExecutionConfigurationResultEntry CurrentStepConfiguration;
    AoCExecutionContext* Context;
protected:
    std::string Input;
private:
    bool IsUnderTest{ false };
    int Step{ 0 };
    double LastGlobalTime{ 0 };
    static vector<AoCBaseExecutionConfigurationAndResult> DaysDatabase;
    friend class _Time;
};




class _Time
{
    friend class AoCBase;
public:
    _Time(string Name)
    {
        name = name;
        QueryPerformanceCounter(&start_step);
    }
    _Time()
    {
        QueryPerformanceCounter(&start_step);
    }
    _Time(AoCBase* _instance)
    {
        aocp = _instance;
        QueryPerformanceCounter(&start_step);
    }
    ~_Time()
    {
        LARGE_INTEGER end_step;
        LARGE_INTEGER frequency;
        QueryPerformanceCounter(&end_step);
        QueryPerformanceFrequency(&frequency);
        double time = static_cast<double>(end_step.QuadPart - start_step.QuadPart) * 1000.0 / frequency.QuadPart;
        if(aocp)
        {
            aocp->LastGlobalTime = time;
        }
        else
        {
            aoc::dout << "Timer " << name << " ended. Time: " << toStringWithPrecision(time, 3) << "ms" << endl;
        }
    }

private:
    LARGE_INTEGER start_step{ 0 };
    AoCBase* aocp{ nullptr };
    string name;
};
#define TOKEN_CONCAT(a, b) a##b
#define UNIQUE_VAR(prefix) TOKEN_CONCAT(prefix, __LINE__)

#define TIME_PART _Time __part_timer__(this) 
#define TIME(n) _Time UNIQUE_VAR(__local_timer__)(n)



// use AoC toolset 
// from this path:
// https://github.com/breakthatbass/eggnog/tree/main/src
//