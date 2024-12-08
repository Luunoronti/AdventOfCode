#include "AoCBase.h"

vector<AoCBaseExecutionResult> AoCBase::ResultReports;

#define PRINT_RESULT_STATUS \
        cout << DIM << "| " << RESET; \
        if(isValid) cout << GREEN << PAD_LEFT(22, ' ') << "OK" << RESET; \
        else if(isWrong){ wrongStr = "Wrong (Exp: " + std::to_string(expected) + ")"; cout << RED << PAD_LEFT(22, ' ') << wrongStr << RESET; } \
        else cout << YELLOW + BLINK << PAD_LEFT(22, ' ') << "Not verified" << RESET; \
        cout << DIM << " | "  << RESET;


int Max(int i1, int i2)
{
    return i1 > i2 ? i1 : i2;
}

string GetResultStatusNoColors(int64_t expected, int64_t actual)
{
    if(expected == 0)
        return "Not verified";

    if(expected == actual)
        return "OK";

    /*if(expected > actual)
        return "Too small, expected " + to_string(expected) + "(delta:" + to_string((expected - actual)) + ")";

    return "Too big, expected " + to_string(expected) + "(delta:" + to_string((actual - expected)) + ")";*/
    return "Wrong";
}
string GetValueColor(const AoCBaseExecutionResultEntry& Entry)
{
    if(Entry.IsNotKnown())
        return YELLOW + BLINK;

    if(Entry.IsValid())
        return GREEN;

    return RED;
}

void printRightPaddedString(const std::string& s, size_t len, string colorCode, bool noDimming = false)
{
    size_t padding = len > s.length() ? len - s.length() : 0;
    std::cout << (noDimming ? "" : DIM) << "|" << RESET << colorCode << s << RESET << std::setw(padding + 1) << std::setfill(' ') << "" << "";
}
void printLeftPaddedString(const std::string& s, size_t len, string colorCode, bool noDimming = false)
{
    size_t padding = len > s.length() ? len - s.length() : 0;
    std::cout << (noDimming ? "" : DIM) << "|" << RESET << colorCode << std::setw(padding + s.length()) << std::setfill(' ') << s << RESET << "";
}
void printCenterPaddedString(const std::string& s, size_t len, string colorCode, bool noDimming = false)
{
    size_t totalPadding = len > s.length() ? len - s.length() : 0;
    size_t leftPadding = totalPadding / 2;
    size_t rightPadding = totalPadding - leftPadding;
    std::cout << (noDimming ? "" : DIM) << "|" << RESET << colorCode << std::setw(leftPadding + s.length()) << std::setfill(' ') << s << RESET << std::setw(rightPadding) << std::setfill(' ') << "" << "";
}

std::string toStringWithPrecision(double value, int precision)
{
    std::stringstream stream;
    stream.imbue(std::locale::classic()); // Use the classic locale to avoid thousands separators
    stream << std::fixed << std::setprecision(precision) << value;
    return stream.str();
}

#define TIME_PRECISION 3

