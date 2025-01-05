// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//
#pragma once

// C RunTime Header Files
#include <malloc.h>
#include <map>
#include <mutex>
#include <vector>
#include <fstream>

#define HLSLPP_FEATURE_TRANSFORM
#include <hlsl++.h>
// normally, we should not declare namespace use in pch
// but this library is used all over and we just want
// same experience as in real HLSL.
using namespace hlslpp;


#include <intrin.h>
#include <Tracy.hpp>
#include <aoc.h>



