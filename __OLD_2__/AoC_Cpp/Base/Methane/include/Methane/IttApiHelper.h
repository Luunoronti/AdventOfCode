﻿/******************************************************************************

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

FILE: Methane/IttApiHelper.h
Helper macro-definitions for ITT instrumentation

******************************************************************************/

#pragma once

#ifdef ITT_INSTRUMENTATION_ENABLED

#include <stdint.h>
#include <string>
#include <type_traits>
#include <thread>

#define INTEL_ITTNOTIFY_API_PRIVATE
#include <ittnotify.h>

#include <stdint.h>

#if ITT_PLATFORM==ITT_PLATFORM_WIN
#include <nowide/convert.hpp>
#endif

#ifdef _WIN32
#define UNICODE_AGNOSTIC(name) name##A
#else
#define UNICODE_AGNOSTIC(name) name
#endif

namespace Methane::ITT
{

class Event
{
public:
    Event(const __itt_domain* domain_ptr, __itt_string_handle* name_ptr) noexcept
    : m_id(__itt_id_make(const_cast<__itt_domain*>(domain_ptr), reinterpret_cast<unsigned long long>(name_ptr))) // NOSONAR
        , m_domain_ptr(domain_ptr)
    { }

    template<class T>
    typename std::enable_if<std::is_floating_point_v<T>, void>::type
    AddArg(__itt_string_handle* name_ptr, const T& value) const noexcept
    {
        double double_value = static_cast<double>(value);
        __itt_metadata_add(m_domain_ptr, m_id, name_ptr, __itt_metadata_double, 1, &double_value);
    }

    void AddArg(__itt_string_handle* name_ptr, int64_t value) const noexcept
    {
        __itt_metadata_add(m_domain_ptr, m_id, name_ptr, __itt_metadata_s64, 1, &value);
    }

    void AddArg(__itt_string_handle* name_ptr, const char* value) const noexcept
    {
#if ITT_PLATFORM==ITT_PLATFORM_WIN && (defined(UNICODE) || defined(_UNICODE))
        // string value must be converted to wchar_t
        __itt_metadata_str_add(m_domain_ptr, m_id, name_ptr, nowide::widen(value).c_str(), 0);
#else
        __itt_metadata_str_add(m_domain_ptr, m_id, name_ptr, value, 0);
#endif
    }

    void AddArg(__itt_string_handle* name_ptr, void const* const value_ptr) const noexcept
    {
        __itt_metadata_add(m_domain_ptr, m_id, name_ptr, __itt_metadata_unknown, 1, const_cast<void*>(value_ptr));
    }

    void AddArg(__itt_string_handle* name_ptr, const std::string& value) const noexcept
    {
        AddArg(name_ptr, value.c_str());
    }

protected:
    __itt_id            m_id = __itt_null;
    const __itt_domain* m_domain_ptr;
};

class Marker : public Event
{
public:
    enum class Scope : int
    {
        Global  = __itt_scope_global,
        Process = __itt_scope_track_group,
        Thread  = __itt_scope_track,
        Task    = __itt_scope_task, //means a task that will long until another marker with task scope in this thread occurs
    };

    Marker(const __itt_domain* domain_ptr, const char* name_ptr, Scope scope) noexcept
        : Marker(domain_ptr, UNICODE_AGNOSTIC(__itt_string_handle_create)(name_ptr), scope)
    { }

    void Notify() const noexcept
    {
        __itt_marker(m_domain_ptr, m_id, m_name_ptr, m_scope);
    }

private:
    Marker(const __itt_domain* domain_ptr, __itt_string_handle* itt_name_ptr, Scope scope) noexcept
        : Event(domain_ptr, itt_name_ptr)
        , m_name_ptr(itt_name_ptr)
        , m_scope(static_cast<__itt_scope>(scope))
    { }

    __itt_string_handle* m_name_ptr;
    __itt_scope          m_scope;
};

template<bool bRegion = true>
class Task : public Event
{
public:
    Task(const __itt_domain* domain_ptr, __itt_string_handle* name_ptr) noexcept
        : Event(domain_ptr, name_ptr)
    {
        if (bRegion)
        {
            __itt_region_begin(m_domain_ptr, m_id, __itt_null, name_ptr);
        }
        else
        {
            __itt_task_begin(m_domain_ptr, m_id, __itt_null, name_ptr);
        }
    }

