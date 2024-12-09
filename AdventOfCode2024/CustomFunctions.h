#pragma once

#include <string>

void CreateFileIfDoesNotExist(const std::string& FileName);
std::string toStringWithPrecision(double value, int precision);