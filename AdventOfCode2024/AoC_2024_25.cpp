#include "pch.h"
#include "AoC_2024_25.h"

bool allCharactersSame(const std::string& s)
{
    return std::all_of(s.begin(), s.end(), [&](char c) { return c == s[0]; });
}
static bool isLockStart(const string& s)
{
    return s[0] == '#' && allCharactersSame(s);
}
static bool isKeyStart(const string& s)
{
    return s[0] == '.' && allCharactersSame(s);
}

static void readLocksAndKeys(const vector<string>& input, vector<vector<int>>& Locks, vector<vector<int>>& Keys, int& LockHeight)
{
    LockHeight = 0;
    for(int i = 0; i < input.size(); i++)
    {
        const string& l = input[i];
        if(isLockStart(l))
        {
            vector<int> lock(l.size());
            for(int j = i; j < input.size(); j++)
            {
                const string& l2 = input[j];
                if(l2[0] == '\0')
                {
                    if(LockHeight == 0)
                        LockHeight = j - i;
                    break;
                }
                for(int c = 0; c < l2.size(); c++) if(l2[c] == '#') lock[c]++;
            }
            Locks.push_back(lock);
            i += LockHeight;
        }
        else if(isKeyStart(l))
        {
            vector<int> key(l.size());
            for(int j = i; j < input.size(); j++)
            {
                const string& l2 = input[j];
                if(l2[0] == '\0')
                {
                    if(LockHeight == 0)
                        LockHeight = j - i;
                    break;
                }
                for(int c = 0; c < l2.size(); c++) if(l2[c] == '#') key[c]++;
            }
            Keys.push_back(key);
            i += LockHeight;
        }
    }
}
const int64_t AoC_2024_25::Step1()
{
    TIME_PART;

    vector<string> input;
    aoc::AoCStream() >> input;

    vector<vector<int>> Locks;
    vector<vector<int>> Keys;
    int LockHeight;
    readLocksAndKeys(input, Locks, Keys, LockHeight);

    int matches = 0;
    for(const auto& lock : Locks)
    {
        for(const auto& key : Keys)
        {
            bool match = true;
            for(int i = 0; i < lock.size(); i++)
            {
                if(key[i] + lock[i] > LockHeight)
                {
                    match = false;
                    break;
                }
            }
            if(match)
                matches++;
        }
    }

    return matches;
}

const int64_t AoC_2024_25::Step2()
{
    TIME_PART;
    return 1;
}
