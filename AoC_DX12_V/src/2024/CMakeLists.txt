include(${CMAKE_CURRENT_SOURCE_DIR}/../../../common.cmake)

set(sources
    stdafx.cpp
    stdafx.h
    Day_01.cpp
    Day_01.h
    )

set(CMAKE_CXX_STANDARD "20")

source_group("Sources" FILES ${sources})

add_library(2024 STATIC ${sources} ${sources})

target_precompile_headers(2024 PRIVATE stdafx.h)

add_dependencies(2024 copied_common_config)
target_link_libraries(2024 TracyClient AoC_Common)
target_include_directories (2024 PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/tracy/public/tracy)
target_include_directories (2024 PUBLIC ${CMAKE_HOME_DIRECTORY}/AoC_DX12_V/src/Common)
target_include_directories (2024 PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/hlsl++)