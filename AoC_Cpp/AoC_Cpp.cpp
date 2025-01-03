// AoC_Cpp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include <sstream>
#include "Base/Tigr/tigr.h"
#include "Base/TimeKeeper.h"
#include "Base/fonts.h"

#include "Base/Tracy/tracy/Tracy.hpp"

const char fxShader[] =
    "void fxShader(out vec4 color, in vec2 uv) {"
    "   vec2 tex_size = vec2(textureSize(image, 0));"
    "   vec4 c = texture(image, (floor(uv * tex_size) + 0.5 * sin(parameters.x)) / tex_size);"
    "   color = c * vec4(0.2f, 0.6f, 1, 1);"
    "}";

int main()
{
    std::cout << "Hello World!\n";

    TimeKeeper keeper;
    int frameNum = 0;
    Tigr* screen = tigrWindow(1920, 1080, "Hello", 0);
    aocLoadFonts();
    tigrBeginOpenGL(screen);
    //tigrSetPostShader(screen, fxShader, sizeof(fxShader) - 1);
    float duration = 1;
    float phase = 0;

    while(!tigrClosed(screen))
    {
        phase += tigrTime();

        keeper.Frame();

        while (phase > duration) 
        {
            phase -= duration;
        }
        float p = 6.28 * phase / duration;
        tigrSetPostFX(screen, 8, 8, 1, 1);

        tigrClear(screen, tigrRGB(0x80, 0x90, 0xa0));

        std::ostringstream oss;
        oss << "Frame: " << frameNum << " (FPS: " << keeper.GetFPS() << ") Delta time: " << keeper.GetFrameTime();
        frameNum++;

        tigrFillRect(screen, 120, 130, 20, 20, tigrRGB(0x00, 0x0f, 0x0f));
        tigrLine(screen, 10, 10, 200, 200, tigrRGB(0x00, 0x0f, 0x0f));
        
        FrameMarkStart("Filling with pixels");
        for(int y = 0; y < min(frameNum, screen->h); y++)
        {
            for(int x = 0; x < min(frameNum, screen->w); x++)
            {
                screen->pix[x + y*screen->w] = tigrRGB(0xff, 0x00, 0x00);
            }
        }
        FrameMarkEnd("Filling with pixels");

        //for(int i = 0; i < frameNum; i++)
        //{
        //    screen->pix[i] = tigrRGB(0xff, 0x00, 0x00);
        //}
        if(frameNum >= screen->w)
            frameNum = 0;

        
        //tigrPrint(screen, cfont_14, 120, 130, tigrRGB(0x00, 0xff, 0xff), "a");
        //tigrPrint(screen, cfont_14, 132, 130, tigrRGB(0x00, 0xff, 0xff), "b");
        //tigrPrint(screen, cfont_14, 144, 130, tigrRGB(0x00, 0xff, 0xff), "c");


        tigrPrint(screen, cfont_10, 120, 110, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        /*tigrPrint(screen, cfont_14, 120, 130, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrPrint(screen, cfont_16, 120, 160, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrPrint(screen, cfont_20, 120, 190, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrPrint(screen, cfont_24, 120, 220, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrPrint(screen, cfont_30, 120, 260, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());*/
        tigrUpdate(screen);

        FrameMark;
    }
    aocReleaseFonts();
    tigrFree(screen);

}



// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
