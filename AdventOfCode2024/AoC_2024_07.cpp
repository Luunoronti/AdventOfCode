#include "AoC_2024_07.h"

void AoC_2024_07::OnInitTestingTests()
{
    InputData.clear();
    auto inputLines = ReadStringLinesFromFile(1);
    for(const string& line : inputLines)
    {
        ParseInputLineAndStoreInputData(line);
    }
}
void AoC_2024_07::OnInitLiveTests()
{
    OnInitTestingTests();
}
void AoC_2024_07::ParseInputLineAndStoreInputData(const string& Line)
{
    istringstream s(Line);

    int64_t expectedValue = 0;
    s >> expectedValue;
    s.ignore(1, ':');

    vector<int> operands;
    int operand;
    while(s >> operand)
        operands.push_back(operand);

    InputData.emplace_back(expectedValue, operands);
}
const int64_t AoC_2024_07::Step1()
{
    int64_t sum = 0;
    parallel_for_each(InputData.begin(), InputData.end(), [&](SingleLineData& Data)
        {
            InterlockedAdd64(&sum, ProcessDataReverse(Data, false));
        });
    return sum;
}
const int64_t AoC_2024_07::Step2()
{
    int64_t sum = 0;
    parallel_for_each(InputData.begin(), InputData.end(), [&](SingleLineData& Data)
          {
              InterlockedAdd64(&sum, ProcessDataReverse(Data, true));
          });
    return sum;
}

const int64_t AoC_2024_07::ProcessDataReverse(const SingleLineData& Data, bool AllowConcat) const
{   
    int64_t expectedValue = Data.first;
    vector<int> operands = Data.second;

    int operandCount = static_cast<int>(operands.size());
    Cache2 cache(operandCount);

    // init cache with expectedValue
    cache[operandCount - 1].push_back(expectedValue);
    int expectedOperand = operands[0];

    for(int position = operandCount - 1; position > 0; --position)
    {
        const auto& lastPositionCache = cache[position];
        cache[position - 1].reserve(lastPositionCache.size() * 3);

        for(const auto& cachedValue : lastPositionCache)
        {
            int currentOp = operands[position];

            int64_t subOpResult = cachedValue - currentOp;
            int64_t divOpResult = cachedValue / currentOp;
            bool dividable = cachedValue % currentOp == 0;
            bool subtractable = subOpResult > 0;

            // there must be some better way for this check
            bool concatenable = AllowConcat && StringEndsWith(to_string(cachedValue), to_string(currentOp));
            int64_t conOpResult = 0;
            if(concatenable)
            {
                AllowConcat = StringEndsWith(to_string(cachedValue), to_string(currentOp));
                conOpResult = RemoveSubNumber(cachedValue, currentOp);
                concatenable = conOpResult >= 0;
            }

            // if last loop iteration, check for exit conditions
            if(position == 1)
            {
                if(subOpResult == expectedOperand) return expectedValue;
                if(divOpResult == expectedOperand && dividable) return expectedValue;
                if(conOpResult == expectedOperand && concatenable) return expectedValue;
            }
            // else add results to cache
            else
            {
                if(subtractable) cache[position - 1].push_back(subOpResult);
                if(dividable) cache[position - 1].push_back(divOpResult);
                if(concatenable) cache[position - 1].push_back(conOpResult);
            }
        }
    }
    return 0;
}


/*
New approach:
 compute all possibilities, keep results in a Hash
 where key is a bit mask of operations performed
 and value is the result

bit mask for each operand:
00(b) = calculation not done yet
01(b) = ADD (already performed)
10(b) = MUL (already performed)
11(b) = CONCAT (already performed)

each loop moves bits of operations left by 2 places, and then
adds bits of operations. this way, we get unique hash key with each
loop cycle and each operation

when we compute new value, we get the last result from the table

while computing, if any result is equal to expected result, we just
return true and quit
* */
#define OP_ADD 0x01
#define OP_MUL 0x02
#define OP_CON 0x03

const int64_t AoC_2024_07::ProcessData(const SingleLineData& Data, bool AllowConcat) const
{
    int64_t expectedValue = Data.first;
    vector<int> operands = Data.second;

    int operandCount = static_cast<int>(operands.size());
    Cache cache(operandCount);

    // to make code even cleaner, define some local macros
#define SAVE_CACHE(OP, VAL) cache[position][(mask| OP)] = VAL;
#define CURRENT_OP operands[position]
#define CACHED_RESULT pair.second

    // init cache with first operand
    cache[0][0] = operands[0];

    // if expected value is not divisible by last operand,
    // operations involving last MUL can be omitted
    // same with CONCAT, if expected value does not end with last operand, it's a no go
    //
    // these two flags shave around 100ms of total execution time, which means 20-25% decrease
    bool lastMulPossible = expectedValue % operands[operandCount - 1] == 0;
    bool lastConPossible = AllowConcat && StringEndsWith(to_string(expectedValue), to_string(operands[operandCount - 1]));

    for(int position = 1; position < operandCount; ++position)
    {
        const auto& lastPositionCache = cache[position - 1];
        cache[position].reserve(lastPositionCache.size() * 3); // this alone gave us around 20-60ms of execution time

        for(const auto& pair : lastPositionCache)
        {
            // make place in new mask for opcodes
            int mask = pair.first << 2;
            // we must check if next operand is == 1, if so, we must add result to cache even if we are equal to expected value
            bool nextOpIsOne = position < operandCount - 1 ? operands[position + 1] == 1 : false;
            // if last loop, check if result equals expected
            bool lastLoop = position == operandCount - 1;

            // always perform ADD
            int64_t addOpResult = CACHED_RESULT + CURRENT_OP;
            if(lastLoop && addOpResult == expectedValue) return expectedValue;
            // add only if we didn't exceed expected value already
            if(addOpResult < expectedValue || nextOpIsOne) { SAVE_CACHE(OP_ADD, addOpResult); }

            // same as above, just do concat
            if(AllowConcat && (!lastLoop || lastConPossible))
            {
                int64_t conOpResult = ConcatValues(CACHED_RESULT, CURRENT_OP);
                if(lastLoop && conOpResult == expectedValue) return expectedValue;
                if(conOpResult < expectedValue || nextOpIsOne) SAVE_CACHE(OP_CON, conOpResult);
            }

            // perform MUL only if possible
            if((!lastLoop || lastMulPossible))
            {
                // attempt all operations and store results
                int64_t mulOpResult = CACHED_RESULT * CURRENT_OP;
                if(lastLoop && (addOpResult == expectedValue || mulOpResult == expectedValue)) return expectedValue;
                if(mulOpResult < expectedValue || nextOpIsOne) { SAVE_CACHE(OP_MUL, mulOpResult); }
            }
        }
    }

    // nothing, we found nothing
    return 0;
}
