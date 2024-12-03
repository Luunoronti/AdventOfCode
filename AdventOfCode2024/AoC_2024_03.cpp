#include "AoC_2024_03.h"

const int AoC_2024_03::GetDay() const
{
    return 3;
}

const int AoC_2024_03::GetExpectedResultStep1() const
{
    return 0;
}

const int AoC_2024_03::GetExpectedResultStep1Test() const
{
    return 161;
}

const int AoC_2024_03::GetExpectedResultStep2() const
{
    return 0;
}

const int AoC_2024_03::GetExpectedResultStep2Test() const
{
    return 0;
}

const long AoC_2024_03::Step1()
{
    string str = ReadStringFromFile(1);


    // given the input, how to do it without regex

    // xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))
    // we can make a simple, state machine parser


    // Regular expression to match mul(x,y) without spaces 
    std::regex reg(R"(mul\((\d+),(\d+)\))"); 
    
    // Iterator to find all matches 
    auto words_begin = std::sregex_iterator(str.begin(), str.end(), reg); 
    auto words_end = std::sregex_iterator(); 

    long sum = 0;

    for (std::sregex_iterator i = words_begin; i != words_end; ++i) 
    { 
        std::smatch match = *i; 
        sum += stol(match[1].str()) * stol(match[2].str());
    }
    return sum;
}

const long AoC_2024_03::Step2()
{
    return 0;
}
