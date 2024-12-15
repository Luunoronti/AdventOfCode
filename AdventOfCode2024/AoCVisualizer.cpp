#include "pch.h"
#include "AoCVisualizer.h"

LONGLONG AoCVisualizer::QPC_Frequency{ 0 };
double AoCVisualizer::QPC_TimeDivider{ 0 };


#pragma region Lifetime    
AoCVisualizer* AoCVisualizer::PrepareDefaultVisualizer()
{
    auto vis = new AoCVisualizer();

    return vis;
}
void AoCVisualizer::Init()
{
    ConsoleInput = GetStdHandle(STD_INPUT_HANDLE);
    if(ConsoleInput == INVALID_HANDLE_VALUE)
        throw std::runtime_error("Failed to get console input handle");

    ConsoleOutput = GetStdHandle(STD_OUTPUT_HANDLE);
    if(ConsoleOutput == INVALID_HANDLE_VALUE)
        throw std::runtime_error("Failed to get console output handle");

    if(!GetConsoleMode(ConsoleInput, &TerminalInputOriginalMode))
        throw std::runtime_error("Failed to store console intput mode");

    if(!GetConsoleMode(ConsoleOutput, &TerminalOutputOriginalMode))
        throw std::runtime_error("Failed to store console output mode");

    // enable virtual terminal escape codes and disable automatic return on newline, so we can 
    // safely print out to the console at last position without it being scrolled
    SetConsoleMode(ConsoleOutput, TerminalOutputOriginalMode | (ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN));
    SetConsoleOutputCP(65001);

    //? Disable stream sync
    cin.sync_with_stdio(false);
    cout.sync_with_stdio(false);

    //? Disable stream ties
    cin.tie(NULL);
    cout.tie(NULL);


    // we for sure need a handler for windows size change
    // about all else, we will see


    // temporary: enable mouse
    EnableMouseInput();
}
void AoCVisualizer::Close()
{
    SetConsoleMode(ConsoleInput, TerminalInputOriginalMode);
    SetConsoleMode(ConsoleOutput, TerminalOutputOriginalMode);
}




void AoCVisualizer::Dispose()
{
    delete this;
}

#pragma endregion





void AoCVisualizer::Present()
{
}

#pragma region DayPart Input
void AoCVisualizer::EnableMouseInput()
{
    DWORD in_consoleMode = 0;
    //if(!GetConsoleMode(ConsoleInput, &in_consoleMode))
    //    throw std::runtime_error("Failed to store console intput mode");
    in_consoleMode |= ENABLE_WINDOW_INPUT | ENABLE_MOUSE_INPUT | ENABLE_INSERT_MODE | ENABLE_EXTENDED_FLAGS;
    in_consoleMode &= ~ENABLE_ECHO_INPUT;
    SetConsoleMode(ConsoleInput, in_consoleMode);
}
#pragma endregion

#pragma region Input Events
VOID AoCVisualizer::MouseEventProc(MOUSE_EVENT_RECORD mer)
{
#ifndef MOUSE_HWHEELED
#define MOUSE_HWHEELED 0x0008
#endif
  //  printf("Mouse event: ");

    switch(mer.dwEventFlags)
    {
    case 0:
        if(mer.dwButtonState & FROM_LEFT_1ST_BUTTON_PRESSED && !(LastMouseButtonsState & FROM_LEFT_1ST_BUTTON_PRESSED))
        {
            printf("LMB Pressed \n");
        }
        else if(!(mer.dwButtonState & FROM_LEFT_1ST_BUTTON_PRESSED) && (LastMouseButtonsState & FROM_LEFT_1ST_BUTTON_PRESSED))
        {
            printf("LMB Released \n");
        }

        if(mer.dwButtonState & RIGHTMOST_BUTTON_PRESSED && !(LastMouseButtonsState & RIGHTMOST_BUTTON_PRESSED))
        {
            printf("RMB Pressed \n");
        }
        else if(!(mer.dwButtonState & RIGHTMOST_BUTTON_PRESSED) && (LastMouseButtonsState & RIGHTMOST_BUTTON_PRESSED))
        {
            printf("RMB Released \n");
        }

        if(mer.dwButtonState & FROM_LEFT_2ND_BUTTON_PRESSED && !(LastMouseButtonsState & FROM_LEFT_2ND_BUTTON_PRESSED))
        {
            printf("MMB Pressed \n");
        }
        else if(!(mer.dwButtonState & FROM_LEFT_2ND_BUTTON_PRESSED) && (LastMouseButtonsState & FROM_LEFT_2ND_BUTTON_PRESSED))
        {
            printf("MMB Released \n");
        }

        LastMouseButtonsState = mer.dwButtonState;
        break;
    case DOUBLE_CLICK:
        // call event
        break;
    case MOUSE_HWHEELED:
        // event
        break;
    case MOUSE_MOVED:
        CurrentMousePosition = mer.dwMousePosition;
        // call event here? 
        break;
    case MOUSE_WHEELED:
        break;
    default:
        break;
    }
}




void AoCVisualizer::ProcessInputEvents()
{
    DWORD events;
    GetNumberOfConsoleInputEvents(ConsoleInput, &events);
    if(events != 0)
    {
        INPUT_RECORD irInBuf[128];
        DWORD cNumRead;
        if(!ReadConsoleInput(ConsoleInput, irInBuf, 128, &cNumRead))
            return;

        for(DWORD i = 0; i < cNumRead; i++)
        {
            switch(irInBuf[i].EventType)
            {
            case KEY_EVENT:
                //   KeyEventProc(irInBuf[i].Event.KeyEvent);
                break;

            case MOUSE_EVENT: MouseEventProc(irInBuf[i].Event.MouseEvent); break;
            case WINDOW_BUFFER_SIZE_EVENT:
                //   ResizeEventProc(irInBuf[i].Event.WindowBufferSizeEvent);
                break;
            case FOCUS_EVENT: break;
            case MENU_EVENT: break;
            default: break;
            }
        }
    }
}
#pragma endregion