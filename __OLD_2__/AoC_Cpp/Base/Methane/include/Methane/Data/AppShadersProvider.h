/******************************************************************************

Copyright 2019-2020 Evgeny Gorodetskiy

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

FILE: Methane/Graphics/AppShadersProvider.h
Application Shader resources provider
either stored in embedded application resources of on file system.

******************************************************************************/

#pragma once

#ifdef SHADER_RESOURCES_NAMESPACE

#ifdef RESOURCE_NAMESPACE
#undef RESOURCE_NAMESPACE //NOSONAR
#endif

#define RESOURCE_NAMESPACE SHADER_RESOURCES_NAMESPACE
#include "ResourceProvider.hpp"

namespace Methane::Data
{
using ShaderProvider = SHADER_RESOURCES_NAMESPACE::ResourceProvider;
}

#else // ifdef SHADER_RESOURCES_NAMESPACE

#include "FileProvider.hpp"

namespace Methane::Data
{
using ShaderProvider = FileProvider;
}

#endif // ifdef SHADER_RESOURCES_NAMESPACE