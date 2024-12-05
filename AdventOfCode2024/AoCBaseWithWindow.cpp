#include "AoCBaseWithWindow.h"

LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    return ::DefWindowProc(hwnd, msg, wParam, lParam);
}

void AoCBaseWithWindow::RegisterWindowClass(HINSTANCE hInst, const wchar_t* windowClassName)
{
    // Register a window class for creating our render window with.
    WNDCLASSEXW windowClass = {};

    windowClass.cbSize = sizeof(WNDCLASSEX);
    windowClass.style = CS_HREDRAW | CS_VREDRAW;
    windowClass.lpfnWndProc = &WndProc;
    windowClass.cbClsExtra = 0;
    windowClass.cbWndExtra = 0;
    windowClass.hInstance = hInst;
    windowClass.hIcon = ::LoadIcon(hInst, nullptr);
    windowClass.hCursor = ::LoadCursor(NULL, IDC_ARROW);
    windowClass.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    windowClass.lpszMenuName = NULL;
    windowClass.lpszClassName = windowClassName;
    windowClass.hIconSm = ::LoadIcon(hInst, NULL);

    static ATOM atom = ::RegisterClassExW(&windowClass);
    assert(atom > 0);
}

void AoCBaseWithWindow::CreateWindow(const wchar_t* WindowClassName, HINSTANCE hInst, const wchar_t* WindowTitle, uint32_t Width, uint32_t Height)
{
    int screenWidth = ::GetSystemMetrics(SM_CXSCREEN);
    int screenHeight = ::GetSystemMetrics(SM_CYSCREEN);

    RECT windowRect = { 0, 0, static_cast<LONG>(Width), static_cast<LONG>(Height) };
    ::AdjustWindowRect(&windowRect, WS_OVERLAPPEDWINDOW, FALSE);

    int windowWidth = windowRect.right - windowRect.left;
    int windowHeight = windowRect.bottom - windowRect.top;

    // Center the window within the screen. Clamp to 0, 0 for the top-left corner.
    int windowX = std::max<int>(0, (screenWidth - windowWidth) / 2);
    int windowY = std::max<int>(0, (screenHeight - windowHeight) / 2);

    hWnd = ::CreateWindowExW(NULL, WindowClassName, WindowTitle, WS_OVERLAPPEDWINDOW, windowX, windowY, windowWidth, windowHeight, NULL, NULL, hInst, nullptr);
    assert(hWnd && "Failed to create window");
}


void AoCBaseWithWindow::OnInitTests()
{
    RegisterWindowClass(::GetModuleHandle(NULL), TEXT("AoCWindowClass"));
    CreateWindow(TEXT("AoCWindowClass"), GetModuleHandle(NULL), TEXT("AoC"), 1024, 768);
    ShowWindow(hWnd, SW_SHOW);
}

void AoCBaseWithWindow::OnCloseTests()
{
    ::CloseWindow(hWnd);
}

