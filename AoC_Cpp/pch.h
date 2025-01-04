// pch.h: This is a precompiled header file.
// Files listed below are compiled only once, improving build performance for future builds.
// This also affects IntelliSense performance, including code completion and many code browsing features.
// However, files listed here are ALL re-compiled if any one of them is updated between builds.
// Do not add files here that you will be updating frequently as this negates the performance advantage.

#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here

//>>>>>>>> HLSL++
#define HLSLPP_FEATURE_TRANSFORM
#include <hlsl++.h>

// normally, we should not declare namespace use in pch
// but this library is used all over and we just want
// same experience as in real HLSL.
using namespace hlslpp;
//<<<<<<<< HLSL++

//>>>>>>>> Methane
//#include <Methane/Kit.h>
//<<<<<<<< Methane

//>>>>>>>> Tracy
#include <Tracy/tracy/Tracy.hpp>
//<<<<<<<< Tracy


//>>>>>>>> Tigr
#include <Tigr/tigr.h>
#include <fonts.h>
//<<<<<<<< Tigr


//>>>>>>>> STD
#include <iostream>
#include <sstream>
//<<<<<<<< STD


//>>>>>>>> LOCAL
#include <PipeOperatorBase.h>
//<<<<<<<< LOCAL

#endif //PCH_H
