#include "pch.h"
#include "AoC_2024_Includes.h"

int _event_handler_main();

int main()
{
    // we must set locale
    std::locale::global(std::locale("pl_PL.UTF-8"));
#include "AoC_2024_Run.h"
    AoCBase::PrintExecutionReport();
    return 0;
}


void lockMouseToConsole()
{
    // Get the handle to the console window
    HWND hConsole = GetConsoleWindow();
    if(hConsole == NULL)
    {
        std::cerr << "Failed to get console window handle" << std::endl;
        return;
    }

    // Get the console window's client area
    RECT consoleRect;
    if(!GetClientRect(hConsole, &consoleRect))
    {
        std::cerr << "Failed to get console window client area" << std::endl;
        return;
    }

    // Convert the client area to screen coordinates
    POINT topLeft = { consoleRect.left, consoleRect.top };
    POINT bottomRight = { consoleRect.right, consoleRect.bottom };
    ClientToScreen(hConsole, &topLeft);
    ClientToScreen(hConsole, &bottomRight);

    // Set the screen coordinates to the RECT structure
    consoleRect.left = topLeft.x;
    consoleRect.top = topLeft.y;
    consoleRect.right = bottomRight.x;
    consoleRect.bottom = bottomRight.y;

    // Clip the cursor to the console window's client area
    ClipCursor(&consoleRect);
}

void unlockMouse() {
    // Release the mouse cursor lock
    ClipCursor(NULL);
}

void centerCursorInConsole(HWND hConsole)
{
    RECT consoleRect;
    GetClientRect(hConsole, &consoleRect);
    POINT center;
    center.x = (consoleRect.right - consoleRect.left) / 2;
    center.y = (consoleRect.bottom - consoleRect.top) / 2;
    ClientToScreen(hConsole, &center);
    SetCursorPos(center.x, center.y);
}

//int main() {
//    std::cout << "Press and hold the 'L' key to lock the mouse cursor to the console window." << std::endl;
//    std::cout << "Press and hold the 'U' key to unlock the mouse cursor." << std::endl;
//
//    while(true)
//    {
//        if(GetAsyncKeyState('L') & 0x8000)
//        { // Press 'L' to lock
//            lockMouseToConsole();
//            std::cout << "Mouse cursor locked to console window." << std::endl;
//        }
//        else if(GetAsyncKeyState('U') & 0x8000)
//        { // Press 'U' to unlock
//            unlockMouse();
//            std::cout << "Mouse cursor unlocked." << std::endl;
//        }
//
//        Sleep(100); // Small delay to prevent excessive CPU usage
//    }
//
//    return 0;
//}



#include <windows.h>
#include <iostream>

bool isMouseLocked = false;

VOID MouseEventProc(MOUSE_EVENT_RECORD mer, POINT& center)
{
#ifndef MOUSE_HWHEELED
#define MOUSE_HWHEELED 0x0008
#endif
    printf("Mouse event: ");

    switch(mer.dwEventFlags)
    {
    case 0:

        if(mer.dwButtonState == FROM_LEFT_1ST_BUTTON_PRESSED)
        {
            printf("left button press                                      \r");
        }
        else if(mer.dwButtonState == RIGHTMOST_BUTTON_PRESSED)
        {
            printf("right button press                                      \r");
            isMouseLocked = true;
        }
        else if(mer.dwButtonState == FROM_LEFT_2ND_BUTTON_PRESSED) // tis is mouse wheel button
        {
            printf("2nd button press                                      \r");
        }
        else
        {
            printf("button press or release                                     \r");
            isMouseLocked = false;
        }
        break;
    case DOUBLE_CLICK:
        printf("double click                                     \r");
        break;
    case MOUSE_HWHEELED:
        printf("horizontal mouse wheel                                     \r");
        break;
    case MOUSE_MOVED:
    {
        if(isMouseLocked)
        {
            POINT screenCursorPos;
            GetCursorPos(&screenCursorPos);
            int deltaX = screenCursorPos.x - center.x;
            int deltaY = screenCursorPos.y - center.y;

            //std::cout << "Mouse Delta - X: " << deltaX << ", Y: " << deltaY << "\r"; std::cout.flush();

            printf("mouse moved %d, %d                  \r", deltaX, deltaY);
            SetCursorPos(screenCursorPos.x - deltaX, screenCursorPos.y - deltaY);
        }
    }
    break;
    case MOUSE_WHEELED:
        printf("vertical mouse wheel                                     \r");
        break;
    default:
        printf("unknown                                     \r");
        break;
    }
}

