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

        Location<T> left()
        {
            return Location<T>(x - 1, y);
        }
        Location<T> right()
        {
            return Location<T>(x + 1, y);
        }
        Location<T> up()
        {
            return Location<T>(x, y - 1);
        }
        Location<T> down()
        {
            return Location<T>(x, y + 1);
        }


        Location<T> upleft()
        {
            return Location<T>(x - 1, y - 1);
        }
        Location<T> upright()
        {
            return Location<T>(x + 1, y - 1);
        }
        Location<T> downleft()
        {
            return Location<T>(x - 1, y + 1);
        }
        Location<T> downright()
        {
            return Location<T>(x + 1, y + 1);
        }

    };

    namespace stream
    {
        struct AoCStreamOmitCharacter
        {
            char characterToOmit;

            AoCStreamOmitCharacter(char characterToOmit)
                : characterToOmit(characterToOmit)
            {
            }
        };
        struct AoCStreamOmitCharacters
        {
            std::string charactersToOmit;

            AoCStreamOmitCharacters(std::string charactersToOmit)
                : charactersToOmit(charactersToOmit)
            {
            }
        };

        template <class _Ty>
        struct AoCStreamSelectedValue
        {
            _Ty _value;

            AoCStreamSelectedValue(_Ty value)
                : _value(value)
            {
            }
        };
    }


    namespace maps
    {
        enum Directions
        {
            None=0x00,
            Left = 0x01,
            Right = 0x02,
            Top = 0x10,
            Bottom = 0x20,
            Cardinal = 0x03,
            Diagonal = 0x30,
            All = 0xFF,
        };

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
            template<typename T>
            class __selected_value_stream
            {
                friend class Map2d;
            public:
                template <class _Ty>
                __selected_value_stream& operator>>(std::vector<Location<_Ty>>& locations)
                {
                    locations.clear();
                    if(map)
                    {
                        for(_Ty y = 0; y < map->Height; ++y)
                        {
                            for(_Ty x = 0; x < map->Height; ++x)
                            {
                                if(map->Get(x, y) == __value)
                                    locations.push_back(Location<_Ty>(x, y));
                            }
                        }
                    }

                    return *this;
                }
            private:
                __selected_value_stream() {}
                T __value{};
                Map2d* map;
            };

            template<typename T, typename _Ty>
            class __find_close_neighbor
            {
                friend class Map2d;
            public:
                __find_close_neighbor& operator>>(std::vector<Location<_Ty>>& locations)
                {
                    locations.clear();
                    if(map)
                    {
                        if((__directions & Left) && map->WithinBounds(_loc.left()) && map->Get(_loc.left()) == __value_to_find)
                            locations.push_back(_loc.left());

                        if((__directions & Right) && map->WithinBounds(_loc.right()) && map->Get(_loc.right()) == __value_to_find)
                            locations.push_back(_loc.right());

                        if((__directions & Top) && map->WithinBounds(_loc.up()) && map->Get(_loc.up()) == __value_to_find)
                            locations.push_back(_loc.up());

                        if((__directions & Bottom) && map->WithinBounds(_loc.down()) && map->Get(_loc.down()) == __value_to_find)
                            locations.push_back(_loc.down());

                        TODO("Add diagonal coordinates");
                    }
                    return *this;
                }
            private:
                //__selected_value_stream() {}
                T __value_to_find{};
                Location<_Ty> _loc{Location<_Ty>(0,0)};
                Directions __directions{Directions::None};
                Map2d* map{ nullptr };
            };
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
            __forceinline void Set(const Location<_Ty>& _loc, const T& value)
            {
                if(!WithinBounds(_loc))
                    return;
                Map[_loc.x + _loc.y * Width] = value;
            }
            template <class _Ty>
            __forceinline bool WithinBounds(const Location<_Ty>& _loc) const
            {
                return !(_loc.x < 0 || _loc.y < 0 || _loc.x >= Width || _loc.y >= Height);
            }
            template <class _Ty>
            __forceinline const T Get(const Location<_Ty>& _loc) const
            {
                if(!WithinBounds(_loc))
                    throw std::runtime_error("Coordinates are not within bounds.");
                return Map[_loc.x + _loc.y * Width];
            }
            template <class _Ty>
            __forceinline const T Get(const Location<_Ty>& _loc, const T& _default) const
            {
                if(!WithinBounds(_loc))
                    return _default;
                return Map[_loc.x + _loc.y * Width];
            }

            template <class _Ty>
            _Ty accumulate(_Ty _Val)
            {
                return std::accumulate(Map.begin(), Map.end(), _Val);
            }

            __forceinline void find(const T& _value, int& x, int& y) const
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

            __selected_value_stream<T>& select_value(T _value)
            {
                __selected_value_stream.map = this;
                __selected_value_stream.__value = _value;
                return __selected_value_stream;
            }

            template <class _Ty>
            __find_close_neighbor<T, _Ty> higher_by_one_neighbors(const Location<_Ty>& _loc, Directions _directions)
            {
                __find_close_neighbor<T, _Ty> str;
                str.map = this;
                str.__value_to_find = Get(_loc) + 1;
                str.__directions = _directions;
                return str;
            }
            __selected_value_stream<T>& operator>>(aoc::stream::AoCStreamSelectedValue<T>& _selValueStream)
            {
                __selected_value_stream.map = this;
                __selected_value_stream.__value = _selValueStream._value;
                return __selected_value_stream;
            }

        private:
            __selected_value_stream<T> __selected_value_stream;
            // __find_close_neighbor<T> __find_close_neighbor;
        };

        class single_digit_map : public Map2d<int8_t>
        {};
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

        class single_digit_list : public std::vector<int8_t>
        {};
    }


    class MemoryPool
    {
    public:
        MemoryPool(size_t blockSize, size_t blockCount)
            : blockSize(blockSize), blockCount(blockCount), pool(nullptr), freeList(nullptr)
        {
            initializePool();
        }

        ~MemoryPool()
        {
            delete[] pool;
        }

        void* allocate()
        {
            if(!freeList)
            {
                std::cerr << "Memory pool exhausted" << std::endl;
                return nullptr;
            }
            void* freeBlock = freeList;
            freeList = freeList->next;
            return freeBlock;
        }
        void deallocate(void* ptr)
        {
            FreeBlock* block = static_cast<FreeBlock*>(ptr);
            block->next = freeList;
            freeList = block;
        }
        void reset()
        {
            freeList = reinterpret_cast<FreeBlock*>(pool);
        }
    private:
        struct FreeBlock
        {
            FreeBlock* next;
        };
        void initializePool()
        {
            pool = new char[blockSize * blockCount];
            freeList = reinterpret_cast<FreeBlock*>(pool);
            FreeBlock* current = freeList;
            for(size_t i = 1; i < blockCount; ++i)
            {
                current->next = reinterpret_cast<FreeBlock*>(pool + i * blockSize);
                current = current->next;
            }
            current->next = nullptr;
        }
        size_t blockSize;
        size_t blockCount;
        char* pool;
        FreeBlock* freeList;
    };
}