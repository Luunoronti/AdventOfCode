include(${CMAKE_CURRENT_SOURCE_DIR}/../../../common.cmake)

set(sources
    stdafx.cpp
    stdafx.h
    pipes.h
    aoc.h
    
    )

source_group("Sources" FILES ${sources})
add_library(AoC_Common STATIC ${sources})

set(config
    ${CMAKE_CURRENT_SOURCE_DIR}/../Common/AoC_Common.json
)

copyTargetCommand("${config}" ${CMAKE_RUNTIME_OUTPUT_DIRECTORY} copied_common_config)

target_link_libraries(AoC_Common LINK_PUBLIC TracyClient Cauldron_DX12 ImGUI amd_ags d3dcompiler D3D12)
target_include_directories (AoC_Common PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/tracy/public/tracy)
target_include_directories (AoC_Common PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/hlsl++)

