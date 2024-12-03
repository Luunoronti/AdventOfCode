#include "AoC_2024_03.h"

const int AoC_2024_03::GetDay() const
{
    return 3;
}

const int AoC_2024_03::GetExpectedResultStep1() const
{
    return 163931492;
}

const int AoC_2024_03::GetExpectedResultStep1Test() const
{
    return 161;
}

const int AoC_2024_03::GetExpectedResultStep2() const
{
    return 76911921;
}

const int AoC_2024_03::GetExpectedResultStep2Test() const
{
    return 48;
}

const long AoC_2024_03::Process(const string& Line, const bool AllowModifiers)
{
    // will use state-machine per-character parsing
    // with the exception to reading string from the file, we do not write any memory
    // so we should be cache friendly
    
    // yes, I should do a single state for full 'mod(', 'do()' and 'don't()' 
    // but, i wanted to make a full state machine for each character, just for fun
    // even if its repeating code

    //@TODO: next time, I will use macros to create a macro-based state machine, it's way easier to read

    State state = State::Idle;
    int x = 0, y = 0;
    long sum = 0;
    bool modSwitch = true;
    for(const char ch : Line)
    {
        switch(state)
        {
        case Idle:
            if(ch == 'm') { state = State::M; }
            else if(AllowModifiers && ch == 'd') { state = State::D; }
            break;
        case M:
            if(ch == 'u') { state = State::Mu; }
            else { state = State::Idle; }
            break;
        case Mu:
            if(ch == 'l') { state = State::Mul; }
            else { state = State::Idle; } 
            break;
        case Mul:
            if(ch == '(') { state = OpenParen; x = 0; y = 0; }
            else { state = Idle; }
            break;
        case OpenParen:
            if(IsDigit(ch)) { x = ToDigit(ch); state = State::X1; }
            else { state = State::Idle; }
            break;
        case X1:
            if(IsDigit(ch)) { x = x * 10 + ToDigit(ch); state = State::X2; }
            else if(ch == ',') { state = State::Comma; }
            else { state = State::Idle; }
            break;
        case X2:
            if(IsDigit(ch)) { x = x * 10 + ToDigit(ch); state = State::X3; }
            else if(ch == ',') { state = State::Comma; }
            else { state = State::Idle; }
            break;
        case X3:
            if(ch == ',') { state = State::Comma; }
            else { state = State::Idle; }
            break;
        case Comma:
            if(IsDigit(ch)) { y = ToDigit(ch); state = State::Y1; }
            else { state = State::Idle; }
            break;
        case Y1:
            if(IsDigit(ch)) { y = y * 10 + ToDigit(ch); state = State::Y2; }
            else if(ch == ')') { sum += AllowModifiers ? modSwitch ? x * y : 0 : x * y;  state = State::Idle; }
            else { state = State::Idle; }
            break;
        case Y2:
            if(IsDigit(ch)) { y = y * 10 + ToDigit(ch); state = State::Y3; }
            else if(ch == ')') { sum += AllowModifiers ? modSwitch ? x * y : 0 : x * y;  state = State::Idle; }
            else { state = State::Idle; }
            break;
        case Y3:
            if(ch == ')') { sum += AllowModifiers ? modSwitch ? x * y : 0 : x * y; state = State::Idle; }
            else { state = State::Idle; }
            break;
        case D:
            if(ch == 'o') { state = State::DO; }
            else { state = State::Idle; } 
            break;
        case DO:
            if(ch == '(') { state = State::DoOpenParen; }
            else if(ch == 'n') { state = State::DON; }
            else { state = State::Idle; } 
            break;
        case DON:
            if(ch == '\'') { state = State::DON_; }
            else { state = State::Idle; } 
            break;
        case DON_:
            if(ch == 't') { state = State::DON_T; }
            else { state = State::Idle; } 
            break;
        case DON_T:
            if(ch == '(') { state = State::DontOpenParen; }
            else { state = State::Idle; } 
            break;
        case DoOpenParen:
            if(ch == ')') { modSwitch = true; }
            state = State::Idle;
            break;
        case DontOpenParen: 
            if(ch == ')') { modSwitch = false; }
            state = State::Idle;
            break;
        default:
            break;
        }
    }

    return sum;
}

const long AoC_2024_03::Step1()
{
    return Process(ReadStringFromFile(1), false);
}

const long AoC_2024_03::Step2()
{
    return Process(ReadStringFromFile(2), true);
}
