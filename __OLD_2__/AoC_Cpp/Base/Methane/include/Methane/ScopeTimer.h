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

FILE: Methane/ScopeTimer.h
Code scope measurement timer with aggregating and averaging of timings.

******************************************************************************/

#pragma once

#include "ILogger.h"

#include <Methane/IttApiHelper.h>
#include <Methane/Timer.hpp>
#include <Methane/Memory.hpp>

#include <string>
#include <map>

namespace Methane
{

class ScopeTimer : public Timer // NOSONAR - custom destructor is required
{
public:
    using ScopeId = uint32_t;

    struct Registration
    {
        const char* name;
        ScopeId     id;
    };

    class Aggregator // NOSONAR - custom destructor is required
    {
        friend class ScopeTimer;

    public:
        struct Timing
        {
            TimeDuration duration;
            uint32_t     count    = 0U;
        };

        [[nodiscard]] static Aggregator& Get() noexcept;

        Aggregator(const Aggregator&) = delete;
        Aggregator(Aggregator&&) = delete;
        ~Aggregator();

        Aggregator& operator=(const Aggregator&) = delete;
        Aggregator& operator=(Aggregator&&) = delete;

        void SetLogger(Ptr<ILogger> logger_ptr) noexcept             { m_logger_ptr = std::move(logger_ptr); }
        [[nodiscard]] const Ptr<ILogger>& GetLogger() const noexcept { return m_logger_ptr; }

        void LogTimings(ILogger& logger) noexcept;
        void Flush() noexcept;

    protected:
        Registration RegisterScope(const char* scope_name);
        void AddScopeTiming(const Registration& scope_registration, TimeDuration duration) noexcept;

    private:
        Aggregator() = default;

        using ScopeIdByName = std::map<const char*, ScopeId>;
        using ScopeTimings  = std::vector<Timing>; // index == ScopeId
        using ScopeCounters = std::vector<ITT_COUNTER_TYPE(uint64_t)>; // index == ScopeId

        ScopeId       m_new_scope_id = 0U;
        ScopeIdByName m_scope_id_by_name;
        ScopeTimings  m_timing_by_scope_id;
        ScopeCounters m_counters_by_scope_id;
        Ptr<ILogger>  m_logger_ptr;
    };

    template<typename TLogger>
    static void InitializeLogger()
    {
        Aggregator::Get().SetLogger(std::make_shared<TLogger>());
    }

    explicit ScopeTimer(const char* scope_name);
    ScopeTimer(const ScopeTimer&) = delete;
    ScopeTimer(ScopeTimer&&) = delete;
    ~ScopeTimer();

    ScopeTimer& operator=(const ScopeTimer&) = delete;
    ScopeTimer& operator=(ScopeTimer&&) = delete;

    [[nodiscard]] const Registration& GetRegistration() const { return m_registration; }
    [[nodiscard]] const char*         GetScopeName() const    { return m_registration.name; }
    [[nodiscard]] ScopeId             GetScopeId() const      { return m_registration.id; }

private:
    using Timer::Reset;
    using Timer::ResetToSeconds;

    const Registration m_registration;
};

} // namespace Methane

#ifdef METHANE_SCOPE_TIMERS_ENABLED

#define META_SCOPE_TIMERS_INITIALIZE(LOGGER_TYPE) Methane::ScopeTimer::InitializeLogger<LOGGER_TYPE>()
#define META_SCOPE_TIMER(SCOPE_NAME) Methane::ScopeTimer scope_timer(SCOPE_NAME)
#define META_FUNCTION_TIMER() META_SCOPE_TIMER(__func__)
#define META_SCOPE_TIMERS_FLUSH() Methane::ScopeTimer::Aggregator::Get().Flush()

#else // ifdef METHANE_SCOPE_TIMERS_ENABLED

#define META_SCOPE_TIMERS_INITIALIZE(LOGGER_TYPE)
#define META_SCOPE_TIMER(SCOPE_NAME)
#define META_FUNCTION_TIMER()
#define META_SCOPE_TIMERS_FLUSH()

#endif // ifdef METHANE_SCOPE_TIMERS_ENABLED
