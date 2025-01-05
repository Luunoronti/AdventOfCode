include(${CMAKE_CURRENT_SOURCE_DIR}/../../../common.cmake)

set(src_g
    "stdafx.cpp"
    "stdafx.h"
    )

set(src15
    "Day_01.cpp"
    "Day_01.h"
    )

set(CMAKE_CXX_STANDARD "20")

add_library(2024 STATIC ${src_g} ${src15})
source_group("Days 1-5" FILES ${src15})
source_group("Source Files" FILES ${src_g})

target_precompile_headers(2024 PRIVATE stdafx.h)

add_dependencies(2024 copied_common_config)
target_link_libraries(2024 TracyClient AoC_Common)
target_include_directories (2024 PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/tracy/public/tracy)
target_include_directories (2024 PUBLIC ${CMAKE_HOME_DIRECTORY}/AoC_DX12_V/src/Common)
target_include_directories (2024 PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/hlsl++)