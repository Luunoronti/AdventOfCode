#include "pch.h"
#include "AoC_2024_17.h"

void ReadProgram(const char* fileName, int32_t& A, int32_t& B, int32_t& C, vector<int32_t>& Program)
{
    FILE* file;
    fopen_s(&file, fileName, "r");
    if(file == NULL)
    {
        throw std::runtime_error("Failed to open file");
    }

    char line[1024];

    fgets(line, sizeof(line), file);
    const char* p = &line[0] + 11; //"Register A: "
    A = strtol(p, (char**)&p, 10);

    fgets(line, sizeof(line), file);
    p = &line[0] + 11; //"Register B: "
    B = strtol(p, (char**)&p, 10);

    fgets(line, sizeof(line), file);
    p = &line[0] + 11; //"Register C: "
    C = strtol(p, (char**)&p, 10);

    fgets(line, sizeof(line), file);
    fgets(line, sizeof(line), file);

    p = &line[0] + 8; //"Program:"
    do
    {
        ++p;
        Program.push_back(strtol(p, (char**)&p, 10));
    } while(*p == ',');

    fclose(file);
}


#undef OUT

#define ADV 0
#define BXL 1
#define BST 2
#define JNZ 3
#define BXC 4
#define OUT 5
#define BDV 6
#define CDV 7

const int64_t AoC_2024_17::Step1()
{
    CreateFileIfDoesNotExist(GetFileName(), GetDay(), GetYear());

    int32_t A, B, C, PC = 0;
    vector<int32_t> Program;
    vector<int32_t> ProgramOutput;
    ReadProgram(GetFileName().c_str(), A, B, C, Program);

    TIME_PART;

#define _LITERAL(o) (o)
#define _COMBO(o) (((o) < 4) ? (o) : ((o)==4 ? A : ((o)==5 ? B : C) ))

#define LITERAL (_LITERAL(Program[PC+1]))
#define COMBO (_COMBO(Program[PC+1]))


    while(PC >= 0 && PC < Program.size())
    {
        uint8_t opcode = Program[PC];

        switch(opcode)
        {
            /////////////////////////////////////////////////////////////////////////////////////////////
            // The adv instruction (opcode 0) performs division. 
            // The numerator is the value in the A register. 
            // The denominator is found by raising 2 to the power of the instruction's combo operand.
            // The result of the division operation is truncated to an 
            // integer and then written to the A register.
        case ADV:
            A = (int32_t)(A / std::pow(2, COMBO));
            PC += 2;
            break;
            /////////////////////////////////////////////////////////////////////////////////////////////
            // The bxl instruction (opcode 1) calculates the bitwise XOR 
            // of register B and the instruction's literal operand, 
            // then stores the result in register B.
        case BXL:
            B = B ^ LITERAL;
            PC += 2;
            break;
            /////////////////////////////////////////////////////////////////////////////////////////////
            // The bst instruction (opcode 2) calculates the value of its combo operand 
            // modulo 8 (thereby keeping only its lowest 3 bits), 
            // then writes that value to the B register
        case BST:
            B = COMBO % 8;
            PC += 2;
            break;
            /////////////////////////////////////////////////////////////////////////////////////////////
            // The jnz instruction (opcode 3) does nothing if the A register is 0. 
            // However, if the A register is not zero, it jumps by setting 
            // the instruction pointer to the value of its literal operand; 
            // if this instruction jumps, the instruction pointer 
            // is not increased by 2 after this instruction.
        case JNZ:
            if(A == 0) { PC += 2; break; }
            PC = LITERAL;
            break;
            /////////////////////////////////////////////////////////////////////////////////////////////
            // The bxc instruction (opcode 4) calculates the 
            // bitwise XOR of register B and register C, 
            // then stores the result in register B. 
            // (For legacy reasons, this instruction reads an operand but ignores it.)
        case BXC:
            B = B ^ C;
            PC += 2;
            break;
            /////////////////////////////////////////////////////////////////////////////////////////////
            // The out instruction (opcode 5) calculates the value 
            // of its combo operand modulo 8, then outputs that value. 
            // (If a program outputs multiple values, they are separated by commas.)
        case OUT:
            ProgramOutput.push_back(COMBO % 8);
            PC += 2;
            break;
            /////////////////////////////////////////////////////////////////////////////////////////////
            // The bdv instruction (opcode 6) works exactly like the adv 
            // instruction except that the result is stored in the B register. 
            // (The numerator is still read from the A register.)
        case BDV:
            B = (int32_t)(A / std::pow(2, COMBO));
            PC += 2;
            break;
            /////////////////////////////////////////////////////////////////////////////////////////////
            // The cdv instruction (opcode 7) works exactly like the adv 
            // instruction except that the result is stored in the C register. 
            // (The numerator is still read from the A register.)
        case CDV:
            C = (int32_t)(A / std::pow(2, COMBO));
            PC += 2;
            break;
            /////////////////////////////////////////////////////////////////////////////////////////////
            // INVALID OP
        default:
            throw std::runtime_error("Invalid opcode");
            break;
        }
    }

    std::stringstream ss; 
    for (int num : ProgramOutput) 
    { 
        ss << num; 
    } 
    
    return std::stoll(ss.str());
};
const int64_t AoC_2024_17::Step2()
{
    TIME_PART;
    return 0;
};