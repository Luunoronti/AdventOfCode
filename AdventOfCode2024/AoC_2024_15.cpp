#include "pch.h"
#include "AoC_2024_15.h"




AoC_2024_15::MoveCommand AoC_2024_15::ToMoveCommand(const char input)
{
    switch(input)
    {
    case '>': return MoveCommand::Right;
    case '<': return MoveCommand::Left;
    case 'v': return MoveCommand::Down;
    case '^': return MoveCommand::Up;
    default:
        return MoveCommand::Unknown;
    }
}
bool AoC_2024_15::IsMoveCommand(const char input)
{
    return input == MoveCommand::Left
        || input == MoveCommand::Up
        || input == MoveCommand::Down
        || input == MoveCommand::Right;
}
bool AoC_2024_15::IsActorSymbol(const char input)
{
    return input == ERobot
        || input == EWall
        || input == EGood;
}
AoC_2024_15::AActor* AoC_2024_15::CreateActor(const ActorSymbol ActorSymbol, int LocX, int LocY)
{
    AActor* actor{ nullptr };
    switch(ActorSymbol)
    {
    case EGood: actor = new AGood(LocX, LocY); break;
    case ERobot: actor = new ARobot(LocX, LocY); break;
    case EWall: actor = new AWall(LocX, LocY); break;
    default:
        return nullptr;
    }
    if(actor)
        actor->GameInstance = this;

    return actor;
}
int64_t AoC_2024_15::ComputeAnswerForCurrentWorld()
{
    uint64_t sum = 0;
    for(const AActor* actor : World->Map)
    {
        if(actor)
        {
            sum += actor->GetGPSCoordinate();
        }
    }
    return sum;
}
void AoC_2024_15::DrawMap()
{
    if(!World)
        return;

    for(int i = 0; i < ConBufferSize.Y * ConBufferSize.X; ++i)
    {
        ConBuffer[i].Char.UnicodeChar = ' ';
        ConBuffer[i].Attributes = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE; // White text
    }

    for(int y = 0; y < World->Height; y++)
    {
        for(int x = 0; x < World->Width; x++)
        {
            if(const AActor* actor = World->Get(x, y, nullptr))
            {
                ConBuffer[x + y * World->Width].Char.UnicodeChar = actor->GetIcon();
                ConBuffer[x + y * World->Width].Attributes = actor->GetIconAttributes();
            }
        }
    }

    // create a simple progress bar
    double done = (double)MovementCommands.size() / (double)MovementCommandsinitialSize;
    double progress = (double)ConBufferSize.X * done;
    for(int i = 0; i < (int)progress; ++i)
    {
        int pos = (ConBufferSize.Y - 1) * ConBufferSize.X + i;
        ConBuffer[pos].Char.UnicodeChar = L'\u2588';
        ConBuffer[pos].Attributes = FOREGROUND_BLUE | FOREGROUND_INTENSITY;
    }

    // actual print
    WriteToConsoleBuffer(ConBuffer, ConBufferSize, ConBufferCoord, ConWriteRegion);
}
void AoC_2024_15::ClearConsole()
{
    if(!World)
        return;

    for(int i = 0; i < ConBufferSize.Y * ConBufferSize.X; ++i)
    {
        ConBuffer[i].Char.UnicodeChar = ' ';
        ConBuffer[i].Attributes = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE; // White text
    }

    // actual print
    WriteToConsoleBuffer(ConBuffer, ConBufferSize, ConBufferCoord, ConWriteRegion);
}
void AoC_2024_15::ReadInput(int& mapDimX, int& mapDimY)
{
    int x = 0, y = 0;
    FILE* file;
    fopen_s(&file, GetFileName().c_str(), "r");
    if(file == NULL)
    {
        throw std::runtime_error("Failed to open file");
    }

    char line[4096]; // this will fit a line for sure
    while(fgets(line, sizeof(line), file))
    {
        const char start = line[0];
        if(IsActorSymbol(start)) // this indicated a wall, and our map lines always start with walls
        {
            x = 0;
            const char* p = &line[0];
            while(IsActorSymbol(*p) || *p == '.')
            {
                if(*p == ERobot)
                {
                    AActor* actor = CreateActor((ActorSymbol)*p, x, y);
                    if(actor)
                    {
                        ActorsToAddToScene.push_back(actor);
                        Robot = (ARobot*)actor;
                    }
                }
                else if(*p == EWall)
                {
                    AActor* actor = CreateActor((ActorSymbol)*p, x, y);
                    if(actor)
                    {
                        ActorsToAddToScene.push_back(actor);
                        if(GetStep() == 2)
                        {
                            // we create second actor right to us
                            // this will just thicken the wall
                            AActor* actor2 = CreateActor((ActorSymbol)*p, x + 1, y);
                            if(actor2)
                            {
                                ActorsToAddToScene.push_back(actor2);
                            }
                        }
                    }
                }
                else if(*p == EGood)
                {
                    AActor* actor = CreateActor((ActorSymbol)*p, x, y);
                    if(actor)
                    {
                        ActorsToAddToScene.push_back(actor);

                        if(GetStep() == 2)
                        {
                            // we create second actor that will represent second part
                            // of the cargo box, as we can have square actors only
                            AActor* actor2 = CreateActor((ActorSymbol)*p, x + 1, y);
                            if(actor2)
                            {
                                ActorsToAddToScene.push_back(actor2);

                                actor->RightJoinedActor = actor2;
                                actor2->LeftJoinedActor = actor;
                                ((AGood*)actor)->SetIcon('[');
                                ((AGood*)actor2)->SetIcon(']');

                            }
                        }

                    }
                }
                x++;
                if(GetStep() == 2)
                    x++;
                p++;
            }
            y++;
        }
        // this line is our movement command buffer
        else if(IsMoveCommand(start))
        {
            const char* p = &line[0];
            while(IsMoveCommand(*p))
            {
                MovementCommands.push_back(ToMoveCommand(*p));
                p++;
                MovementCommandsinitialSize++;
            }
        }
    }
    fclose(file);

    mapDimX = x;
    mapDimY = y;
}

