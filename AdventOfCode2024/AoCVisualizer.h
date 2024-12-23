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
struct JsonColor
{
    int r{ 0 };
    int g{ 0 };
    int b{ 0 };

    mutil::Vector3 ToColor()
    {
        return mutil::Vector3((float)r / 255, (float)g / 255, (float)b / 255);
    }
    static JsonColor FromColor(const mutil::Vector3& color)
    {
        JsonColor c;
        c.r = (int)(color.x * 255);
        c.g = (int)(color.y * 255);
        c.b = (int)(color.z * 255);
        return c;
    }
};
struct AoCVisualizerConfig
{
    bool AllowMouseCapture{ false };
    bool AlternateBuffer{ false };
    bool HideCursor{ false };
    bool ClearScreenOnExit{ true };
    bool Force16colorsMode{ false };

    int clearMode{ 0 }; // 0: fill with color, 1: fill with gradient, 2: fill with animated gradient
    int legendVisibility{ 1 }; // 0: never, 1: when camera move pressed, 2: always
    int cameraMoveMouseButton{ 0x04 }; // camera move mouse button. LMB: 1, RMB: 2, MMB: 4
    JsonColor clearColor;
    JsonColor infoColor;
    JsonColor gradientStartColor;
    JsonColor gradientEndColor;
    float animatedGradientSpeed{ 0.01f };

    bool EnableVT{ true }; // always true for now


    bool visualizeNormals{ false };
    bool visualizeHitPoints{ false };

};

struct AoCCharacterInfo
{
    AoCCharacterInfo(mutil::Vector3 front, mutil::Vector3 back, wchar_t _char)
        : Front(front), Back(back), Char(_char)
    {
    }
    AoCCharacterInfo()
        : Front(0), Back(0), Char(0)
    {
    }
    mutil::Vector3 Front;
    mutil::Vector3 Back;
    wchar_t Char;
};

struct AoCVisTransform
{
    mutil::Vector3 Location;
    mutil::Vector3 Direction;
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
{};
class AoCVisDirectionalLight : public AoCVisLight
{};

class AoCVisInputKey
{
public:
    AoCVisInputKey(const int KeyCode) : KeyCode(KeyCode) {}
    void Update();

    int KeyCode;
    bool IsPressed{ false };
    bool IsReleased{ false };
    bool IsDown{ false };
};
class AoCVisInput
{
public:
    AoCVisInput(AoCVisualizerConfig* Config) : Config(Config) { }
    void Update();
    void Release();
    void AddKey(int code);
    unordered_map<int, shared_ptr<AoCVisInputKey>> Keys;
    mutil::IntVector2 MouseLocation;
    mutil::IntVector2 LastMouseLocation;
    mutil::IntVector2 MouseDelta;
    AoCVisualizerConfig* Config;
    bool wasPressedLastFrame{ false };
};

class AoCVisCamera : public AoCVisActor
{
public:
    AoCVisCamera(AoCVisualizerConfig* config, AoCVisInput* Input);

    void Update(float deltaTime);

    __forceinline void InvalidateRayCache() { RayDirectionsNeedToRecompute = true; }
public:
    const mutil::Vector3& GetLocation() const { return Transform.Location; }
    const mutil::Vector3& GetDirection() const { return Transform.Direction; }

    void SetLocation(const mutil::Vector3& Position);
    void SetDirection(const mutil::Vector3& Direction);

    const mutil::Matrix4& GetProjection() const { return Projection; }
    const mutil::Matrix4& GetInvProjection() const { return InvProjection; }
    const mutil::Matrix4& GetView() const { return View; }
    const mutil::Matrix4& GetInvView() const { return InvView; }

    __forceinline const float GetVerticalFoV() const { return VerticalFoV; }
    __forceinline const float GetNearClip() const { return NearClip; }
    __forceinline const float GetFarClip() const { return FarClip; }
    __forceinline const float GetSpeed() const { return Speed; }
    __forceinline const float GetRotationSpeed() const { return RotationSpeed; }

    const std::vector<mutil::Vector3>& GetRayDirections();

    void SetVerticalFoV(const float vFOV);
    void SetNearClip(const float NearClip);
    void SetFarClip(const float FarClip);
    void SetSpeed(const float Speed) { this->Speed = Speed; }

    void SetViewportSize(const mutil::IntVector2& size) { this->ViewPortSize = size; }


    void RecalculateProjection();
    void RecalculateView();
private:

private:
    mutil::Vector2 LastMousePos;

    mutil::Matrix4 Projection;
    mutil::Matrix4 InvProjection;
    mutil::Matrix4 View;
    mutil::Matrix4 InvView;

    float VerticalFoV{ 45.0f };
    float NearClip{ 0.1f };
    float FarClip{ 100.0f };
    float Speed{ 1.0f };
    float RotationSpeed{ 180 };

    mutil::IntVector2 ViewPortSize;
    std::vector<mutil::Vector3> RayDirections;
    bool RayDirectionsNeedToRecompute{ true };

    AoCVisualizerConfig* Config;
    AoCVisInput* Input;

    bool singleKeyPressKeyPressed{ false };
};

typedef std::function<void(const int ObjectHandle, const mutil::Vector3& hitPoint, const mutil::IntVector2& screenPos, AoCCharacterInfo& pixel)> PixelShaderFunc;

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

    HANDLE ActiveScreenBuffer;
    HANDLE BackBuffer;

#pragma region Actual drawing
    FPSCounter FPS;
    CHAR_INFO* ConsoleBuffer{ nullptr };

    COORD ViewportSize{ 0 };
    AoCCharacterInfo* CharacterBuffer{ nullptr };
    wchar_t* Intermediatebuffer{ nullptr };
    WORD* Intermediate16ColorAttributeBuffer{ nullptr };

    int BackBufferSPrintfCounter{ 0 };
    int BackBufferSPrintfFullColorCounter{ 0 };

    __forceinline const bool GetHitLocation_WithSphere(const mutil::Vector3& RayOrigin, const mutil::Vector3& RayDirection, const float SphereRadius, mutil::Vector3& HitLocation) const;

    void Pixel(const int x, const int y, AoCCharacterInfo& pixel) const;
    void Render();
    void Draw();
    void DrawText(int x, int y, const wstring& text);
    void DrawText(int x, int y, const string& text);
    void Present();
    void CheckBufferSizeChange();
    void RecreateBuffers(const COORD& NewSize);
    void FillGradient(int startR, int startG, int startB, int endR, int endG, int endB, float phase);

    void InitPixelShaders();

    PixelShaderFunc PixelShader;
    PixelShaderFunc ActualPixelShader;

    PixelShaderFunc Vis_HitInfoPixelShader;
    PixelShaderFunc Vis_NormalPixelShader;

#pragma endregion

#pragma region Scene
private:
    std::vector<std::shared_ptr<AoCVisLight>> Lights;
    std::vector<std::shared_ptr<AoCVisActor>> Actors;
    std::shared_ptr<AoCVisCamera> Camera;
private:
    void InitializeDefaultScene();

    void AddActor(std::shared_ptr<AoCVisActor> Actor);
    void AddActors(std::vector<std::shared_ptr<AoCVisActor>> Actors);
    void RemoveActor(std::shared_ptr<AoCVisActor> Actor);
    void RemoveActors(std::vector<std::shared_ptr<AoCVisActor>> Actors);

    void AddLight(std::shared_ptr<AoCVisLight> Lights);
    void RemoveLight(std::shared_ptr<AoCVisLight> Lights);
#pragma endregion

    std::shared_ptr<AoCVisInput> Input;

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

