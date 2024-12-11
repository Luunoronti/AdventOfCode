#include "pch.h"
#include "AoC_2024_03.h"

bool StartsWithStringAtIndex(const string& Input, const int Index, const string& TestString)
{
    if(Input.size() <= Index + TestString.size())
        return false;

    for(uint8_t i = 0; i < TestString.size(); ++i)
    {
        if(Input[i + Index] != TestString[i])
            return false;
    }
    return true;
}



// just to make switch/case a little bit nicer, we will use some macros
#define IF_DIGIT if(IsDigit(ch)) {
#define ADD_DIGIT_TO(v) v = v * 10 + ToDigit(ch);
#define SET_STATE(s) state = s;
#define AND_THEN_GO_TO_STATE(s) nextState = s;
#define IF_CHAR(c) if(ch == c) {
#define ELSE_IF_CHAR(c) } else if(ch == c) {
#define ELSE_IF_CHAR_AND(c, b) } else if((b) && ch == c) {
#define ELSE_IDLE } else { state =  State::Idle; }; break;

#define IF_STRING(s) if(ch == s[0] && StartsWithStringAtIndex(Line, lineIndex, s)) { 
#define ELSE_IF_STRING(s) } else if(ch == s[0] && StartsWithStringAtIndex(Line, lineIndex, s)) { 
#define SKIP_CHARS(c) charsToSkip = c; state = State::Skip;
#define SKIP_STRING(s) charsToSkip = (int)strlen(s) - 2; state = State::Skip;

#define ADD_MUL_TO_SUM sum += AllowModifiers ? modSwitch ? x * y : 0 : x * y;


const long AoC_2024_03::Process(const string& Line, const bool AllowModifiers)
{
    // look bellow for:
    // Process_NOMACROS => very unreadable code
    // Process_MACROS => same code actually, but hidden behind macros
    // this method tries to look for starting strings and then, acts accordingly.

    State state = State::Idle;
    State nextState = State::Idle;
    int x = 0, y = 0;
    long sum = 0;
    bool modSwitch = true;
    int lineIndex = -1;
    int charsToSkip = 0;
    int8_t numberCount = 0;
    for(const char ch : Line)
    {
        ++lineIndex;

        switch(state)
        {
        case Idle:
            IF_STRING("mul(")
                SKIP_STRING("mul(")
                x = 0;
                y = 0;
                AND_THEN_GO_TO_STATE(MulOpenParen)
            ELSE_IF_STRING("do()")
                modSwitch = true;
            ELSE_IF_STRING("don't()")
                modSwitch = false;
            ELSE_IDLE
        case MulOpenParen:
            IF_DIGIT
                ADD_DIGIT_TO(x)
                SET_STATE(X1)
            ELSE_IDLE
        case X1:
            IF_DIGIT
                ADD_DIGIT_TO(x)
                SET_STATE(X2)
            ELSE_IF_CHAR(',')
                SET_STATE(Comma);
            ELSE_IDLE
        case X2:
            IF_DIGIT
                ADD_DIGIT_TO(x)
                SET_STATE(X3)
            ELSE_IF_CHAR(',')
                SET_STATE(Comma);
            ELSE_IDLE
        case X3:
            IF_CHAR(',')
                SET_STATE(Comma);
            ELSE_IDLE
        case Comma:
            IF_DIGIT
                ADD_DIGIT_TO(y)
                SET_STATE(Y1)
                ELSE_IDLE
        case Y1:
            IF_DIGIT
                ADD_DIGIT_TO(y)
                SET_STATE(Y2)
            ELSE_IF_CHAR(')')
                ADD_MUL_TO_SUM 
            SET_STATE(Idle);
            ELSE_IDLE
        case Y2:
            IF_DIGIT
                ADD_DIGIT_TO(y)
                SET_STATE(Y3)
            ELSE_IF_CHAR(')')
                ADD_MUL_TO_SUM 
            SET_STATE(Idle);
            ELSE_IDLE
        case Y3:
            IF_CHAR(')')
                ADD_MUL_TO_SUM 
            SET_STATE(Idle);
            ELSE_IDLE

        case Skip:
            if(charsToSkip == 0) SET_STATE(nextState)
            else --charsToSkip;
        default:
            break;
        }
    }

    return sum;
}

const int64_t AoC_2024_03::Step1()
{
    string input;
    aoc::aocs >> input;
    TIME_PART;
    return Process(input, false);
}