void handleWindowSizeChange()
{
    INPUT_RECORD inputRecord;
    DWORD events;
    COORD newWindowSize;

    //HANDLE handleOut = GetStdHandle(STD_OUTPUT_HANDLE);
    HANDLE hConsoleInput = GetStdHandle(STD_INPUT_HANDLE);


    POINT center, cursorPos;
    RECT consoleRect;
    GetClientRect(GetConsoleWindow(), &consoleRect);
    center.x = (consoleRect.right - consoleRect.left) / 2;
    center.y = (consoleRect.bottom - consoleRect.top) / 2;
    ClientToScreen(GetConsoleWindow(), &center);

    while(true)
    {
        GetNumberOfConsoleInputEvents(hConsoleInput, &events);
        if(events != 0)
        {
            DWORD cNumRead, fdwMode, i;
            INPUT_RECORD irInBuf[128];
            int counter = 0;

            if(!ReadConsoleInput(
                hConsoleInput,      // input buffer handle
                irInBuf,     // buffer to read into
                128,         // size of read buffer
                &cNumRead)) // number of records read
                break;



            for(i = 0; i < cNumRead; i++)
            {

                switch(irInBuf[i].EventType)
                {
                case KEY_EVENT: // keyboard input
                    //  cout << "Key event" << endl;
                      //   KeyEventProc(irInBuf[i].Event.KeyEvent);
                    break;

                case MOUSE_EVENT: // mouse input
                    //   cout << "Key event" << endl;
                    MouseEventProc(irInBuf[i].Event.MouseEvent, center);
                    break;

                case WINDOW_BUFFER_SIZE_EVENT: // scrn buf. resizing
                    //    cout << "Key event" << endl;
                        //   ResizeEventProc(irInBuf[i].Event.WindowBufferSizeEvent);
                    break;

                case FOCUS_EVENT:  // disregard focus events
                    cout << "Focus event" << endl;
                    break;

                case MENU_EVENT:   // disregard menu events
                    cout << "Menu event" << endl;
                    break;

                default:
                    cout << "Unknown event" << endl;
                    //   ErrorExit("Unknown event type");
                    break;
                }


                //if(inputRecord.EventType == WINDOW_BUFFER_SIZE_EVENT)
                //{
                //    newWindowSize = inputRecord.Event.WindowBufferSizeEvent.dwSize;
                //    std::cout << "New Window Size - Width: " << newWindowSize.X << ", Height: " << newWindowSize.Y << "\r";
                //    std::cout.flush();
                //}
                //if(inputRecord.EventType == KEY_EVENT)
                //{
                //    //                newWindowSize = inputRecord.Event.WindowBufferSizeEvent.dwSize;
                //    //                std::cout << "New Window Size - Width: " << newWindowSize.X << ", Height: " << newWindowSize.Y << "\r";
                //}
                //if(inputRecord.EventType == MOUSE_EVENT)
                //{
                //    COORD mousePos;
                //    mousePos = inputRecord.Event.MouseEvent.dwMousePosition;
                //    std::cout << "Mouse Position - X: " << mousePos.X << ", Y: " << mousePos.Y << "\r";
                //    std::cout.flush();
                //}
                //if(inputRecord.EventType == MENU_EVENT)
                //{
                //    //newWindowSize = inputRecord.Event.WindowBufferSizeEvent.dwSize;
                //    //std::cout << "New Window Size - Width: " << newWindowSize.X << ", Height: " << newWindowSize.Y << "\r";
                //    //std::cout.flush();
                //}
                //if(inputRecord.EventType == FOCUS_EVENT)
                //{
                //    //newWindowSize = inputRecord.Event.WindowBufferSizeEvent.dwSize;
                //    //std::cout << "New Window Size - Width: " << newWindowSize.X << ", Height: " << newWindowSize.Y << "\r";
                //    //std::cout.flush();
                //}
            }
        }

        // Check for a specific key to break the loop 
        if(GetAsyncKeyState(VK_ESCAPE) & 0x8000)
        {
            // Press ESC to exit 
            break;
        }
        Sleep(50); // Small delay to prevent excessive CPU usage
    }

}

