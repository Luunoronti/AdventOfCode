#pragma once

#include <iostream> 
#include <sstream> 
#include <unordered_map> 
#include <vector> 
#include <string> 
#include <set>
#include <type_traits>

#include "CustomTypes.h"
#include "CustomFunctions.h"

namespace aoc
{
    
    // Define the manipulator for omit_char 
    inline stream::AoCStreamOmitCharacter omit_char(char c)
    {
        return stream::AoCStreamOmitCharacter(c);
    }

    inline stream::AoCStreamOmitCharacters omit_char(std::string characters)
    {
        return stream::AoCStreamOmitCharacters(characters);
    }
    
    template <class _Ty>
    inline stream::AoCStreamSelectedValue<_Ty> select_value(_Ty value)
    {
        return stream::AoCStreamSelectedValue(value);
    }



    class AoCStream
    {
    public:
        AoCStream(const std::string& fileName)
            : FileName(fileName), Width(0), Height(0)
        {
        }

        AoCStream(const std::string& input, int width, int height)
            : Input(input), Width(width), Height(height)
        {
        }

        AoCStream& operator>>(aoc::maps::sparse_map<char, Location2di>& map)
        {
            ReadStringFromFileWithWH();
            map.Map.clear();
            for(int y = 0; y < Height; ++y)
            {
                for(int x = 0; x < Width; ++x)
                {
                    char c = Input[y * Width + x];
                    if(CharsToOmmit.find(c) == CharsToOmmit.end())
                    {
                        map.Map[c].emplace_back(x, y);
                    }
                }
            }
            map.Width = Width;
            map.Height = Height;
            return *this;
        }

        AoCStream& omit_char(char c)
        {
            CharsToOmmit.insert(c);
            return *this;
        }
        AoCStream& operator>>(stream::AoCStreamOmitCharacter omit_character)
        {
            CharsToOmmit.insert(omit_character.characterToOmit);
            return *this;
        }
        AoCStream& omit_char(std::string characters)
        {
            for(const auto& c : characters) CharsToOmmit.insert(c);
            return *this;
        }
        AoCStream& operator>>(stream::AoCStreamOmitCharacters omit_characters)
        {
            return omit_char(omit_characters.charactersToOmit);
        }



        AoCStream& operator>>(std::vector<std::string>& strings)
        {
            CreateFileIfDoesNotExist(FileName);
            strings.clear();
            std::ifstream file(FileName);
            if(!file.is_open())
            {
                throw std::runtime_error("Error opening file " + FileName);
            }
            std::string line;
            while(std::getline(file, line)) strings.push_back(line);
            return *this;
        }
        AoCStream& operator>>(aoc::numeric::single_digit_list& ints)
        {
            aoc::numeric::single_digit_list list;

            CreateFileIfDoesNotExist(FileName);
            std::ifstream file(FileName);
            if(!file.is_open())
            {
                throw std::runtime_error("Error opening file " + FileName);
            }

            std::string line;
            while(std::getline(file, line))
            {
                for(const char& c : line)
                {
                    list.push_back(c - '0');
                }
            }
            ints = list;
            return *this;
        }

        AoCStream& operator>>(aoc::numeric::columns<long>& map)
        {
            map = readColumns<long>();
            return *this;
        }
        AoCStream& operator>>(aoc::numeric::columns<int>& map)
        {
            map = readColumns<int>();
            return *this;
        }
        AoCStream& operator>>(aoc::numeric::columns<int64_t>& map)
        {
            map = readColumns<int64_t>();
            return *this;
        }

        AoCStream& operator>>(std::vector<std::vector<int>>& map)
        {
            map = readLists<int>();
            return *this;
        }
        AoCStream& operator>>(std::vector<std::vector<long>>& map)
        {
            map = readLists<long>();
            return *this;
        }
        AoCStream& operator>>(std::vector<std::vector<int64_t>>& map)
        {
            map = readLists<int64_t>();
            return *this;
        }


