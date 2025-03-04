/******************************************************************************

Copyright 2020 Evgeny Gorodetskiy

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

FILE: Methane/Platform/Logger.h
Logger implementation

******************************************************************************/

#pragma once

#include "Utils.h"

#include <Methane/ILogger.h>

namespace Methane::Platform
{

class Logger : public ILogger
{
    // Data::ILogger interface
    void Log(std::string_view message) override
    {
        PrintToDebugOutput(message);
    }
};

} // namespace Methane::Data
