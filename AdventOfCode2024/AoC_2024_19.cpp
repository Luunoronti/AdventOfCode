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

uint64_t CheckDesign(const std::string& design, int startindex)
{
    uint64_t localSum = 0;
    for(auto towelType : AllTowels)
    {
        const int len = towelType.first;
        const unordered_set<int> towelSums = towelType.second;

        int cs = CalculateChecksum(design, startindex, len);
        // there can be one and only one checksum of this length
        // technically, we should assert if count > 1
        if(towelSums.count(cs) == 1)
        {
            // we may also add this to our checksum
            // to speed up our processing, because if 'aa' and then 'bb' is possible
            // then 'aabb' is also possible
            //AllTowels[startindex + len].insert(CalculateChecksum(design, 0, startindex + len));
            auto test = testMap[cs];
            // found. if we are at the end of the string
            // just add 1 found to answer.
            //  else, keep looking inside
            if(startindex + len >= design.size())
            {
                ++localSum;
                cout << test << endl;
            }
            else
            {
                localSum += CheckDesign(design, startindex + len);
                cout << test << " ";
            }


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
    }

    int64_t sum = 0;
    for(const auto& d : Designs)
    {
        if(CheckDesign(d, 0))
            sum++;
        cout << endl;
    }
    return sum;
};
const int64_t AoC_2024_19::Step2()
{
    TIME_PART;
    return 0;
};
