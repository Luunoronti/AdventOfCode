#pragma once
#include "AoCBase.h"


class AoC_2024_05 : public AoCBase
{
    struct PageRule
    {
        int x;
        int y;

        PageRule() = default;

        PageRule(int x, int y)
            : x(x), y(y)
        {
        }
    };

    typedef vector<int> PageList;


    const virtual __forceinline int GetDay() const override { return 5; }
    // Inherited via AoCBase
    const int64_t Step1() override;
    const int64_t Step2() override;
    friend class AoCBase;

    vector<PageRule> Rules;
    vector<PageList> PageLists;

    vector<PageList> ErrorLists;

    void ReadInput();
    const bool CheckRule(PageList InList, PageRule InRule, long& i1, long& i2) const;
    const bool CheckAllRules(PageList InList, long& errIndex1, long& errIndex2) const;

};

