#include "pch.h"
#include "AoCVisualizer.h"

LONGLONG AoCVisualizer::QPC_Frequency{ 0 };
double AoCVisualizer::QPC_TimeDivider{ 0 };


#define ESC "\x1b"
#define CSI "\x1b["

#define WESC L"\x1b"
#define WCSI L"\x1b["

#pragma region Helpers
DWORD WINAPI threadFunction(LPVOID lpParam)
{
    auto func = static_cast<std::function<void()>*>(lpParam);
    (*func)();
    return 0;
}


#pragma endregion

#pragma region Lifetime    
AoCVisualizer* AoCVisualizer::PrepareDefaultVisualizer(AoCVisualizerConfig& Config)
{
    auto vis = new AoCVisualizer();
    vis->Config = Config;
    return vis;
}
void AoCVisualizer::Init()
{
    InitializeCriticalSection(&MainCS);

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
    if(!SetConsoleMode(ConsoleOutput, TerminalOutputOriginalMode | (ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN)))
        throw std::runtime_error("Unable to enter VT Processing mode");

    //SetConsoleOutputCP(65001);
    SetConsoleOutputCP(CP_UTF8);

    // we need mouse support for camera and other stuff.
    TODO("Have this controlled by setting");
    if(Config.AllowMouseCapture)
        EnableMouseInput();

    //? Disable stream sync
    cin.sync_with_stdio(false);
    cout.sync_with_stdio(false);

    //? Disable stream ties
    cin.tie(NULL);
    cout.tie(NULL);

    // Enter the alternate buffer
    if(Config.AlternateBuffer)
        printf(CSI "?1049h");

    // hide the cursor
    if(Config.HideCursor)
        printf(CSI "?25l");

    // initialize scene with default camera and one default light
    InitializeDefaultScene();

    FPS = FPSCounter(GetQPCFrequency());
    // we will start a separated thread
    // that will process events, camera and draw 

    // we can then copy Map2D<char> or other types to
    // visualizer and it will start drawing them

    // later, we will introduce a concept of shaders
    // so, we need to start a drawing thread
    ProcessLambda = [&]()
        {
            while(ProcessIsEnabled)
                this->Process();
        };
    ProcessIsEnabled = true;

    ProcessingThread = CreateThread(nullptr, 0, threadFunction, &ProcessLambda, 0, nullptr);
    if(ProcessingThread == nullptr)
        throw std::runtime_error("Failed to create thread");

}
void AoCVisualizer::Close()
{
    ProcessIsEnabled = false;
    if(ProcessingThread)
    {
        WaitForSingleObject(ProcessingThread, INFINITE);
        CloseHandle(ProcessingThread);
        ProcessingThread = nullptr;
    }

    DeleteCriticalSection(&MainCS);

    if(Config.ClearScreenOnExit)
    {
        printf(CSI "1;1H");
        printf(CSI "2J"); // Clear screen
    }

    // Exit the alternate buffer
    if(Config.AlternateBuffer)
        printf(CSI "?1049l");

    // show the cursor
    if(Config.HideCursor)
        printf(CSI "?25h");

    SetConsoleMode(ConsoleInput, TerminalInputOriginalMode);
    SetConsoleMode(ConsoleOutput, TerminalOutputOriginalMode);
}
void AoCVisualizer::Dispose()
{
    if(CharacterBuffer)
        delete[] CharacterBuffer;
    if(Intermediatebuffer)
        delete[] Intermediatebuffer;
    Camera.reset();
    for(auto p : Lights) p.reset();
    for(auto p : Actors) p.reset();

    delete this;
}
#pragma endregion


void AoCVisualizer::Process()
{
    if(!ConsoleOutput || !ConsoleInput)
        return;

    FPS.Frame();
    const float frameTime = FPS.GetFrameTime();
    ProcessSystemEvents();
    CheckBufferSizeChange();

    Camera->Update(frameTime);

    Draw();
    printf(CSI "H"); // Move to (0,0)
    Present();
}


#pragma region Actual drawing
int frameNum = 0;
float phase = 0;