void AoC_2024_15::BeginPlay()
{
    taskCompleted = false;
    MovementCommandsinitialSize = 0;
    MovementCommands.clear();

    // if these are not empty here, we may have a memory leak
    if(!ActorsToAddToScene.empty())
        throw std::runtime_error("Actors to add to the scene is not empty");
    if(World && !World->Map.empty())
        throw std::runtime_error("Scene is not empty");

    int mapWidth = 0, mapHeight = 0;
    ReadInput(mapWidth, mapHeight);

    if(Robot)
    {
        // construct controller
        RobotController = new ARobotController();
        // possess robot
        RobotController->Possess(Robot);
    }
    else
    {
        throw std::runtime_error("Unable to find robot");
    }

    // construct map
    World = new UWorld(mapWidth, mapHeight);
    for(AActor* actor : ActorsToAddToScene)
    {
        World->Set(actor->startPosX, actor->startPosY, actor);
    }
    ActorsToAddToScene.clear();


    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // ALL OF THIS WOULD BE NORMALLY DONE BY ENGINE 
    // OR SOME SORT OF VISUALIZER
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // hide cursor
    HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
    if(hConsole != INVALID_HANDLE_VALUE)
    {
        CONSOLE_CURSOR_INFO cursorInfo;
        GetConsoleCursorInfo(hConsole, &cursorInfo);
        cursorInfo.bVisible = FALSE;
        SetConsoleCursorInfo(hConsole, &cursorInfo);
    }

    // allocate console
    ConBufferSize.X = mapWidth;
    ConBufferSize.Y = mapHeight + 2;

    ConBufferCoord.X = 0;
    ConBufferCoord.Y = 0;

    ConWriteRegion.Left = 0;
    ConWriteRegion.Top = 0;
    ConWriteRegion.Right = mapWidth;
    ConWriteRegion.Bottom = mapHeight + 1;

    // Allocate a buffer to hold the CHAR_INFO data
    ConBuffer = new CHAR_INFO[2 + mapWidth * (mapHeight + 3)];

    if(this->Context->PartConfig->EnableVisualization)
        DrawMap();
    // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
}
void AoC_2024_15::EndPlay()
{
    if(this->Context->PartConfig->EnableVisualization)
        ClearConsole();

    // clear up
    if(World)
    {
        for(AActor* actor : World->Map)
        {
            if(actor)
            {
                delete actor;
            }
        }
        World->Map.clear();
        delete World;
    }
    World = nullptr;
    if(RobotController)
        delete RobotController;
    RobotController = nullptr;
    Robot = nullptr;

    delete[] ConBuffer;
    ConBuffer = nullptr;

    ConBufferSize.X = 0;
    ConBufferSize.Y = 0;

    ConBufferCoord.X = 0;
    ConBufferCoord.Y = 0;

    ConWriteRegion.Left = 0;
    ConWriteRegion.Top = 0;
    ConWriteRegion.Right = 0;
    ConWriteRegion.Bottom = 0;
}

