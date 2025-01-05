include(${CMAKE_CURRENT_SOURCE_DIR}/../../../common.cmake)

set(sources
    stdafx.cpp
    stdafx.h
    Day_01.cpp
    Day_01.h
    )


source_group("Sources" FILES ${sources})

add_library(2024 STATIC ${sources} ${sources})

target_precompile_headers(2024 PRIVATE stdafx.h)

add_dependencies(2024 copied_common_config)
target_link_libraries(2024 AoC_Common)