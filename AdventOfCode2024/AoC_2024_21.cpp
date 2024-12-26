#include "pch.h"
#include "AoC_2024_21.h"

#define PRINT_DEBUG 0

struct CustomNumPunct : std::numpunct<char>
{
protected:
    char do_thousands_sep() const override { return ','; }
    std::string do_grouping() const override { return "\3"; }
};

class Robot
{
public:
    Robot(shared_ptr<Robot> Next) :Next(Next) {}
    virtual void Reset() = 0;
    virtual void Process(char Input) = 0;
protected:
    shared_ptr<Robot> Next;
};


class CountingRobot : public Robot
{
public:
    CountingRobot() : Robot(nullptr)
    {
    }

    // Inherited via Robot
    void Reset() override
    {
        Sum = 0;
    }
    __forceinline void Process(char Input) override
    {
        cout << Input;
        ++Sum;
#if PRINT_DEBUG
        if(Sum > 100000000 && Sum % 100000000 == 0)
            cout << Sum << endl;
#endif
    }
    const int64_t GetResult(const string& line) const
    {
        int lineMultiplier = std::stoi(line.substr(0, 3));
        return lineMultiplier * Sum;
    }
private:
    int64_t Sum{ 0 };
};


/**
    * Given our k-pad:
    +---+---+---+
    | 7 | 8 | 9 |
    +---+---+---+
    | 4 | 5 | 6 |
    +---+---+---+
    | 1 | 2 | 3 |
    +---+---+---+
        | 0 | A |
        +---+---+

        // we need to figure out the shortest path required, from current location
        // so first, get the position required
        // then find the shortest path
        // then output that as a list of moves
*/

class KeypadRobot : public Robot
{
public:
    KeypadRobot(shared_ptr<Robot> Next) : Robot(Next) {}
    void Reset() override
    {
        CurrentLocation = GetLocationForCharacter('A');
    }
    void ProcessInput(const string& inputLine)
    {
        for(const char c : inputLine)
            Process(c);
    }
    void Process(char Input) override
    {
        if(!Next)
        {
            cerr << "No next robot specified. Work chain is broken." << endl;
            return;
        }

#if PRINT_DEBUG
        cout << endl << endl << Input << endl << endl;
#endif

        auto newLoc = GetLocationForCharacter(Input);
        // now, depending on where do we need to go, we take different paths:
        // if we need to go up, we do that BEFORE we go left or right
        // if we have to go down, we first go left or right
        // this way, we are sure to get to the location, given one 'empty' spot
        while(newLoc != CurrentLocation)
        {
            if(newLoc.y < CurrentLocation.y)
            {
                CurrentLocation.y--;
                Next->Process('^');
            }
            else if(newLoc.x < CurrentLocation.x)
            {
                CurrentLocation.x--;
                Next->Process('<');
            }
            else if(newLoc.x > CurrentLocation.x)
            {
                CurrentLocation.x++;
                Next->Process('>');
            }
            else if(newLoc.y > CurrentLocation.y)
            {
                CurrentLocation.y++;
                Next->Process('v');
            }
        }
        Next->Process('A');
    };
private:
    mutil::IntVector2 GetLocationForCharacter(const char c)
    {
        switch(c)
        {
        case '7': return mutil::IntVector2(0, 0);
        case '8': return mutil::IntVector2(1, 0);
        case '9': return mutil::IntVector2(2, 0);
        case '4': return mutil::IntVector2(0, 1);
        case '5': return mutil::IntVector2(1, 1);
        case '6': return mutil::IntVector2(2, 1);
        case '1': return mutil::IntVector2(0, 2);
        case '2': return mutil::IntVector2(1, 2);
        case '3': return mutil::IntVector2(2, 2);
        case '0': return mutil::IntVector2(1, 3);
        case 'A': return mutil::IntVector2(2, 3);
        }
    }

private:
    mutil::IntVector2 CurrentLocation;
};


/**
    * Given our k-pad:
        +---+---+
        | ^ | A |
    +---+---+---+
    | < | v | > |
    +---+---+---+
    // we need to figure out the shortest path required, from current location
    // so first, get the position required
    // then find the shortest path
    // then output that as a list of moves
*/

class ControlRobot : public Robot
{
public:
    ControlRobot(shared_ptr<Robot> Next) : Robot(Next) {}

    // Inherited via Robot
    void Reset() override
    {
        CurrentLocation = GetLocationForCharacter('A');
    }