void AoCBase::PrintExecutionReport()
{
    TODO("Add a list of wrong answers for each part, so that when such answer is detected, the notification will be printed.");

#pragma region Macros
#define EST_WIDTH(SP) \
requiredWidth[index] = (int)std::strlen("Value");\
for(const AoCBaseExecutionResult& result : ResultReports) { requiredWidth[index] = Max(requiredWidth[index], (int)std::to_string(SP.Result).size()); }\
requiredWidth[index] += 2;\
index++;\
requiredWidth[index] = (int)std::strlen("Time (ms)");\
for(const AoCBaseExecutionResult& result : ResultReports) { requiredWidth[index] = Max(requiredWidth[index], (int)(toStringWithPrecision(SP.Time, TIME_PRECISION).size())); }\
requiredWidth[index] += 2;\
index++;

#define PRINT_STEP(SP, i1, i2) \
printLeftPaddedString(to_string(SP.Result), requiredWidth[i1], GetValueColor(SP)); \
printLeftPaddedString(toStringWithPrecision(SP.Time, TIME_PRECISION), requiredWidth[i2], (SP.Time > 50) ? (SP.Time > 500 ? RED+DIM : YELLOW+DIM) : "");

#define PRINT_HORIZONTAL_DIVIDER std::cout << DIM << "+" << PAD_LEFT(totalWidth, '-') << "" << "+" << RESET << endl
#define PRINT_NOTE(s, c) { printRightPaddedString(" " + s, totalWidth - 1, c); std::cout << DIM << "|" << RESET << endl; }

#define PRINT_SP_UNDEFINED_NOTE(sp) if(sp.IsNotKnown()) { PRINT_NOTE(sp.Name + " is not verified.", YELLOW); }

#define PRINT_SP_ERROR_IF_PRESENT(sp)  \
if(sp.IsError()) { if(sp.Result > sp.ExpectedResult)\
PRINT_NOTE(sp.Name + " result is too big, expected " + to_string(sp.ExpectedResult) + " (delta: " + to_string(sp.Delta()) + ")", RED) else \
PRINT_NOTE(sp.Name + " result is too small, expected " + to_string(sp.ExpectedResult) + " (delta: " + to_string(sp.Delta()) + ")", RED) }

#define PRINT_KNOWN_ERROR_VALUES(sp) \
if(std::find(sp.KnownErrorResults.begin(), sp.KnownErrorResults.end(), sp.Result) != sp.KnownErrorResults.end()) \
{PRINT_NOTE(sp.Name + " result " + to_string(sp.Result) + " is in the list of known bad results.", RED) } 

#define PRINT_ALL_STEP_NOTES(sp) \
PRINT_SP_UNDEFINED_NOTE(sp); \
PRINT_SP_ERROR_IF_PRESENT(sp); \
PRINT_KNOWN_ERROR_VALUES(sp); 


#pragma endregion

    int requiredWidth[9];
    {
        int index = 0;
        requiredWidth[index] = 9; // this is date, fixed
        index++;

        EST_WIDTH(result.Step1Test);
        EST_WIDTH(result.Step1Live);
        EST_WIDTH(result.Step2Test);
        EST_WIDTH(result.Step2Live);
    }

    // then, add some to make above columns

    // and then, above that, sum up more
    int totalWidth = 0;
    for(int i = 0; i < 9; ++i) { totalWidth += requiredWidth[i]; }
    totalWidth += 8;

#pragma region Print Header
    std::cout << DIM << "+" << PAD_LEFT(totalWidth, '=') << "" << "+" << RESET << endl;

    std::cout << DIM << "|" << PAD_LEFT(requiredWidth[0], ' ') << "";
    printCenterPaddedString("Step 1", requiredWidth[1] + requiredWidth[2] + requiredWidth[3] + requiredWidth[4] + 3, CYAN);
    printCenterPaddedString("Step 2", requiredWidth[5] + requiredWidth[6] + requiredWidth[7] + requiredWidth[8] + 3, BLUE);
    std::cout << DIM << "|" << RESET << endl;

    PRINT_HORIZONTAL_DIVIDER;

    std::cout << DIM << "|" << PAD_LEFT(requiredWidth[0], ' ') << "";
    printCenterPaddedString("Test", requiredWidth[1] + requiredWidth[2] + 1, MAGENTA);
    printCenterPaddedString("Live", requiredWidth[3] + requiredWidth[4] + 1, GREEN);
    printCenterPaddedString("Test", requiredWidth[5] + requiredWidth[6] + 1, MAGENTA);
    printCenterPaddedString("Live", requiredWidth[7] + requiredWidth[8] + 1, GREEN);
    std::cout << DIM << "|" << RESET << endl;

    PRINT_HORIZONTAL_DIVIDER;

    printCenterPaddedString("Day", requiredWidth[0], DIM);
    printCenterPaddedString("Value", requiredWidth[1], DIM, true);
    //printCenterPaddedString("Status", requiredWidth[2], DIM);
    printCenterPaddedString("Time (ms)", requiredWidth[2], DIM);

    printCenterPaddedString("Value", requiredWidth[3], DIM, true);
    //printCenterPaddedString("Status", requiredWidth[5], DIM);
    printCenterPaddedString("Time (ms)", requiredWidth[4], DIM);

    printCenterPaddedString("Value", requiredWidth[5], DIM, true);
    //printCenterPaddedString("Status", requiredWidth[8], DIM);
    printCenterPaddedString("Time (ms)", requiredWidth[6], DIM);

    printCenterPaddedString("Value", requiredWidth[7], DIM, true);
    //printCenterPaddedString("Status", requiredWidth[11], DIM);
    printCenterPaddedString("Time (ms)", requiredWidth[8], DIM);
    std::cout << DIM << "|" << RESET << endl;

    PRINT_HORIZONTAL_DIVIDER;
#pragma endregion


    for(int i = 0; i < ResultReports.size(); ++i)
    {
        const AoCBaseExecutionResult& result = ResultReports[i];
        if(i > 0)
        {
            // PRINT_HORIZONTAL_DIVIDER;
        }

        std::cout << DIM << "| " << RESET << result.Year << "/" << PAD_LEFT(2, '0') << (int)result.Day << " ";
        PRINT_STEP(result.Step1Test, 1, 2);
        PRINT_STEP(result.Step1Live, 3, 4);
        PRINT_STEP(result.Step2Test, 5, 6);
        PRINT_STEP(result.Step2Live, 7, 8);

        std::cout << DIM << "|" << RESET << endl;

        bool hasNotes =
            !result.Step1Test.IsValid()
            || !result.Step2Test.IsValid()
            || !result.Step1Live.IsValid()
            || !result.Step2Live.IsValid()
            ;

        if(hasNotes)
        {
            //PRINT_HORIZONTAL_DIVIDER;
        }

        PRINT_ALL_STEP_NOTES(result.Step1Test);
        PRINT_ALL_STEP_NOTES(result.Step1Live);
        PRINT_ALL_STEP_NOTES(result.Step2Test);
        PRINT_ALL_STEP_NOTES(result.Step2Live);

        if(hasNotes)
        {
            //PRINT_HORIZONTAL_DIVIDER;
        }
    }
    std::cout << DIM << "+" << PAD_LEFT(totalWidth, '=') << "" << "+" << RESET << endl;
    std::cout << endl << "Legend:" << endl;
    std::cout << "Value colors: " << GREEN << "OK" << RESET << ", " << RED << "Wrong" << RESET << ", " << YELLOW << BLINK << "Not yet verified (not known)" << RESET << endl;
    std::cout << "Time colors: " << RESET << "Under 50ms" << RESET << ", " << RED << DIM  << "Above 500ms" << RESET << ", " << YELLOW+DIM << "Above 50 but under 500ms" << RESET << endl;
    std::cout << "Time is in milliseconds, 1.000 means 1 millisecond while 0.745 means 745 microseconds." << endl;
}




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
void AoCBase::OnInitTestingTests() {}
void AoCBase::OnCloseTestingTests() {}
void AoCBase::OnInitLiveTests() {}
void AoCBase::OnCloseLiveTests() {}


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