int _event_handler_main()
{
    DWORD fdwSaveOldMode;

    DWORD out_saved_mode;
    DWORD in_saved_mode;


    //HANDLE hConsoleInput = GetStdHandle(STD_INPUT_HANDLE);
    //if(hConsoleInput == INVALID_HANDLE_VALUE)
    //{
    //    std::cerr << "Failed to get console input handle" << std::endl;
    //    return 1;
    //}

    //if(!GetConsoleMode(hConsoleInput, &fdwSaveOldMode))
    //{
    //    std::cerr << "Failed to get console input handle" << std::endl;
    //    return 1;
    //}

    //// Enable mouse input 
    //DWORD consoleMode;
    //if(!GetConsoleMode(hConsoleInput, &consoleMode))
    //{
    //    std::cerr << "Failed to get console mode" << std::endl; return 1;
    //}
    //consoleMode = ENABLE_WINDOW_INPUT | ENABLE_MOUSE_INPUT;
    //if(!SetConsoleMode(hConsoleInput, consoleMode))
    //{
    //    std::cerr << "Failed to set console mode" << std::endl; return 1;
    //}



    HANDLE handleOut = GetStdHandle(STD_OUTPUT_HANDLE);
    HANDLE handleIn = GetStdHandle(STD_INPUT_HANDLE);
    GetConsoleMode(handleOut, &out_saved_mode);
    GetConsoleMode(handleIn, &in_saved_mode);

    DWORD out_consoleMode = out_saved_mode;
    out_consoleMode |= (ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN);
    SetConsoleMode(handleOut, out_consoleMode);
    SetConsoleOutputCP(65001);

    DWORD in_consoleMode = 0;
    in_consoleMode = ENABLE_WINDOW_INPUT | ENABLE_MOUSE_INPUT | ENABLE_INSERT_MODE | ENABLE_EXTENDED_FLAGS;
    in_consoleMode &= ~ENABLE_ECHO_INPUT;
    SetConsoleMode(handleIn, in_consoleMode);

    //? Disable stream sync
    cin.sync_with_stdio(false);
    cout.sync_with_stdio(false);

    //? Disable stream ties
    cin.tie(NULL);
    cout.tie(NULL);
    //refresh();

    std::cout << "Resize the console window to see the new size." << std::endl;
    std::cout << "Move mouse for mouse events." << std::endl;
    handleWindowSizeChange();

    SetConsoleMode(handleOut, out_saved_mode);
    SetConsoleMode(handleIn, in_saved_mode);

    return 0;
}



#include <windows.h>
#include <iostream>


void print(char c, int frame)
{
    // Convert the number to a 
    char numBuff[32];
    ZeroMemory(&numBuff[0], 32);
    sprintf_s(numBuff, 32, "%d", frame);


    auto att = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;
    auto attRnd = 1;// std::rand() % 3;
    if(attRnd == 0) att = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;
    else if(attRnd == 1) att = FOREGROUND_INTENSITY | FOREGROUND_GREEN | FOREGROUND_BLUE;
    else if(attRnd == 2) att = FOREGROUND_RED | FOREGROUND_BLUE;

    // Define the region to read/write
    SMALL_RECT region = { 0, 0, 32, 32 }; // Top-left corner to (4, 4)

    // Define the size of the buffer to read/write
    COORD bufferSize = { 32, 32 }; // 5x5 buffer
    COORD bufferCoord = { 0, 0 }; // Coordinate in the buffer

    // Allocate a buffer to hold the console data
    CHAR_INFO buffer[1024]; // 5x5 buffer

    // Read the console buffer
    ReadFromConsoleBuffer(buffer, bufferSize, bufferCoord, region);

    // Modify the buffer (for example, change all characters to 'X')
    for(int i = 0; i < 1024; ++i)
    {
        buffer[i].Char.AsciiChar = c;
        buffer[i].Attributes = att; // White text
    }

    for(int i = 0; i < 32; ++i)
    {
        if(numBuff[i] != 0)
            buffer[i].Char.AsciiChar = numBuff[i];
    }

    // Write the modified buffer back to the console
    WriteToConsoleBuffer(buffer, bufferSize, bufferCoord, region);
}

