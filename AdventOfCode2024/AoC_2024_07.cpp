#include "AoC_2024_07.h"

const int64_t AoC_2024_07::Step1()
{
    vector<string> lines = ReadStringLinesFromFile(1);
    concurrency::combinable<int64_t> sum_combiner([]() { return 0; });
    concurrency::parallel_for(0, static_cast<int>(lines.size()), [&](int i)
        {
            const auto& line = lines[i];
            sum_combiner.local() += TestSingleLine(line, false);
        });

    return sum_combiner.combine(std::plus<int64_t>());
}
const int64_t AoC_2024_07::Step2()
{
    vector<string> lines = ReadStringLinesFromFile(1);
    concurrency::combinable<int64_t> sum_combiner([]() { return 0; });
    concurrency::parallel_for(0, static_cast<int>(lines.size()), [&](int i)
        {
            const auto& line = lines[i];
            sum_combiner.local() += TestSingleLine(line, true);
        });

    return sum_combiner.combine(std::plus<int64_t>());
}



const bool AoC_2024_07::TestForValidResultOnOperators(const int64_t& ExpectedResult, const vector<int> Operands, int Operators, bool AllowThird) const
{
    uint8_t op;
    uint8_t opPos;
    uint8_t size = static_cast<uint8_t>(Operands.size());
    int64_t actualResult = Operands[0];

    // check if any op is 10 or 01, these are not valid
    for(int i = 1; i < Operands.size(); ++i)
    {
        op = (Operators >> ((i - 1) * 2)) & 0x03;
        if(op == 0x03)
            return false;
    }

    for(int i = 1; i < Operands.size(); ++i)
    {
        opPos = 16 - i; // there are 16 possible positions in our mask
        op = (Operators >> ((i - 1) * 2)) & 0x03;
        const auto& o1 = Operands[i];
        switch(op)
        {
        case 00:
            actualResult = actualResult + o1;
            break;
        case 01:
            actualResult = actualResult * o1;
            break;
        case 02:
            if(AllowThird)
                actualResult = std::stoll(std::to_string(actualResult) + std::to_string(o1));
            break;
        }
    }
    return actualResult == ExpectedResult;
}

const int64_t AoC_2024_07::TestSingleLine(const string& Line, bool AllowThird) const
{
    int64_t expectedValue = 0;
    vector<int> operands;
    int operatorsMark = 0;

    ParseInputLine(Line, expectedValue, operands);

    while(true)
    {
        if(TestForValidResultOnOperators(expectedValue, operands, operatorsMark, AllowThird))
        {
            return expectedValue;
        }
        ++operatorsMark;
        if(operatorsMark > (1 << ((operands.size() - 1) * 2)))
        {
            return 0; // no result
        }
        TODO("if there is any bit set above the expected number of operands, skip.we have an error");
    }
}


void AoC_2024_07::ParseInputLine(const string& Line, int64_t& ExpectedResult, vector<int>& Operands) const
{
    istringstream s(Line);
    int operand;

    s >> ExpectedResult;
    s.ignore(1, ':');

    while(s >> operand)
    {
        Operands.push_back(operand);
    }
}