    __forceinline void Process(char Input) override
    {
        if(!Next)
        {
            cerr << "No next robot specified. Work chain is broken." << endl;
            return;
        }

        auto newLoc = GetLocationForCharacter(Input);

        // we need to go down BEFORE going left/right
        // we need to go left/right BEFORE going up

        while(newLoc != CurrentLocation)
        {
            // new logic is a bit more complex:
            // we prefer to move left/right before up/down
            // this calls for check if we can actually move in 
            // that direction
            if(newLoc.x < CurrentLocation.x)
            {
                //if(CurrentLocation.x == 1 && CurrentLocation.y == 0 && newLoc.y > CurrentLocation.y)
                //{
                //    // can't go left. go down instead
                //    CurrentLocation.y++;
                //    Next->Process('v');
                //    continue;
                //}
                CurrentLocation.x--;
                Next->Process('<');
                continue;
            }
            else if(newLoc.x > CurrentLocation.x)
            {
                CurrentLocation.x++;
                Next->Process('>');
                continue;
            }
            else if(newLoc.y > CurrentLocation.y)
            {
                CurrentLocation.y++;
                Next->Process('v');
                continue;
            }
            else if(newLoc.y < CurrentLocation.y)
            {
                //if(CurrentLocation.x == 0 && newLoc.x > CurrentLocation.x)
                //{
                //    // can't go up. go right instead
                //    CurrentLocation.x++;
                //    Next->Process('>');
                //    continue;
                //}

                CurrentLocation.y--;
                Next->Process('^');
                continue;
            }


            continue;

            if(newLoc.y > CurrentLocation.y)
            {
                CurrentLocation.y++;
                Next->Process('v');
            }
            else if(newLoc.x < CurrentLocation.x)
            {
                CurrentLocation.x--;
                Next->Process('<');
            }
            else if(newLoc.x > CurrentLocation.x)
            {
                CurrentLocation.x++;
                Next->Process('>');
            }
            else if(newLoc.y < CurrentLocation.y)
            {
                CurrentLocation.y--;
                Next->Process('^');
            }
        }
        Next->Process('A');
    }

private:
    mutil::IntVector2 GetLocationForCharacter(const char c)
    {
        // not nice, but will do the job
        switch(c)
        {
        case '^': return mutil::IntVector2(1, 0);
        case 'A': return mutil::IntVector2(2, 0);
        case '<': return mutil::IntVector2(0, 1);
        case 'v': return mutil::IntVector2(1, 1);
        case '>': return mutil::IntVector2(2, 1);
        }
    }
private:
    mutil::IntVector2 CurrentLocation;
};




void PrintChars(const vector<char>& vc)
{
    for(auto c : vc)
    {
        cout << c;
    }
    cout << endl;
}


const int64_t AoC_2024_21::Process(int robotsCount)
{
    vector<string> lines;
    aoc::AoCStream() >> lines;

    // our accountant will count the result steps
    shared_ptr<CountingRobot> Accountant = make_shared<CountingRobot>();

    // create X control robots
    shared_ptr<Robot> ctrl = Accountant;
    vector<shared_ptr<ControlRobot>> ControlRobots;
    for(int i = 0; i < robotsCount - 1; i++)
    {
        shared_ptr<ControlRobot> newRobot = make_shared<ControlRobot>(ctrl);
        ControlRobots.push_back(newRobot);
        ctrl = newRobot;
    }

    // and kpad robot that will actually press
    // the digits keypad
    shared_ptr<KeypadRobot> kpadRobot = make_shared<KeypadRobot>(ctrl);

    int64_t sum = 0;
    for(auto& line : lines)
    {
        // reset everything
        Accountant->Reset();
        kpadRobot->Reset();
        std::for_each(ControlRobots.begin(), ControlRobots.end(), [](const auto& r) { r->Reset(); });

        // process
        kpadRobot->ProcessInput(line);

#if PRINT_DEBUG
        cout << " done. Line result: " << Accountant->GetResult(line) << endl;
#endif
        cout << endl;
        // count result
        sum += Accountant->GetResult(line);
    }
    return sum;
}
const int64_t AoC_2024_21::Step1()
{
    if(!IsTest())return 0;

    // Apply the locale to cout 
    std::locale customLocale(std::locale::classic(), new CustomNumPunct);
    std::cout.imbue(customLocale);

    TIME_PART;
    return Process(3);
};
const int64_t AoC_2024_21::Step2()
{
    if(!IsTest())return 0;
    return 0;
    std::locale customLocale(std::locale::classic(), new CustomNumPunct);
    std::cout.imbue(customLocale);

    TIME_PART;
    return Process(25);
};



/*
* v<A<AA>>^AvAA<^A>Av<<A>>^AvA^Av<<A>>^AAv<A>A^A<A>Av<A<A>>^AAAvA<^A>A
* <vA<AA>>^AvAA<^A>A<v<A>>^AvA^A<vA>^A<v<A>^A>AAvA^A<v<A>A>^AAAvA<^A>A
* 
*
*
*
* v<<A>>^AvA^Av<<A>>^AAv<A<A>>^AAvAA<^A>Av<A>^AA<A>Av<A<A>>^AAAvA<^A>A
*
* */