#include "pch.h"
#include "AoC_2024_21.h"


class KeypadRobot
{
public:
    KeypadRobot()
    {
        Reset();
    }
    void Reset()
    {
        CurrentLocation = GetLocationForCharacter('A');
    }

private:
    mutil::IntVector2 GetLocationForCharacter(const char c)
    {
        // not nice, but will do the job
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
public:
    vector<char> ProduceRequiredMovesForInput(const string& Input)
    {
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
        vector<char> output;

        for(const auto c : Input)
        {
            auto newLoc = GetLocationForCharacter(c);

            // now, depending on where do we need to go, we take different paths:
            // if we need to go up, we do that BEFORE we go left or right
            // if we have to go down, we first go left or right
            // this way, we are sure to get to the location, given one 'empty' spot
            while(newLoc != CurrentLocation)
            {
                if(newLoc.y < CurrentLocation.y)
                {
                    CurrentLocation.y--;
                    output.push_back('^');
                }
                else if(newLoc.x < CurrentLocation.x)
                {
                    CurrentLocation.x--;
                    output.push_back('<');
                }
                else if(newLoc.x > CurrentLocation.x)
                {
                    CurrentLocation.x++;
                    output.push_back('>');
                }
                else if(newLoc.y > CurrentLocation.y)
                {
                    CurrentLocation.y++;
                    output.push_back('v');
                }
            }

            output.push_back('A');
            int aa = 0;
        }
        return output;
    }
private:
    mutil::IntVector2 CurrentLocation;
};
class ControlRobot
{
public:
    ControlRobot()
    {
        Reset();
    }
    void Reset()
    {
        CurrentLocation = GetLocationForCharacter('A');
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

public:
    vector<char> ProduceRequiredMovesForInput(vector<char>& Input)
    {
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

        vector<char> output;

        for(const auto c : Input)
        {
            auto newLoc = GetLocationForCharacter(c);

            // we need to go down BEFORE going left/right
            // we need to go left/right BEFORE going up

            while(newLoc != CurrentLocation)
            {
                if(newLoc.y > CurrentLocation.y)
                {
                    CurrentLocation.y++;
                    output.push_back('v');
                }
                else if(newLoc.x < CurrentLocation.x)
                {
                    CurrentLocation.x--;
                    output.push_back('<');
                }
                else if(newLoc.x > CurrentLocation.x)
                {
                    CurrentLocation.x++;
                    output.push_back('>');
                }
                else if(newLoc.y < CurrentLocation.y)
                {
                    CurrentLocation.y--;
                    output.push_back('^');
                }
            }

            output.push_back('A');
            int aa = 0;
        }
        return output;
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


class CommandCounter
{
public:
    CommandCounter() : count(0) {}
    void PushCharacter(const char c)
    {
        if(c == '\0')
        {
            // end of stream, produce counter
            cout << endl << "EOL: " << count << endl;
            return;
        }

        // for test, all we do is to print out our
        // characters
        count++;
    }

    int count;
};

class ControlRobotStream
{
public:
    void PushCharacter(const char c)
    {
        if(c == '\0')
        {
            if(downStream) downStream->PushCharacter('\0');
            if(downCounter) downCounter->PushCharacter('\0');
            return;
        }

        // refer to ControlRobot::ProduceRequiredMovesForInput() for logic explanation
        auto newLoc = GetLocationForCharacter(c);

        // we need to go down BEFORE going left/right
        // we need to go left/right BEFORE going up

        while(newLoc != CurrentLocation)
        {
            if(newLoc.y > CurrentLocation.y)
            {
                CurrentLocation.y++;
                if(downStream) downStream->PushCharacter('v');
                if(downCounter) downCounter->PushCharacter('v');
            }
            else if(newLoc.x < CurrentLocation.x)
            {
                CurrentLocation.x--;
                if(downStream) downStream->PushCharacter('<');
                if(downCounter) downCounter->PushCharacter('<');
            }
            else if(newLoc.x > CurrentLocation.x)
            {
                CurrentLocation.x++;
                if(downStream) downStream->PushCharacter('>');
                if(downCounter) downCounter->PushCharacter('>');
            }
            else if(newLoc.y < CurrentLocation.y)
            {
                CurrentLocation.y--;
                if(downStream) downStream->PushCharacter('^');
                if(downCounter) downCounter->PushCharacter('^');
            }
        }
        if(downStream) downStream->PushCharacter('A');
        if(downCounter) downCounter->PushCharacter('A');

    }

    ControlRobotStream& operator>>(ControlRobotStream controlRobot)
    {
        downStream = &controlRobot;
        return *this;
    }
    ControlRobotStream& operator>>(CommandCounter counter)
    {
        downCounter = &counter;
        return *this;
    }
private:
    static mutil::IntVector2 GetLocationForCharacter(const char c)
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
    ControlRobotStream* downStream{ nullptr };
    CommandCounter* downCounter{ nullptr };
    mutil::IntVector2 CurrentLocation;
    friend class KeypadRobotStream;
};

class KeypadRobotStream
{
public:
    KeypadRobotStream(const string& Input)
        : input(Input)
    {
        CurrentLocation = GetLocationForCharacter('A');
    }
    KeypadRobotStream& operator>>(ControlRobotStream controlRobot)
    {
        // push required commands to next stream
        // one by one (so we produce them one by one as well
        // refer to KeypadRobot::ProduceRequiredMovesForInput() for
        // information on our logic
        for(const auto c : input)
        {
            auto newLoc = GetLocationForCharacter(c);

            // we need to go down BEFORE going left/right
            // we need to go left/right BEFORE going up
            while(newLoc != CurrentLocation)
            {
                if(newLoc.y > CurrentLocation.y)
                {
                    CurrentLocation.y++;
                    controlRobot.PushCharacter('v');
                    if(commandCounter) commandCounter->PushCharacter('v');
                }
                else if(newLoc.x < CurrentLocation.x)
                {
                    CurrentLocation.x--;
                    controlRobot.PushCharacter('<');
                    if(commandCounter) commandCounter->PushCharacter('<');
                }
                else if(newLoc.x > CurrentLocation.x)
                {
                    CurrentLocation.x++;
                    controlRobot.PushCharacter('>');
                    if(commandCounter) commandCounter->PushCharacter('>');
                }
                else if(newLoc.y < CurrentLocation.y)
                {
                    CurrentLocation.y--;
                    controlRobot.PushCharacter('^');
                    if(commandCounter) commandCounter->PushCharacter('^');
                }
            }
            controlRobot.PushCharacter('A');
            if(commandCounter) commandCounter->PushCharacter('A');
        }

        // push EOL so that the stream knows it's the end of stream
        controlRobot.PushCharacter('\0');

        return *this;
    }
    KeypadRobotStream& operator>>(CommandCounter counter)
    {
        commandCounter = &counter;
        return *this;
    }

private:
    static mutil::IntVector2 GetLocationForCharacter(const char c)
    {
        // not nice, but will do the job
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

    CommandCounter* commandCounter{ nullptr };
    string input;
    mutil::IntVector2 CurrentLocation;
};


const int64_t AoC_2024_21::Step1()
{


    //while(true)
    //{
    //    while(!_kbhit())
    //    {
    //        // Get the current state of the mouse buttons 
    //        if (GetAsyncKeyState(VK_RBUTTON) & 0x8000) 
    //        { 
    //            std::cout << "Right mouse button pressed!" << std::endl; 
    //        }

    //        Sleep(1);
    //    }
    //    char ch = _getch(); 
    //    std::cout << "Key pressed: " << ch << std::endl;
    //}



    return 0;

    TIME_PART;
    vector<string> lines;
    aoc::AoCStream() >> lines;

    KeypadRobot kpadRobot;
    vector<ControlRobot> ControlRobots;

    ControlRobots.push_back(ControlRobot());
    ControlRobots.push_back(ControlRobot());
    // ControlRobots.push_back(ControlRobot());

    int64_t sum = 0;
    for(const auto& line : lines)
    {
        cout << line << endl;
        kpadRobot.Reset();
        auto stepsRequired = kpadRobot.ProduceRequiredMovesForInput(line);
        //PrintChars(stepsRequired);
        int index = 0;
        for(auto& ctrl : ControlRobots)
        {
            ctrl.Reset();
            stepsRequired = ctrl.ProduceRequiredMovesForInput(stepsRequired);
            //PrintChars(stepsRequired);
        }
        int count = stepsRequired.size();

        // well.. lol
        char lineTmp[4]{ 0 };
        lineTmp[0] = line[0];
        lineTmp[1] = line[1];
        lineTmp[2] = line[2];
        int num = stoi(lineTmp);

        sum += (num * count);
    }
    return sum;
};
const int64_t AoC_2024_21::Step2()
{
    vector<string> lines;
    aoc::AoCStream() >> lines;

    for(const auto& line : lines)
    {
        CommandCounter counter;
        cout << line << endl;
        KeypadRobotStream(line)
            >> ControlRobotStream()
            >> ControlRobotStream()
            >> ControlRobotStream()
            >> counter
            ;

        cout << endl;
    }

    return 0;

    TIME_PART;

    //return 0;

    KeypadRobot kpadRobot;
    vector<ControlRobot> ControlRobots;

    for(int i = 0; i < 25; i++)
        ControlRobots.push_back(ControlRobot());

    int64_t sum = 0;
    for(const auto& line : lines)
    {
        cout << line << endl;
        kpadRobot.Reset();
        vector<char> stepsRequired;
        stepsRequired = kpadRobot.ProduceRequiredMovesForInput(line);
        //PrintChars(stepsRequired);
        cout << "Starting" << endl;
        int index = 0;
        for(auto& ctrl : ControlRobots)
        {
            cout << "Robot # " << index << endl;
            ctrl.Reset();
            stepsRequired = ctrl.ProduceRequiredMovesForInput(stepsRequired);
            //PrintChars(stepsRequired);
            index++;
        }

        int count = 0;// stepsRequired.size();

        // well.. lol
        char lineTmp[4]{ 0 };
        lineTmp[0] = line[0];
        lineTmp[1] = line[1];
        lineTmp[2] = line[2];
        int num = stoi(lineTmp);

        sum += (num * count);
    }
    return sum;
};

