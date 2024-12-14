#include "pch.h"
#include "AoC_2024_Includes.h"

int main()
{
    // we must set locale
    std::locale::global(std::locale("pl_PL.UTF-8"));
#include "AoC_2024_Run.h"
    AoCBase::PrintExecutionReport();
    return 0;
}




#include <windows.h>
#include <iostream>


void print(char c, int frame)
{
    // Convert the number to a 
    char numBuff[32];
    ZeroMemory(&numBuff[0], 32);
    sprintf_s(numBuff, 32, "%d", frame);
    

    auto att = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;
    auto attRnd = 1;// std::rand() % 3;
    if(attRnd == 0) att = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;
    else if(attRnd == 1) att = FOREGROUND_INTENSITY | FOREGROUND_GREEN | FOREGROUND_BLUE;
    else if(attRnd == 2) att = FOREGROUND_RED | FOREGROUND_BLUE;

    // Define the region to read/write
    SMALL_RECT region = { 0, 0, 32, 32 }; // Top-left corner to (4, 4)

    // Define the size of the buffer to read/write
    COORD bufferSize = { 32, 32 }; // 5x5 buffer
    COORD bufferCoord = { 0, 0 }; // Coordinate in the buffer

    // Allocate a buffer to hold the console data
    CHAR_INFO buffer[1024]; // 5x5 buffer

    // Read the console buffer
    ReadFromConsoleBuffer(buffer, bufferSize, bufferCoord, region);

    // Modify the buffer (for example, change all characters to 'X')
    for(int i = 0; i < 1024; ++i)
    {
        buffer[i].Char.AsciiChar = c;
        buffer[i].Attributes = att; // White text
    }

    for(int i = 0; i < 32; ++i)
    {
        if(numBuff[i] != 0)
        buffer[i].Char.AsciiChar = numBuff[i];
    }

    // Write the modified buffer back to the console
    WriteToConsoleBuffer(buffer, bufferSize, bufferCoord, region);
}

//int main() {
//
//    int index = 0;
//    char c = '0';
//    while(true)
//    {
//        print(c, index++);
//        c++;
//        if(c > 'Z')c = '0';
//    }
//    return 0;
//}
//

/*
int main() {
    // Define the table size
    const int width = 20;
    const int height = 10;
    //SetConsoleOutputCP(65001);
    SetConsoleCP(CP_UTF8);

    // Define the buffer size and region
    COORD bufferSize = { width, height };
    COORD bufferCoord = { 0, 0 };
    SMALL_RECT writeRegion = { 0, 0, width - 1, height - 1 };

    // Allocate a buffer to hold the CHAR_INFO data
    CHAR_INFO buffer[width * height];

    // Fill the buffer with blank spaces
    for(int i = 0; i < width * height; ++i)
    {
        buffer[i].Char.UnicodeChar = ' ';
        buffer[i].Attributes = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE; // White text
    }

    // Draw the table with the specified characters
    for(int x = 1; x < width - 1; ++x)
    {
        buffer[x].Char.UnicodeChar = '─'; // Top border
        buffer[x + width * (height - 1)].Char.UnicodeChar = '─'; // Bottom border
    }

    for(int y = 1; y < height - 1; ++y)
    {
        buffer[y * width].Char.UnicodeChar = '│'; // Left border
        buffer[(y + 1) * width - 1].Char.UnicodeChar = '│'; // Right border
    }

    // Draw the corners
    buffer[0].Char.UnicodeChar = '┌'; // Top-left corner
    buffer[width - 1].Char.UnicodeChar = '┐'; // Top-right corner
    buffer[width * (height - 1)].Char.UnicodeChar = '└'; // Bottom-left corner
    buffer[width * height - 1].Char.UnicodeChar = '┘'; // Bottom-right corner

    // Draw the intersections and additional characters
    for(int y = 2; y < height - 2; y += 2)
    {
        for(int x = 2; x < width - 2; x += 2)
        {
            buffer[x + y * width].Char.UnicodeChar = '╫'; // Intersection
            buffer[x + 1 + y * width].Char.UnicodeChar = '╟'; // Right of intersection
            buffer[x - 1 + y * width].Char.UnicodeChar = '╢'; // Left of intersection
            buffer[x + (y - 1) * width].Char.UnicodeChar = '╦'; // Above intersection
            buffer[x + (y + 1) * width].Char.UnicodeChar = '╙'; // Below intersection
        }
    }

    // Write the buffer to the console
    writeToConsoleBuffer(buffer, bufferSize, bufferCoord, writeRegion);

    return 0;
}
*/