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

add_executable(AoC_VK WIN32 ${sources} ${icon_src})

target_link_libraries(AoC_VK LINK_PUBLIC AoC_Common TracyClient Cauldron_VK ImGUI Vulkan::Vulkan)

set_target_properties(AoC_VK PROPERTIES VS_DEBUGGER_WORKING_DIRECTORY "${CMAKE_HOME_DIRECTORY}/bin" DEBUG_POSTFIX "d")
target_include_directories (AoC_VK PUBLIC ${CMAKE_HOME_DIRECTORY}/Tools/tracy/public/tracy)