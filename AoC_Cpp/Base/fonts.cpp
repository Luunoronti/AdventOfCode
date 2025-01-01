#include "pch.h"
#include "fonts.h"
#include "fonts_bin.h"


       
TigrFont* cfont = nullptr;
TigrFont* cfont_10 = nullptr;
TigrFont* cfont_14 = nullptr;
TigrFont* cfont_16 = nullptr;
TigrFont* cfont_20 = nullptr;
TigrFont* cfont_24 = nullptr;
TigrFont* cfont_30 = nullptr;

void aocLoadFonts()
{
     cfont = tigrLoadFont(tigrLoadImageMem(_acCascadiaCodePL, (int)sizeof(_acCascadiaCodePL)), 1252);
     cfont_10 = tigrLoadFont(tigrLoadImageMem(_acCascadiaCodePL_10, (int)sizeof(_acCascadiaCodePL_10)), 1252);
     cfont_20 = tigrLoadFont(tigrLoadImageMem(_acCascadiaCodePL_20, (int)sizeof(_acCascadiaCodePL_20)), 1252);
     cfont_30 = tigrLoadFont(tigrLoadImageMem(_acCascadiaCodePL_30, (int)sizeof(_acCascadiaCodePL_30)), 1252);
     cfont_24 = tigrLoadFont(tigrLoadImageMem(_acCascadiaCodePL_24, (int)sizeof(_acCascadiaCodePL_24)), 1252);
     cfont_16 = tigrLoadFont(tigrLoadImageMem(_acCascadiaCodePL_16, (int)sizeof(_acCascadiaCodePL_16)), 1252);
     cfont_14 = tigrLoadFont(tigrLoadImageMem(_acCascadiaCodePL_14, (int)sizeof(_acCascadiaCodePL_14)), 1252);
}

void aocReleaseFonts()
{
    if(cfont) tigrFreeFont(cfont);
    if(cfont_10) tigrFreeFont(cfont_10);
    if(cfont_14) tigrFreeFont(cfont_14);
    if(cfont_16) tigrFreeFont(cfont_16);
    if(cfont_20) tigrFreeFont(cfont_20);
    if(cfont_30) tigrFreeFont(cfont_30);
    if(cfont_24) tigrFreeFont(cfont_24);
    cfont = nullptr;
    cfont_10 = nullptr;
    cfont_20 = nullptr;
    cfont_30 = nullptr;
    cfont_24 = nullptr;
    cfont_14 = nullptr;
    cfont_16 = nullptr;
}







/*************************** End of file ****************************/


