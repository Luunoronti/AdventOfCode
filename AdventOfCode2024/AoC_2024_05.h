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
    const long Step1() override;
    const long Step2() override;
    friend class AoCBase;

    vector<PageRule> Rules;
    vector<PageList> PageLists;

    vector<PageList> ErrorLists;

    void ReadInput();
    const bool CheckRule(PageList InList, PageRule InRule, int& i1, int& i2) const;
    const bool CheckAllRules(PageList InList, int& errIndex1, int& errIndex2) const;

};

