// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//
#pragma once

#define WIN32_LEAN_AND_MEAN             // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>
#include <windowsx.h>

// C RunTime Header Files
#include <malloc.h>
#include <map>
#include <mutex>
#include <vector>
#include <fstream>

#include "../../libs/cauldron/libs/d3d12x/d3dx12.h"

// Pull in math library
#include "../../libs/cauldron/libs/vectormath/vectormath.hpp"

// TODO: reference additional headers your program requires here
#include "../../libs/cauldron/src/DX12/base/Imgui.h"
#include "../../libs/cauldron/src/common/base/ImguiHelper.h"
#include "../../libs/cauldron/src/DX12/base/Fence.h"
#include "../../libs/cauldron/src/DX12/base/Helper.h"
#include "../../libs/cauldron/src/DX12/base/Device.h"
#include "../../libs/cauldron/src/DX12/base/Texture.h"
#include "../../libs/cauldron/src/DX12/base/FrameworkWindows.h"
#include "../../libs/cauldron/src/DX12/base/FreeSyncHDR.h"
#include "../../libs/cauldron/src/DX12/base/SwapChain.h"
#include "../../libs/cauldron/src/DX12/base/UploadHeap.h"
#include "../../libs/cauldron/src/DX12/base/SaveTexture.h"
#include "../../libs/cauldron/src/DX12/base/UserMarkers.h"
#include "../../libs/cauldron/src/DX12/base/GPUTimestamps.h"
#include "../../libs/cauldron/src/DX12/base/CommandListRing.h"
#include "../../libs/cauldron/src/DX12/base/StaticBufferPool.h"
#include "../../libs/cauldron/src/DX12/base/DynamicBufferRing.h"
#include "../../libs/cauldron/src/DX12/base/ResourceViewHeaps.h"
#include "../../libs/cauldron/src/common/base/ShaderCompilerCache.h"
#include "../../libs/cauldron/src/DX12/base/ShaderCompilerHelper.h"
#include "../../libs/cauldron/src/DX12/base/StaticConstantBufferPool.h"

#include "../../libs/cauldron/src/DX12/GLTF/GltfPbrPass.h"
#include "../../libs/cauldron/src/DX12/GLTF/GltfBBoxPass.h"
#include "../../libs/cauldron/src/DX12/GLTF/GltfDepthPass.h"
#include "../../libs/cauldron/src/DX12/GLTF/GltfMotionVectorsPass.h"

#include "../../libs/cauldron/src/Common/Misc/Misc.h"
#include "../../libs/cauldron/src/Common/Misc/Error.h"
#include "../../libs/cauldron/src/Common/Misc/Camera.h"

#include "../../libs/cauldron/src/DX12/PostProc/TAA.h"
#include "../../libs/cauldron/src/DX12/PostProc/Bloom.h"
#include "../../libs/cauldron/src/DX12/PostProc/BlurPS.h"
#include "../../libs/cauldron/src/DX12/PostProc/SkyDome.h"
#include "../../libs/cauldron/src/DX12/PostProc/SkyDomeProc.h"
#include "../../libs/cauldron/src/DX12/PostProc/PostProcCS.h"
#include "../../libs/cauldron/src/DX12/PostProc/ToneMapping.h"
#include "../../libs/cauldron/src/DX12/PostProc/ToneMappingCS.h"
#include "../../libs/cauldron/src/DX12/PostProc/ColorConversionPS.h"
#include "../../libs/cauldron/src/DX12/PostProc/DownSamplePS.h"
#include "../../libs/cauldron/src/DX12/PostProc/ShadowResolvePass.h"

#include "../../libs/cauldron/src/dx12/widgets/wireframe.h"


using namespace CAULDRON_DX12;
