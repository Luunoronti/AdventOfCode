#include "pch.h"
#include "AoC_2024_13.h"

int AoC_2024_13::ParseLine(const char* line, AoC_2024_13::SingleGame* pGame)
{
    /*
        format:
        Button A: X+69, Y+23
        Button B: X+27, Y+71
        Prize: X=18641, Y=10279
        [..]
        */
    if(strstr(line, "Button A:") != NULL)
    {
        sscanf_s(line, "Button A: X+%d, Y+%d", &pGame->ax, &pGame->ay);
    }
    else if(strstr(line, "Button B:") != NULL)
    {
        sscanf_s(line, "Button B: X+%d, Y+%d", &pGame->bx, &pGame->by);
    }
    else if(strstr(line, "Prize:") != NULL)
    {
        sscanf_s(line, "Prize: X=%d, Y=%d", &pGame->px, &pGame->py);
    }
    return 0;
}

int AoC_2024_13::ParseLineStream(const char* line, AoC_2024_13::SingleGame* pGame)
{
    /*
        format:
        Button A: X+69, Y+23
        Button B: X+27, Y+71
        Prize: X=18641, Y=10279
        [..]
        */
    if(strstr(line, "Button A:") != NULL)
    {
        sscanf_s(line, "Button A: X+%d, Y+%d", &pGame->ax, &pGame->ay);
    }
    else if(strstr(line, "Button B:") != NULL)
    {
        sscanf_s(line, "Button B: X+%d, Y+%d", &pGame->bx, &pGame->by);
    }
    else if(strstr(line, "Prize:") != NULL)
    {
        sscanf_s(line, "Prize: X=%d, Y=%d", &pGame->px, &pGame->py);
    }
    return 0;
}


void AoC_2024_13::ParseGames(const char* fileName)
{

    FILE* file;
    fopen_s(&file, fileName, "r");
    if(file == NULL)
    {
        perror("Failed to open file");
        return;
    }

    SingleGame game;
    char line[128];

    while(fgets(line, sizeof(line), file))
    {
        ParseLine(line, &game);
        if(strcmp(line, "\n") == 0)
        {
            Games.push_back(game);
            game = SingleGame();
        }
    }
    if(game.ax != 0 || game.ay != 0 || game.bx != 0 || game.by != 0 || game.px != 0 || game.py != 0)
    {
        Games.push_back(game);
    }
    fclose(file);
}
const int64_t AoC_2024_13::Step1()
{
    TIME_PART;

    Games.clear();
    ParseGames(GetFileName().c_str());

    int64_t sum = 0;
    for(const auto& game : Games)
        sum += game.Solve32();
    return sum;
};
const int64_t AoC_2024_13::Step2()
{
    TIME_PART;
    int64_t sum = 0;
    for(const auto& game : Games)
        sum += game.Solve64(10000000000000);
    return sum;
};
