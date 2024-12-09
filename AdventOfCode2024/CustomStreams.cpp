#include "pch.h"

namespace aoc
{
    void AoCStream::ReadStringFromFileWithWH()
    {
        CreateFileIfDoesNotExist(FileName);
        Height = 0;
        Width = 0;

        std::ifstream file(FileName);
        std::string content;
        std::string line;
        if(!file.is_open())
        {
            throw std::runtime_error("Error opening file " + FileName);
        }
        while(std::getline(file, line))
        {
            content += line;
            ++Height;
        }
        Width = (int)line.size();
        file.close();
        Input = content;
    }
}