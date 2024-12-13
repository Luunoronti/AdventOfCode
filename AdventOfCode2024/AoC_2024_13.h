#pragma once
#include "AoC2024.h"
#include <cmath> // For std::fmod

class AoC_2024_13 : public AoC2024
{

    struct SingleGame
    {
        int64_t ax, ay;
        int64_t bx, by;
        int64_t px, py;

        SingleGame()
            : ax(0), ay(0), bx(0), by(0), px(0), py(0)
        {
        }

        __forceinline const int64_t Solve(int64_t prizeAdd = 0) const
        {
            int64_t pX = px + prizeAdd;
            int64_t pY = py + prizeAdd;
            double a = ((double)pX * by - pY * bx) / ((double)ax * by - ay * bx);
            double b = ((double)pY * ax - pX * ay) / ((double)ax * by - ay * bx);

            if(std::fmod(a, 1.0) == 0 && std::fmod(b, 1.0) == 0)
                return (3 * (int64_t)a) + (int64_t)b;

            return 0;
        }


    };

public:
    const virtual __forceinline int GetDay() const override { return 13; }
    const int64_t Step1() override;
    const int64_t Step2() override;

    int ParseLine(const char* line, AoC_2024_13::SingleGame* pGame);
    void ParseGames(const char* fileName);

private:
    std::deque<SingleGame> Games;
};

