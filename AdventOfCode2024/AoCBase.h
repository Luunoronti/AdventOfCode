#pragma once

#include <iostream> 
#include <fstream> 
#include <vector> 
#include <sstream>
#include <iomanip>
#include <algorithm>
#include <cstdlib>
#include <numeric>

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
        static_assert(std::is_base_of<AoCBase, T>::value, "T must derive from AoCBase"); 
        T instance;

        cout << DIM << "====================================================================================" << RESET << endl;

        instance.SetTest(true);
        const auto& Step1ResultTest = instance.Step1();
        const auto& Step2ResultTest = instance.Step2();
        instance.SetTest(false);
        const auto& Step1ResultLive = instance.Step1();
        const auto& Step2ResultLive = instance.Step2();

        cout << "" << instance.GetYear() << "/" << std::setw(2) << std::setfill('0') << instance.GetDay() << ":  ";
        instance.PrintStepResult(1, true, Step1ResultTest);
        instance.PrintStepResult(1, false, Step1ResultLive); 

        instance.PrintStepResult(2, true, Step2ResultTest);
        instance.PrintStepResult(2, false, Step2ResultLive); 

        //cout << endl << DIM << "====================================================================================" << endl;
        cout << endl;
    }

protected:
    void PrintStepResult(const int Step, bool IsTesting, const long& Result);
   
    const bool IsTest() const;
    void SetTest(const bool IsTest);

    virtual const int GetYear() const;
    virtual const int GetDay() const = 0;

    virtual const int GetExpectedResultStep1() const = 0;
    virtual const int GetExpectedResultStep1Test() const = 0;

    virtual const int GetExpectedResultStep2() const = 0;
    virtual const int GetExpectedResultStep2Test() const = 0;

    LongListList ReadLongVectorsFromFile(int Step) const;

    LongListList ReadVerticalVectorsFromFile(int Step) const;
    string ReadStringFromFile(int Step, int& LinesCount, int& LastLineWidth) const;
    string ReadStringFromFile(int Step) const;

    const int GetFileSingleLineWidth(int Step) const;
    static long GetMinimum(const vector<long>& List);


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
};



// use AoC toolset 
// from this path:
// https://github.com/breakthatbass/eggnog/tree/main/src
//