void AoCVisualizer::RecreateBuffers(const COORD& NewSize)
{
    if(CharacterBuffer)
        delete CharacterBuffer;
    if(Intermediatebuffer)
        delete Intermediatebuffer;

    CharacterBuffer = new AoCCharacterInfo[NewSize.X * NewSize.Y];
    Intermediatebuffer = new wchar_t[NewSize.X * NewSize.Y * 50]; // this makes us safe, as each 'pixel' may have this much information
}
void AoCVisualizer::FillGradient(int startR, int startG, int startB, int endR, int endG, int endB, float phase)
{
    for(int y = 0; y < ViewportSize.Y; ++y)
    {
        for(int x = 0; x < ViewportSize.X; ++x)
        {
            float t = static_cast<float>(y) / (ViewportSize.Y - 1);
            int br = static_cast<int>((1 - t) * startR + t * endR + (std::sin(phase) * 128));
            int bg = static_cast<int>((1 - t) * startG + t * endG + (std::sin(phase + 2.0f) * 128));
            int bb = static_cast<int>((1 - t) * startB + t * endB + (std::sin(phase + 4.0f) * 128));
            mutil::Vector3 bk = { (float)std::abs(br) / 255 , (float)std::abs(bg) / 255, (float)std::abs(bb) / 255 };
            CharacterBuffer[y * ViewportSize.X + x] = AoCCharacterInfo({ 1, 1, 1 }, bk * 0.3f, L' ');
        }
    }
}

void AoCVisualizer::Present()
{
    ZeroMemory(Intermediatebuffer, (ViewportSize.Y * ViewportSize.X * 50) * sizeof(wchar_t));
    size_t pos = 0;
    mutil::Vector3 lastFClr{ -1 };
    mutil::Vector3 lastBClr{ -1 };

    for(int y = 0; y < ViewportSize.Y; ++y)
    {
        for(int x = 0; x < ViewportSize.X; ++x)
        {
            int idx = y * ViewportSize.X + x;
            const auto& ci = CharacterBuffer[idx];
            wchar_t ch = ci.Char;

            if(lastFClr != ci.Front || (lastBClr != ci.Back))
            {
                lastFClr = ci.Front;

                int fgR = mutil::clamp((int)(lastFClr.r * 255), 0, 255);
                int fgG = mutil::clamp((int)(lastFClr.g * 255), 0, 255);
                int fgB = mutil::clamp((int)(lastFClr.b * 255), 0, 255);

                lastBClr = ci.Back;
                int bgR = mutil::clamp((int)(lastBClr.r * 255), 0, 255);
                int bgG = mutil::clamp((int)(lastBClr.g * 255), 0, 255);
                int bgB = mutil::clamp((int)(lastBClr.b * 255), 0, 255);

                ++BackBufferSPrintfCounter;
                ++BackBufferSPrintfFullColorCounter;
                pos += swprintf(Intermediatebuffer + pos, (ViewportSize.Y * ViewportSize.X * 50) - pos,
                    WCSI L"38;2;%d;%d;%d;48;2;%d;%d;%dm%lc", fgR, fgG, fgB, bgR, bgG, bgB, ch);
            }
            else
            {
                ++BackBufferSPrintfCounter;
                pos += swprintf(Intermediatebuffer + pos, (ViewportSize.Y * ViewportSize.X * 50) - pos, L"%lc", ch);
            }
        }
    }
    wprintf(L"%s", Intermediatebuffer);
}
void AoCVisualizer::CheckBufferSizeChange()
{
    CONSOLE_SCREEN_BUFFER_INFO ScreenBufferInfo;
    GetConsoleScreenBufferInfo(ConsoleOutput, &ScreenBufferInfo);
    COORD NewVS;
    NewVS.X = ScreenBufferInfo.srWindow.Right - ScreenBufferInfo.srWindow.Left + 1;
    NewVS.Y = ScreenBufferInfo.srWindow.Bottom - ScreenBufferInfo.srWindow.Top + 1;

    char buff[128];
    sprintf_s(buff, "(%dx%d)\n", NewVS.X, NewVS.Y);
    OutputDebugStringA(buff);

    if(ViewportSize.X != NewVS.X || ViewportSize.Y != NewVS.Y)
    {
        RecreateBuffers(NewVS);
        ViewportSize.X = NewVS.X;
        ViewportSize.Y = NewVS.Y;

        Camera->SetViewportSize({ NewVS.X, NewVS.Y });
        Camera->RecalculateProjection();
        Camera->InvalidateRayCache();
    }
}


