#include "stdafx.h"
#include "AoCConfiguration.h"


// Define to_json and from_json functions for MyStruct 
void to_json(nlohmann::json& j, const JsonColor& s)
{
    j = nlohmann::json{
        {"x", s.r},
        {"y", s.g},
        {"z", s.b} };
}

void to_json(nlohmann::json& j, const AoCBaseExecutionConfigurationKnownErrors& s)
{
    j = nlohmann::json{
        {"tooBig", s.TooBig},
        {"tooSmall", s.TooSmall},
        {"general", s.General}
    };
}

void to_json(nlohmann::json& j, const AoCBaseExecutionConfigurationEntry& s)
{
    j = nlohmann::json{
        {"expectedResult", s.ExpectedResult},
        {"ExpectedResultStr", s.ExpectedResultStr},
        {"enableDebugStream", s.EnableDebugOutput},
        {"knownErrors", s.KnownErrorResults} };
}
void to_json(nlohmann::json& j, const AoCBaseExecutionConfiguration& s)
{
    j = nlohmann::json{
        {"year", s.Year},
        {"day", s.Day},
        {"run", s.OkToRunInRelease},
        {"debugRun", s.OkToRunInDebug},
        {"enableDebugStream", s.EnableDebugOutput},
        {"name", s.Name},
        {"testStep1", s.Part1Test},
        {"liveStep1", s.Part1Live},
        {"testStep2", s.Part2Test},
        {"liveStep2", s.Part2Live} };
}
//void to_json(nlohmann::json& j, const AoCVisualizerConfig& s)
//{
//    j = nlohmann::json{
//        {"allowMouseCapture", s.AllowMouseCapture},
//        {"alternateBuffer", s.AlternateBuffer},
//        {"hideCursor", s.HideCursor},
//        {"clearMode", s.clearMode},
//        {"legendVisibility", s.legendVisibility},
//        {"cameraMoveMouseButton", s.cameraMoveMouseButton},
//        {"clearColor", s.clearColor},
//        {"infoColor", s.infoColor},
//        {"gradientStartColor", s.gradientStartColor},
//        {"gradientEndColor", s.gradientEndColor},
//        {"animatedGradientSpeed", s.animatedGradientSpeed},
//        {"clearScreenOnExit", s.ClearScreenOnExit},
//        {"force16colorsMode", s.Force16colorsMode}
//    };
//}
//void to_json(nlohmann::json& j, const AoCProgramConfiguration& s)
//{
//    j = nlohmann::json{
//        {"runAllInDebug", s.ForceAllRunsInDebug},
//        {"runAllInRelease", s.ForceAllRunsInRelease},
//        {"visualizer", s.VisualizerConfig}
//    };
//}


void from_json(const nlohmann::json& j, JsonColor& s)
{
    if(j.contains("r")) j.at("r").get_to(s.r);
    if(j.contains("g")) j.at("g").get_to(s.g);
    if(j.contains("b")) j.at("b").get_to(s.b);
}
void from_json(const nlohmann::json& j, AoCBaseExecutionConfigurationKnownErrors& s)
{
    if(j.contains("tooBig")) j.at("tooBig").get_to(s.TooBig);
    if(j.contains("tooSmall")) j.at("tooSmall").get_to(s.TooSmall);
    if(j.contains("general")) j.at("general").get_to(s.General);
}
void from_json(const nlohmann::json& j, AoCBaseExecutionConfigurationEntry& s)
{
    if(j.contains("expectedResultStr")) j.at("expectedResultStr").get_to(s.ExpectedResultStr);
    if(j.contains("expectedResult")) j.at("expectedResult").get_to(s.ExpectedResult);
    if(j.contains("knownErrors")) j.at("knownErrors").get_to(s.KnownErrorResults);
    if(j.contains("enableDebugStream")) j.at("enableDebugStream").get_to(s.EnableDebugOutput);
    if(j.contains("enableVisualization")) j.at("enableVisualization").get_to(s.EnableVisualization);
}
void from_json(const nlohmann::json& j, AoCBaseExecutionConfiguration& s)
{
    if(j.contains("day")) j.at("day").get_to(s.Day);
    if(j.contains("run")) j.at("run").get_to(s.OkToRunInRelease);
    if(j.contains("debugRun")) j.at("debugRun").get_to(s.OkToRunInDebug);
    if(j.contains("enableDebugStream")) j.at("enableDebugStream").get_to(s.EnableDebugOutput);
    if(j.contains("testStep1")) j.at("testStep1").get_to(s.Part1Test);
    if(j.contains("liveStep1")) j.at("liveStep1").get_to(s.Part1Live);
    if(j.contains("testStep2")) j.at("testStep2").get_to(s.Part2Test);
    if(j.contains("liveStep2")) j.at("liveStep2").get_to(s.Part2Live);
    if(j.contains("year")) j.at("year").get_to(s.Year);
    if(j.contains("name")) j.at("name").get_to(s.Name);
    if(j.contains("enableVisualization")) j.at("enableVisualization").get_to(s.EnableVisualization);
}
//void from_json(const nlohmann::json& j, AoCVisualizerConfig& s)
//{
//    if(j.contains("allowMouseCapture")) j.at("allowMouseCapture").get_to(s.AllowMouseCapture);
//    if(j.contains("alternateBuffer")) j.at("alternateBuffer").get_to(s.AlternateBuffer);
//    if(j.contains("hideCursor")) j.at("hideCursor").get_to(s.HideCursor);
//    if(j.contains("clearScreenOnExit")) j.at("clearScreenOnExit").get_to(s.ClearScreenOnExit);
//    if(j.contains("clearMode")) j.at("clearMode").get_to(s.clearMode);
//    if(j.contains("cameraMoveMouseButton")) j.at("cameraMoveMouseButton").get_to(s.cameraMoveMouseButton);
//    if(j.contains("legendVisibility")) j.at("legendVisibility").get_to(s.legendVisibility);
//    if(j.contains("animatedGradientSpeed")) j.at("animatedGradientSpeed").get_to(s.animatedGradientSpeed);
//    if(j.contains("clearColor")) j.at("clearColor").get_to(s.clearColor);
//    if(j.contains("infoColor")) j.at("infoColor").get_to(s.infoColor);
//    if(j.contains("gradientStartColor")) j.at("gradientStartColor").get_to(s.gradientStartColor);
//    if(j.contains("gradientEndColor")) j.at("gradientEndColor").get_to(s.gradientEndColor);
//    if(j.contains("force16colorsMode")) j.at("force16colorsMode").get_to(s.Force16colorsMode);
//}
//void from_json(const nlohmann::json& j, AoCProgramConfiguration& s)
//{
//    if(j.contains("runAllInDebug")) j.at("runAllInDebug").get_to(s.ForceAllRunsInDebug);
//    if(j.contains("runAllInRelease")) j.at("runAllInRelease").get_to(s.ForceAllRunsInRelease);
//    if(j.contains("visualizer")) j.at("visualizer").get_to(s.VisualizerConfig);
//}




