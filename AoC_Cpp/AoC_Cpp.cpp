// AoC_Cpp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include "pch.h"
#include <TimeKeeper.h>

//#include <Methane/Kit.h>

//#include <Methane/Data/AppShadersProvider.h>
//#include <Methane/Data/Math.hpp>
//#include <Methane/Instrumentation.h>


const char fxShader[] =
"void fxShader(out vec4 color, in vec2 uv) {"
"   vec2 tex_size = vec2(textureSize(image, 0));"
"   vec4 c = texture(image, (floor(uv * tex_size) + 0.5 * sin(parameters.x)) / tex_size);"
"   color = c * vec4(0.2f, 0.6f, 1, 1);"
"}";

#include <random>

int main()
{
    // pipe operation tests
    std::string text = "This is line 1\nThis is line 2\nThis is line 3\nThis is line 4";
    auto lines = text | split('\n');

    // given this struct:
    std::vector<std::vector<int>> data = { {1, 2, 3, 4}, { 5, 6, 7, 8, 9, 10} };
    // we would like to have a rotate_cw and rotate_ccw pipe operators

    // first real API: swap_axis
    //data = data | swap_axis | swap_axis;
    int64_t _sum1 = (data | swap_axis)[0] | mul2 | sum;


    std::cout << "Hello World!\n";
    std::cout << _sum1;

    TimeKeeper keeper;
    int frameNum = 0;
    Tigr* screen = tigrWindow(1920, 1080, "Hello", 0);
    aocLoadFonts();
    tigrBeginOpenGL(screen);
    //tigrSetPostShader(screen, fxShader, sizeof(fxShader) - 1);
    float duration = 1;
    double phase = 0;

    float3 test(1, 0, 2);


    while(!tigrClosed(screen))
    {
        keeper.AddFrameTime(tigrTime());
        phase += keeper.GetFrameTime();

        while(phase > duration)
        {
            phase -= duration;
        }
        double p = 6.28f * phase / duration;
        tigrSetPostFX(screen, 8, 8, 1, 1);

        tigrClear(screen, tigrRGB(0x80, 0x90, 0xa0));

        std::ostringstream oss;
        oss << "Frame: " << frameNum << " (FPS: " << keeper.GetFPS() << ") Delta time: " << keeper.GetFrameTime();
        frameNum++;

        tigrFillRect(screen, 120, 130, 20, 20, tigrRGB(0x00, 0x0f, 0x0f));
        tigrLine(screen, 10, 10, 200, 200, tigrRGB(0x00, 0x0f, 0x0f));

        FrameMarkStart("Filling with pixels");
        for(int y = 0; y < std::min(frameNum, screen->h); y++)
        {
            for(int x = 0; x < std::min(frameNum, screen->w); x++)
            {
                screen->pix[x + y * screen->w] = tigrRGB(0xff, 0x00, 0x00);
            }
        }
        FrameMarkEnd("Filling with pixels");


        if(frameNum >= screen->w)
            frameNum = 0;

        tigrPrint(screen, cfont_10, 120, 110, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrPrint(screen, cfont_14, 120, 130, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrPrint(screen, cfont_16, 120, 160, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrPrint(screen, cfont_20, 120, 190, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrPrint(screen, cfont_24, 120, 220, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrPrint(screen, cfont_30, 120, 260, tigrRGB(0x00, 0xff, 0xff), oss.str().c_str());
        tigrUpdate(screen);

        FrameMark;

        const int width = 256;
        const int height = 256;
        std::vector<uint32_t> image(width * height);
        std::random_device rd;
        std::mt19937 gen(rd());
        std::uniform_int_distribution<uint32_t> dis(0, 0xFFFFFFFF);
        for(int y = 0; y < height; ++y)
        {
            for(int x = 0; x < width; ++x)
            {
                // image is in ABGR format!!
                image[x + y * width] = 0xff00 << 16 | y << 8 | x;
            }
        }
        TracyMessageLC("Frame end", 0xff2030);
        FrameImage(&image[0], width, height, 0, false);
    }
    aocReleaseFonts();
    tigrFree(screen);

}

