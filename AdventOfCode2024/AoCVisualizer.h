#pragma once
class AoCVisualizer
{

private:
    void Present();
    void ProcessInputEvents();
    friend class AoCBase;

public:
#pragma region Lifetime    
    static AoCVisualizer* PrepareDefaultVisualizer();
    void Init();
    void Close();
    void Dispose();
#pragma endregion

    DWORD TerminalOutputOriginalMode;
    DWORD TerminalInputOriginalMode;
    HANDLE ConsoleInput;
    HANDLE ConsoleOutput;

#pragma region DayPart Input
public:
    __forceinline const COORD GetCurrentMousePosition() const
    {
        return CurrentMousePosition;
    }
    void EnableMouseInput();
private:
    COORD CurrentMousePosition;
#pragma endregion

#pragma region Input Events
    DWORD LastMouseButtonsState{ 0 };
    VOID MouseEventProc(MOUSE_EVENT_RECORD mer);

#pragma endregion

#pragma region Timing
public:
    __forceinline static LONGLONG GetQPCFrequency()
    {
        if(QPC_Frequency == 0)
        {
            LARGE_INTEGER freq;
            QueryPerformanceFrequency(&freq);
            QPC_Frequency = freq.QuadPart;
            QPC_TimeDivider = 1000.0 / QPC_Frequency;
        }
        return QPC_Frequency;
    }
    __forceinline static LONGLONG GetQPCTicks()
    {
        LARGE_INTEGER steps;
        QueryPerformanceCounter(&steps);
        return steps.QuadPart;
    }
    __forceinline static double GetQPCTime(const LONGLONG& start)
    {
        LARGE_INTEGER steps;
        QueryPerformanceCounter(&steps);
        return static_cast<double>(steps.QuadPart - start) * QPC_TimeDivider;
    }
    __forceinline static double GetQPCTimeDelta(LONGLONG& start)
    {
        LARGE_INTEGER steps;
        QueryPerformanceCounter(&steps);
        double time = static_cast<double>(steps.QuadPart - start) * QPC_TimeDivider;
        start = steps.QuadPart;
        return time;
    }
private:
    static LONGLONG QPC_Frequency;
    static double QPC_TimeDivider;
#pragma endregion
};

