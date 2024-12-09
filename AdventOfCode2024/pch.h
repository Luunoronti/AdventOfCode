#pragma once

#define _STR(x) #x
#define STR(x) _STR(x)
#define TODO(x) __pragma(message(__FILE__"("STR(__LINE__)"): TODO: "_STR(x) ))

#define CompilerMessage(desc) __pragma(message(__FILE__ "(" STR(__LINE__) ") :" #desc))

#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

#include <iostream> 
#include <fstream> 
#include <vector> 
#include <unordered_map> 
#include <sstream>
#include <iomanip>
#include <algorithm>
#include <cstdlib>
#include <numeric>
#include <sys/stat.h>
#include <cassert>
#include <ppl.h>

#include <ppl.h>
#include <algorithm>
#include <array>
#include <set>
#include <deque>

#include "json.hpp"

#include "DebugBuffer.h"
#include "CustomTypes.h"
#include "CustomStreams.h"
#include "CustomFunctions.h"

using namespace std;
using namespace concurrency;

const string RESET = "\033[0m";
const string BOLD = "\033[1m";
const string CLEAR_SCREEN = "\033[2J\033[1;1H";
const string LINESTART = "\033[0G";

const string DIM = "\033[2m";
const string UNDERLINE = "\033[4m";
const string BLINK = "\033[5m";

const string BLACK = "\033[30m";
const string RED = "\033[31m";
const string GREEN = "\033[32m";
const string YELLOW = "\033[33m";
const string BLUE = "\033[34m";
const string MAGENTA = "\033[35m";
const string CYAN = "\033[36m";
const string WHITE = "\033[37m";

const string BKBLACK = "\033[40m";
const string BKRED = "\033[41m";
const string BKGREEN = "\033[42m";
const string BKYELLOW = "\033[43m";
const string BKBLUE = "\033[44m";
const string BKMAGENTA = "\033[45m";
const string BKCYAN = "\033[46m";
const string BKWHITE = "\033[47m";
