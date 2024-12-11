#pragma once

#include <string>

void CreateFileIfDoesNotExist(const std::string& FileName, int day, int year, bool isLocal = true);
std::string toStringWithPrecision(double value, int precision);
