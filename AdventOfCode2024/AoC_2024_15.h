#pragma once
#include "AoC2024.h"

/*
We will define 3 types of objects (Actors)
that can exists on the map (scene).
These are:
Walls - not movable
Goods - movable
Robot - movable

There is also 4th type of object, Controller.
It is virtual in a sense that it does not exist on the scene.
It's job is to move Robot it possesses, based on input.

Each Actor, before attempting to move, check the scene in front of it
and should there be something in front, our actor asks that other actor to move.
The movement query will return true if possible and we can the proceed to actually move the Actor.
Movement will first attempt to move actor(s) in front.
As movement query was positive, the movement itself is assumed to be possible.

The "front" is defined not by Actor rotation, but by a movement direction,
passed to the method.


This code is not exactly 1:1 with standard unreal gamedev. These are some
basic differences, to name a few:

- The AActor and child classes are not nested inside another class
  nor other namespace

- There is some sort of memory management, so new AActor() would not ask OS heap to
  allocate object directly. Direct allocation causes memory losses (each allocation
  can result in 4kB page reservation) and heap fragmentation, which OS takes time
  to deal with.

- Some objects are under garbage collection. In Unreal, actors are such objects.
  Therefore, one does not delete, but calls DestroyActor() on them. This registers
  them for destruction (which can be, and often is, delayed).

- No rendering routine is mixed with logic. The code bellow deals with console modes (disabling cursor)
  allocating console buffers, drawing, etc. All of this is being handled by systems external
  to game logic, and a lot of it happens on separated thread(s).

- Instead of using std:: for collections (vector, list), unreal uses it's own TMap<T>, TList<T> and so on.
  The advantage of those collections is they are natively supported by editor and are instantly visible
  in object properties panel.

*/



class AoC_2024_15 : public AoC2024
{
    class AActor;

    typedef aoc::maps::Map2d<AActor*> UWorld;

#define MOVE_CMD_UP '^'
#define MOVE_CMD_DOWN '^'
#define MOVE_CMD_UP '^'
#define MOVE_CMD_UP '^'

    enum MoveCommand : char
    {
        Unknown = ' ',
        Up = '^',
        Down = 'v',
        Left = '<',
        Right = '>'
    };
    enum ActorSymbol : char
    {
        EUnknown = ' ',
        EWall = '#',
        EGood = 'O',
        ERobot = '@'
    };

    class AActor
    {
    public:
        int startPosX;
        int startPosY;
        int posX;
        int posY;
        AoC_2024_15* GameInstance{ nullptr };
        AActor* LeftJoinedActor{ nullptr };
        AActor* RightJoinedActor{ nullptr };

        virtual char GetIcon() const { return '.'; };
        virtual int GetIconAttributes() const { return FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE; };
        virtual void Move(int dirX, int dirY, bool MoveJoined);
        virtual bool CanMove(int dirX, int dirY);
        UWorld* GetWorld() { return GameInstance ? GameInstance->World : nullptr; }
        virtual int64_t GetGPSCoordinate() const;

        AActor(int startPosX, int startPosY)
            : startPosX(startPosX), startPosY(startPosY), posX(startPosX), posY(startPosY)
        {
        }
    };
    class AGood : public AActor
    {
        virtual char GetIcon() const override { return Icon; };
        virtual int GetIconAttributes() const { return FOREGROUND_BLUE; };
    public:
        AGood(int startPosX, int startPosY)
            : AActor(startPosX, startPosY)
        {
        }
        virtual int64_t GetGPSCoordinate() const override;
        void SetIcon(const char InIcon);
        char Icon{ 'O' };
    };
    class AWall : public AActor
    {
        virtual char GetIcon() const override { return '#'; };
        virtual int GetIconAttributes() const { return FOREGROUND_RED; };

        virtual void Move(int dirX, int dirY, bool MoveJoined) override {};
        virtual bool CanMove(int dirX, int dirY) override { return false; }
    public:
        AWall(int startPosX, int startPosY)
            : AActor(startPosX, startPosY)
        {
        }
    };
    class ARobot : public AActor
    {
        virtual char GetIcon() const override { return '@'; };
        virtual int GetIconAttributes() const { return FOREGROUND_GREEN | FOREGROUND_INTENSITY; };

    public:
        ARobot(int startPosX, int startPosY)
            : AActor(startPosX, startPosY)
        {
        }
    };
    class ARobotController : public AActor
    {
        virtual char GetIcon() const override { return '!'; };
        virtual int GetIconAttributes() const { return FOREGROUND_BLUE | FOREGROUND_INTENSITY; };

    public:
        ARobotController()
            : AActor(-100, -100)
        {
        }

        void Possess(AActor* ActorToPossess);
        void Move(const MoveCommand MoveCommand);
    private:
        AActor* ControlledPawn{ nullptr };
    };

public:
    const virtual __forceinline int GetDay() const override { return 15; }
    void Tick(double timeDelta) override;
    void BeginPlay() override;
    void EndPlay() override;

private:
    void ReadInput(int& mapDimX, int& mapDimY);
    MoveCommand ToMoveCommand(const char input);
    bool IsMoveCommand(const char input);
    bool IsActorSymbol(const char input);
    AActor* CreateActor(const ActorSymbol ActorSymbol, int LocX, int LocY);
    int64_t ComputeAnswerForCurrentWorld();
    void DrawMap();
    void ClearConsole();
private:
    // this is a temporary list of all Actors.
    // it is being filled in while reading input
    // and then used to construct the scene.
    std::deque<AActor*> ActorsToAddToScene;

    // we also have a scene, which is a map of AActors
    UWorld* World;

    // this is our controller. we will feed input to it later
    ARobotController* RobotController{ nullptr };

    // this is our robot, we will possess it later
    ARobot* Robot{ nullptr };

    // and movement commands for the Robot Controller
    std::deque<MoveCommand> MovementCommands;
    int MovementCommandsinitialSize{ 0 };

    double initialDelay{ 20 };
    double tickDelayTime{ 0 };
    bool taskCompleted{ false };

    // before we move it to visualizer,
    // lets create console buffer directly
private:
    COORD ConBufferSize;
    COORD ConBufferCoord;
    SMALL_RECT ConWriteRegion;
    CHAR_INFO* ConBuffer;
};