void AoCConfiguration::ReadDaysDatabaseIfNotDoneAlready()
{
    if(DaysDatabase.size() > 0)
        return;

    std::string FileName = "..\\Database\\DaysData.json";
    std::ifstream file(FileName);
    if(!file.is_open())
    {
        std::ofstream outFile(FileName);
        if(outFile.is_open())
        {
            outFile << "Insert json here";
            outFile.close();
        }
        else
        {
            std::cerr << "Unable to open file for writing." << std::endl;
        }
        return;
    }


    nlohmann::json jsonData;
    file >> jsonData;
    DaysDatabase = jsonData.get<std::vector<AoCBaseExecutionConfiguration>>();
    file.close();
}

AoCBaseExecutionConfiguration AoCConfiguration::GetResultJsonEntry(int Year, int Day)
{
    ReadDaysDatabaseIfNotDoneAlready();
    for(const auto& day : DaysDatabase)
    {
        if(day.Year == Year && day.Day == Day)
            return day;
    }
    throw std::runtime_error("Unable to find database entry for year " + std::to_string(Year) + " day " + std::to_string(Day));
}

const void AoCBaseExecutionConfigurationKnownErrors::GetGeneral(std::vector<std::string>& general) const
{
    general.clear();
    for(const auto& g : General)
        general.push_back(g);
}
const void AoCBaseExecutionConfigurationKnownErrors::GetTooBig(std::vector<int64_t>& tooBig) const
{
    tooBig.clear();
    for(const auto& g : TooBig)
        tooBig.push_back(g);
}
const void AoCBaseExecutionConfigurationKnownErrors::GetTooSmall(std::vector<int64_t>& tooSmall) const
{
    tooSmall.clear();
    for(const auto& g : TooSmall)
        tooSmall.push_back(g);
}

const int64_t AoCBaseExecutionConfigurationEntry::GetExpectedResult() const
{
    return ExpectedResult;
}
const std::string AoCBaseExecutionConfigurationEntry::GetExpectedResultStr() const
{
    return ExpectedResultStr;
}

const AoCBaseExecutionConfigurationKnownErrors* AoCBaseExecutionConfigurationEntry::GetKnownErrorResults() const
{
    return &KnownErrorResults;
}


const int AoCBaseExecutionConfiguration::GetYear() const { return Year; };
const int AoCBaseExecutionConfiguration::GetDay() const { return Day; };
const bool AoCBaseExecutionConfiguration::GetOkToRunInRelease() const { return OkToRunInRelease; };
const bool AoCBaseExecutionConfiguration::GetOkToRunInDebug() const { return OkToRunInDebug; };
const bool AoCBaseExecutionConfiguration::GetEnableDebugOutput() const { return EnableDebugOutput; };
const bool AoCBaseExecutionConfiguration::GetEnableVisualization() const { return EnableVisualization; };
const std::string AoCBaseExecutionConfiguration::GetName() const { return Name; }