    ~Task()
    {
        if (bRegion)
        {
            __itt_region_end(m_domain_ptr, m_id);
        }
        else
        {
            __itt_task_end(m_domain_ptr);
        }
    }
};

template<typename Type> inline __itt_metadata_type GetMetadataType(Type)     noexcept { return __itt_metadata_unknown; }
template<>              inline __itt_metadata_type GetMetadataType(double)   noexcept { return __itt_metadata_double; }
template<>              inline __itt_metadata_type GetMetadataType(float)    noexcept { return __itt_metadata_float; }
template<>              inline __itt_metadata_type GetMetadataType(int16_t)  noexcept { return __itt_metadata_s16; }
template<>              inline __itt_metadata_type GetMetadataType(uint16_t) noexcept { return __itt_metadata_u16; }
template<>              inline __itt_metadata_type GetMetadataType(int32_t)  noexcept { return __itt_metadata_s32; }
template<>              inline __itt_metadata_type GetMetadataType(uint32_t) noexcept { return __itt_metadata_u32; }
template<>              inline __itt_metadata_type GetMetadataType(int64_t)  noexcept { return __itt_metadata_s64; }
template<>              inline __itt_metadata_type GetMetadataType(uint64_t) noexcept { return __itt_metadata_u64; }

template<typename ValueType>
class Counter
{
public:
    Counter(const char* name_ptr, const char* domain_ptr) noexcept
        : m_id(UNICODE_AGNOSTIC(__itt_counter_create_typed)(name_ptr, domain_ptr, GetMetadataType(ValueType{})))
    { }
    Counter(Counter&& other) : m_id(std::move(other.m_id)) {}
    ~Counter() { __itt_counter_destroy(m_id); }

    void SetValue(ValueType value) const noexcept     { __itt_counter_set_value(m_id, &value); }
    void IncrementDelta(uint64_t delta) const noexcept{ __itt_counter_inc_delta(m_id, delta); }
    void DecrementDelta(uint64_t delta) const noexcept{ __itt_counter_dec_delta(m_id, delta); }
    void Increment() const noexcept                   { __itt_counter_inc(m_id); }
    void Decrement() const noexcept                   { __itt_counter_inc(m_id); }

private:
    __itt_counter m_id;
};

#define ITT_DOMAIN_LOCAL(/*const char* */domain) \
    static const __itt_domain* __itt_domain_instance = UNICODE_AGNOSTIC(__itt_domain_create)(domain)

#define ITT_DOMAIN_GLOBAL(/*const char* */domain) \
    const __itt_domain* GetIttDomainInstance() \
    { \
        ITT_DOMAIN_LOCAL(domain); \
        return __itt_domain_instance; \
    }

#define ITT_DOMAIN_EXTERN()\
    const __itt_domain* GetIttDomainInstance();

#define ITT_SCOPE(region, name)\
    static __itt_string_handle* __itt_scope_name = UNICODE_AGNOSTIC(__itt_string_handle_create)(name);\
    Methane::ITT::Task<region> __itt_scope_item(GetIttDomainInstance(), __itt_scope_name)

#define ITT_SCOPE_TASK(/*const char* */name) ITT_SCOPE(false, name)
#define ITT_SCOPE_REGION(/*const char* */name) ITT_SCOPE(true, name)

#define ITT_MARKER(/*Methane::ITT::Marker::Scope*/scope, /*const char* */name) \
    static const Methane::ITT::Marker __itt_marker_item(GetIttDomainInstance(), name, scope); \
    __itt_marker_item.Notify()

#define ITT_ARG(item_variable, /*const char* */name, /*number or string*/ value) { \
    static __itt_string_handle* __itt_arg_name = UNICODE_AGNOSTIC(__itt_string_handle_create)(name); \
    item_variable.AddArg(__itt_arg_name, value); \
}

#ifdef ITT_ARGUMENTS_METADATA_ENABLED

#define ITT_MARKER_ARG(/*const char* */name, /*number or string*/ value) \
    ITT_ARG(__itt_marker_item, name, value)

#define ITT_FUNCTION_ARG(/*const char* */name, /*number or string*/ value) \
    ITT_ARG(__itt_scope_item, name, value)

