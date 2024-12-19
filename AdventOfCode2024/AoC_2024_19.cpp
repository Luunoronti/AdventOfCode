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
unordered_map<int, unordered_set<int>> AllTowels;
unordered_set<int> allKnownLengths;

uint64_t tt = 0;
uint64_t CheckDesign(const std::string& design, int startindex)
{
    if(startindex == design.size())
    {
        cout << "Found! " << ++tt << endl;
        return 1;
    }

    uint64_t localSum = 0;
    for(auto towelLen : allKnownLengths)
    {
        if(towelLen + startindex > design.size())
            continue;
        const unordered_set<int> towelSums = AllTowels[towelLen];
        int cs = CalculateChecksum(design, startindex, towelLen);
        if(towelSums.count(cs) == 1)
        {
            localSum += CheckDesign(design, startindex + towelLen);
        }
    }
    return localSum;
}
const int64_t AoC_2024_19::Step1()
{
    //if(!IsTest())return 0;
    TIME_PART;
    AllTowels.clear();
    testMap.clear();
    vector<string> TowelsStr;
    vector<string> Designs;
    if(0 != ReadInput(GetFileName(), TowelsStr, Designs))
        return 0;

    for(const auto& towel : TowelsStr)
    {
        const int len = towel.length();
        int cs = CalculateChecksum(towel, 0, len);
        AllTowels[len].insert(cs);
        testMap[cs] = towel;
        allKnownLengths.insert(len);
    }

    int64_t sum = 0;
    for(const auto& d : Designs)
    {
        if(CheckDesign(d, 0))
            sum++;
    }
    return sum;
};
const int64_t AoC_2024_19::Step2()
{
    TIME_PART;
    return 0;
};