void AoC_2024_15::Tick(double timeDelta)
{
    //if(IsTest())return;

    // if we have visualization disabled, just simulate everything
    // at once
    if(!this->Context->PartConfig->EnableVisualization)
    {
        TIME_PART;
        for(const MoveCommand command : this->MovementCommands)
        {
            RobotController->Move(command);
        }
        this->Context->PartConfig->Result = ComputeAnswerForCurrentWorld();
    }
    else
    {
        // this is to allow for hooking up OBS
        // to console window :)
        initialDelay -= timeDelta;
        if(GetStep() == 1 && IsTest() && initialDelay > 0)
        {
            this->RepeatTick();
            return;
        }

        // normal Tick() starts here
        tickDelayTime -= timeDelta;
        if(tickDelayTime <= 0)
        {
            if(taskCompleted)
            {
                return;
            }

            if(!this->MovementCommands.empty())
            {
                const MoveCommand command = this->MovementCommands.front();
                this->MovementCommands.pop_front();
                RobotController->Move(command);

                // if command buffer is empty, compute the last thing they want us to
                // also set timer to some higher value so that we will see
                // the result for a bit before it is being discarded

                if(this->MovementCommands.empty())
                {
                    this->Context->PartConfig->Result = ComputeAnswerForCurrentWorld();
                    tickDelayTime = 2;
                    taskCompleted = true;
                }
            }

            DrawMap(); // this would be normally done by the engine in rendering thread
            if(!taskCompleted)
                tickDelayTime = IsTest() ? (GetStep() == 1 ? 0.01 : .001) : 0.0001;
        }
        this->RepeatTick();
    }
}

void AoC_2024_15::ARobotController::Possess(AActor* ActorToPossess)
{
    ControlledPawn = ActorToPossess;
}

void AoC_2024_15::ARobotController::Move(const MoveCommand MoveCommand)
{
    if(ControlledPawn)
    {
        // establish the direction to move
        int dx = 0;
        int dy = 0;
        switch(MoveCommand)
        {
        case MoveCommand::Down: dy = 1; break;
        case MoveCommand::Left: dx = -1; break;
        case MoveCommand::Right: dx = 1; break;
        case MoveCommand::Up: dy = -1; break;
        }

        // move pawn
        if(ControlledPawn->CanMove(dx, dy))
            ControlledPawn->Move(dx, dy, false);
    }
}


bool AoC_2024_15::AActor::CanMove(int dirX, int dirY)
{
    if(UWorld* World = GetWorld())
    {
        // cant move outside of the world
        if(!World->WithinBounds(posX + dirX, posY + dirY))
            return false;
        if(AActor* neighbor = World->Get(posX + dirX, posY + dirY, nullptr))
        {
            if(neighbor->CanMove(dirX, dirY) == false)
                return false;

            if(dirY != 0)
            {
                if(neighbor->RightJoinedActor != nullptr)
                    if(!neighbor->RightJoinedActor->CanMove(dirX, dirY))
                        return false;
                if(neighbor->LeftJoinedActor != nullptr)
                    if(!neighbor->LeftJoinedActor->CanMove(dirX, dirY))
                        return false;
            }
        }
        return true;
    }
    return false;

}
void AoC_2024_15::AActor::Move(int dirX, int dirY, bool MoveJoined)
{
    if(UWorld* World = GetWorld())
    {
        if(!World->WithinBounds(posX + dirX, posY + dirY))
            return;

        // move anything that is in our path
        if(AActor* neighbor = World->Get(posX + dirX, posY + dirY, nullptr))
        {
            if(neighbor)
            {
                neighbor->Move(dirX, dirY, false);
            }
        }

        if(!MoveJoined && dirY != 0)
        {
            if(RightJoinedActor != nullptr)
                RightJoinedActor->Move(dirX, dirY, true);
            if(LeftJoinedActor != nullptr)
                LeftJoinedActor->Move(dirX, dirY, true);
        }

        // first, clear our location
        World->Set(posX, posY, nullptr);

        // then, move us
        posX += dirX;
        posY += dirY;

        // and set on map
        World->Set(posX, posY, this);
    }
}

int64_t AoC_2024_15::AActor::GetGPSCoordinate() const
{
    return 0;
}



int64_t AoC_2024_15::AGood::GetGPSCoordinate() const
{
    if(LeftJoinedActor != nullptr)return 0;
    return (100 * posY) + posX;
}

void AoC_2024_15::AGood::SetIcon(const char InIcon)
{
    Icon = InIcon;
}

