#pragma once

#define _STR(x) #x
#define STR(x) _STR(x)
#define TODO(x) __pragma(message(__FILE__ "(" STR(__LINE__) "): TODO: "_STR(x)))

#ifdef _WIN32
#ifdef AOCLIBRARY_EXPORTS
#define AOCLIBRARY_API __declspec(dllexport)
#else
#define AOCLIBRARY_API __declspec(dllimport)
#endif
#else
#define AOCLIBRARY_API
#endif