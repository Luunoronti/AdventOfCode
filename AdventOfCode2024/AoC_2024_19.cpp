#include "pch.h"
#include "AoC_2024_19.h"
#include <unordered_set>

using namespace std;

void rtrim(string& str)
{
    size_t len = str.size();
    while(len > 0 && std::isspace(str[len - 1]))
    {
        str[len - 1] = '\0';
        len--;
    }
}
int ReadInput(const string& FileName, vector<string>& Towels, vector<string>& Designs)
{
    vector<string> lines;
    aoc::AoCStream() >> lines;

    string line = lines[0];
    // Read the first line
    rtrim(line);
    char* context = nullptr;
    char* token = strtok_s(&line[0], ", ", &context);
    while(token)
    {
        Towels.push_back(token);
        token = strtok_s(nullptr, ", ", &context);
    }

    // Skip the blank line
    for(int i = 2; i < lines.size(); i++)
    {
        string l = lines[i];
        rtrim(l);
        if(l.size() > 0)
            Designs.push_back(l);
    }
    return 0;
}

bool startsWith(const std::string& a, const std::string& b)
{
    if(b.size() > a.size())
        return false;
    return a.compare(0, b.size(), b) == 0;
}
bool isSame(const std::string& a, const std::string& b)
{
    if(b.size() != a.size())
        return false;
    return a.compare(0, b.size(), b) == 0;
}

bool IsDesignPossible(string design, vector<string> towels, unordered_map<string, bool>& cache)
{
    if(cache.find(design) != cache.end())
        return cache[design];

    for(int i = 0; i < towels.size(); ++i)
    {
        if(isSame(design, towels[i]))
            return true;
    }

    for(int i = 1; i < design.size(); ++i)
    {
        if(IsDesignPossible(design.substr(0, i), towels, cache)
            && IsDesignPossible(design.substr(i), towels, cache))
        {
            cache[design] = true;
            return true;
        }
    }
    cache[design] = false;
    return false;
}

int64_t SumPossibleDesigns(string design, vector<string> towels, unordered_map<string, int64_t>& cache)
{
    int64_t loc = 0;
    if(cache.find(design) != cache.end())
        return cache[design];

    for(int i = 0; i < towels.size(); ++i)
    {
        if(isSame(design, towels[i]))
            loc++;
    }

    for(int i = 0; i < towels.size(); ++i)
    {
        if(design.size() > towels[i].size() && startsWith(design, towels[i]))
            loc += SumPossibleDesigns(design.substr(towels[i].size()), towels, cache);
    }

    cache[design] = loc;
    return loc;
}


const int64_t AoC_2024_19::Step1()
{
    //if(!IsTest())return 0;
    TIME_PART;
    vector<string> TowelsStr;
    vector<string> Designs;
    if(0 != ReadInput(GetFileName(), TowelsStr, Designs))
        return 0;

    std::sort(TowelsStr.begin(), TowelsStr.end(), [](const string& a, const string& b) { return a.size() > b.size(); });

    int sum = 0;
    unordered_map<string, bool> cache;
    for(const auto& d : Designs)
    {
        if(IsDesignPossible(d, TowelsStr, cache))
            sum++;
    }
    return sum;
};
const int64_t AoC_2024_19::Step2()
{
    //if(!IsTest())return 0;

    TIME_PART;
    vector<string> TowelsStr;
    vector<string> Designs;
    if(0 != ReadInput(GetFileName(), TowelsStr, Designs))
        return 0;

    std::sort(TowelsStr.begin(), TowelsStr.end(), [](const string& a, const string& b) { return a.size() > b.size(); });

    int64_t sum = 0;
    unordered_map<string, int64_t> cache;
    for(const auto& d : Designs)
    {
        sum += SumPossibleDesigns(d, TowelsStr, cache);
    }
    return sum;
};
