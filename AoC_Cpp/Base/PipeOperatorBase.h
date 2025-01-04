#pragma once

#include <iostream>
#include <vector>
#include <unordered_map>
#include <string>
#include <ranges>
#include <utility> // for std::pair
#include <sstream> // for std::pair


#define REQUIRES(...) class=std::enable_if_t<(__VA_ARGS__)>


template<class T>
struct wrapper
{
    T value;
    template<class X, REQUIRES(std::is_convertible<T, X>())>
    wrapper(X&& x) : value(std::forward<X>(x)) {}
    T get() const { return std::move(value); }
};
template<class T>
auto make_wrapper(T&& x) { return wrapper<T>(std::forward<T>(x)); }

template<class F>
struct pipe_closure : F
{
    template<class... Xs>
    pipe_closure(Xs&&... xs) : F(std::forward<Xs>(xs)...) {}
};
template<class F>
auto make_pipe_closure(F f) { return pipe_closure<F>(std::move(f)); }


template<class F>
struct pipable
{
    template<class... Ts>
    auto operator()(Ts&&... xs) const
    {
        return make_pipe_closure([](auto... ws)
            {
                return [=](auto&& x) -> decltype(auto) { return F()(x, ws.get()...); };
            }(make_wrapper(std::forward<Ts>(xs)...)));
    }
};


template<class T, class F>
decltype(auto) operator|(T&& x, const pipe_closure<F>& p) { return p(std::forward<T>(x)); }
template<class T, class F>
decltype(auto) operator|(T&& x, const pipable<F>& p) { return F()(std::forward<T>(x)); }


// Custom enumerate view
//template <std::ranges::input_range R>
//auto enumerate(R&& range)
//{
//    return std::views::transform([](int x) { return x * 2; });
//}

// Define the custom range adaptor
inline constexpr auto square = std::views::transform([](int x) { return x * x; });
const constexpr auto mul2 = std::views::transform([](int x) { return x * 2; });

//inline constexpr auto split = std::views::transform([](std::string& s)
//    {
//        std::vector<std::string> result;
//        std::istringstream stream(s);
//        std::string line;
//        while(std::getline(stream, line, '\n'))
//        {
//            result.push_back(line);
//        }
//        return result;
//    });


struct split_f
{
    template<class T>
    auto operator()(T s, char splitChar) const
    {
        ZoneScopedN("split");
        std::vector<std::string> result;
        std::istringstream stream(s);
        std::string line;
        while(std::getline(stream, line, splitChar))
        {
            result.push_back(line);
        }
        return result;
    }
};
const constexpr pipable<split_f> split = {};

struct sum_f
{
    template<typename T>
    auto operator()(std::vector<T> s) const
    {
        int64_t sum = 0;
        for(const auto& t : s)
            sum += t;
        return sum;
    }

    template<typename T, typename T2>
    auto operator()(std::ranges::transform_view<T, T2> s) const
    {
        int64_t sum = 0;
        for(const auto& t : s)
            sum += t;
        return sum;
    }

    template<typename T1, typename T2>
    auto operator()(std::unordered_map<T1, T2> s, bool byValue = true) const
    {
        int64_t sum = 0;
        if(byValue)
        {
            for(const auto& [k, v] : s)
                sum += v;
        }
        else
        {
            for(const auto& [k, v] : s)
                sum += k;
        }
        return sum;
    }
};
const constexpr pipable<sum_f> sum = {};

struct swap_axis_f
{
    template<typename T>
    auto operator()(std::vector<std::vector<T>> s, bool ccw = false) const
    {
        // create a vector of vectors that is rotated cw
        // that is, columns become rows and vice versa
        std::vector<std::vector<T>> ret;
        size_t _s1 = 0;
        for(const auto& v : s) _s1 = std::max(v.size(), _s1);
        if(_s1 == 0) return ret; // empty stuff

        for(size_t i = 0; i < _s1; ++i) ret.push_back(std::vector<T>(s.size()));
        for(size_t i = 0; i < s.size(); ++i)
            for(int i2 = 0; i2 < _s1; ++i2)
                ret[i2][i] = s[i].size() > i2 ? s[i][i2] : T();
        return ret;
    }
};
const constexpr pipable<swap_axis_f> swap_axis = {};
//TODO: implement rotate_cw and rotate_ccw
 
//struct rotate_ccw_f
//{
//    template<typename T>
//    auto operator()(std::vector<std::vector<T>> s) const
//    {
//        // create a vector of vectors that is rotated ccw
//        // that is, columns become rows and vice versa
//        std::vector<std::vector<T>> ret;
//        size_t _s1 = 0;
//        for(const auto& v : s) _s1 = std::max(v.size(), _s1);
//        if(_s1 == 0) return ret; // empty stuff
//
//        for(size_t i = 0; i < _s1; ++i) ret.push_back(std::vector<T>(s.size()));
//        for(size_t i = 0; i < s.size(); ++i)
//            for(int i2 = 0; i2 < _s1; ++i2)
//                ret[i2][i] = s[i].size() > i2 ? s[i][i2] : T();
//        return ret;
//    }
//};
//const constexpr pipable<rotate_ccw_f> rotate_ccw = {};

