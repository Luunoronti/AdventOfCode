#pragma once
struct AoCCharacterInfo
{
    AoCCharacterInfo(int fr, int fg, int fb, int br, int bg, int bb, wchar_t wchar)
        : FRgb((fr << 24) | (fg<<16) | fb), BRgb((br << 24) | (bg<<16) | bb), Wchar(wchar)
    {
    }
    AoCCharacterInfo(int frgb, int brgb, wchar_t wchar)
        : FRgb(frgb), BRgb(brgb), Wchar(wchar)
    {
    }
    AoCCharacterInfo()
        : FRgb(0), BRgb(0), Wchar(0)
    {
    }
    int FRgb;
    int BRgb;
    wchar_t Wchar;
};

class AoCVisualizer
{
private:
    void Process();
    void ProcessSystemEvents();
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

#pragma region Actual drawing
    CHAR_INFO* ConsoleBuffer{ nullptr };

    COORD ViewportSize{ 0 };
    AoCCharacterInfo* CharacterBuffer{ nullptr };
    wchar_t* Intermediatebuffer{ nullptr };

    void Draw();
    void Present();
    void CheckBufferSizeChange();
    void RecreateBuffers(const COORD& NewSize);
    void FillGradient(int startR, int startG, int startB, int endR, int endG, int endB, float phase);
#pragma endregion

#pragma region Scene and Camera (camera is not an actual object like in proper engines)

    // camera stuff

private:
    void UpdateCamera();

#pragma endregion


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
    std::function<void()> ProcessLambda;
    bool ProcessIsEnabled{ false };
    HANDLE ProcessingThread{ 0 };
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

