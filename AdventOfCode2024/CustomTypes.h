#pragma once
#include <unordered_map> 
#include <vector> 
#include <queue>

namespace aoc
{
    struct Location2di
    {
        int32_t x;
        int32_t y;
        Location2di(int32_t x, int32_t y)
            : x(x), y(y)
        {
        }

        Location2di left()
        {
            return Location2di(x - 1, y);
        }
        Location2di right()
        {
            return Location2di(x + 1, y);
        }
        Location2di up()
        {
            return Location2di(x, y - 1);
        }
        Location2di down()
        {
            return Location2di(x, y + 1);
        }
        Location2di upleft()
        {
            return Location2di(x - 1, y - 1);
        }
        Location2di upright()
        {
            return Location2di(x + 1, y - 1);
        }
        Location2di downleft()
        {
            return Location2di(x - 1, y + 1);
        }
        Location2di downright()
        {
            return Location2di(x + 1, y + 1);
        }

        bool operator<(const Location2di& other) const
        {
            return (x < other.x) || (x == other.x && y < other.y);
        }
        bool operator==(const Location2di& other) const
        {
            return (x == other.x) && (y == other.y);
        }

    };
    // Overload the operator<< to print Location2di 
    inline std::ostream& operator<<(std::ostream& os, const Location2di& loc)
    {
        os << "(" << loc.x << ", " << loc.y << ")";
        return os;
    }


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
            None = 0x00,
            Left = 0x01,
            Right = 0x02,
            Top = 0x04,
            Bottom = 0x08,
            Cardinal = 0x0F,
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
                __selected_value_stream& operator>>(std::vector<Location2di>& locations)
                {
                    locations.clear();
                    if(map)
                    {
                        for(int y = 0; y < map->Height; ++y)
                        {
                            for(int x = 0; x < map->Height; ++x)
                            {
                                if(map->Get(x, y) == __value)
                                    locations.push_back(Location2di(x, y));
                            }
                        }
                    }

                    return *this;
                }

