#include "pch.h"
#include "AoC_2024_04.h"


// these are just for added macro readability
// debugging is out of the window anyway :)
#define AND &&
#define OR ||

#define IS_CHAR(_x, _y, c)  (Map.Get(_x, _y, '\0') == c)

#define IS_X(_x, _y) (IS_CHAR(x+(_x), y+(_y), 'X'))
#define IS_M(_x, _y) (IS_CHAR(x+(_x), y+(_y), 'M'))
#define IS_A(_x, _y) (IS_CHAR(x+(_x), y+(_y), 'A'))
#define IS_S(_x, _y) (IS_CHAR(x+(_x), y+(_y), 'S'))

#define IS_X_CURRENT (IS_X(0, 0))
#define IS_M_CURRENT (IS_A(0, 0))
#define IS_A_CURRENT (IS_A(0, 0))
#define IS_S_CURRENT (IS_A(0, 0))

#define IS_MAS_EAST      (IS_M(+1, +0) AND IS_A(+2, +0) AND IS_S(+3, +0))
#define IS_MAS_WEST      (IS_M(-1, +0) AND IS_A(-2, +0) AND IS_S(-3, +0))
#define IS_MAS_NORTH     (IS_M(+0, -1) AND IS_A(+0, -2) AND IS_S(+0, -3))
#define IS_MAS_SOUTH     (IS_M(+0, +1) AND IS_A(+0, +2) AND IS_S(+0, +3))

#define IS_MAS_NORTHEAST (IS_M(+1, -1) AND IS_A(+2, -2) AND IS_S(+3, -3))
#define IS_MAS_NORTWEST  (IS_M(-1, -1) AND IS_A(-2, -2) AND IS_S(-3, -3))
#define IS_MAS_SOUTHEAST (IS_M(+1, +1) AND IS_A(+2, +2) AND IS_S(+3, +3))
#define IS_MAS_SOUTHWEST (IS_M(-1, +1) AND IS_A(-2, +2) AND IS_S(-3, +3))

#define IS_MxS_45  (IS_M(-1, -1) AND IS_S(+1, +1))
#define IS_SxM_45  (IS_S(-1, -1) AND IS_M(+1, +1))
#define IS_MxS_135 (IS_M(-1, +1) AND IS_S(+1, -1))
#define IS_SxM_135 (IS_S(-1, +1) AND IS_M(+1, -1))

const int64_t AoC_2024_04::Step1()
{
    long sum = 0;
    aoc::maps::Map2d<char> Map;
    aoc::aocs >> Map;

    TIME_PART;
    // for each position in buffer, we check 4 cardinal and 4 diagonal directions
    // but ONLY if current char == 'X'
    for(int y = 0; y < Map.Height; ++y)
    {
        for(int x = 0; x < Map.Width; ++x)
        {
            if(!IS_X_CURRENT) continue;

            if(IS_MAS_EAST) ++sum;
            if(IS_MAS_WEST) ++sum;
            if(IS_MAS_NORTH) ++sum;
            if(IS_MAS_SOUTH) ++sum;

            if(IS_MAS_NORTHEAST) ++sum;
            if(IS_MAS_NORTWEST) ++sum;
            if(IS_MAS_SOUTHEAST) ++sum;
            if(IS_MAS_SOUTHWEST) ++sum;
        }
    }
    return sum;
}



const int64_t AoC_2024_04::Step2()
{
    long sum = 0;

    aoc::maps::Map2d<char> Map;
    aoc::aocs >> Map;

    TIME_PART;
    // the position of interest must be an 'A'
    // then we check two possibilities for each diagonal
    for(int y = 0; y < Map.Height; ++y)
    {
        for(int x = 0; x < Map.Width; ++x)
        {
            if(!IS_A_CURRENT) continue;

            if((IS_MxS_45 || IS_SxM_45) && (IS_MxS_135 || IS_SxM_135)) ++sum;
        }
    }
    return sum;
}

