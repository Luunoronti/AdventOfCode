@echo off
setlocal enabledelayedexpansion

echo Checking pre-requisites... 

:: Check if CMake is installed
cmake --version > nul 2>&1
if %errorlevel% NEQ 0 (
    echo Cannot find path to cmake. Is CMake installed? Exiting...
    exit /b -1
) else (
    echo    CMake      - Ready.
) 

:: Check if submodule is initialized (first time) to avoid CMake file not found errors
if not exist AoC_DX12_V\libs\cauldron\common.cmake (
    echo File: common.cmake  doesn't exist in 'AoC_DX12_V\libs\cauldron\'  -  Initializing submodule... 

    :: attempt to initialize submodule
    cd ..
    echo.
    git submodule sync --recursive
    git submodule update --init --recursive
    cd build 


    :: check if submodule initialized properly
    if not exist AoC_DX12_V\libs\cauldron\common.cmake (
        echo.
        echo 'AoC_DX12_V\libs\cauldron\common.cmake is still not there.'
        echo Could not initialize submodule. Make sure all the submodules are initialized and updated.
        echo Exiting...
        echo.
        exit /b -1 
    ) else (
        echo    Cauldron   - Ready.
    )
) else (
    echo    Cauldron   - Ready.
)

:: Check if VULKAN_SDK is installed but don't bail out
if "%VULKAN_SDK%"=="" (
    echo Vulkan SDK is not installed -Environment variable VULKAN_SDK is not defined- : Please install the latest Vulkan SDK from LunarG.
) else (
    echo    Vulkan SDK - Ready : %VULKAN_SDK%
)

:: Call CMake
cd AoC_DX12_V
cd build
mkdir DX12
mkdir VK
cd ..
cd ..

cmake -Wno-dev -A x64 .\
:: cmake -Wno-dev -A x64 .\ -DGFX_API=VK
