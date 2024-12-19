#include "pch.h"
#include "AoC_2024_19.h"
#include <unordered_set>

using namespace std;

void rtrim(char* str)
{
    size_t len = std::strlen(str);
    while(len > 0 && std::isspace(str[len - 1]))
    {
        str[len - 1] = '\0';
        len--;
    }
}

int ReadInput(const string& FileName, vector<string>& Towels, vector<string>& Designs)
{
    FILE* file;
    fopen_s(&file, FileName.c_str(), "r");
    if(!file)
    {
        std::cerr << "Error opening file: " << FileName.c_str() << std::endl;
        return -1;
    }

    char line[1024 * 10];
    // Read the first line
    if(std::fgets(line, sizeof(line), file))
    {
        rtrim(line);
        char* context = nullptr;
        char* token = strtok_s(line, ", ", &context);
        while(token)
        {
            Towels.push_back(token);
            token = strtok_s(nullptr, ", ", &context);
        }
    }
    // Skip the blank line
    if(std::fgets(line, sizeof(line), file) && line[0] != '\n')
    {
        std::cerr << "Expected a blank line after the first line." << std::endl;
        std::fclose(file);
        return -2;
    }
    // Read the remaining lines
    while(std::fgets(line, sizeof(line), file))
    {
        rtrim(line);
        if(std::strlen(line) > 0)
        {
            Designs.push_back(line);
        }
    }
    std::fclose(file);
    return 0;
}

int CalculateChecksum(const std::string& str, size_t startIndex, size_t requiredLength)
{
    if(startIndex + requiredLength > str.length())
    {
        return 0;
    }
    int checksum = 0;
    for(size_t i = startIndex; i < startIndex + requiredLength; ++i)
    {
        checksum = (checksum * 31 + str[i]) % 1000000007;
    }
    return checksum;
}

unordered_map<int, string> testMap;
int CountMatchesInDesign(const string& Design, int startIndex, const unordered_set<int> Towels, const int maxTowelLen)
{
    int total = 0;
    for(size_t len = 1; len <= maxTowelLen; ++len)
    {
        int checksum = CalculateChecksum(Design, startIndex, len);
        if(checksum != 0)
        {
            if(Towels.count(checksum))
            {
                string test = testMap[checksum];
                if(startIndex + len == Design.length())
                {
                    return total + 1;
                }
                else
                {
                    total += CountMatchesInDesign(Design, startIndex + len, Towels, maxTowelLen);
                }
            }
        }
    }
    return total;
}

bool containsAtLocation(const std::string& mainStr, const std::string& subStr, size_t location)
{
    if(location + subStr.length() > mainStr.length()) { return false; }
    for(size_t i = 0; i < subStr.length(); ++i)
    {
        if(mainStr[location + i] != subStr[i])
        {
            return false;
        }
    }
    return true;
}

int CountMatchesInDesign2(const string& Design, const vector<string> Towels)
{
    int total = 0;
    for(const auto& towel : Towels)
    {
        if(containsAtLocation(Design, towel, 0))
        {
            if(Design.length() == towel.length())
                total += 1;
            else
                total += CountMatchesInDesign2(Design.substr(0, towel.length()), Towels);
        }
    }
    return total;
}

const int64_t AoC_2024_19::Step1()
{
    //if(!IsTest())return 0;
    TIME_PART;
    vector<string> TowelsStr;
    vector<string> Designs;
    if(0 != ReadInput(GetFileName(), TowelsStr, Designs))
    {
        return 0;
    }

    unordered_set<int> Towels;
    int maxTowelLen = 0;
    for(const auto& towel : TowelsStr)
    {
        int cs = CalculateChecksum(towel, 0, towel.length());
        Towels.insert(cs);
        testMap[cs] = towel;
        maxTowelLen = max(maxTowelLen, towel.length());
    }


    int sum = 0;

    for(const auto& d : Designs)
    {
        if(CountMatchesInDesign2(d, TowelsStr))
            sum++;
    }

    return sum;
};
const int64_t AoC_2024_19::Step2()
{
    TIME_PART;
    return 0;
};


#include <iostream>
#include <cstdio>
#include <vector>
#include <string>
#include <cstring>

