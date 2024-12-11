#include "pch.h"
#include "CustomFunctions.h"

#include <windows.h>
#include <wininet.h>
#include <iostream>
#include <fstream>

#pragma comment(lib, "wininet.lib")


void CreateFileIfDoesNotExist(const std::string& FileName, int day, int year, bool isLocal)
{
    struct stat buffer;
    if(stat(FileName.c_str(), &buffer) == 0) return;

    if(isLocal)
    {
        std::string command = "code \"" + FileName + "\"";
        int result = system(command.c_str());
        if(result == 0)
        {
            std::cout << "VS Code opened successfully on file " << FileName << std::endl;
            while(stat(FileName.c_str(), &buffer) != 0)
            {
                ::Sleep(1);
            }
            return;
        }
        else std::cerr << "Failed to open VS Code. Attempting to revert to Notepad" << std::endl;

        command = "notepad \"" + FileName + "\"";
        result = system(command.c_str());
        if(result == 0)
        {
            std::cout << "Notepad opened successfully on file " << FileName << std::endl;
            return;
        }
        else std::cerr << "Failed to open Notepad." << std::endl;
    }
    else
    {
        std::string command = "aocnetagent download-input " + to_string(year) + " " + to_string(day) + " \"" + FileName + "\"";
        int result = system(command.c_str());
        if(result == 0)
        {
            std::cout << "aocnetagent opened successfully on file " << FileName << std::endl;
            while(stat(FileName.c_str(), &buffer) != 0)
            {
                ::Sleep(1);
            }
            return;
        }
        else std::cerr << "Failed to open aocnetagent." << std::endl;

    }
}

std::string toStringWithPrecision(double value, int precision)
{
    std::stringstream stream;
    stream.imbue(std::locale::classic()); // Use the classic locale to avoid thousands separators
    stream << std::fixed << std::setprecision(precision) << value;
    return stream.str();
}