std::vector<int64_t> ParseStringToVector_Helper(const std::string& input)
{
    std::vector<int64_t> result;
    std::stringstream ss(input);
    std::string token;

    while(std::getline(ss, token, ','))
    {
        token.erase(0, token.find_first_not_of(' '));
        result.push_back(std::stoll(token));
    }
    return result;
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

    int64_t knownError;

    // comments
    std::string line;
    std::getline(file, line);
    std::getline(file, line);
    std::getline(file, line);
    std::getline(file, line);

    {
        std::getline(file, line);
        std::getline(file, line);
        ExpectedTestResultForStep1 = std::stoll(line);

        std::getline(file, line);
        Step1_Test_KnownErrors = ParseStringToVector_Helper(line);
    }
    {
        std::getline(file, line);
        std::getline(file, line);
        std::getline(file, line);
        ExpectedLiveResultForStep1 = std::stoll(line);
        std::getline(file, line);
        Step1_Live_KnownErrors = ParseStringToVector_Helper(line);
    }
    {
        std::getline(file, line);
        std::getline(file, line);
        std::getline(file, line);
        ExpectedTestResultForStep2 = std::stoll(line);
        std::getline(file, line);
        Step2_Test_KnownErrors = ParseStringToVector_Helper(line);
    }
    {
        std::getline(file, line);
        std::getline(file, line);
        std::getline(file, line);
        ExpectedLiveResultForStep2 = std::stoll(line);
        std::getline(file, line);
        Step2_Live_KnownErrors = ParseStringToVector_Helper(line);
    }
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

