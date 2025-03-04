/******************************************************************************

Copyright 2019-2023 Evgeny Gorodetskiy

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

FILE: Methane/Platform/AppView.h
Methane application view used both by IRenderContext in RHI
and by Methane App implementations.

******************************************************************************/

#pragma once

#ifdef __OBJC__

#ifdef APPLE_MACOS
#import "MacOS/AppViewMetal.hh"
#else
#import "iOS/AppViewMetal.hh"
#endif

#else // __OBJC__

#include <stdint.h>

#endif // __OBJC__

#include <stdexcept>

namespace Methane::Platform
{

#ifdef __OBJC__

using NativeAppView = AppViewMetal;
using NativeAppViewPtr = NativeAppView* _Nonnull;

#else // __OBJC__

using NativeAppView = uint8_t;
using NativeAppViewPtr = NativeAppView*;

#endif // __OBJC__

struct AppView
{
    NativeAppViewPtr native_view_ptr;
};

class AppViewResizeRequiredError
    : public std::runtime_error
{
public:
    AppViewResizeRequiredError();
};

} // namespace Methane::Platform