const int64_t AoC_2024_03::Step2()
{
    string input;
    aoc::aocs >> input;
    TIME_PART;
    return Process(input, true);
}




const long AoC_2024_03::Process_MACROS(const string& Line, const bool AllowModifiers)
{
    State state = State::Idle;
    int x = 0, y = 0;
    long sum = 0;
    bool modSwitch = true;
    for(const char ch : Line)
    {
        switch(state)
        {
        case Idle:
            IF_CHAR('m')
                SET_STATE(M);
            ELSE_IF_CHAR_AND('d', AllowModifiers)
                SET_STATE(D)
                ELSE_IDLE
        case M:
            IF_CHAR('u')
                SET_STATE(Mu);
            ELSE_IDLE
        case Mu:
            IF_CHAR('l')
                SET_STATE(Mul);
            ELSE_IDLE
        case Mul:
            IF_CHAR('(')
                SET_STATE(MulOpenParen)
                x = 0;
            y = 0;
            ELSE_IDLE
        case MulOpenParen:
            IF_DIGIT
                ADD_DIGIT_TO(x)
                SET_STATE(X1)
                ELSE_IDLE
        case X1:
            IF_DIGIT
                ADD_DIGIT_TO(x)
                SET_STATE(X2)
                ELSE_IF_CHAR(',')
                SET_STATE(Comma);
            ELSE_IDLE
        case X2:
            IF_DIGIT
                ADD_DIGIT_TO(x)
                SET_STATE(X3)
                ELSE_IF_CHAR(',')
                SET_STATE(Comma);
            ELSE_IDLE
        case X3:
            IF_CHAR(',')
                SET_STATE(Comma);
            ELSE_IDLE
        case Comma:
            IF_DIGIT
                ADD_DIGIT_TO(y)
                SET_STATE(Y1)
                ELSE_IDLE
        case Y1:
            IF_DIGIT
                ADD_DIGIT_TO(y)
                SET_STATE(Y2)
                ELSE_IF_CHAR(')')
                sum += AllowModifiers ? modSwitch ? x * y : 0 : x * y;
            SET_STATE(Idle);
            ELSE_IDLE
        case Y2:
            IF_DIGIT
                ADD_DIGIT_TO(y)
                SET_STATE(Y3)
                ELSE_IF_CHAR(')')
                sum += AllowModifiers ? modSwitch ? x * y : 0 : x * y;
            SET_STATE(Idle);
            ELSE_IDLE
        case Y3:
            IF_CHAR(')')
                sum += AllowModifiers ? modSwitch ? x * y : 0 : x * y;
            SET_STATE(Idle);
            ELSE_IDLE
        case D:
            IF_CHAR('o')
                SET_STATE(DO);
            ELSE_IDLE
        case DO:
            IF_CHAR('(')
                SET_STATE(DoOpenParen);
            ELSE_IF_CHAR('n')
                SET_STATE(DON);
            ELSE_IDLE
        case DON:
            IF_CHAR('\'')
                SET_STATE(DON_);
            ELSE_IDLE
        case DON_:
            IF_CHAR('t')
                SET_STATE(DON_T);
            ELSE_IDLE
        case DON_T:
            IF_CHAR('(')
                SET_STATE(DontOpenParen);
            ELSE_IDLE
        case DoOpenParen:
            IF_CHAR(')')
                modSwitch = true;
            SET_STATE(Idle);
            ELSE_IDLE
        case DontOpenParen:
            IF_CHAR(')')
                modSwitch = false;
            SET_STATE(Idle);
            ELSE_IDLE
        default:
            break;
        }
    }

    return sum;
}


const long AoC_2024_03::Process_NOMACROS(const string& Line, const bool AllowModifiers)
{
    // will use state-machine per-character parsing
    // with the exception to reading string from the file, we do not write any memory
    // so we should be cache friendly

    // yes, I should do a single state for full 'mod(', 'do()' and 'don't()' 
    // and maybe read numbers in one state
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
            if(ch == '(') { state = State::MulOpenParen; x = 0; y = 0; }
            else { state = Idle; }
            break;
        case MulOpenParen:
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
            // do and don't processing
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
            // if we get here, we may have a valid do()
        case DoOpenParen:
            if(ch == ')') { modSwitch = true; }
            state = State::Idle;
            break;
            // or a valid don't()
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
