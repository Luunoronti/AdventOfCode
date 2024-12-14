#include "pch.h"
#include "AoCBase.h"

using namespace aoc;

vector<AoCBaseExecutionConfigurationAndResult> AoCBase::ResultReports;
AoCProgramConfiguration AoCBase::ProgramConfiguration;
bool AoCBase::ProgramConfigurationLoaded{ false };

#pragma region Execution
void AoCBase::ExecuteStep(AoCBase& instance)
{
    if(!ProgramConfigurationLoaded)
    {
        LoadProgramConfig();
    }
    if(instance.GetDay() == 0)
    {
        cout << RED << BLINK << "Error: Class instance has no day defined." << RESET << " Please override GetDay() and return proper day." << endl;
        return;
    }

    instance.CurrentDayConfiguration = GetResultJsonEntry(instance.GetYear(), instance.GetDay());

#if _DEBUG 
    if(!instance.CurrentDayConfiguration.OkToRunInDebug) if(!ProgramConfiguration.ForceAllRunsInDebug) return;
#else
    if(!instance.CurrentDayConfiguration.OkToRunInRelease) if(!ProgramConfiguration.ForceAllRunsInRelease) return;
#endif

    auto& result = instance.CurrentDayConfiguration;

    result.Step1Test.Name = "Part 1 (TEST)";
    result.Step2Test.Name = "Part 2 (TEST)";
    result.Step1Live.Name = "Part 1 (LIVE)";
    result.Step2Live.Name = "Part 2 (LIVE)";

    if(result.EnableVisualization) 
    {
        result.Step1Live.EnableVisualization
            = result.Step2Test.EnableVisualization
            = result.Step2Live.EnableVisualization
            = result.Step1Test.EnableVisualization
            = true;

    }

    if(result.EnableDebugOutput)
    {
        dout.setEnabled(true);
    }

    AoCStream::SetFileData(instance.GetFileName(), result.Year, result.Day, instance.IsTest());
    instance.OnInitTests();
    instance.SetTest(true);
    instance.OnInitTestingTests();
    dout.setEnabled(false);

    if(result.EnableDebugOutput || result.Step1Test.EnableDebugOutput)
    {
        dout.setEnabled(true);
        dout << RESET << endl << "=> Running " << result.GetNameWithDate() << " " << result.Step1Test.Name << endl;
    }
    instance.Step = 1;
    instance.CurrentStepConfiguration = result.Step1Test;
    AoCStream::SetFileData(instance.GetFileName(), result.Year, result.Day, instance.IsTest());
    instance.OnInitStep(1);
    instance.LastGlobalTime = 0;
    result.Step1Test.Result = instance.Step1();
    result.Step1Test.Time = instance.LastGlobalTime;
    instance.OnCloseStep(1);
    dout.setEnabled(false);

    if(result.EnableDebugOutput || result.Step2Test.EnableDebugOutput)
    {
        dout.setEnabled(true);
        dout << endl << "=> Running " << result.GetNameWithDate() << " " << result.Step2Test.Name << endl;
    }
    instance.Step = 2;
    instance.CurrentStepConfiguration = result.Step2Test;
    AoCStream::SetFileData(instance.GetFileName(), result.Year, result.Day, instance.IsTest());
    instance.OnInitStep(2);
    instance.LastGlobalTime = 0;
    result.Step2Test.Result = instance.Step2();
    result.Step2Test.Time = instance.LastGlobalTime;
    instance.OnCloseStep(2);
    dout.setEnabled(false);

    instance.OnCloseTestingTests();
    instance.SetTest(false);

    if(result.EnableDebugOutput) dout.setEnabled(true);
    instance.OnInitLiveTests();
    dout.setEnabled(false);

    if(result.EnableDebugOutput || result.Step1Live.EnableDebugOutput)
    {
        dout.setEnabled(true);
        dout << endl << "=> Running " << result.GetNameWithDate() << " " << result.Step1Live.Name << endl;
    }
    instance.Step = 1;
    instance.CurrentStepConfiguration = result.Step1Live;
    AoCStream::SetFileData(instance.GetFileName(), result.Year, result.Day, instance.IsTest());
    instance.OnInitStep(1);
    instance.LastGlobalTime = 0;
    result.Step1Live.Result = instance.Step1();
    result.Step1Live.Time = instance.LastGlobalTime;
    instance.OnCloseStep(1);
    dout.setEnabled(false);

    if(result.EnableDebugOutput || result.Step2Live.EnableDebugOutput)
    {
        dout.setEnabled(true);
        dout << endl << "=> Running " << result.GetNameWithDate() << " " << result.Step2Live.Name << endl;
    }

    instance.Step = 2;
    instance.CurrentStepConfiguration = result.Step2Live;
    AoCStream::SetFileData(instance.GetFileName(), result.Year, result.Day, instance.IsTest());
    instance.OnInitStep(2);
    instance.LastGlobalTime = 0;
    result.Step2Live.Result = instance.Step2();
    result.Step2Live.Time = instance.LastGlobalTime;
    instance.OnCloseStep(2);
    dout.setEnabled(false);

    if(result.EnableDebugOutput) dout.setEnabled(true);
    instance.OnCloseLiveTests();
    instance.OnCloseTests();
    dout.setEnabled(false);

    ResultReports.push_back(result);
}
#pragma endregion

