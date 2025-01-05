include(${CMAKE_CURRENT_SOURCE_DIR}/../../../common.cmake)

set(src_g
    "stdafx.cpp"
    "stdafx.h"
    )

set(src_1-5
    "Days 1-5//Day 01.cpp"
    "Days 1-5//Day 01.h"
    "Days 1-5//Day 02.cpp"
    "Days 1-5//Day 02.h"
    "Days 1-5//Day 03.cpp"
    "Days 1-5//Day 03.h"
    "Days 1-5//Day 04.cpp"
    "Days 1-5//Day 04.h"
    "Days 1-5//Day 05.cpp"
    "Days 1-5//Day 05.h"
    )
set(src_6-10
    "Days 6-10//Day 06.cpp"
    "Days 6-10//Day 06.h"
    "Days 6-10//Day 07.cpp"
    "Days 6-10//Day 07.h"
    "Days 6-10//Day 08.cpp"
    "Days 6-10//Day 08.h"
    "Days 6-10//Day 09.cpp"
    "Days 6-10//Day 09.h"
    "Days 6-10//Day 10.cpp"
    "Days 6-10//Day 10.h"
    )
set(src_11-15
    "Days 11-15//Day 11.cpp"
    "Days 11-15//Day 11.h"
    "Days 11-15//Day 12.cpp"
    "Days 11-15//Day 12.h"
    "Days 11-15//Day 13.cpp"
    "Days 11-15//Day 13.h"
    "Days 11-15//Day 14.cpp"
    "Days 11-15//Day 14.h"
    "Days 11-15//Day 15.cpp"
    "Days 11-15//Day 15.h"
    )
set(src_16-20
    "Days 16-20//Day 16.cpp"
    "Days 16-20//Day 16.h"
    "Days 16-20//Day 17.cpp"
    "Days 16-20//Day 17.h"
    "Days 16-20//Day 18.cpp"
    "Days 16-20//Day 18.h"
    "Days 16-20//Day 19.cpp"
    "Days 16-20//Day 19.h"
    "Days 16-20//Day 20.cpp"
    "Days 16-20//Day 20.h"
    )
set(src_21-25
    "Days 21-25//Day 21.cpp"
    "Days 21-25//Day 21.h"
    "Days 21-25//Day 22.cpp"
    "Days 21-25//Day 22.h"
    "Days 21-25//Day 23.cpp"
    "Days 21-25//Day 23.h"
    "Days 21-25//Day 24.cpp"
    "Days 21-25//Day 24.h"
    "Days 21-25//Day 25.cpp"
    "Days 21-25//Day 25.h"
    )



add_library(2024 STATIC ${src_g} ${src_1-5} ${src_6-10} ${src_11-15} ${src_16-20} ${src_21-25})
set_property(TARGET 2024 PROPERTY CXX_STANDARD 23)

source_group("Days 1-5" FILES ${src_1-5})
source_group("Days 6-10" FILES ${src_6-10})
source_group("Days 11-15" FILES ${src_11-15})
source_group("Days 16-20" FILES ${src_16-20})
source_group("Days 21-25" FILES ${src_21-25})


target_precompile_headers(2024 PRIVATE stdafx.h)

add_dependencies(2024 copied_common_config)
target_link_libraries(2024 TracyClient AoC_Common)
target_include_directories (2024 PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/tracy/public/tracy)
target_include_directories (2024 PUBLIC ${CMAKE_HOME_DIRECTORY}/AoC_DX12_V/src/Common)
target_include_directories (2024 PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/hlsl++)
