#include "pch.h"
#include "AoC_2024_09.h"

using namespace aoc;

// define a file descriptor
// this will also hold free space blocks
// which are just files with id == -1
struct FileDescriptor
{
    int location{ 0 };
    uint8_t size;
    int id; // if -1, this is an empty space descriptor
    int checksum{ 0 };

    // implement this as two way linked list
    FileDescriptor* prev{ nullptr };
    FileDescriptor* next{ nullptr };

    FileDescriptor(const uint8_t& size, int id)
        : size(size), id(id)
    {
    }

    void ComputeChecksum() 
    {
        checksum = 0;
        for(int i = 0; i < size; i++)
            checksum += ((location+i) * id);
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

    static void* operator new(size_t size)
    {
        return memoryPool.allocate();
    }
    static void operator delete(void* ptr)
    {
        memoryPool.deallocate(ptr);
    }
private:
    static MemoryPool memoryPool;
};

// Define the static memory pool 

// MemoryPool does not support automatic growth yet
// so allocate as much as we think is more than enough
// it's just a couple of MBs, so that's ok, we can go 100x more and still be fine
TODO("Change initial size once MemoryPool implements auto-growth");
MemoryPool FileDescriptor::memoryPool(sizeof(FileDescriptor), 1024 * 1024);

void print_fs(FileDescriptor* fs)
{
    while(fs)
    {
        for(int i = 0; i < fs->size; ++i)
            (fs->id == -1) ? (aoc::dout << '.') : (aoc::dout << fs->id);
        fs = fs->next;
    }
    aoc::dout << endl;
}

void ConstructFileSystem(const aoc::numeric::single_digit_list& input, FileDescriptor** fileSystem, FileDescriptor** lastDescriptor)
{
    FileDescriptor* first{ nullptr };
    FileDescriptor* next{ nullptr };
    FileDescriptor* _new{ nullptr };

    int fileIndex{ 0 };
    uint8_t size;
    int actualIndex;

    for(int i = 0; i < input.size(); ++i)
    {
        size = input[i];
        actualIndex = (i == 0 || !(i % 2)) ? fileIndex++ : -1; // compute this before continuing due to size == 0 so that fileIndex is actual
        if(size == 0) continue;

        _new = new FileDescriptor(size, actualIndex);
        first == nullptr ? next = first = _new : (_new->prev = next, next = next->next = _new); // easiest to read line ever ;)
    }
    *fileSystem = first;
    *lastDescriptor = next;
}
FileDescriptor* GetFirstFreeSpaceDescriptor(FileDescriptor* firstFreeSpace)
{
    FileDescriptor* next = firstFreeSpace;
    while(next != nullptr && next->id != -1)
        next = next->next;
    return next;
}

const int64_t AoC_2024_09::Step1()
{
    aoc::numeric::single_digit_list input;
    aoc::AoCStream(GetFileName()) >> input;

    TIME_PART;
    FileDescriptor* fileSystem{ nullptr };
    FileDescriptor* lastDescriptor{ nullptr };
    ConstructFileSystem(input, &fileSystem, &lastDescriptor);

    FileDescriptor* firstFreeSpace = fileSystem;
    firstFreeSpace = GetFirstFreeSpaceDescriptor(firstFreeSpace);

    while(firstFreeSpace != nullptr && lastDescriptor != nullptr && (firstFreeSpace = GetFirstFreeSpaceDescriptor(firstFreeSpace))/* && lastDescriptor > firstFreeSpace*/)
    {
        //print_fs(fileSystem);

        if(lastDescriptor->id == -1)
        {
            // it's a free space at the back. unlink, delete
            lastDescriptor = lastDescriptor->prev;
            delete lastDescriptor->next;
            lastDescriptor->next = nullptr;
            continue;
        }

        // if size is the same, copy file id and then unlink lastDescriptor
        if(firstFreeSpace->size == lastDescriptor->size)
        {
            firstFreeSpace->id = lastDescriptor->id;
            firstFreeSpace->location = firstFreeSpace->prev ? firstFreeSpace->prev->location + firstFreeSpace->prev->size : 0;
            // unlink, delete
            lastDescriptor = lastDescriptor->prev;
            delete lastDescriptor->next;
            lastDescriptor->next = nullptr;
            continue;
        }
        // if size of the lastDescriptor is larger, just copy id, and change the size of last desc
        // DO NOT unlink last descriptor
        if(firstFreeSpace->size < lastDescriptor->size)
        {
            firstFreeSpace->id = lastDescriptor->id;
            firstFreeSpace->location = firstFreeSpace->prev ? firstFreeSpace->prev->location + firstFreeSpace->prev->size : 0;
            lastDescriptor->size -= firstFreeSpace->size;
            continue;
        }

        // if size of lastDescriptor is smaller, do as above, but make lastDescriptor new empty space
        if(firstFreeSpace->size > lastDescriptor->size)
        {
            uint8_t s = lastDescriptor->size;

            lastDescriptor->size = firstFreeSpace->size - lastDescriptor->size;
            firstFreeSpace->id = lastDescriptor->id;
            lastDescriptor->id = -1;
            firstFreeSpace->size = s;

            // this relink is pretty scary, but it makes sense, trust me :)
            FileDescriptor* ldp = lastDescriptor->prev;
            firstFreeSpace->next->prev = lastDescriptor;
            lastDescriptor->prev = firstFreeSpace;
            lastDescriptor->next = firstFreeSpace->next;
            firstFreeSpace->next = lastDescriptor;
            lastDescriptor = ldp;
            lastDescriptor->next = nullptr;
            continue;
        }
    }

    // we will now establish location of each descriptor and count checksum
    FileDescriptor* next = fileSystem;
    int64_t checksum{ 0 };
    while(next)
    {
        next->location = next->prev ? next->prev->location + next->prev->size : 0;
        next->ComputeChecksum();
        checksum += next->checksum;
        next = next->next;
    }
    //print_fs(fileSystem);

    return checksum;
};
const int64_t AoC_2024_09::Step2()
{
    TIME_PART;

    aoc::numeric::single_digit_list input;
    aoc::AoCStream(GetFileName()) >> input;


    return 0;
};