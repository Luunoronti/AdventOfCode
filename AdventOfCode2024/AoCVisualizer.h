#pragma once
class FPSCounter
{
public:
    FPSCounter() :
        frameCount(0), fps(0.0), frequency(1)
    {
    }
    FPSCounter(LONGLONG Frequency) :
        frameCount(0), fps(0.0), frequency(Frequency)
    {
        QueryPerformanceCounter(&startTime);
    }
    void Frame();
    double GetFPS() const { return fps; }
    double GetFrameTime() const { return frameTime; }
private: 
    int frameCount; 
    double fps;
    double frameTimeAcc{ 0 };
    double frameTime{ 0 };
    LONGLONG frequency; 
    LARGE_INTEGER startTime;
    LARGE_INTEGER frameQPFTime;
};
struct AoCVisualizerConfig
{
    bool AllowMouseCapture{ false };
    bool AlternateBuffer{ false };
    bool HideCursor{ false };
    bool ClearScreenOnExit{ true };

    bool EnableVT{ true }; // always true for now
};

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

struct AoCVisTransform
{
    mutil::Vector3 Location;
    mutil::Quaternion Rotation;
    mutil::Vector3 Scale;

    const mutil::Vector3 GetActorForward() const;
    const mutil::Vector3 GetActorUp() const;
    const mutil::Vector3 GetActorRight() const;
};


class AoCVisActor
{
protected:
    AoCVisTransform Transform;
};

class AoCVisLight : public AoCVisActor
{
};
class AoCVisDirectionalLight : public AoCVisLight
{
};

class AoCVisCamera : public AoCVisActor
{
public:
    AoCVisCamera();
    AoCVisCamera(const mutil::Vector3& StartLocation, const mutil::Quaternion& StartRotation);
 };
class AoCDefaultVisCamera : public AoCVisCamera
{
public:
    AoCDefaultVisCamera();
    AoCDefaultVisCamera(const mutil::Vector3& StartLocation, const mutil::Quaternion& StartRotation);
};

class AoCVisualizer
{
private:
    void Process();
    void ProcessSystemEvents();
    friend class AoCBase;

public:
#pragma region Lifetime   
    CRITICAL_SECTION MainCS;
    static AoCVisualizer* PrepareDefaultVisualizer(AoCVisualizerConfig& Config);
    void Init();
    void Close();
    void Dispose();
#pragma endregion

    AoCVisualizerConfig Config;

    DWORD TerminalOutputOriginalMode;
    DWORD TerminalInputOriginalMode;
    HANDLE ConsoleInput;
    HANDLE ConsoleOutput;

#pragma region Actual drawing
    FPSCounter FPS;
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
private:
    std::vector<std::shared_ptr<AoCVisLight>> Lights;
    std::vector<std::shared_ptr<AoCVisActor>> Actors;
    std::shared_ptr<AoCVisCamera> Camera;
private:
    void InitializeDefaultScene();
    void UpdateCamera();

    void AddActor(std::shared_ptr<AoCVisActor> Actor);
    void AddActors(std::vector<std::shared_ptr<AoCVisActor>> Actors);
    void RemoveActor(std::shared_ptr<AoCVisActor> Actor);
    void RemoveActors(std::vector<std::shared_ptr<AoCVisActor>> Actors);

    void AddLight(std::shared_ptr<AoCVisLight> Lights);
    void RemoveLight(std::shared_ptr<AoCVisLight> Lights);
#pragma endregion

#pragma region Helpers
public:
    static void SetConsoleTitle(const std::string& Title);
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

