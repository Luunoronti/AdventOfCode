#pragma once
#include <unordered_map> 
#include <vector> 

namespace aoc
{
    template<typename T>
    struct Location
    {
        T x;
        T y;
        Location(T x, T y)
            : x(x), y(y)
        {
        }
    };

    namespace maps
    {
        template<typename T1, typename T2>
        struct sparse_map
        {
            int Width{ 0 };
            int Height{ 0 };
            std::unordered_map<T1, std::vector<T2>> Map;
        };

        template<typename T>
        class Map2d
        {
        public:
            int Width{ 0 };
            int Height{ 0 };
            std::vector<T> Map;

            Map2d()
                : Width(0), Height(0)
            {
            }

            Map2d(int Width, int Height)
                : Width(Width), Height(Height)
            {
                if(Width > 0 && Height > 0)
                    Map.resize(Width * Height);
            }

            __forceinline void Set(const int x, const int y, const T& value)
            {
                if(!WithinBounds(x, y))
                    return;
                Map[x + y * Width] = value;
            }
            __forceinline bool WithinBounds(const int x, const int y) const
            {
                return !(x < 0 || y < 0 || x >= Width || y >= Height);
            }
            __forceinline const T Get(const int x, const int y) const
            {
                if(!WithinBounds(x, y))
                    throw std::runtime_error("Coordinates are not within bounds.");
                return Map[x + y * Width];
            }
            __forceinline const T Get(const int x, const int y, const T& _default) const
            {
                if(!WithinBounds(x, y))
                    return _default;
                return Map[x + y * Width];
            }

            template <class _Ty>
            _Ty accumulate(_Ty _Val)
            {
                return std::accumulate(Map.begin(), Map.end(), _Val);
            }

            __forceinline void find(const T& _value, int& x, int &y) const
            {
                for(int _y = 0; _y < Height; ++_y)
                {
                    for(int _x = 0; _x < Width; ++_x)
                    {
                        if(Map[_x + _y * Width] == _value)
                        {
                            x = _x;
                            y = _y;
                            return;
                        }
                    }
                }
            }

            void print() const
            {
                for(int _y = 0; _y < Height; ++_y)
                {
                    for(int _x = 0; _x < Width; ++_x)
                    {
                        aoc::dout << Map[_x + _y * Width];
                    }
                    aoc::dout << std::endl;
                }
            }
        };
    }

    namespace numeric
    {
        template<typename T>
        class columns
        {
        public:
            template <class _Ty>
            _Ty accumulate(_Ty _Val)
            {
                T sum = T();
                for(const auto& vec : Map)
                {
                    sum = std::accumulate(vec.begin(), vec.end(), sum);
                }
                return sum;
            }

            template <class _Ty>
            _Ty accumulate(const int column, _Ty _Val)
            {
                if(column < 0 || column >= Map.size()) return _Val;
                return std::accumulate(Map[column].begin(), Map[column].end(), _Val);
            }

            size_t size()
            {
                return Map.size();
            }

            std::vector<std::vector<T>> Map;
        };

    }
}