const bool AoCVisualizer::GetHitLocation_WithSphere(const mutil::Vector3& RayOrigin, const mutil::Vector3& RayDirection, const float SphereRadius, mutil::Vector3& HitLocation) const
{
    // (bx^2 + by^2)t^2 + (2(ax*bx + ay*by))t + (ax^2 + ay^2 - r^2) = 0

    float a = mutil::dot(RayDirection, RayDirection);
    float b = 2.0f * mutil::dot(RayOrigin, RayDirection);
    float c = mutil::dot(RayOrigin, RayOrigin) - (SphereRadius * SphereRadius);

    // b^2 - 4ac
    float disc = b * b - 4 * a * c;

    if(disc < 0)
        return false;

    const auto& normRayDir = mutil::normalize(RayDirection);
    float t1 = (-b - mutil::sqrt(disc)) / (2.0f * a);
    HitLocation = RayOrigin + normRayDir * t1;

    return true;
}
void AoCVisualizer::Pixel(const int x, const int y, AoCCharacterInfo& pixel) const
{
    mutil::Vector3 rayOrigin = Camera->GetLocation();
    mutil::Vector3 rayDirection = Camera->GetRayDirections()[x + y * ViewportSize.X];

    float radius = 0.5f;

    mutil::Vector3 hitPoint;
    if(GetHitLocation_WithSphere(rayOrigin, rayDirection, radius, hitPoint))
    {
        // we will move this to the shader later
        // so that we will be able to switch between
        // what's outputed to the buffer

        pixel.Back = { 0, 0, 0 };// hitPoint;// { 0.5f, 0.2f, 0 };
        pixel.Front = { 0, 128, 128 };
        pixel.Char = L'#';
    }
}
void AoCVisualizer::Render()
{
    static mutil::Vector2 one{ 1, 1 };

    for(int y = 0; y < ViewportSize.Y; ++y)
    {
        for(int x = 0; x < ViewportSize.X; ++x)
        {
            //mutil::Vector2 cord = { (float)x / (float)ViewportSize.X, (float)y / (float)ViewportSize.Y };
            //cord = (cord * 2.0f) - one;

            // cast a ray and perform shading on result
            Pixel(x, y, CharacterBuffer[y * ViewportSize.X + x]);

            // perform shader on result

            // put in buffer
        }
    }
    // as we now have full image, we can do post
}
void AoCVisualizer::DrawText(int x, int y, const wstring& text)
{
    const int drawTextBuffSize = 1024;
    static wchar_t buff[drawTextBuffSize]{ 0 };
    ZeroMemory(buff, drawTextBuffSize * sizeof(wchar_t));

    swprintf(buff, drawTextBuffSize, L"%s", text.c_str());

    if(CharacterBuffer)
    {
        for(int i = x; i < min(x + ViewportSize.X, drawTextBuffSize); i++)
        {
            if(buff[i] == 0)
                return;
            CharacterBuffer[(i)+y * ViewportSize.X].Char = buff[i];
        }
    }
}
void AoCVisualizer::DrawText(int x, int y, const string& text)
{
    const int drawTextBuffSize = 1024;
    static char buff[drawTextBuffSize]{ 0 };
    ZeroMemory(buff, drawTextBuffSize * sizeof(char));

    sprintf_s(buff, drawTextBuffSize, "%s", text.c_str());

    if(CharacterBuffer)
    {
        for(int i = x; i < min(x + ViewportSize.X, drawTextBuffSize); i++)
        {
            if(buff[i] == 0)
                return;
            CharacterBuffer[(i)+y * ViewportSize.X].Char = buff[i];
        }
    }
}
void AoCVisualizer::Draw()
{
    // Define gradient colors (start and end colors) 
    int startR = 0, startG = 0, startB = 255; // Start color (blue) 
    int endR = 255, endG = 255, endB = 255; // End color (white)

    FillGradient(startR, startG, startB, endR, endG, endB, phase);
    phase += 0.002f;


    // render our scene (sync single threaded for now)
    Render();

    // draw UI on top
    // draw any other things on top of scene and UI

    frameNum++;


    // phase = phase + (0.005f * FPS.GetFrameTime());
    std::ostringstream oss;
    oss << "Frame: " << frameNum << " (FPS: " << FPS.GetFPS() << ") Delta time: " << FPS.GetFrameTime() << " ViewPort: (" << ViewportSize.X << ", " << ViewportSize.Y << ")";
    DrawText(0, 0, oss.str());

    // ::SetConsoleTitleW(buff);

    std::ostringstream oss2;
    auto& cl = Camera->GetLocation();
    auto& cd = Camera->GetDirection();
    oss2 << "Camera: Location: " << cl.x << ", " << cl.y << ", " << cl.z << ", Direction: " << cd.x << ", " << cd.y << ", " << cd.z;
    DrawText(0, 1, oss2.str());

    std::ostringstream oss3;
    oss3 << "Present: BackBuff SPrint #: " << BackBufferSPrintfCounter << ", Full color SPrint #: " << BackBufferSPrintfFullColorCounter;
    DrawText(0, 2, oss3.str());
    BackBufferSPrintfCounter = 0;
    BackBufferSPrintfFullColorCounter = 0;



}
#pragma endregion