#define ITT_FUNCTION_TASK() \
    ITT_SCOPE_TASK(__FUNCTION__); \
    ITT_FUNCTION_ARG("__file__", __FILE__); \
    ITT_FUNCTION_ARG("__line__", __LINE__)

#else

#define ITT_MARKER_ARG(/*const char* */name, /*number or string*/ value)
#define ITT_FUNCTION_ARG(/*const char* */name, /*number or string*/ value)
#define ITT_FUNCTION_TASK() \
    ITT_SCOPE_TASK(__FUNCTION__)

#endif

#define ITT_GLOBAL_MARKER(/*const char* */name) ITT_MARKER(Methane::ITT::Marker::Scope::Global, name)
#define ITT_PROCESS_MARKER(/*const char* */name) ITT_MARKER(Methane::ITT::Marker::Scope::Process, name)
#define ITT_THREAD_MARKER(/*const char* */name) ITT_MARKER(Methane::ITT::Marker::Scope::Thread, name)
#define ITT_TASK_MARKER(/*const char* */name) ITT_MARKER(Methane::ITT::Marker::Scope::Task, name)

#define ITT_FUNCTION_MARKER(/*Methane::ITT::Marker::Scope*/scope) ITT_MARKER(scope, __FUNCTION__);
#define ITT_FUNCTION_GLOBAL_MARKER() ITT_FUNCTION_MARKER(Methane::ITT::Marker::Scope::Global)
#define ITT_FUNCTION_PROCESS_MARKER() ITT_FUNCTION_MARKER(Methane::ITT::Marker::Scope::Process)
#define ITT_FUNCTION_THREAD_MARKER() ITT_FUNCTION_MARKER(Methane::ITT::Marker::Scope::Thread)
#define ITT_FUNCTION_TASK_MARKER() ITT_FUNCTION_MARKER(Methane::ITT::Marker::Scope::Task)

#define ITT_COUNTER_TYPE(value_type) Methane::ITT::Counter<value_type>
#define ITT_COUNTER_INIT(/*const char* */name, /*const char* */domain) name, domain
#define ITT_COUNTER_VALUE(counter_var, value) counter_var.SetValue(value)

class ScopeTrack
{
public:
    ScopeTrack(__itt_track* track)
    {
        __itt_set_track(track);
    }
    ~ScopeTrack()
    {
        __itt_set_track(nullptr);
    }
};

//'group' defines virtual process (null means current process), track defines virtual thread
#define ITT_SCOPE_TRACK(/*const char* */group, /*const char* */ track)\
    static __itt_track* itt_track_name = __itt_track_create(__itt_track_group_create(((group) ? UNICODE_AGNOSTIC(__itt_string_handle_create)(group) : nullptr), __itt_track_group_type_normal), UNICODE_AGNOSTIC(__itt_string_handle_create)(track), __itt_track_type_normal);\
    Methane::ITT::ScopeTrack itt_track(itt_track_name);

#define ITT_THREAD_NAME(/*const char* */name) \
    UNICODE_AGNOSTIC(__itt_thread_set_name)(name)

} // namespace Methane::ITT

#else

#define ITT_DOMAIN_LOCAL(domain)
#define ITT_DOMAIN_GLOBAL(domain)
#define ITT_DOMAIN_EXTERN()
#define ITT_SCOPE(region, name)
#define ITT_SCOPE_TASK(name)
#define ITT_SCOPE_REGION(name)
#define ITT_FUNCTION_TASK()
#define ITT_FUNCTION_ARG(name, value)
#define ITT_MARKER(scope, name)
#define ITT_MARKER_ARG(name, value)
#define ITT_GLOBAL_MARKER(name)
#define ITT_PROCESS_MARKER(name)
#define ITT_THREAD_MARKER(name)
#define ITT_FUNCTION_MARKER(scope)
#define ITT_FUNCTION_GLOBAL_MARKER()
#define ITT_FUNCTION_PROCESS_MARKER()
#define ITT_FUNCTION_THREAD_MARKER()
#define ITT_FUNCTION_TASK_MARKER()
#define ITT_TASK_MARKER(name)
#define ITT_COUNTER_TYPE(value_type) void*
#define ITT_COUNTER_INIT(/*const char* */name, /*const char* */domain) nullptr
#define ITT_COUNTER_VALUE(counter_var, value)
#define ITT_SCOPE_TRACK(group, track)
#define ITT_THREAD_NAME(/*const char* */name)

#endif
