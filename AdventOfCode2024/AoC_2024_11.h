#pragma once
#include "AoC2024.h"


class AoC_2024_11 : public AoC2024
{
public:
    typedef std::unordered_map<int64_t, int64_t> Map;
    typedef std::pair<int64_t, int64_t> MapPair;
    typedef std::unordered_map<int64_t, std::pair<int64_t, int64_t>> DoubleNumberCache;
    typedef std::unordered_map<int64_t, int64_t> SingleNumberCache;


public:
    const virtual __forceinline int GetDay() const override { return 11; }
    const int64_t Step1() override;
    const int64_t Step2() override;

    void CountAll(const vector<int64_t>& list, int steps, int stepsForPart1, Map& map, Map& map2);
    int64_t AdvanceOneStep(Map* map, Map* target);

    int64_t secondPart_Test{ 0 };
    int64_t secondPart_Live{ 0 };

    int64_t firstPart_Test{ 0 };
    int64_t firstPart_Live{ 0 };
};

extern int cacheHits;
extern int cacheMisses;
extern int dblNumCacheHits;
extern int sngNumCacheHits;
extern int powCacheHits;
extern int dblNumCacheMisses;
extern int sngNumCacheMisses;
extern int powCacheMisses;