#pragma region Scene
void AoCVisualizer::InitializeDefaultScene()
{
    Camera = std::make_shared<AoCVisCamera>();
    Lights.push_back(std::make_shared<AoCVisDirectionalLight>());
}

void AoCVisualizer::AddLight(std::shared_ptr<AoCVisLight> Light)
{
    ::EnterCriticalSection(&MainCS);
    this->Lights.push_back(Light);
    ::LeaveCriticalSection(&MainCS);
}
void AoCVisualizer::RemoveLight(std::shared_ptr<AoCVisLight> Light)
{
    ::EnterCriticalSection(&MainCS);
    this->Lights.erase(std::remove(this->Lights.begin(), this->Lights.end(), Light), this->Lights.end());
    ::LeaveCriticalSection(&MainCS);
}
void AoCVisualizer::AddActor(std::shared_ptr<AoCVisActor> Actor)
{
    ::EnterCriticalSection(&MainCS);
    this->Actors.push_back(Actor);
    ::LeaveCriticalSection(&MainCS);
}
void AoCVisualizer::RemoveActor(std::shared_ptr<AoCVisActor> Actor)
{
    ::EnterCriticalSection(&MainCS);
    this->Actors.erase(std::remove(this->Actors.begin(), this->Actors.end(), Actor), this->Actors.end());
    ::LeaveCriticalSection(&MainCS);
}
void AoCVisualizer::AddActors(std::vector<std::shared_ptr<AoCVisActor>> Actors)
{
    ::EnterCriticalSection(&MainCS);
    for(auto actor : Actors) this->Actors.push_back(actor);
    ::LeaveCriticalSection(&MainCS);
}
void AoCVisualizer::RemoveActors(std::vector<std::shared_ptr<AoCVisActor>> Actors)
{
    ::EnterCriticalSection(&MainCS);
    for(auto actor : Actors)
        this->Actors.erase(std::remove(this->Actors.begin(), this->Actors.end(), actor), this->Actors.end());
    ::LeaveCriticalSection(&MainCS);
}

#pragma endregion


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

