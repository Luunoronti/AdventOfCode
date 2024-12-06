#pragma once

#define _STR(x) #x
#define STR(x) _STR(x)
#define TODO(x) __pragma(message(__FILE__"("STR(__LINE__)"): TODO: "_STR(x) ))

#define CompilerMessage(desc) __pragma(message(__FILE__ "(" STR(__LINE__) ") :" #desc))

#include <iostream> 
#include <fstream> 
#include <vector> 
#include <sstream>
#include <iomanip>
#include <algorithm>
#include <cstdlib>
#include <numeric>
#include <sys/stat.h>
#include <cassert>
#include <ppl.h>

#include <Windows.h>


using namespace std;

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

typedef std::vector<std::vector<long>> LongListList;

class AoCBase
{
public:
    template<typename T>
    static void ExecuteSteps()
    {
        LARGE_INTEGER frequency;
        LARGE_INTEGER start_step1;
        LARGE_INTEGER start_step2;
        LARGE_INTEGER end_step1;
        LARGE_INTEGER end_step2;
        // Get the frequency of the high-resolution performance counter 
        QueryPerformanceFrequency(&frequency);


        static_assert(std::is_base_of<AoCBase, T>::value, "T must derive from AoCBase");
        T instance;

        cout << DIM << "====================================================================================" << RESET << endl;
        if(instance.GetDay() == 0)
        {
            cout << RED << BLINK << "Error: Class instance has no day defined." << RESET << " Please override GetDay() and return proper day." << endl;
            return;
        }

        instance.ReadExpectedStepResults();

        instance.OnInitTests();
        instance.SetTest(true);
        instance.OnInitStep(1);
        const auto& Step1ResultTest = instance.Step1();
        const auto& Step2ResultTest = instance.Step2();
        instance.OnCloseStep(1);
        instance.SetTest(false);
        instance.OnInitStep(2);

        QueryPerformanceCounter(&start_step1);
        const auto& Step1ResultLive = instance.Step1();
        QueryPerformanceCounter(&end_step1);

        QueryPerformanceCounter(&start_step2);
        const auto& Step2ResultLive = instance.Step2();
        QueryPerformanceCounter(&end_step2);

        instance.OnCloseStep(2);
        instance.OnCloseTests();

        cout << "" << instance.GetYear() << "/" << std::setw(2) << std::setfill('0') << instance.GetDay() << ":  ";
        instance.PrintStepResult(1, true, Step1ResultTest);
        instance.PrintStepResult(1, false, Step1ResultLive);

        double elapsedTime = static_cast<double>(end_step1.QuadPart - start_step1.QuadPart) * 1000.0 / frequency.QuadPart;
        cout << " Time: " << elapsedTime << " ms   ";

        instance.PrintStepResult(2, true, Step2ResultTest);
        instance.PrintStepResult(2, false, Step2ResultLive);

        elapsedTime = static_cast<double>(end_step2.QuadPart - start_step2.QuadPart) * 1000.0 / frequency.QuadPart;
        cout << " Time: " << elapsedTime << " ms";

        cout << endl;
    }

protected:
    void PrintStepResult(const int Step, bool IsTesting, const long& Result);

    void ReadExpectedStepResults();
    void CreateEmptyExpectedStepResultsFile(const std::string& FileName);

    const bool IsTest() const;
    void SetTest(const bool IsTest);

    virtual void OnInitTests();
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
    virtual const long Step1() = 0;
    virtual const long Step2() = 0;

private:
    const std::string GetFileName(const int Step) const;
    bool IsUnderTest{ false };

    long ExpectedTestResultForStep1{ 0 };
    long ExpectedLiveResultForStep1{ 0 };
    long ExpectedTestResultForStep2{ 0 };
    long ExpectedLiveResultForStep2{ 0 };
};



// use AoC toolset 
// from this path:
// https://github.com/breakthatbass/eggnog/tree/main/src
//