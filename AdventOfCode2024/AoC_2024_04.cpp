#include "AoC_2024_04.h"

const int AoC_2024_04::GetDay() const { return 4; }
const int AoC_2024_04::GetExpectedResultStep1() const { return 2543; }
const int AoC_2024_04::GetExpectedResultStep1Test() const { return 18; }
const int AoC_2024_04::GetExpectedResultStep2() const { return 1930; }
const int AoC_2024_04::GetExpectedResultStep2Test() const { return 9; }

// these are just for added macro readability
// debugging is out of the window anyway :)
#define AND &&
#define OR ||

#define IS_CHAR(_x, _y, c)  ((_x)>=0 && (_y) >= 0 && (_x) < width && (_y) < height && buffer[((_x) + width*(_y))] == c)

#define IS_X_CURRENT (IS_CHAR(x, y, 'X'))
#define IS_A_CURRENT (IS_CHAR(x, y, 'A'))

#define IS_A(_x, _y) (IS_CHAR(x+(_x), y+(_y), 'A'))
#define IS_M(_x, _y) (IS_CHAR(x+(_x), y+(_y), 'M'))
#define IS_S(_x, _y) (IS_CHAR(x+(_x), y+(_y), 'S'))

#define IS_MAS_EAST  (IS_M(+1, +0) AND IS_A(+2, +0) AND IS_S(+3, +0))
#define IS_MAS_WEST  (IS_M(-1, +0) AND IS_A(-2, +0) AND IS_S(-3, +0))
#define IS_MAS_NORTH (IS_M(+0, -1) AND IS_A(+0, -2) AND IS_S(+0, -3))
#define IS_MAS_SOUTH (IS_M(+0, +1) AND IS_A(+0, +2) AND IS_S(+0, +3))

#define IS_MAS_NORTHEAST (IS_M(+1, -1) AND IS_A(+2, -2) AND IS_S(+3, -3))
#define IS_MAS_NORTWEST  (IS_M(-1, -1) AND IS_A(-2, -2) AND IS_S(-3, -3))
#define IS_MAS_SOUTHEAST (IS_M(+1, +1) AND IS_A(+2, +2) AND IS_S(+3, +3))
#define IS_MAS_SOUTHWEST (IS_M(-1, +1) AND IS_A(-2, +2) AND IS_S(-3, +3))

#define IS_MxS_45  (IS_M(-1, -1) AND IS_S(+1, +1))
#define IS_SxM_45  (IS_S(-1, -1) AND IS_M(+1, +1))
#define IS_MxS_135 (IS_M(-1, +1) AND IS_S(+1, -1))
#define IS_SxM_135 (IS_S(-1, +1) AND IS_M(+1, -1))

const long AoC_2024_04::Step1()
{
    long sum = 0;
    int width = 0, height = 0;
    const string buffer = ReadStringFromFile(1, height, width);
    const int size = height * width;

    // for each position in buffer, we check 4 cardinal and 4 diagonal directions
    // but ONLY if current char == 'X'
    for(int y = 0; y < height; ++y)
    {
        for(int x = 0; x < width; ++x)
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



const long AoC_2024_04::Step2()
{
    long sum = 0;
    int width = 0, height = 0;
    const string buffer = ReadStringFromFile(2, height, width);
    const int size = height * width;

    // the position of interest must be an 'A'
    // then we check two possibilities for each diagonal
    for(int y = 0; y < height; ++y)
    {
        for(int x = 0; x < width; ++x)
        {
            if(!IS_A_CURRENT) continue;
            if((IS_MxS_45 || IS_SxM_45) && (IS_MxS_135 || IS_SxM_135)) ++sum;
        }
    }
    return sum;
}