        AoCStream& operator>>(maps::Map2d<char>& map)
        {
            CreateFileIfDoesNotExist(FileName);
            std::ifstream file(FileName);
            if(!file.is_open()) throw std::runtime_error("Error opening file " + FileName);

            std::string line;
            int lines = 0;
            while(std::getline(file, line))
            {
                map.Width = static_cast<int>(line.size());
                for(const char& c : line) map.Map.push_back(c);
                lines++;
            }
            map.Height = lines;

            aoc::dout << "Map2d() >> " << FileName << " " << map.Width << " " << map.Height << " " << map.Map.size() << std::endl;

            file.close();
            return *this;
        }
        AoCStream& operator>>(maps::single_digit_map& map)
        {
            map.Map.clear();

            CreateFileIfDoesNotExist(FileName);
            std::ifstream file(FileName);
            if(!file.is_open()) throw std::runtime_error("Error opening file " + FileName);

            std::string line;
            int lines = 0;
            while(std::getline(file, line))
            {
                map.Width = static_cast<int>(line.size());
                for(const char& c : line) map.Map.push_back(c - '0');
                lines++;
            }
            map.Height = lines;

            aoc::dout << "Map2d() >> " << FileName << " " << map.Width << " " << map.Height << " " << map.Map.size() << std::endl;

            file.close();
            return *this;
        }

        // uint8_t
        AoCStream& operator>>(std::string& str)
        {
            CreateFileIfDoesNotExist(FileName);
            std::ifstream file(FileName);
            if(!file.is_open()) throw std::runtime_error("Error opening file " + FileName);
            std::string line;
            while(std::getline(file, line)) str += line;
            file.close();
            return *this;
        }


        template <class _Ty>
        maps::Map2d<_Ty> readMap()
        {
            maps::Map2d<_Ty> map;

            CreateFileIfDoesNotExist(FileName);
            std::ifstream file(FileName);
            if(!file.is_open()) throw std::runtime_error("Error opening file " + FileName);

            std::string line;
            int lines = 0;
            while(std::getline(file, line))
            {
                map.Width = static_cast<int>(line.size());

                std::stringstream ss(line);
                _Ty _value;
                while(ss >> _value)
                {
                    map.Map.push_back(_value);
                }

                lines++;
            }
            map.Height = lines;

            aoc::dout << "Map2d() >> " << FileName << " " << map.Width << " " << map.Height << " " << map.Map.size() << std::endl;

            file.close();
            return map;
        }



        template <class _Ty>
        std::vector<_Ty> readList()
        {
            std::vector<_Ty> list;

            CreateFileIfDoesNotExist(FileName);
            std::ifstream file(FileName);
            if(!file.is_open())
            {
                throw std::runtime_error("Error opening file " + FileName);
            }

            std::string line;
            while(std::getline(file, line))
            {
                std::stringstream ss(line);
                _Ty _value;
                while(ss >> _value)
                {
                    list.push_back(_value);
                }
            }
            file.close();
            return list;
        }

        template <class _Ty>
        std::vector<std::vector<_Ty>> readLists()
        {
            std::vector<std::vector<_Ty>> lists;

            CreateFileIfDoesNotExist(FileName);
            std::ifstream file(FileName);
            if(!file.is_open())
            {
                throw std::runtime_error("Error opening file " + FileName);
            }

            std::string line;
            while(std::getline(file, line))
            {
                std::stringstream ss(line);
                std::vector<_Ty> vector;
                _Ty _value;
                while(ss >> _value)
                {
                    vector.push_back(_value);
                }
                lists.push_back(vector);
            }
            file.close();
            return lists;
        }

        template <class _Ty>
        aoc::numeric::columns<_Ty> readColumns()
        {
            aoc::numeric::columns<_Ty> map;

            CreateFileIfDoesNotExist(FileName);
            std::ifstream file(FileName);
            if(!file.is_open())
            {
                throw std::runtime_error("Error opening file " + FileName);
            }

            std::string line;
            _Ty _value;
            int _count = 0;
            while(std::getline(file, line))
            {
                if(map.Map.size() == 0)
                {
                    std::stringstream ss2(line);
                    while(ss2 >> _value) _count++;
                    map.Map.resize(_count);
                    _count = 0;
                }

                std::stringstream ss(line);
                _count = 0;
                while(ss >> _value)
                {
                    map.Map[_count++].push_back(_value);
                }
            }
            file.close();
            return map;
        }
    private:


    private:
        void ReadStringFromFileWithWH();
    private:

        std::string FileName;

        std::string Input;
        int Width;
        int Height;
        std::set<char> CharsToOmmit;
    };


    
}