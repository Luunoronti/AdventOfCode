#include "pch.h"
#include "CustomFunctions.h"


void CreateFileIfDoesNotExist(const std::string& FileName)
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

std::string toStringWithPrecision(double value, int precision)
{
    std::stringstream stream;
    stream.imbue(std::locale::classic()); // Use the classic locale to avoid thousands separators
    stream << std::fixed << std::setprecision(precision) << value;
    return stream.str();
}


