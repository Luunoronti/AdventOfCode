#include "AoCBase.h"


const int AoCBase::GetYear() const
{
    return 2024;
}

const bool AoCBase::IsTest() const
{
    return IsUnderTest;
}
void AoCBase::SetTest(const bool IsTest)
{
    IsUnderTest = IsTest;
}

void AoCBase::PrintStepResult(const int Step, bool IsTesting, const long& Result)
{
    bool isValid = false;
    bool isWrong = false;

    const long& ExpectedResult = Step == 1 ? (IsTesting ? GetExpectedResultStep1Test() : GetExpectedResultStep1()) : (IsTesting ? GetExpectedResultStep2Test() : GetExpectedResultStep2());
    isValid = ExpectedResult && ExpectedResult == Result;
    isWrong = ExpectedResult && ExpectedResult != Result;

    cout << UNDERLINE << "Step " << Step << RESET << (IsTesting ? BOLD + MAGENTA + " (TEST)" + RESET : BOLD + GREEN + " (LIVE)" + RESET) << ": "
        << (isValid ? GREEN : "") << (isWrong ? RED : "") << Result << RESET
        << (isWrong ? RED + " !===! " + std::to_string(ExpectedResult) + BLINK + " -> WRONG. " + RESET : "")
        << (isValid ? GREEN + " -> GOOD. " + RESET : "")
        << ((!isValid && !isWrong) ? YELLOW + BLINK + " -> Not verified. " + RESET : "")
        << "  ";
}

string AoCBase::ReadStringFromFile(int Step) const
{
    int w, h;
    return ReadStringFromFile(Step, w, h);
}
string AoCBase::ReadStringFromFile(int Step, int& LinesCount, int& LastLineWidth) const
{
    std::ifstream file(GetFileName(Step));
    std::string output;
    std::string line;
    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName(Step) << RESET << std::endl;
        return line;
    }
    while(std::getline(file, line))
    {
        output += line;
        ++LinesCount;
    }
    LastLineWidth = line.size();
    file.close();
    return output;
}

const int AoCBase::GetFileSingleLineWidth(int Step) const
{
    std::ifstream file(GetFileName(Step));
    std::string line;
    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName(Step) << RESET << std::endl;
        return -1;
    }
    if(!std::getline(file, line))
        return -1;
    file.close();
    return line.size();
}
LongListList AoCBase::ReadVerticalVectorsFromFile(int Step) const
{
    std::ifstream file(GetFileName(Step));
    LongListList vectors;

    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName(Step) << RESET << std::endl;
        return vectors;
    }

    std::string line;
    while(std::getline(file, line))
    {
        if(vectors.size() == 0)
        {
            std::stringstream ss2(line);
            long number;
            long count = 0;
            while(ss2 >> number)
                count++;

            for(int i = 0; i < count; i++)
            {
                vectors.push_back(std::vector<long>());
            }
        }

        std::stringstream ss(line);
        long number;
        int count = 0;
        while(ss >> number)
        {
            vectors[count].push_back(number);
            count++;
        }
    }

    file.close();
    return vectors;
}



LongListList AoCBase::ReadLongVectorsFromFile(int Step) const
{
    std::ifstream file(GetFileName(Step));
    LongListList vectors;

    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName(Step) << RESET << std::endl;
        return vectors;
    }

    std::string line;
    while(std::getline(file, line))
    {
        std::stringstream ss(line);
        std::vector<long> vector;
        long number;
        while(ss >> number)
        {
            vector.push_back(number);
        }
        vectors.push_back(vector);
    }

    file.close();
    return vectors;
}

const std::string AoCBase::GetFileName(const int Step) const
{
    std::string folder = IsTest() ? "test" : "live";
    std::string filename = ".\\" + folder + "\\" + std::to_string(GetYear()) + "_" + std::to_string(GetDay()) + "_" + std::to_string(Step) + ".txt";
    return filename;
}


long AoCBase::GetMinimum(const vector<long>& List)
{
    auto minVector = std::min_element(List.begin(), List.end());

    if(minVector != List.end())
        return *minVector;
    else
        return 0;//@TODO: give out error instead
}