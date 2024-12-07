#include "AoC_2024_07.h"

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

const int64_t AoC_2024_07::Step1()
{
    vector<string> lines = ReadStringLinesFromFile(1);
    concurrency::combinable<int64_t> sum_combiner([]() { return 0; });
    concurrency::parallel_for(0, static_cast<int>(lines.size()), [&](int i)
        {
            sum_combiner.local() += TestSingleLine(lines[i], false);
        });

    return sum_combiner.combine(std::plus<int64_t>());
}
const int64_t AoC_2024_07::Step2()
{
    vector<string> lines = ReadStringLinesFromFile(2);
    
    concurrency::combinable<int64_t> sum_combiner([]() { return 0; });
    concurrency::parallel_for(0, static_cast<int>(lines.size()), [&](int i)
        {
            sum_combiner.local() += TestSingleLine(lines[i], true);
        });

    return sum_combiner.combine(std::plus<int64_t>());
}


/*

The Operators variable is a single int that holds operators for all operands in Operands vector.
Each two bits represent the flag for operation,
0x00 = ADD
0x01 = MUL
0x10 = CONCAT
0x11 = NO_OP

And now, you can check for every possible combination of operations
just by incrementing Operators variable, because ++Operators will just move bits around
and produce unique sets of operators.

For example if Operators = 00011000(b) (ADD, MUL, CON, ADD)
and you do ++Operators, it becomes 00011001(b) (ADD, MUL, CON, MUL)
and then do ++Operators, it becomes 00011010(b) (ADD, MUL, CON, CON)
and so on.

* */
const bool AoC_2024_07::TestForValidResultOnOperators(const int64_t& ExpectedResult, const vector<int> Operands, int Operators, bool AllowThird) const
{
    uint8_t op;
    uint8_t opPos;
    uint8_t size = static_cast<uint8_t>(Operands.size());
    int64_t actualResult = Operands[0];

    for(int i = 0; i < 16; ++i)
    {
        op = (Operators >> (i * 2)) & 0x03;
        if(AllowThird)
        {
            if(op == 0x03)
                return false;
        }
        else
        {
            if(op == 0x02 || op == 0x03)
                return false;
        }
    }

    for(int i = 1; i < Operands.size(); ++i)
    {
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
        TODO("We may have an infinite loop in case no solution was found and operatorMark size check fails");
    }
}


