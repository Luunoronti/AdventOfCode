#pragma once

#include "AoCBase.h"


#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

// The min/max macros conflict with like-named member functions.
// Only use std::min and std::max defined in <algorithm>.
#if defined(min)
#undef min
#endif

#if defined(max)
#undef max
#endif

// In order to define a function called CreateWindow, the Windows macro needs to
// be undefined.
#if defined(CreateWindow)
#undef CreateWindow
#endif


//
//
//class AoCBaseWithWindow : public AoCBase
//{
//
//    friend class AoCBase;
//
//protected:
//    virtual void ProcessWindowMessages();
//
//public:
//    virtual void OnPaint(HDC hdc);
//    virtual void OnWindowDestroyed();
//
//protected:
//    void RegisterWindowClass(HINSTANCE hInst, const wchar_t* windowClassName);
//    void CreateWindow(const wchar_t* WindowClassName, HINSTANCE hInst, const wchar_t* WindowTitle, uint32_t Width, uint32_t Height);
//    void RequestWindowRedraw() const;
//    void WaitForWindowClose();
//
//    // Window handle.
//    HWND hWnd;
//    // Window rectangle (used to toggle fullscreen state).
//    RECT WindowRect;
//
//
//};
//