#pragma region Report printing
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
string GetValueColor(const AoCBaseExecutionConfigurationResultEntry& Entry)
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
void printLeftPaddedString(const std::string& s, size_t len, string colorCode, bool noDimming = false, bool noBar = false)
{
    size_t padding = len > s.length() ? len - s.length() : 0;
    std::cout << (noDimming ? "" : DIM) << (noBar ? "" : "|") << RESET << colorCode << std::setw(padding + s.length()) << std::setfill(' ') << s << RESET << "";
}
void printCenterPaddedString(const std::string& s, size_t len, string colorCode, bool noDimming = false)
{
    size_t totalPadding = len > s.length() ? len - s.length() : 0;
    size_t leftPadding = totalPadding / 2;
    size_t rightPadding = totalPadding - leftPadding;
    std::cout << (noDimming ? "" : DIM) << "|" << RESET << colorCode << std::setw(leftPadding + s.length()) << std::setfill(' ') << s << RESET << std::setw(rightPadding) << std::setfill(' ') << "" << "";
}



#define TIME_PRECISION 3

void AoCBase::PrintExecutionReport()
{
    // Get the current system time 
    SYSTEMTIME st;
    GetLocalTime(&st);
    // Get the day of the month 
    int dayOfMonth = st.wDay;


#pragma region Macros
#define EST_WIDTH(SP) \
requiredWidth[index] = (int)std::strlen("Value");\
for(const AoCBaseExecutionConfigurationAndResult& result : ResultReports) { requiredWidth[index] = Max(requiredWidth[index], (int)std::to_string(SP.Result).size()); }\
requiredWidth[index] += 2;\
index++;\
requiredWidth[index] = (int)std::strlen("Time (ms)");\
for(const AoCBaseExecutionConfigurationAndResult& result : ResultReports) { requiredWidth[index] = Max(requiredWidth[index], (int)(toStringWithPrecision(SP.Time, TIME_PRECISION).size())); }\
requiredWidth[index] += 2;\
index++;

#define PRINT_STEP(SP, i1, i2) \
printLeftPaddedString(to_string(SP.Result), requiredWidth[i1], GetValueColor(SP)); \
printLeftPaddedString(toStringWithPrecision(SP.Time, TIME_PRECISION), requiredWidth[i2], (SP.Time > 16.6) ? (SP.Time > 500 ? RED+DIM : YELLOW+DIM) : "");

#define PRINT_HORIZONTAL_DIVIDER std::cout << DIM << "+" << PAD_LEFT(totalWidth, '-') << "" << "+" << RESET << endl
#define PRINT_NOTE(s, c) { printRightPaddedString("             " + s, totalWidth - 1, c); std::cout << DIM << "|" << RESET << endl; }

#define PRINT_SP_UNDEFINED_NOTE(sp, s) if(sp.IsNotKnown()) if(s.Day <= dayOfMonth) { PRINT_NOTE(sp.Name + " is not verified.", YELLOW); }

#define PRINT_SP_ERROR_IF_PRESENT(sp)  \
if(sp.IsError()) { if(sp.Result > sp.ExpectedResult)\
PRINT_NOTE(sp.Name + " result is too big, expected " + to_string(sp.ExpectedResult) + " (delta: " + to_string(sp.Delta()) + ")", RED) else \
PRINT_NOTE(sp.Name + " result is too small, expected " + to_string(sp.ExpectedResult) + " (delta: " + to_string(sp.Delta()) + ")", RED) }

#define PRINT_KNOWN_ERROR_VALUES(sp) \
if(std::find(sp.KnownErrorResults.begin(), sp.KnownErrorResults.end(), sp.Result) != sp.KnownErrorResults.end()) \
{PRINT_NOTE(sp.Name + " result " + to_string(sp.Result) + " is in the list of known bad results.", RED) } 

#define PRINT_ALL_STEP_NOTES(sp, s) \
{ PRINT_SP_UNDEFINED_NOTE(sp, s); } \
{ PRINT_KNOWN_ERROR_VALUES(sp); } \
{ PRINT_SP_ERROR_IF_PRESENT(sp); } 


#pragma endregion

    int requiredWidth[9];
    {
        requiredWidth[0] = (int)std::strlen("Name");
        for(const AoCBaseExecutionConfigurationAndResult& result : ResultReports) { requiredWidth[0] = Max(requiredWidth[0], 7 + (int)result.Name.size()); }
        requiredWidth[0] += 2;

        int index = 1;
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
    printCenterPaddedString("Part 1", requiredWidth[1] + requiredWidth[2] + requiredWidth[3] + requiredWidth[4] + 3, CYAN);
    printCenterPaddedString("Part 2", requiredWidth[5] + requiredWidth[6] + requiredWidth[7] + requiredWidth[8] + 3, BLUE);
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

    double TotalTimings[7]; // 0: total, 1: total test, 2: total live
    for(int i = 0; i < 7; ++i)
    {
        TotalTimings[i] = 0;
    }
    for(int i = 0; i < ResultReports.size(); ++i)
    {
        const AoCBaseExecutionConfigurationAndResult& result = ResultReports[i];
        if(i > 0)
        {
            // PRINT_HORIZONTAL_DIVIDER;
        }
        std::stringstream ss;
        ss << std::setfill('0') << std::setw(2) << (int)result.Day;

        printRightPaddedString(to_string(result.Year) + "/" + ss.str() + " " + result.Name, requiredWidth[0] - 1, "");

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

        PRINT_ALL_STEP_NOTES(result.Step1Test, result);
        PRINT_ALL_STEP_NOTES(result.Step1Live, result);
        PRINT_ALL_STEP_NOTES(result.Step2Test, result);
        PRINT_ALL_STEP_NOTES(result.Step2Live, result);


        TotalTimings[0] += result.Step1Test.Time;
        TotalTimings[0] += result.Step1Live.Time;
        TotalTimings[0] += result.Step2Test.Time;
        TotalTimings[0] += result.Step2Live.Time;

        TotalTimings[1] += result.Step1Test.Time;
        TotalTimings[1] += result.Step2Test.Time;

        TotalTimings[2] += result.Step1Live.Time;
        TotalTimings[2] += result.Step2Live.Time;

        TotalTimings[3] += result.Step1Test.Time;
        TotalTimings[4] += result.Step2Test.Time;

        TotalTimings[5] += result.Step1Live.Time;
        TotalTimings[6] += result.Step2Live.Time;

        if(hasNotes)
        {
            //PRINT_HORIZONTAL_DIVIDER;
        }
    }
#define GET_TIME_FLAGS(t) ((TotalTimings[3] > 500.0) ? (TotalTimings[3] > 2000 ? RED + DIM : YELLOW + DIM) : "")

    PRINT_HORIZONTAL_DIVIDER;
    printLeftPaddedString("Time summary: ", requiredWidth[0], "");

    printLeftPaddedString(toStringWithPrecision(TotalTimings[3], TIME_PRECISION), requiredWidth[1] + requiredWidth[2] + 1, GET_TIME_FLAGS(TotalTimings[3]));
    printLeftPaddedString(toStringWithPrecision(TotalTimings[5], TIME_PRECISION), requiredWidth[3] + requiredWidth[4] + 1, GET_TIME_FLAGS(TotalTimings[5]));
    printLeftPaddedString(toStringWithPrecision(TotalTimings[4], TIME_PRECISION), requiredWidth[5] + requiredWidth[6] + 1, GET_TIME_FLAGS(TotalTimings[4]));
    printLeftPaddedString(toStringWithPrecision(TotalTimings[6], TIME_PRECISION), requiredWidth[7] + requiredWidth[8] + 1, GET_TIME_FLAGS(TotalTimings[6]));

    std::cout << RESET << DIM << "|" << RESET << endl;
    PRINT_HORIZONTAL_DIVIDER;

#if _DEBUG
    printCenterPaddedString("DEBUG MODE", requiredWidth[0], RED + BLINK);
#else
    printCenterPaddedString("", requiredWidth[0], RED + BLINK);
#endif

    int div4 = (totalWidth - requiredWidth[0] - 1) / 4;
    printLeftPaddedString("Total times:", div4, "");

    {
        std::stringstream ss;
        ss << "Test: " << GET_TIME_FLAGS(TotalTimings[1]) << toStringWithPrecision(TotalTimings[1], TIME_PRECISION) << "ms";
        printLeftPaddedString(ss.str(), div4, "", false, true);
    }

    {
        std::stringstream ss;
        ss << "Live: " << GET_TIME_FLAGS(TotalTimings[2]) << toStringWithPrecision(TotalTimings[2], TIME_PRECISION) << "ms";
        printLeftPaddedString(ss.str(), div4, "", false, true);
    }

    {
        std::stringstream ss;
        ss << "Sum: " << GET_TIME_FLAGS(TotalTimings[0]) << toStringWithPrecision(TotalTimings[0], TIME_PRECISION) << "ms";
        printLeftPaddedString(ss.str(), div4, "", false, true);
    }

    std::cout << RESET << DIM << " |" << RESET << endl;

    std::cout << DIM << "+" << PAD_LEFT(totalWidth, '=') << "" << "+" << RESET << endl;
    std::cout << endl << DIM << "Legend:" << endl;
    //std::cout << "Value colors: " << GREEN << "OK" << RESET << ", " << RED << "Wrong" << RESET << ", " << YELLOW << BLINK << "Not yet verified (not known)" << RESET << endl;
    std::cout << DIM << "  Time colors: " << RESET << "Under 16.6ms" << RESET << DIM << ", " << RED << DIM << "Above 500ms" << RESET << DIM << ", " << YELLOW + DIM << "Above 16.6 but under 500ms" << RESET << endl;
    std::cout << DIM << "  Time is in milliseconds, 1.000 means 1 millisecond while 0.745 means 745 microseconds." << endl;
}

#pragma endregion


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


#pragma region File IO
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


string AoCBase::ReadStringFromFile(int Step) const
{
    int w, h;
    return ReadStringFromFile(Step, w, h);
}
string AoCBase::ReadStringFromFile(int Step, int& LinesCount, int& LastLineWidth) const
{
    const std::string FileName = GetFileName();
    CreateFileIfDoesNotExist(FileName, GetDay(), GetYear(), IsTest());
    LinesCount = 0;
    LastLineWidth = 0;

    std::ifstream file(FileName);
    std::string output;
    std::string line;
    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName() << RESET << std::endl;
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
    const std::string FileName = GetFileName();
    CreateFileIfDoesNotExist(FileName, GetDay(), GetYear(), IsTest());

    std::ifstream file(FileName);
    std::string line;
    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName() << RESET << std::endl;
        return -1;
    }
    if(!std::getline(file, line))
        return -1;
    file.close();
    return (int)line.size();
}
LongListList AoCBase::ReadVerticalVectorsFromFile(int Step) const
{
    const std::string FileName = GetFileName();
    CreateFileIfDoesNotExist(FileName, GetDay(), GetYear(), IsTest());

    std::ifstream file(FileName);
    LongListList vectors;

    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName() << RESET << std::endl;
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
    const std::string FileName = GetFileName();
    CreateFileIfDoesNotExist(FileName, GetDay(), GetYear(), IsTest());

    std::ifstream file(FileName);
    vector<string> vector;

    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName() << RESET << std::endl;
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
    const std::string FileName = GetFileName();
    CreateFileIfDoesNotExist(FileName, GetDay(), GetYear(), IsTest());

    std::ifstream file(FileName);
    LongListList vectors;

    if(!file.is_open())
    {
        std::cerr << RED << BLINK << "Error opening file " << GetFileName() << RESET << std::endl;
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

const std::string AoCBase::GetFileName() const
{
    std::string folder = IsTest() ? "test" : "live";
    std::string filename = ".\\" + folder + "\\" + std::to_string(GetYear()) + "_" + std::to_string(GetDay()) + "_" + std::to_string(Step) + ".txt";
    return filename;
}
#pragma endregion

long AoCBase::GetMinimum(const vector<long>& List)
{
    auto minVector = std::min_element(List.begin(), List.end());

    if(minVector != List.end())
        return *minVector;
    else
        throw std::exception();
}


#pragma region Json database
// Define to_json and from_json functions for MyStruct 
void to_json(nlohmann::json& j, const AoCBaseExecutionConfigurationResultEntry& s)
{
    j = nlohmann::json{
        {"expectedResult", s.ExpectedResult},
        {"enableDebugStream", s.EnableDebugOutput},
        {"knownErrors", s.KnownErrorResults} };
}
void to_json(nlohmann::json& j, const AoCBaseExecutionConfigurationAndResult& s)
{
    j = nlohmann::json{
        {"year", s.Year},
        {"day", s.Day},
        {"run", s.OkToRunInRelease},
        {"debugRun", s.OkToRunInDebug},
        {"enableDebugStream", s.EnableDebugOutput},
        {"name", s.Name},
        {"testStep1", s.Step1Test},
        {"liveStep1", s.Step1Live},
        {"testStep2", s.Step2Test},
        {"liveStep2", s.Step2Live} };
}
void to_json(nlohmann::json& j, const AoCProgramConfiguration& s)
{
    j = nlohmann::json{
        {"runAllInDebug", s.ForceAllRunsInDebug},
        {"runAllInRelease", s.ForceAllRunsInRelease} };
}

void from_json(const nlohmann::json& j, AoCBaseExecutionConfigurationResultEntry& s)
{
    if(j.contains("expectedResult")) j.at("expectedResult").get_to(s.ExpectedResult);
    if(j.contains("knownErrors")) j.at("knownErrors").get_to(s.KnownErrorResults);
    if(j.contains("enableDebugStream")) j.at("enableDebugStream").get_to(s.EnableDebugOutput);
    if(j.contains("enableVisualization")) j.at("enableVisualization").get_to(s.EnableVisualization);
}
void from_json(const nlohmann::json& j, AoCBaseExecutionConfigurationAndResult& s)
{
    if(j.contains("day")) j.at("day").get_to(s.Day);
    if(j.contains("run")) j.at("run").get_to(s.OkToRunInRelease);
    if(j.contains("debugRun")) j.at("debugRun").get_to(s.OkToRunInDebug);
    if(j.contains("enableDebugStream")) j.at("enableDebugStream").get_to(s.EnableDebugOutput);
    if(j.contains("testStep1")) j.at("testStep1").get_to(s.Step1Test);
    if(j.contains("liveStep1")) j.at("liveStep1").get_to(s.Step1Live);
    if(j.contains("testStep2")) j.at("testStep2").get_to(s.Step2Test);
    if(j.contains("liveStep2")) j.at("liveStep2").get_to(s.Step2Live);
    if(j.contains("year")) j.at("year").get_to(s.Year);
    if(j.contains("name")) j.at("name").get_to(s.Name);
    if(j.contains("enableVisualization")) j.at("enableVisualization").get_to(s.EnableVisualization);
}
void from_json(const nlohmann::json& j, AoCProgramConfiguration& s)
{
    if(j.contains("runAllInDebug")) j.at("runAllInDebug").get_to(s.ForceAllRunsInDebug);
    if(j.contains("runAllInRelease")) j.at("runAllInRelease").get_to(s.ForceAllRunsInRelease);
}

vector<AoCBaseExecutionConfigurationAndResult> AoCBase::DaysDatabase;
void AoCBase::ReadDaysDatabaseIfNotDoneAlready()
{
    if(DaysDatabase.size() > 0)
        return;

    // check if file exists
    std::string FileName = ".\\Database\\DaysData.json";
    std::ifstream file(FileName);
    if(!file.is_open())
    {
#pragma region Create new json file
        vector<AoCBaseExecutionConfigurationAndResult> entries;
        AoCBaseExecutionConfigurationAndResult e1;
        e1.Day = 0;
        e1.Year = 2024;
        e1.OkToRunInDebug = true;
        e1.OkToRunInRelease = true;

        e1.Step1Test.ExpectedResult = 2;
        e1.Step1Test.KnownErrorResults.push_back(-1);
        e1.Step1Test.KnownErrorResults.push_back(-2);

        e1.Step1Live.ExpectedResult = 3;
        e1.Step2Test.ExpectedResult = 4;
        e1.Step2Live.ExpectedResult = 5;

        entries.push_back(e1);

        nlohmann::json jsonData = entries;

        std::ofstream outFile(FileName);
        if(outFile.is_open())
        {
            outFile << jsonData.dump(4); // Indentation level of 4 for pretty printing 
            outFile.close();
        }
        else
        {
            std::cerr << "Unable to open file for writing." << std::endl;
        }
#pragma endregion

        file.open(FileName);
        if(!file.is_open())
        {
            std::cerr << RED << BLINK << "Error opening expected results file " << FileName << RESET << std::endl;
            return;
        }
    }


    nlohmann::json jsonData;
    file >> jsonData;

    DaysDatabase = jsonData.get<vector<AoCBaseExecutionConfigurationAndResult>>();

    file.close();
}
AoCBaseExecutionConfigurationAndResult AoCBase::GetResultJsonEntry(int Year, int Day)
{
    ReadDaysDatabaseIfNotDoneAlready();
    for(const auto& day : DaysDatabase)
    {
        if(day.Year == Year && day.Day == Day)
            return day;
    }
    throw std::runtime_error("Unable to find database entry for year " + to_string(Year) + " day " + to_string(Day));
}
void AoCBase::LoadProgramConfig()
{
    std::string FileName = ".\\Database\\ProgramConfig.json";
    std::ifstream file(FileName);
    if(!file.is_open())
    {
#pragma region Create new json file
        AoCProgramConfiguration cfg;
        cfg.ForceAllRunsInDebug = false;
        cfg.ForceAllRunsInRelease = false;

        nlohmann::json jsonData = cfg;
        std::ofstream outFile(FileName);
        if(outFile.is_open())
        {
            outFile << jsonData.dump(4); // Indentation level of 4 for pretty printing 
            outFile.close();
        }
        else
        {
            std::cerr << "Unable to open file for writing." << std::endl;
        }
#pragma endregion

        file.open(FileName);
        if(!file.is_open())
        {
            std::cerr << RED << BLINK << "Error opening expected results file " << FileName << RESET << std::endl;
            return;
        }
    }


    nlohmann::json jsonData;
    file >> jsonData;

    ProgramConfiguration = jsonData.get<AoCProgramConfiguration>();
    ProgramConfigurationLoaded = true;

    file.close();
}
#pragma endregion





