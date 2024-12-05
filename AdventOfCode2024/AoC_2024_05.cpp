#include "AoC_2024_05.h"

const bool AoC_2024_05::CheckRule(PageList InList, PageRule InRule) const
{
    int i1 = IndexOf(InList, InRule.x);
    int i2 = IndexOf(InList, InRule.y);

    bool good = (i1 == -1 || i2 == -1 || i1 < i2);

    if(!good)
    {
        cout << i1 << "   " << i2 << endl;
        cout << InRule.x << "   " << InRule.y << endl;
    }
    return (i1 == -1 || i2 == -1 || i1 < i2);
}
const bool AoC_2024_05::CheckAllRules(PageList InList) const
{
    for(const auto& r : Rules)
    {
        if(!CheckRule(InList, r))
            return false;
    }
    return true;
}

const long AoC_2024_05::Step1()
{
    ReadInput();

    long sum = 0;
    for(PageList& pageList : PageLists)
    {
        if(CheckAllRules(pageList))
        {
            sum += pageList[pageList.size() / 2];
        }
    }
    return sum;
}

const long AoC_2024_05::Step2()
{
    return 0;
}



void AoC_2024_05::ReadInput()
{
    vector<string> lines = ReadStringLinesFromFile(1);

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
