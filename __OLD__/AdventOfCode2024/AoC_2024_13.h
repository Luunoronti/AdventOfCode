#pragma once
#include "AoC2024.h"
#include <cmath> // For std::fmod

class AoC_2024_13 : public AoC2024
{

public:
    struct SingleGame
    {
        int ax, ay;
        int bx, by;
        int px, py;

        SingleGame()
            : ax(0), ay(0), bx(0), by(0), px(0), py(0)
        {
        }

        /*
        P(A) * ax + P(B) * bx = pX
        P(A) * ay + P(b) * by = pY
         
         
        a = (pX * by - pY * bx) / (ax * by - ay * bx);
        b = (pY * ax - pX * ay) / (ax * by - ay * bx);
        */
        __forceinline const int64_t Solve64(int64_t prizeAdd = 0) const
        {
            int64_t pX = px + prizeAdd;
            int64_t pY = py + prizeAdd;
            long double div = ((long double)ax * by - ay * bx);
            long double a = ((long double)pX * by - pY * bx) / div;
            long double b = ((long double)pY * ax - pX * ay) / div;

            if(std::fmod(a, 1.0) == 0 && std::fmod(b, 1.0) == 0)
                return (3 * (int64_t)a) + (int64_t)b;
            return 0;
        }

        __forceinline const int Solve32() const
        {
            float div = ((float)ax * by - ay * bx);
            float a = ((float)px * by - py * bx) / div;
            float b = ((float)py * ax - px * ay) / div;

            if(std::fmod(a, 1.0) == 0 && std::fmod(b, 1.0) == 0)
                return (3 * (int)a) + (int)b;
            return 0;
        }
    };

public:
    const virtual __forceinline int GetDay() const override { return 13; }
    const int64_t Step1() override;
    const int64_t Step2() override;

    const int64_t Step1_NoVector();

    int ParseLine(const char* line, AoC_2024_13::SingleGame* pGame);
    void ParseGames(const char* fileName);

private:
    std::deque<SingleGame> Games;

    int64_t Part2Answer{ 0 };
};

