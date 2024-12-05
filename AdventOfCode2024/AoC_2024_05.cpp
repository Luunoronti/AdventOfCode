#include "AoC_2024_05.h"

void AoC_2024_05::OnPaint(HDC hdc)
{
    FillRect(hdc, &rect, (HBRUSH)(COLOR_WINDOWTEXT + 1));
}

const long AoC_2024_05::Step1()
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

const long AoC_2024_05::Step2()
{
    WaitForWindowClose();

    return 0;
}