//int main() {
//
//    int index = 0;
//    char c = '0';
//    while(true)
//    {
//        print(c, index++);
//        c++;
//        if(c > 'Z')c = '0';
//    }
//    return 0;
//}
//

/*
int main() {
    // Define the table size
    const int width = 20;
    const int height = 10;
    //SetConsoleOutputCP(65001);
    SetConsoleCP(CP_UTF8);

    // Define the buffer size and region
    COORD bufferSize = { width, height };
    COORD bufferCoord = { 0, 0 };
    SMALL_RECT writeRegion = { 0, 0, width - 1, height - 1 };

    // Allocate a buffer to hold the CHAR_INFO data
    CHAR_INFO buffer[width * height];

    // Fill the buffer with blank spaces
    for(int i = 0; i < width * height; ++i)
    {
        buffer[i].Char.UnicodeChar = ' ';
        buffer[i].Attributes = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE; // White text
    }

    // Draw the table with the specified characters
    for(int x = 1; x < width - 1; ++x)
    {
        buffer[x].Char.UnicodeChar = '─'; // Top border
        buffer[x + width * (height - 1)].Char.UnicodeChar = '─'; // Bottom border
    }

    for(int y = 1; y < height - 1; ++y)
    {
        buffer[y * width].Char.UnicodeChar = '│'; // Left border
        buffer[(y + 1) * width - 1].Char.UnicodeChar = '│'; // Right border
    }

    // Draw the corners
    buffer[0].Char.UnicodeChar = '┌'; // Top-left corner
    buffer[width - 1].Char.UnicodeChar = '┐'; // Top-right corner
    buffer[width * (height - 1)].Char.UnicodeChar = '└'; // Bottom-left corner
    buffer[width * height - 1].Char.UnicodeChar = '┘'; // Bottom-right corner

    // Draw the intersections and additional characters
    for(int y = 2; y < height - 2; y += 2)
    {
        for(int x = 2; x < width - 2; x += 2)
        {
            buffer[x + y * width].Char.UnicodeChar = '╫'; // Intersection
            buffer[x + 1 + y * width].Char.UnicodeChar = '╟'; // Right of intersection
            buffer[x - 1 + y * width].Char.UnicodeChar = '╢'; // Left of intersection
            buffer[x + (y - 1) * width].Char.UnicodeChar = '╦'; // Above intersection
            buffer[x + (y + 1) * width].Char.UnicodeChar = '╙'; // Below intersection
        }
    }

    // Write the buffer to the console
    writeToConsoleBuffer(buffer, bufferSize, bufferCoord, writeRegion);

    return 0;
}
*/



// System headers
#include <windows.h>

// Standard library C-style
#include <wchar.h>
#include <stdlib.h>
#include <stdio.h>

#define ESC "\x1b"
#define CSI "\x1b["

bool EnableVTMode()
{
    // Set output mode to handle virtual terminal sequences
    HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
    if(hOut == INVALID_HANDLE_VALUE)
    {
        return false;
    }

    DWORD dwMode = 0;
    if(!GetConsoleMode(hOut, &dwMode))
    {
        return false;
    }

    dwMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
    if(!SetConsoleMode(hOut, dwMode))
    {
        return false;
    }
    return true;
}

void PrintVerticalBorder()
{
    printf(ESC "(0"); // Enter Line drawing mode
    printf(CSI "104;93m"); // bright yellow on bright blue
    printf("x"); // in line drawing mode, \x78 -> \u2502 "Vertical Bar"
    printf(CSI "0m"); // restore color
    printf(ESC "(B"); // exit line drawing mode
}

void PrintHorizontalBorder(COORD const Size, bool fIsTop)
{
    printf(ESC "(0"); // Enter Line drawing mode
    printf(CSI "104;93m"); // Make the border bright yellow on bright blue
    printf(fIsTop ? "l" : "m"); // print left corner 

    for(int i = 1; i < Size.X - 1; i++)
        printf("q"); // in line drawing mode, \x71 -> \u2500 "HORIZONTAL SCAN LINE-5"

    printf(fIsTop ? "k" : "j"); // print right corner
    printf(CSI "0m");
    printf(ESC "(B"); // exit line drawing mode
}