            private:
                __selected_value_stream() {}
                T __value{};
                Map2d* map{ nullptr };
            };

            template<typename T>
            class __find_close_neighbor
            {
                friend class Map2d;
            public:
                __find_close_neighbor& operator>>(std::vector<Location2di>& locations)
                {
                    locations.clear();
                    T _myVal = map->Get(_loc);
                    T _nVal{};
                    if(map)
                    {
                        if(_loc.x == 3 && _loc.y == 4)
                        {
                            int aaqa = 0;
                        }
                        Location2di _nloc = _loc.left();
                        if((__directions & Left) && map->WithinBounds(_nloc))
                        {
                            _nVal = map->Get(_nloc);
                            if(_function(_loc, _nloc, _myVal, _nVal, Left))
                                locations.push_back(_nloc);
                        }

                        _nloc = _loc.right();
                        if((__directions & Right) && map->WithinBounds(_loc.right()))
                        {
                            _nVal = map->Get(_nloc);
                            if(_function(_loc, _nloc, _myVal, _nVal, Right))
                                locations.push_back(_nloc);
                        }

                        _nloc = _loc.up();
                        if((__directions & Top) && map->WithinBounds(_loc.up()))
                        {
                            _nVal = map->Get(_nloc);
                            if(_function(_loc, _nloc, _myVal, _nVal, Top))
                                locations.push_back(_nloc);
                        }

                        _nloc = _loc.down();
                        if((__directions & Bottom) && map->WithinBounds(_loc.down()))
                        {
                            _nVal = map->Get(_nloc);
                            if(_function(_loc, _nloc, _myVal, _nVal, Bottom))
                                locations.push_back(_nloc);
                        }

                        TODO("Add diagonal coordinates");
                    }
                    return *this;
                }
            private:
                Location2di _loc{ Location2di(0,0) };
                Directions __directions{ Directions::None };
                int distance{ 0 };
                std::function<bool(const Location2di&, const Location2di&, T, T, Directions)> _function;
                const Map2d* map{ nullptr };
            };
        public:
            int Width{ 0 };
            int Height{ 0 };
            std::vector<T> Map;

            Map2d()
                : Width(0), Height(0)
            {
            }

            Map2d(int Width, int Height, bool zeroMemory = false)
                : Width(Width), Height(Height)
            {
                if(Width > 0 && Height > 0)
                {
                    Map.resize(Width * Height);
                    if(zeroMemory)
                    {
                        ZeroMemory(&Map[0], sizeof(T) * Width * Height);
                    }
                }
            }

            template<typename _T>
            void createFrom(const Map2d<_T>& source, std::function<T(const _T& sourceValue)> convertFunction)
            {
                Width = source.Width;
                Height = source.Height;
                if(Width > 0 && Height > 0)
                {
                    Map.resize(Width * Height);
                    ZeroMemory(&Map[0], sizeof(T) * Width * Height);
                
                    for(int y = 0; y < source.Height; ++y)
                    {
                        for(int x = 0; x < source.Width; ++x)
                        {
                            Set(x, y, convertFunction(source.Get(x, y)));
                        }
                    }

                }
            }

            template<typename _T>
            Map2d<_T> ToMap(std::function<T(const _T& sourceValue)> convertFunction)
            {
                Map2d<_T> dst;
                dst.Width = Width;
                dst.Height = Height;
                if(Width > 0 && Height > 0)
                {
                    dst.Map.resize(Width * Height);
                    ZeroMemory(&dst.Map[0], sizeof(T) * Width * Height);

                    for(int y = 0; y < Height; ++y)
                    {
                        for(int x = 0; x < Width; ++x)
                        {
                            dst.Set(x, y, convertFunction(Get(x, y)));
                        }
                    }
                }
                return dst;
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
            __forceinline bool WithinBounds(const mutil::IntVector2& Location) const
            {
                return !(Location.x < 0 || Location.y < 0 || Location.x >= Width || Location.y >= Height);
            }
            __forceinline const T Get(const int x, const int y) const
            {
                if(!WithinBounds(x, y))
                    throw std::runtime_error("Coordinates are not within bounds.");
                return Map[x + y * Width];
            }

            __forceinline const T Get(const mutil::IntVector2& Location) const
            {
                if(!WithinBounds(Location))
                    throw std::runtime_error("Coordinates are not within bounds.");
                return Map[Location.x + Location.y * Width];
            }

            __forceinline const T Get(const mutil::IntVector2& Location, const T& _default) const
            {
                if(!WithinBounds(Location))
                    return _default;
                return Map[Location.x + Location.y * Width];
            }

            __forceinline const T Get(const int x, const int y, const T& _default) const
            {
                if(!WithinBounds(x, y))
                    return _default;
                return Map[x + y * Width];
            }
            __forceinline void Set(const mutil::IntVector2& _loc, const T& value)
            {
                if(!WithinBounds(_loc))
                    return;
                Map[_loc.x + _loc.y * Width] = value;
            }

            __forceinline void Set(const Location2di& _loc, const T& value)
            {
                if(!WithinBounds(_loc))
                    return;
                Map[_loc.x + _loc.y * Width] = value;
            }
            __forceinline bool WithinBounds(const Location2di& _loc) const
            {
                return !(_loc.x < 0 || _loc.y < 0 || _loc.x >= Width || _loc.y >= Height);
            }
            __forceinline const T Get(const Location2di& _loc) const
            {
                if(!WithinBounds(_loc))
                    throw std::runtime_error("Coordinates are not within bounds.");
                return Map[_loc.x + _loc.y * Width];
            }
            __forceinline const T Get(const Location2di& _loc, const T& _default) const
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

            __forceinline bool find(const T& _value, int& x, int& y) const
            {
                for(int _y = 0; _y < Height; ++_y)
                {
                    for(int _x = 0; _x < Width; ++_x)
                    {
                        if(Map[_x + _y * Width] == _value)
                        {
                            x = _x;
                            y = _y;
                            return true;
                        }
                    }
                }
                return false;
            }

            void print() const
            {
                print([](int, int, const T& v) {return v; }, [](int, int, const T& v) {return mutil::Vector3(1, 1, 1); });
            }
            void print(std::function<char(const int x, const int y, const T& value)> toPrint) const
            {
                print(toPrint, [](int, int, const T& v) {return mutil::Vector3(1,1,1); });
            }
            void print(std::function<mutil::Vector3(int x, int y, T value)> foregroundColor) const
            {
                print([](int, int, const T& v) {return v; }, foregroundColor);
            }

            void print(std::function<char(const int x, const int y, const T& value)> toPrint,
                std::function<mutil::Vector3(int x, int y, T value)> foregroundColor) const
            {
                std::ostringstream oss;
                for(int _y = 0; _y < Height; ++_y)
                {
                    for(int _x = 0; _x < Width; ++_x)
                    {
                        mutil::Vector3 c = foregroundColor(_x, _y, Get(_x, _y));
                        int fgR = mutil::clamp((int)(c.r * 255), 0, 255);
                        int fgG = mutil::clamp((int)(c.g * 255), 0, 255);
                        int fgB = mutil::clamp((int)(c.b * 255), 0, 255);

                        oss << "\x1b[38;2;" << fgR << ";" << fgG << ";" << fgB << "m";
                        oss << toPrint(_x, _y, Map[_x + _y * Width]);
                    }
                    oss << std::endl;
                }
                oss << "\033[0m";

                std::cout << oss.str();
            }


            __selected_value_stream<T>& select_value(T _value)
            {
                __selected_value_stream.map = this;
                __selected_value_stream.__value = _value;
                return __selected_value_stream;
            }

            __find_close_neighbor<T> get_neighbors(const Location2di& _loc, Directions _directions, int distance, std::function<bool(const Location2di&, const Location2di&, T, T, Directions)> function) const
            {
                __find_close_neighbor<T> str;
                str.map = this;
                str._loc = _loc;
                str.__directions = _directions;
                str.distance = distance;
                str._function = function;
                return str;
            }
            __selected_value_stream<T>& operator>>(aoc::stream::AoCStreamSelectedValue<T>& _selValueStream)
            {
                __selected_value_stream.map = this;
                __selected_value_stream.__value = _selValueStream._value;
                return __selected_value_stream;
            }

            __forceinline void clear()
            {
                ZeroMemory(&Map[0], sizeof(T) * Width * Height);
            }

            int64_t GetLengthOfShortestPath(const mutil::IntVector2& Start, const mutil::IntVector2& End, const std::function<bool(const mutil::IntVector2&, T)> isValid)
            {
                if(!WithinBounds(Start) || !isValid(Start, Get(Start)) || !WithinBounds(End) || !isValid(End, Get(End)))
                    return -1;

                std::vector<mutil::IntVector2> directions = { {0, 1}, {1, 0}, {0, -1}, {-1, 0} }; // Right, Down, Left, Up
                std::queue<mutil::IntVector2> queue;
                aoc::maps::Map2d<int8_t> visited(Width, Height, true);
                queue.push(Start);
                visited.Set(Start, 1);
                int64_t steps = 0;
                while(!queue.empty())
                {
                    int qSize = (int)queue.size();
                    for(int i = 0; i < qSize; ++i)
                    {
                        mutil::IntVector2 curr = queue.front();
                        queue.pop();

                        if(curr == End)
                        {
                            return steps;
                        }
                        for(const auto& dir : directions)
                        {
                            mutil::IntVector2 _new = curr + dir;
                            if(!visited.Get(_new, 1) && WithinBounds(_new) && isValid(_new, Get(_new)))
                            {
                                queue.push(_new);
                                visited.Set(_new, 1);
                            }
                        }
                    }
                    ++steps;
                }
                return -1;
            }


            void fill(const mutil::IntVector2& Start, const T& value)
            {
                // fill(Start, [&value](const T& v, const int x, const int y) {return value; });
            }
            void fill(const mutil::IntVector2& Start, std::function<bool(const T& value, const int x, const int y)> allowFunc, 
                std::function<T(const T& value, const int x, const int y)> valueFunc)
            {
                std::deque<std::pair<int, int>> queue;
                queue.push_back({ Start.x, Start.y });
                Map2d<int8_t> filled(Width, Height, true);
                int64_t area{ 0 };

                while(!queue.empty())
                {
                    const auto [cx, cy] = queue.back();
                    queue.pop_back();

                    if(!WithinBounds(cx, cy)) continue;
                    if(1 == filled.Get(cx, cy)) continue;
                    if(!allowFunc(Get(cx, cy), cx, cy)) continue;

                    Set(cx, cy, valueFunc(Get(cx, cy), cx, cy));
                    filled.Set(cx, cy, 1);

                    queue.push_back({ cx + 1, cy });
                    queue.push_back({ cx - 1, cy });
                    queue.push_back({ cx, cy + 1 });
                    queue.push_back({ cx, cy - 1 });
                }
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


    // Custom hash function for IntVector4
    struct IntVector4Hash
    {
        __forceinline std::size_t operator()(const mutil::IntVector4& vec) const noexcept
        {
            std::size_t h1 = std::hash<int>()(vec.x);
            std::size_t h2 = std::hash<int>()(vec.y);
            std::size_t h3 = std::hash<int>()(vec.z);
            std::size_t h4 = std::hash<int>()(vec.w);
            return h1 ^ (h2 << 1) ^ (h3 << 2) ^ (h4 << 3);
        }
    };

    // Custom equality function for IntVector4
    struct IntVector4Equal
    {
        __forceinline bool operator()(const mutil::IntVector4& vec1, const mutil::IntVector4& vec2) const noexcept
        {
            return vec1.x == vec2.x && vec1.y == vec2.y && vec1.z == vec2.z && vec1.w == vec2.w;
        }
    };
}