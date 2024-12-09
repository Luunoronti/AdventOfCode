#include "pch.h"
#include "AoC_2024_32.h"

// this is a test day, not for actual problem solving
void AoC_2024_32::OnPaint(HDC hdc)
{
    FillRect(hdc, &rect, (HBRUSH)(COLOR_WINDOWTEXT + 1));
}

const int64_t AoC_2024_32::Step1()
{
    //DWORD startTime = ::GetTickCount64();
    //DWORD endTime = startTime + duration;

    //while(GetTickCount() < endTime)
    //{
    //    ProcessWindowMessages();

    //    rect.left += speed; 
    //    rect.right += speed; 
    //    if (rect.right > 800 || rect.left < 0) 
    //    { 
    //        speed = -speed; 
    //    }

    //    RequestWindowRedraw();
    //    Sleep(interval);
    //}

    WaitForWindowClose();

    return 0;
}

const int64_t AoC_2024_32::Step2()
{
    WaitForWindowClose();

    return 0;
}