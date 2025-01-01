#include "pch.h"
////#include "AoC_2024_32.h"
//
//
//// this is a test day, not for actual problem solving
////void AoC_2024_32::OnPaint(HDC hdc)
////{
////    FillRect(hdc, &rect, (HBRUSH)(COLOR_WINDOWTEXT + 1));
////}
//
////#include "immintrin.h"
////#ifdef _MSC_VER 
////#include <intrin.h> // For __cpuid 
////#else 
////#include <cpuid.h> // For __get_cpuid and __get_cpuid_max 
////#endif
////
////bool isAVX2Supported()
////{
////    std::array<int, 4> cpui;
////#ifdef _MSC_VER
////    __cpuid(cpui.data(), 0);
////    int nIds = cpui[0];
////    if(nIds >= 7)
////    {
////        __cpuidex(cpui.data(), 7, 0);
////        return (cpui[1] & (1 << 5)) != 0;
////    }
////#else
////    if(__get_cpuid_max(0, nullptr) >= 7)
////    {
////        unsigned int eax, ebx, ecx, edx;
////        __cpuid_count(7, 0, eax, ebx, ecx, edx);
////        return (ebx & (1 << 5)) != 0;
////    }
////#endif
////    return false;
////}
////
////
//
//
//const int64_t AoC_2024_32::Step1()
//{
//    //DWORD startTime = ::GetTickCount64();
//    //DWORD endTime = startTime + duration;
//
//    //while(GetTickCount() < endTime)
//    //{
//    //    ProcessWindowMessages();
//
//    //    rect.left += speed; 
//    //    rect.right += speed; 
//    //    if (rect.right > 800 || rect.left < 0) 
//    //    { 
//    //        speed = -speed; 
//    //    }
//
//    //    RequestWindowRedraw();
//    //    Sleep(interval);
//    //}
//
//   // WaitForWindowClose();
//
//    return 0;
//}
//
//const int64_t AoC_2024_32::Step2()
//{
//   // WaitForWindowClose();
//
//    return 0;
//}