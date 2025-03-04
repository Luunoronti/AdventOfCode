/******************************************************************************

Copyright 2022 Evgeny Gorodetskiy

Licensed under the Apache License, Version 2.0 (the "License"),
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

*******************************************************************************

FILE: Methane/Graphics/RHI/Implementations.h
Methane graphics RHI PIMPL implementations.

******************************************************************************/

#pragma once

#include "Device.h"
#include "System.h"
#include "CommandListSet.h"
#include "CommandQueue.h"
#include "CommandListDebugGroup.h"
#include "CommandKit.h"
#include "Fence.h"
#include "Shader.h"
#include "Program.h"
#include "ProgramBindings.h"
#include "RenderPattern.h"
#include "RenderPass.h"
#include "RenderContext.h"
#include "RenderState.h"
#include "ComputeContext.h"
#include "ComputeState.h"
#include "ViewState.h"
#include "Buffer.h"
#include "BufferSet.h"
#include "Texture.h"
#include "Sampler.h"
#include "ResourceBarriers.h"
#include "RenderCommandList.h"
#include "ParallelRenderCommandList.h"
#include "TransferCommandList.h"
#include "ComputeCommandList.h"
