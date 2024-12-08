#pragma once

#define _STR(x) #x
#define STR(x) _STR(x)
#define TODO(x) __pragma(message(__FILE__"("STR(__LINE__)"): TODO: "_STR(x) ))

#define CompilerMessage(desc) __pragma(message(__FILE__ "(" STR(__LINE__) ") :" #desc))

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

#include <iostream> 
#include <fstream> 
#include <vector> 
#include <unordered_map> 
#include <sstream>
#include <iomanip>
#include <algorithm>
#include <cstdlib>
#include <numeric>
#include <sys/stat.h>
#include <cassert>
#include <ppl.h>

#include <ppl.h>
#include <iostream>
#include <algorithm>
#include <array>
#include <set>

#include "json.hpp"

using namespace std;
using namespace concurrency;

const string RESET = "\033[0m";
const string BOLD = "\033[1m";
const string CLEAR_SCREEN = "\033[2J\033[1;1H";
const string LINESTART = "\033[0G";

const string DIM = "\033[2m";
const string UNDERLINE = "\033[4m";
const string BLINK = "\033[5m";

const string BLACK = "\033[30m";
const string RED = "\033[31m";
const string GREEN = "\033[32m";
const string YELLOW = "\033[33m";
const string BLUE = "\033[34m";
const string MAGENTA = "\033[35m";
const string CYAN = "\033[36m";
const string WHITE = "\033[37m";

const string BKBLACK = "\033[40m";
const string BKRED = "\033[41m";
const string BKGREEN = "\033[42m";
const string BKYELLOW = "\033[43m";
const string BKBLUE = "\033[44m";
const string BKMAGENTA = "\033[45m";
const string BKCYAN = "\033[46m";
const string BKWHITE = "\033[47m";

#define PAD_LEFT(num, ch) std::setw(num) << std::setfill(ch)

typedef std::vector<std::vector<long>> LongListList;

struct AoCBaseExecutionResultEntry
{
    int64_t Result{ 0 };
    int64_t ExpectedResult{ 0 };
    double Time{ 0 };
    string Name;
    vector<int64_t> KnownErrorResults;

    inline bool IsValid() const { return !IsError() && !IsNotKnown() && Result == ExpectedResult; }
    inline bool IsError() const { return !IsNotKnown() && Result != ExpectedResult; }
    inline bool IsNotKnown() const { return 0 == ExpectedResult; }
    
    inline int64_t Delta() const { return std::abs(Result - ExpectedResult); }
};
struct AoCBaseExecutionResult
{
    int Year{ 0 };
    int8_t Day{ 0 };
    string Name;

    AoCBaseExecutionResultEntry Step1Test;
    AoCBaseExecutionResultEntry Step2Test;
    AoCBaseExecutionResultEntry Step1Live;
    AoCBaseExecutionResultEntry Step2Live;
};
                                                                               
class AoCBase
{
public:

    static vector<AoCBaseExecutionResult> ResultReports;

    static void PrintExecutionReport();
    static void ReadDaysDatabaseIfNotDoneAlready();

    template<typename T>
    static void ExecuteSteps()
    {
        LARGE_INTEGER frequency;
        LARGE_INTEGER start_step;
        LARGE_INTEGER end_step;
#define TIMING_START  QueryPerformanceCounter(&start_step)
#define TIMING_END  QueryPerformanceCounter(&end_step)
#define TIME static_cast<double>(end_step.QuadPart - start_step.QuadPart) * 1000.0 / frequency.QuadPart

        // Get the frequency of the high-resolution performance counter 
        QueryPerformanceFrequency(&frequency);

        static_assert(std::is_base_of<AoCBase, T>::value, "T must derive from AoCBase");
        T instance;

        if(instance.GetDay() == 0)
        {
            cout << RED << BLINK << "Error: Class instance has no day defined." << RESET << " Please override GetDay() and return proper day." << endl;
            return;
        }

        AoCBaseExecutionResult result = GetResultJsonEntry(instance.GetYear(), instance.GetDay());

        result.Step1Test.Name = "Step 1 (TEST)";
        result.Step2Test.Name = "Step 2 (LIVE)";
        result.Step1Live.Name = "Step 1 (TEST)";
        result.Step2Live.Name = "Step 2 (LIVE)";

        instance.OnInitTests();
        instance.SetTest(true);
        instance.OnInitTestingTests();

        instance.OnInitStep(1);
        TIMING_START;
        result.Step1Test.Result = instance.Step1();
        TIMING_END;
        result.Step1Test.Time = TIME;
        instance.OnCloseStep(1);

        instance.OnInitStep(2);
        TIMING_START;
        result.Step2Test.Result = instance.Step2();
        TIMING_END;
        result.Step2Test.Time = TIME;
        instance.OnCloseStep(2);

        instance.OnCloseTestingTests();
        instance.SetTest(false);

        instance.OnInitLiveTests();

        instance.OnInitStep(1);
        TIMING_START;
        result.Step1Live.Result = instance.Step1();
        TIMING_END;
        result.Step1Live.Time = TIME;
        instance.OnCloseStep(1);

        instance.OnInitStep(2);
        TIMING_START;
        result.Step2Live.Result = instance.Step2();
        TIMING_END;
        result.Step2Live.Time = TIME;
        instance.OnCloseStep(2);

        instance.OnCloseLiveTests();
        instance.OnCloseTests();

        ResultReports.push_back(result);
    }

protected:
    static AoCBaseExecutionResult GetResultJsonEntry(int Year, int Day);

    const bool IsTest() const;
    void SetTest(const bool IsTest);

    virtual void OnInitTests();
    virtual void OnInitTestingTests();
    virtual void OnCloseTestingTests();

    virtual void OnInitLiveTests();
    virtual void OnCloseLiveTests();

    virtual void OnInitStep(const int Step);

    virtual void OnCloseStep(const int Step);
    virtual void OnCloseTests();

    const virtual __forceinline int GetYear() const { return 2024; };
    const virtual __forceinline int GetDay() const = 0;

    LongListList ReadLongVectorsFromFile(int Step) const;

    LongListList ReadVerticalVectorsFromFile(int Step) const;
    string ReadStringFromFile(int Step, int& LinesCount, int& LastLineWidth) const;
    string ReadStringFromFile(int Step) const;
    vector<string> ReadStringLinesFromFile(int Step) const;
    stringstream ReadStringStreamFromFile(int Step) const;

    void CreateFileIfDoesNotExist(const std::string& FileName) const;

    const int GetFileSingleLineWidth(int Step) const;



    static long GetMinimum(const vector<long>& List);

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

public:
    virtual const int64_t Step1() = 0;
    virtual const int64_t Step2() = 0;

private:
    const std::string GetFileName(const int Step) const;
    bool IsUnderTest{ false };

    static vector<AoCBaseExecutionResult> DaysDatabase;
};



// use AoC toolset 
// from this path:
// https://github.com/breakthatbass/eggnog/tree/main/src
//