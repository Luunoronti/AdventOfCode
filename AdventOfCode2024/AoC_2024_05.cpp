#include "pch.h"
#include "AoC_2024_05.h"

const bool AoC_2024_05::CheckRule(PageList InList, PageRule InRule, long& i1, long& i2) const
{
    i1 = IndexOf(InList, InRule.x);
    i2 = IndexOf(InList, InRule.y);
    return (i1 == -1 || i2 == -1 || i1 < i2);
}
const bool AoC_2024_05::CheckAllRules(PageList InList, long& errIndex1, long& errIndex2) const
{
    for(const auto& r : Rules)
    {
        if(!CheckRule(InList, r, errIndex1, errIndex2))
            return false;
    }
    return true;
}
const int64_t AoC_2024_05::Step1()
{
    Rules.clear();
    PageLists.clear();
    ErrorLists.clear();

    ReadInput();
    TIME_PART;

    long sum = 0;
    long i1 = 0, i2 = 0;
    for(PageList& pageList : PageLists)
    {
        if(CheckAllRules(pageList, i1, i2))
        {
            sum += pageList[pageList.size() / 2];
        }
        else
        {
            ErrorLists.push_back(pageList);
        }
    }
    return sum;
}

const int64_t AoC_2024_05::Step2()
{
    TIME_PART;
    long sum = 0;

    concurrency::combinable<long> sum_combiner([]() { return 0; });
    concurrency::parallel_for(0, static_cast<int>(ErrorLists.size()), [&](int i)
        {
            PageList& pageList = ErrorLists[i];

            long tmp, i1, i2;
            while(true)
            {
                if(!CheckAllRules(pageList, i1, i2)) // CheckAllRules only checks till first invalid is found, not actual 'all'
                {
                    tmp = pageList[i1];
                    pageList[i1] = pageList[i2];
                    pageList[i2] = tmp;
                }
                else
                {
                    break;
                }
            };

            sum_combiner.local() += pageList[pageList.size() / 2];
        });
    return sum_combiner.combine(std::plus<long>());
}



void AoC_2024_05::ReadInput()
{
    vector<string> lines;
    aoc::AoCStream(GetFileName()) >> lines;

    stringstream s;
    for(const auto& l : lines)
    {
        s.clear();
        s.str(l);

        std::string val_x, val_y;
        if(std::getline(s, val_x, '|') && std::getline(s, val_y, '|'))
        {
            Rules.push_back(PageRule(std::stoi(val_x), std::stoi(val_y)));
            continue;
        }

        if(lines.size() == 0)
            continue;

        s.clear();
        s.seekg(0, s.beg);
        PageList pages;
        while(std::getline(s, val_x, ','))
        {
            pages.push_back(std::stoi(val_x));
        }
        if(pages.size() > 0)
            PageLists.push_back(pages);
    }
}
