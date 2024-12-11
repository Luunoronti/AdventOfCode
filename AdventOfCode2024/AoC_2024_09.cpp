#include "pch.h"
#include "AoC_2024_09.h"

using namespace aoc;

/*

IMPORTANT NOTE:
I wanted to learn how to create own memory allocators
and to remind myself of double-linked lists. So, i've done all that
by hand, which makes this code somewhat big and dirty.
    
*/


// define a file descriptor
// this will also hold free space blocks
// which are just files with id == -1
struct FileDescriptor
{
    uint32_t location : 24;
    uint8_t size;
    int32_t id; // if -1, this is an empty space descriptor

    // implement this as two way linked list (without using std)
    FileDescriptor* prev{ nullptr };
    FileDescriptor* next{ nullptr };

    FileDescriptor(const uint8_t& size, int id)
        : size(size), id(id), location(0)
    {
    }
 
    static void* operator new(size_t size)
    {
        return memoryPool.allocate();
    }
    static void operator delete(void* ptr)
    {
        memoryPool.deallocate(ptr);
    }
public:
    static MemoryPool memoryPool;
};

// Define the static memory pool 
// MemoryPool does not support automatic growth yet
// so allocate as much as we think is more than enough
// it's just a couple of MBs (512 * 512 * (8+16) = 6,291,456 bytes, to be exact), so that's ok, we can go 100x more and still be fine
TODO("Change initial size once MemoryPool implements auto-growth");
MemoryPool FileDescriptor::memoryPool(sizeof(FileDescriptor), 512 * 512);

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
        actualIndex = (i == 0 || !(i % 2)) ? fileIndex++ : -1; // compute this before continuing the loop due to size == 0 so that fileIndex is actual
        if(size == 0) continue;

        _new = new FileDescriptor(size, actualIndex);
        first == nullptr ? next = first = _new : (_new->prev = next, next = next->next = _new);
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
void PrintFS(FileDescriptor* fs)
{
     /*while(fs)
     {
         for(int i = 0; i < fs->size; ++i)
             (fs->id == -1) ? (aoc::dout << '.') : (aoc::dout << fs->id);
         fs = fs->next;
     }
     aoc::dout << endl;*/
}

const int64_t AoC_2024_09::Step1()
{
    aoc::numeric::single_digit_list input;
    aoc::aocs >> input;

    TIME_PART;

    FileDescriptor* fileSystem{ nullptr };
    FileDescriptor* lastDescriptor{ nullptr };
    ConstructFileSystem(input, &fileSystem, &lastDescriptor);

    FileDescriptor* firstFreeSpace = fileSystem;
    firstFreeSpace = GetFirstFreeSpaceDescriptor(firstFreeSpace);

    while(firstFreeSpace != nullptr && lastDescriptor != nullptr && (firstFreeSpace = GetFirstFreeSpaceDescriptor(firstFreeSpace)))
    {
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
    int location{ 0 };
    while(next)
    {
        for(int i = location; i < location + next->size; ++i) checksum += i * next->id;
        location += next->size;
        next = next->next;
    }

    return checksum;
};



const int64_t AoC_2024_09::Step2()
{
    //if(!IsTest())return 0;
    TIME_PART;

    aoc::numeric::single_digit_list input;
    aoc::aocs >> input;

    FileDescriptor* fileSystem{ nullptr };
    FileDescriptor* lastDescriptor{ nullptr };
    ConstructFileSystem(input, &fileSystem, &lastDescriptor);

    FileDescriptor* next = fileSystem;
    unordered_map<int8_t, std::map<int, FileDescriptor*>> freeSpace;
    int loc = 0;
    while(next)
    {
        next->location = loc;
        if(next->id == -1)
            freeSpace[next->size][next->location] = next;
        loc += next->size;
        next = next->next;
    }
    PrintFS(fileSystem);
    while(lastDescriptor)
    {
        if(lastDescriptor->id >= 0)
        {
            // find first fitting space
            // this does not seem to work for live input (works in test ofc)
            // fallback loop bellow finds proper empty space
            next = nullptr;
            {
                // TIME("Map organized empty space descriptors");
                for(int i = lastDescriptor->size; i < 10; ++i)
                {
                    auto& sizedSpace = freeSpace[i];
                    if(!sizedSpace.empty())
                    {
                        const auto& b = sizedSpace.begin();
                        next = b->second;
                        sizedSpace.erase(b);
                        break;
                    }
                }
            }
            // fallback start << this causes around 230ms of Part2 time
            next = fileSystem;
            while(next != nullptr)
            {
                if(next == lastDescriptor)
                {
                    next = nullptr; // cannot go beyond last descriptor
                    break;
                }
                if(next->id == -1 && next->size >= lastDescriptor->size)
                {
                    // found
                    break;
                }
                next = next->next;
            }
            // fallback end

            if(next != nullptr)
            {
                next->id = lastDescriptor->id;
                lastDescriptor->id = -1;

                if(next->size == lastDescriptor->size)
                {
                    continue;
                }

                // if size of lastDescriptor is smaller, do as above, but make lastDescriptor new empty space
                if(next->size > lastDescriptor->size)
                {
                    // create new empty space descriptor
                    // and link it in
                    FileDescriptor* _new = new FileDescriptor(next->size - lastDescriptor->size, -1);
                    _new->location = next->location + lastDescriptor->size;

                    _new->prev = next;
                    if(next->next) _new->next = next->next;
                    if(next->next) next->next->prev = _new;
                    next->next = _new;

                    freeSpace[_new->size][_new->location] = _new;
                }
                next->size = lastDescriptor->size;
            }
        }
        else
        {
            freeSpace[lastDescriptor->size].erase(lastDescriptor->location);
        }
        PrintFS(fileSystem);
        lastDescriptor = lastDescriptor->prev;
    }

    PrintFS(fileSystem);
    next = fileSystem;
    int64_t checksum{ 0 };
    int location{ 0 };
    while(next)
    {
        if(next->id >= 0) for(int i = location; i < location + next->size; ++i) checksum += i * next->id;
        location += next->size;
        next = next->next;
    }

    return checksum;
};