#pragma region System Events
VOID AoCVisualizer::MouseEventProc(MOUSE_EVENT_RECORD mer)
{
#ifndef MOUSE_HWHEELED
#define MOUSE_HWHEELED 0x0008
#endif
    switch(mer.dwEventFlags)
    {
    case 0:
        if(mer.dwButtonState & FROM_LEFT_1ST_BUTTON_PRESSED && !(LastMouseButtonsState & FROM_LEFT_1ST_BUTTON_PRESSED))
        {
            //printf("LMB Pressed \n");
        }
        else if(!(mer.dwButtonState & FROM_LEFT_1ST_BUTTON_PRESSED) && (LastMouseButtonsState & FROM_LEFT_1ST_BUTTON_PRESSED))
        {
            //printf("LMB Released \n");
        }

        if(mer.dwButtonState & RIGHTMOST_BUTTON_PRESSED && !(LastMouseButtonsState & RIGHTMOST_BUTTON_PRESSED))
        {
            //printf("RMB Pressed \n");
        }
        else if(!(mer.dwButtonState & RIGHTMOST_BUTTON_PRESSED) && (LastMouseButtonsState & RIGHTMOST_BUTTON_PRESSED))
        {
            //printf("RMB Released \n");
        }

        if(mer.dwButtonState & FROM_LEFT_2ND_BUTTON_PRESSED && !(LastMouseButtonsState & FROM_LEFT_2ND_BUTTON_PRESSED))
        {
            //printf("MMB Pressed \n");
        }
        else if(!(mer.dwButtonState & FROM_LEFT_2ND_BUTTON_PRESSED) && (LastMouseButtonsState & FROM_LEFT_2ND_BUTTON_PRESSED))
        {
            //printf("MMB Released \n");
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
    {
        COORD newSize = { 80, 25 };
        if(!SetConsoleScreenBufferSize(ConsoleOutput, newSize))
        {
            std::cerr << "Error setting console screen buffer size." << std::endl;
            return;
        }
    }
    break;
    default:
        break;
    }
}

void AoCVisualizer::SetConsoleTitle(const std::string& Title)
{
    ::SetConsoleTitleA(Title.c_str());
}

void AoCVisualizer::ProcessSystemEvents()
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


void FPSCounter::Frame()
{
    frameCount++;
    LARGE_INTEGER currentTime;
    QueryPerformanceCounter(&currentTime);
    double elapsedTime = static_cast<double>(currentTime.QuadPart - startTime.QuadPart) / frequency;
    frameTime = static_cast<double>(currentTime.QuadPart - frameQPFTime.QuadPart) / frequency;
    frameTime *= 1000;
    frameQPFTime = currentTime;
    if(elapsedTime >= 0.5)
    {
        fps = frameCount / elapsedTime;
        frameCount = 0;
        startTime = frameQPFTime;
    }
}





#pragma region Camera
AoCVisCamera::AoCVisCamera()
{
    this->Transform.Location = { 0, 0, 3 };
    this->Transform.Direction = { 0, 0, -1 };
    this->Transform.Scale = { 1,1,1 };


}
const std::vector<mutil::Vector3>& AoCVisCamera::GetRayDirections()
{
    if(RayDirectionsNeedToRecompute)
    {
        RayDirectionsNeedToRecompute = false;

        RayDirections.resize(ViewPortSize.x * ViewPortSize.y);

        for(uint32_t y = 0; y < ViewPortSize.y; y++)
        {
            for(uint32_t x = 0; x < ViewPortSize.x; x++)
            {
                mutil::Vector2 coord = { (float)x / (float)ViewPortSize.x, (float)y / (float)ViewPortSize.y };
                coord = coord * 2.0f; // -1 -> 1
                coord.x -= 1.f;
                coord.y -= 1.f;

                mutil::Vector4 target = InvProjection * mutil::Vector4(coord.x, coord.y, 1, 1);
                mutil::Vector3 rayDirection = mutil::Vector3(InvView * mutil::Vector4(mutil::normalize(mutil::Vector3(target) / target.w), 0)); // World space
                RayDirections[x + y * ViewPortSize.x] = rayDirection;
            }
        }
    }

    return RayDirections;
}

void AoCVisCamera::Update(float deltaTime)
{
    mutil::Vector2 mousePos;

    POINT mousePoint;
    GetCursorPos(&mousePoint);
    HWND consoleWindow = GetConsoleWindow();
    RECT consoleRect;
    GetWindowRect(consoleWindow, &consoleRect);
    mousePos.x = mousePoint.x - consoleRect.left;
    mousePos.y = mousePoint.y - consoleRect.top;

    mutil::Vector2 delta = (mousePos - LastMousePos) * 0.002f; // this is mouse sensitivity
    LastMousePos = mousePos;

    // movement here. 
    // but ONLY if mouse RMB is being pressed
    if(GetAsyncKeyState(VK_RBUTTON) & 0x8000)
    {
        mutil::Vector3 up{ 0.0f, 1.0f, 0.0f };
        mutil::Vector3 right = mutil::cross(Transform.Direction, up);
        bool moved{ false };
        if(GetAsyncKeyState('W') & 0x8000) // forward
        {
            Transform.Location += Transform.Direction * GetSpeed() * deltaTime;
            moved = true;
        }
        else if(GetAsyncKeyState('S') & 0x8000) // backward
        {
            Transform.Location -= Transform.Direction * GetSpeed() * deltaTime;
            moved = true;
        }
        else if(GetAsyncKeyState('A') & 0x8000) // left
        {
            Transform.Location -= right * GetSpeed() * deltaTime;
            moved = true;
        }
        else if(GetAsyncKeyState('D') & 0x8000) // right
        {
            Transform.Location += right * GetSpeed() * deltaTime;
            moved = true;
        }
        else if(GetAsyncKeyState('Q') & 0x8000) // pan up
        {
            Transform.Location += up * GetSpeed() * deltaTime;
            moved = true;
        }
        else if(GetAsyncKeyState('E') & 0x8000) // pan down
        {
            Transform.Location -= up * GetSpeed() * deltaTime;
            moved = true;
        }
        else if(GetAsyncKeyState('R') & 0x8000) // camera speed up
        {
            SetSpeed(max(10, GetSpeed() + 0.01f));
        }
        else if(GetAsyncKeyState('F') & 0x8000) // camera speed down
        {
            SetSpeed(min(0.001f, GetSpeed() - 0.01f));
        }

        if(delta.x != 0 || delta.y != 0)
        {
            float pd = delta.y * GetRotationSpeed() * 5;
            float yd = delta.x * GetRotationSpeed();

            Transform.Rotation = mutil::normalize(mutil::cross(mutil::angleAxis(pd, right), mutil::angleAxis(-yd, up)));
            Transform.Direction = mutil::normalize(mutil::rotatevector(Transform.Rotation, Transform.Direction));
            moved = true;
        }

        if(moved)
        {
            RecalculateView();
            InvalidateRayCache();
        }
    }
}

void AoCVisCamera::RecalculateProjection()
{
    // int gcd = std::gcd(ViewPortSize.x, ViewPortSize.y);
    //Projection = mutil::perspective(mutil::radians(VerticalFoV), (float)ViewPortSize.y/gcd, NearClip, FarClip);
    Projection = mutil::perspectiveFov(mutil::radians(VerticalFoV), (float)ViewPortSize.x / 2, (float)ViewPortSize.y, NearClip, FarClip);
    InvProjection = mutil::inverse(Projection);
}

void AoCVisCamera::RecalculateView()
{
    View = mutil::lookAt(Transform.Location, Transform.Location + Transform.Direction, { 0, 1, 0 });
    InvView = mutil::inverse(View);
}

void AoCVisCamera::SetLocation(const mutil::Vector3& Location)
{
    this->Transform.Location = Location;
    InvalidateRayCache();
}
void AoCVisCamera::SetDirection(const mutil::Vector3& Direction)
{
    this->Transform.Direction = Direction;
    InvalidateRayCache();
}

void AoCVisCamera::SetVerticalFoV(const float vFOV)
{
    this->VerticalFoV = vFOV;
    InvalidateRayCache();
}
void AoCVisCamera::SetNearClip(const float NearClip)
{
    this->NearClip = NearClip;
    InvalidateRayCache();
}
void AoCVisCamera::SetFarClip(const float FarClip)
{
    this->FarClip = FarClip;
    InvalidateRayCache();
}


#pragma endregion