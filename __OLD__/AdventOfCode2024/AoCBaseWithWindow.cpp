#include "pch.h"
#include "AoCBaseWithWindow.h"
//
//LRESULT CALLBACK WndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam)
//{
//    AoCBaseWithWindow* aoc;
//    if(msg == WM_CREATE)
//    {
//        LPCREATESTRUCT pCreate = (LPCREATESTRUCT)lParam;
//        aoc = (AoCBaseWithWindow*)pCreate->lpCreateParams;
//        SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)aoc);
//    }
//    else
//    {
//        aoc = (AoCBaseWithWindow*)GetWindowLongPtr(hWnd, GWLP_USERDATA);
//    }
//
//    if(aoc)
//    {
//        switch(msg)
//        {
//        case WM_PAINT:
//        {
//            PAINTSTRUCT ps; HDC hdc = BeginPaint(hWnd, &ps);
//            aoc->OnPaint(hdc);
//            EndPaint(hWnd, &ps);
//            return 0;
//        }
//        case WM_DESTROY:
//            aoc->OnWindowDestroyed();
//            return 0;
//        }
//    }
//    return ::DefWindowProc(hWnd, msg, wParam, lParam);
//}
//void AoCBaseWithWindow::RequestWindowRedraw() const
//{
//    InvalidateRect(hWnd, nullptr, TRUE);
//}
//void AoCBaseWithWindow::WaitForWindowClose()
//{
//    MSG msg = {};
//    while(hWnd)
//    {
//        while(PeekMessage(&msg, hWnd, 0, 0, PM_REMOVE))
//        {
//            TranslateMessage(&msg);
//            DispatchMessage(&msg);
//        }
//        Sleep(1);
//    }
//}
//void AoCBaseWithWindow::RegisterWindowClass(HINSTANCE hInst, const wchar_t* windowClassName)
//{
//    // Register a window class for creating our render window with.
//    WNDCLASSEXW windowClass = {};
//
//    windowClass.cbSize = sizeof(WNDCLASSEX);
//    windowClass.style = CS_HREDRAW | CS_VREDRAW;
//    windowClass.lpfnWndProc = &WndProc;
//    windowClass.cbClsExtra = 0;
//    windowClass.cbWndExtra = 0;
//    windowClass.hInstance = hInst;
//    windowClass.hIcon = ::LoadIcon(hInst, nullptr);
//    windowClass.hCursor = ::LoadCursor(NULL, IDC_ARROW);
//    windowClass.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
//    windowClass.lpszMenuName = NULL;
//    windowClass.lpszClassName = windowClassName;
//    windowClass.hIconSm = ::LoadIcon(hInst, NULL);
//
//    static ATOM atom = ::RegisterClassExW(&windowClass);
//    assert(atom > 0);
//}
//
//void AoCBaseWithWindow::CreateWindow(const wchar_t* WindowClassName, HINSTANCE hInst, const wchar_t* WindowTitle, uint32_t Width, uint32_t Height)
//{
//    int screenWidth = ::GetSystemMetrics(SM_CXSCREEN);
//    int screenHeight = ::GetSystemMetrics(SM_CYSCREEN);
//
//    RECT windowRect = { 0, 0, static_cast<LONG>(Width), static_cast<LONG>(Height) };
//    ::AdjustWindowRect(&windowRect, WS_OVERLAPPEDWINDOW, FALSE);
//
//    int windowWidth = windowRect.right - windowRect.left;
//    int windowHeight = windowRect.bottom - windowRect.top;
//
//    // Center the window within the screen. Clamp to 0, 0 for the top-left corner.
//    int windowX = std::max<int>(0, (screenWidth - windowWidth) / 2);
//    int windowY = std::max<int>(0, (screenHeight - windowHeight) / 2);
//
//    hWnd = ::CreateWindowExW(NULL, WindowClassName, WindowTitle, WS_OVERLAPPEDWINDOW, windowX, windowY, windowWidth, windowHeight, NULL, NULL, hInst, this);
//    assert(hWnd && "Failed to create window");
//}
//
//
//void AoCBaseWithWindow::OnInitTests()
//{
//    RegisterWindowClass(::GetModuleHandle(NULL), TEXT("AoCWindowClass"));
//}
//
//void AoCBaseWithWindow::OnCloseTests()
//{
//}
//
//void AoCBaseWithWindow::OnInitStep(const int Step)
//{
//    CreateWindow(TEXT("AoCWindowClass"), GetModuleHandle(NULL), TEXT("AoC"), 1024, 768);
//    ShowWindow(hWnd, SW_SHOW);
//    UpdateWindow(hWnd);
//}
//
//void AoCBaseWithWindow::OnCloseStep(const int Step)
//{
//    ::DestroyWindow(hWnd);
//    while(hWnd)
//    {
//        Sleep(1);
//    }
//}
//
//void AoCBaseWithWindow::OnPaint(HDC hdc)
//{
//}
//
//void AoCBaseWithWindow::OnWindowDestroyed()
//{
//    hWnd = nullptr;
//}
//
//void AoCBaseWithWindow::ProcessWindowMessages()
//{
//    MSG msg = {};
//    while(PeekMessage(&msg, nullptr, 0, 0, PM_REMOVE))
//    {
//        TranslateMessage(&msg);
//        DispatchMessage(&msg);
//    }
//}
//
