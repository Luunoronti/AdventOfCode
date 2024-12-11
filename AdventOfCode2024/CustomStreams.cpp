#include "pch.h"

namespace aoc
{
    std::string AoCStream::FileName;
    int AoCStream::Year{ 0 };
    int AoCStream::Day{ 0 };
    bool AoCStream::IsSourceLive{ false };

    aoc::AoCStream aocs;

    void AoCStream::ReadStringFromFileWithWH()
    {
        CreateFileIfDoesNotExist(FileName, Day, Year, IsSourceLive);
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