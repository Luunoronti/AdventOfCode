#pragma once

#include <string>

void CreateFileIfDoesNotExist(const std::string& FileName, int day, int year, bool isLocal = true);
std::string toStringWithPrecision(double value, int precision);

void ReadFromConsoleBuffer(CHAR_INFO* buffer, COORD bufferSize, COORD bufferCoord, SMALL_RECT readRegion);
void WriteToConsoleBuffer(const CHAR_INFO* buffer, COORD bufferSize, COORD bufferCoord, SMALL_RECT writeRegion);