void PrintStatusLine(const char* const pszMessage, COORD const Size)
{
    printf(CSI "%d;1H", Size.Y);
    printf(CSI "K"); // clear the line
    printf(pszMessage);
}

int __cdecl console_codes_wmain(int argc, WCHAR* argv[])
{
    argc; // unused
    argv; // unused
    //First, enable VT mode
    bool fSuccess = EnableVTMode();
    if(!fSuccess)
    {
        printf("Unable to enter VT processing mode. Quitting.\n");
        return -1;
    }
    HANDLE hOut = GetStdHandle(STD_OUTPUT_HANDLE);
    if(hOut == INVALID_HANDLE_VALUE)
    {
        printf("Couldn't get the console handle. Quitting.\n");
        return -1;
    }

    CONSOLE_SCREEN_BUFFER_INFO ScreenBufferInfo;
    GetConsoleScreenBufferInfo(hOut, &ScreenBufferInfo);
    COORD Size;
    Size.X = ScreenBufferInfo.srWindow.Right - ScreenBufferInfo.srWindow.Left + 1;
    Size.Y = ScreenBufferInfo.srWindow.Bottom - ScreenBufferInfo.srWindow.Top + 1;

    // Enter the alternate buffer
    printf(CSI "?1049h");

    // Clear screen, tab stops, set, stop at columns 16, 32
    printf(CSI "1;1H");
    printf(CSI "2J"); // Clear screen

    int iNumTabStops = 4; // (0, 20, 40, width)
    printf(CSI "3g"); // clear all tab stops
    printf(CSI "1;20H"); // Move to column 20
    printf(ESC "H"); // set a tab stop

    printf(CSI "1;40H"); // Move to column 40
    printf(ESC "H"); // set a tab stop

    // Set scrolling margins to 3, h-2
    printf(CSI "3;%dr", Size.Y - 2);
    int iNumLines = Size.Y - 4;

    printf(CSI "1;1H");
    printf(CSI "102;30m");
    printf("Windows 10 Anniversary Update - VT Example");
    printf(CSI "0m");

    // Print a top border - Yellow
    printf(CSI "2;1H");
    PrintHorizontalBorder(Size, true);

    // // Print a bottom border
    printf(CSI "%d;1H", Size.Y - 1);
    PrintHorizontalBorder(Size, false);

    wchar_t wch;

    // draw columns
    printf(CSI "3;1H");
    int line = 0;
    for(line = 0; line < iNumLines * iNumTabStops; line++)
    {
        PrintVerticalBorder();
        if(line + 1 != iNumLines * iNumTabStops) // don't advance to next line if this is the last line
            printf("\t"); // advance to next tab stop

    }

    PrintStatusLine("Press any key to see text printed between tab stops.", Size);
    wch = _getwch();

    // Fill columns with output
    printf(CSI "3;1H");
    for(line = 0; line < iNumLines; line++)
    {
        int tab = 0;
        for(tab = 0; tab < iNumTabStops - 1; tab++)
        {
            PrintVerticalBorder();
            printf("line=%d", line);
            printf("\t"); // advance to next tab stop
        }
        PrintVerticalBorder();// print border at right side
        if(line + 1 != iNumLines)
            printf("\t"); // advance to next tab stop, (on the next line)
    }

    PrintStatusLine("Press any key to demonstrate scroll margins", Size);
    wch = _getwch();

    printf(CSI "3;1H");
    for(line = 0; line < iNumLines * 2; line++)
    {
        printf(CSI "K"); // clear the line
        int tab = 0;
        for(tab = 0; tab < iNumTabStops - 1; tab++)
        {
            PrintVerticalBorder();
            printf("line=%d", line);
            printf("\t"); // advance to next tab stop
        }
        PrintVerticalBorder(); // print border at right side
        if(line + 1 != iNumLines * 2)
        {
            printf("\n"); //Advance to next line. If we're at the bottom of the margins, the text will scroll.
            printf("\r"); //return to first col in buffer
        }
    }

    PrintStatusLine("Press any key to exit", Size);
    wch = _getwch();

    // Exit the alternate buffer
    printf(CSI "?1049l");

}