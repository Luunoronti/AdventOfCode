#include "pch.h"
#include "AoC_2024_09.h"


// define a file descriptor that we will hold
struct FileDescriptor
{
    uint8_t size;
    int id; // if -1, this is an empty space descriptor

    FileDescriptor(const uint8_t& size, int id)
        : size(size), id(id)
    {
    }

    bool operator==(const FileDescriptor& other) const
    {
        return size == other.size && id == other.id;
    }

    // this method will alter size of both descriptors passed
    // and create new one to add to new filesystem

    __forceinline static FileDescriptor CompactDescriptors(FileDescriptor& first, FileDescriptor& second)
    {
        if(first.id >= 0)
        {
            FileDescriptor d(first.size, second.id);
            first.size = 0;
            return d;
        }
        if(second.id < 0)
        {
            second.size = 0;
            return FileDescriptor(0, -1);
        }

        const int newSize = min(first.size, second.size);
        first.size -= newSize;
        second.size -= newSize;
        return FileDescriptor(newSize, second.id);
    }

};

void print_fs(const vector<FileDescriptor>& fs)
{
    for(const auto& fd : fs)
    {
        for(int i = 0; i < fd.size; ++i)
            aoc::dout << fd.id;
    }

    aoc::dout << endl;
}

const int64_t AoC_2024_09::Step1()
{
    if(!IsTest()) return 0;

    TIME_PART;
    aoc::numeric::single_digit_list input;
    aoc::AoCStream(GetFileName()) >> input;

    // must be careful here, we may exceed the stack
    std::deque<FileDescriptor> fileSystem;

    std::vector<FileDescriptor> newFileSystem; // for now, debug only

    for(int i = 0; i < input.size(); i += 2)
    {
        fileSystem.push_back(FileDescriptor(input[i], i / 2));
        if((i + 1) < input.size())
            fileSystem.push_back(FileDescriptor(input[i + 1], -1));
    }


    while(fileSystem.size() > 0)
    {
        auto& first = fileSystem.front();
        auto& last = fileSystem.back();

        print_fs(newFileSystem);

        if(first.size == 0)
        {
            fileSystem.pop_front();
            continue;
        }
        if(last.size == 0)
        {
            fileSystem.pop_back();
            continue;
        }

        if(first == last)
        {
            newFileSystem.push_back(first);
            fileSystem.pop_front();
            continue;
        }


        const auto& newFs = FileDescriptor::CompactDescriptors(first, last);
        if(newFs.size>0)
        {
           newFileSystem.push_back(newFs);
        }
        if(first.size == 0)
        {
            fileSystem.pop_front();
        }
        if(last.size == 0)
        {
            fileSystem.pop_back();
        }
    }



    return 0;
};
const int64_t AoC_2024_09::Step2()
{
    TIME_PART;

    aoc::numeric::single_digit_list input;
    aoc::AoCStream(GetFileName()) >> input;


    return 0;
};