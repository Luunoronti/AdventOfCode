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

FILE: Methane/Graphics/TypeConverters.hpp
Graphics type converter template functions.

******************************************************************************/

#pragma once

#include "Types.h"

namespace Methane::Graphics
{

template<typename TIndex>
[[nodiscard]] PixelFormat GetIndexFormat(TIndex) noexcept          { return PixelFormat::Unknown; }

template<>
[[nodiscard]] inline PixelFormat GetIndexFormat(uint32_t) noexcept { return PixelFormat::R32Uint; }

template<>
[[nodiscard]] inline PixelFormat GetIndexFormat(int32_t) noexcept  { return PixelFormat::R32Sint; }

template<>
[[nodiscard]] inline PixelFormat GetIndexFormat(uint16_t) noexcept { return PixelFormat::R16Uint; }

template<>
[[nodiscard]] inline PixelFormat GetIndexFormat(int16_t) noexcept  { return PixelFormat::R16Sint; }

} // namespace Methane::Graphics