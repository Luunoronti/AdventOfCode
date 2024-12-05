#include "AoCBase.h"

const bool AoCBase::IsTest() const
{
    return IsUnderTest;
}
void AoCBase::SetTest(const bool IsTest)
{
    IsUnderTest = IsTest;
}

void AoCBase::OnInitTests() {}
void AoCBase::OnInitStep(const int Step) {}
void AoCBase::OnCloseStep(const int Step) {}
void AoCBase::OnCloseTests() {}

void AoCBase::PrintStepResult(const int Step, bool IsTesting, const long& Result)
{
    bool isValid = false;
    bool isWrong = false;

    const long& ExpectedResult = Step == 1 ? (IsTesting ? ExpectedTestResultForStep1 : ExpectedLiveResultForStep1) : (IsTesting ? ExpectedTestResultForStep2 : ExpectedLiveResultForStep2);
    isValid = ExpectedResult && ExpectedResult == Result;
    isWrong = ExpectedResult && ExpectedResult != Result;

    cout << UNDERLINE << "Step " << Step << RESET << (IsTesting ? BOLD + MAGENTA + " (TEST)" + RESET : BOLD + GREEN + " (LIVE)" + RESET) << ": "
        << (isValid ? GREEN : "") << (isWrong ? RED : "") << Result << RESET
        << (isWrong ? RED + " !===! " + std::to_string(ExpectedResult) + BLINK + " -> WRONG. " + RESET : "")
        << (isValid ? GREEN + " -> GOOD. " + RESET : "")
        << ((!isValid && !isWrong) ? YELLOW + BLINK + " -> Not verified. " + RESET : "")
        << "  ";
}

void AoCBase::CreateEmptyExpectedStepResultsFile(const std::string& FileName)
{
    std::ofstream file(FileName);
    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error creating new expected results file " << FileName << RESET << std::endl;
        return;
    }
    file << "#This is an expected results file for AoC year " << GetYear() << " day " << GetDay() << endl;
    file << "#Do not alter the format of this file" << endl;
    file << "#There is no parsing and values are being taken directly from the positioning of lines within." << endl;
    file << endl;
    file << "Expected TEST value for step 1" << endl;
    file << "0" << endl;
    file << endl;
    file << "Expected LIVE value for step 1" << endl;
    file << "0" << endl;
    file << endl;
    file << "Expected TEST value for step 2" << endl;
    file << "0" << endl;
    file << endl;
    file << "Expected LIVE value for step 2" << endl;
    file << "0" << endl;

    file.close();
    std::cout << YELLOW << BLINK << "Created new expected results file " << FileName << ". Please fill it in. " << RESET << std::endl;
}
void AoCBase::ReadExpectedStepResults()
{
    // read step results from a file
    // this file is to be created if does not exist, and is to be filled with 0s
    // format of the file is very simple
    std::string FileName = ".\\ExpectedResults\\year_ " + std::to_string(GetYear()) + "_day_" + std::to_string(GetDay()) + ".txt";
    std::ifstream file(FileName);
    if(!file.is_open())
    {
        CreateEmptyExpectedStepResultsFile(FileName);

        file.open(FileName);
        if(!file.is_open())
        {
            std::cerr << RED << BLINK << "Error opening expected results file " << FileName << RESET << std::endl;
            return;
        }
    }

    std::string line;
    std::getline(file, line);
    std::getline(file, line);
    std::getline(file, line);
    std::getline(file, line);

    std::getline(file, line);
    std::getline(file, line);
    ExpectedTestResultForStep1 = std::stol(line);

    std::getline(file, line);
    std::getline(file, line);
    std::getline(file, line);
    ExpectedLiveResultForStep1 = std::stol(line);

    std::getline(file, line);
    std::getline(file, line);
    std::getline(file, line);
    ExpectedTestResultForStep2 = std::stol(line);

    std::getline(file, line);
    std::getline(file, line);
    std::getline(file, line);
    ExpectedLiveResultForStep2 = std::stol(line);
}

string AoCBase::ReadStringFromFile(int Step) const
{
    int w, h;
    return ReadStringFromFile(Step, w, h);
}
void AoCBase::CreateFileIfDoesNotExist(const std::string& FileName) const
{
    struct stat buffer;
    if(stat(FileName.c_str(), &buffer) == 0)
    {
        return;
    }

    std::string command = "notepad \"" + FileName + "\"";

    int result = system(command.c_str());
    if(result == 0)
    {
        std::cout << "Notepad closed successfully." << std::endl;
    }
    else
    {
        std::cerr << "Failed to open Notepad." << std::endl;
    }
}
string AoCBase::ReadStringFromFile(int Step, int& LinesCount, int& LastLineWidth) const
{
    const std::string FileName = GetFileName(Step);
    CreateFileIfDoesNotExist(FileName);

    std::ifstream file(FileName);
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
    LastLineWidth = (int)line.size();
    file.close();
    return output;
}

const int AoCBase::GetFileSingleLineWidth(int Step) const
{
    const std::string FileName = GetFileName(Step);
    CreateFileIfDoesNotExist(FileName);

    std::ifstream file(FileName);
    std::string line;
    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName(Step) << RESET << std::endl;
        return -1;
    }
    if(!std::getline(file, line))
        return -1;
    file.close();
    return (int)line.size();
}
LongListList AoCBase::ReadVerticalVectorsFromFile(int Step) const
{
    const std::string FileName = GetFileName(Step);
    CreateFileIfDoesNotExist(FileName);

    std::ifstream file(FileName);
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

stringstream AoCBase::ReadStringStreamFromFile(int Step) const
{
    return stringstream(ReadStringFromFile(Step));
}
vector<string> AoCBase::ReadStringLinesFromFile(int Step) const
{
    const std::string FileName = GetFileName(Step);
    CreateFileIfDoesNotExist(FileName);

    std::ifstream file(FileName);
    vector<string> vector;

    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName(Step) << RESET << std::endl;
        return vector;
    }

    std::string line;
    while(std::getline(file, line))
    {
        vector.push_back(line);
    }

    file.close();
    return vector;
}


LongListList AoCBase::ReadLongVectorsFromFile(int Step) const
{
    const std::string FileName = GetFileName(Step);
    CreateFileIfDoesNotExist(FileName);

    std::ifstream file(FileName);
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
        throw std::exception();
}

