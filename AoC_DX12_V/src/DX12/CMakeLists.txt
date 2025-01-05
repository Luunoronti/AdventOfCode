set(sources
    GLTFSample.cpp
    GLTFSample.h
    Renderer.cpp
    Renderer.h
    UI.cpp
    UI.h
    stdafx.cpp
    stdafx.h
    dpiawarescaling.manifest)

source_group("Sources" FILES ${sources})
source_group("Icon"    FILES ${icon_src}) # defined in top-level CMakeLists.txt

add_executable(AoC_DX12 ${sources} ${common} ${icon_src})
target_link_libraries(AoC_DX12 LINK_PUBLIC AoC_Common 2024 Cauldron_DX12 ImGUI amd_ags d3dcompiler D3D12)
set_target_properties(AoC_DX12 PROPERTIES VS_DEBUGGER_WORKING_DIRECTORY "${CMAKE_HOME_DIRECTORY}/bin" DEBUG_POSTFIX "d" LINK_FLAGS "/SUBSYSTEM:CONSOLE")
