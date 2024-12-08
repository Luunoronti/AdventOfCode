#pragma once
#include "AoC2024.h"




class AoC_2024_07 : public AoC2024
{
    typedef unordered_map<int, int64_t> CacheLine;
    typedef vector<CacheLine> Cache;
    typedef std::pair<int64_t, std::vector<int>> SingleLineData;

    typedef vector<vector<int64_t>> Cache2;

    // Inherited via AoCBase
public:
    const int GetDay() const override { return 7; };

    const int64_t Step1() override;
    const int64_t Step2() override;
    void OnInitTestingTests() override;
    void OnInitLiveTests() override;

private:
    void ParseInputLineAndStoreInputData(const string& Line);
    const int64_t ProcessData(const SingleLineData& Data, bool AllowConcat) const;
    const int64_t ProcessDataReverse(const SingleLineData& Data, bool AllowConcat) const;

    static __forceinline const int64_t ConcatValues(const int64_t& i1, const int64_t& i2)
    {
        // Determine the number of digits in operand_2
        int64_t numDigits = static_cast<int64_t>(log10(i2)) + 1;
        // Multiply op1 by 10 raised to the number of digits in op2
        int64_t multiplier = static_cast<int64_t>(pow(10, numDigits));
        return i1 * multiplier + i2;
    }

    static __forceinline const bool StringEndsWith(const std::string& a, const std::string& b)
    {
        // Check if b is longer than a 
        if(b.size() > a.size())
        {
            return false;                              
        }
        // Find b in a starting from the end 
        return a.rfind(b) == (a.size() - b.size());
    }
    static __forceinline const int64_t RemoveSubNumber(int64_t a, int64_t b) 
    { 
        std::string strA = std::to_string(a); 
        std::string strB = std::to_string(b); 
        size_t pos = strA.rfind(strB); 

        if (pos != std::string::npos) 
        { 
            strA.erase(pos, strB.length()); 
        } 
        if(strA.size() == 0) return 0;

        return std::stoll(strA); 
    }


private:
    std::vector<SingleLineData> InputData;
};

