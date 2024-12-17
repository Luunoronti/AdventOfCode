#include "pch.h"
#include "AoC_2024_17.h"




void ReadProgram(const char* fileName, vector<MachineWord>& Registers, vector<ProgramWord>& Program);

const int64_t AoC_2024_17::Step1()
{
    TIME_PART;

    CreateFileIfDoesNotExist(GetFileName(), GetDay(), GetYear());

    // load initial state and program from file
    vector<ProgramWord> Program;
    vector<MachineWord> Registers;
    ReadProgram(GetFileName().c_str(), Registers, Program);

    // add 4th register, PC
    Registers.push_back(0);

    // initialize the machine (and optional debugger)
    std::shared_ptr<AoCMachineDebugger> Debugger = nullptr;// std::make_shared<AoCMachineDebugger>();
    std::shared_ptr<AoCMachine> TheMachine = std::make_shared<AoCMachine>();

    // load program to the machine
    TheMachine->LoadProgram(Program);
    TheMachine->SetRegisters(Registers);

    // run
    TheMachine->Run(Debugger);

    // read output
    vector<MachineWord> Output;
    TheMachine->ReadOutput(Output);

    // release the machine (and debugger)
    TheMachine.reset();
    Debugger.reset();

    // convert output to a number
    // this is not the nicest way of doing things,
    // i'd like to make it nicer at the later stage
    std::stringstream ss;
    for(const auto& num : Output)
        ss << num;

    return std::stoll(ss.str());
};

const int64_t AoC_2024_17::Step2()
{
    TIME_PART;
    return 0;
};

void ReadProgram(const char* fileName, vector<MachineWord>& Registers, vector<ProgramWord>& Program)
{
    FILE* file;
    fopen_s(&file, fileName, "r");
    if(file == NULL)
    {
        throw std::runtime_error("Failed to open file");
    }

    char line[1024];

    while(fgets(line, sizeof(line), file))
    {
        const char* p = &line[0];
        if(*p == '\n')continue;

        if(*p == 'R') // start of 'Register X: '
        {
            p += 11;
            Registers.push_back(strtol(p, (char**)&p, 10));
        }
        else
        {
            p = &line[0] + 8; //"Program:"
            do
            {
                ++p;
                Program.push_back((ProgramWord)strtol(p, (char**)&p, 10));
            } while(*p == ',');
        }
    }
    fclose(file);
}


void AoCMachine::LoadProgram(const vector<ProgramWord>& Input)
{
    this->Program.clear();
    for(const auto& i : Input)
        this->Program.push_back(i);
}
void AoCMachine::SetRegisters(const vector<MachineWord>& Registers)
{
    this->Registers.clear();
    for(const auto& r : Registers)
        this->Registers.push_back(r);

    // add PC register
    if(this->Registers.size() <= PC)
        this->Registers.push_back(0);

    // simple sanity check
    if(this->Registers.size() <= PC)
        throw std::runtime_error("Wrong number of registers");
}
void AoCMachine::Run(std::shared_ptr<AoCMachineDebugger> Debugger)
{
#define O_ADV 0
#define O_BXL 1
#define O_BST 2
#define O_JNZ 3
#define O_BXC 4
#define O_OUT 5
#define O_BDV 6
#define O_CDV 7

    while(Registers[PC] >= 0 && Registers[PC] < static_cast<MachineWord>(Program.size()))
    {
        switch(Program[Registers[PC]])
        {
        case O_ADV: adv(); break;
        case O_BXL: bxl(); break;
        case O_BST: bst(); break;
        case O_JNZ: jnz(); break;
        case O_BXC: bxc(); break;
        case O_OUT: out(); break;
        case O_BDV: bdv(); break;
        case O_CDV: cdv(); break;
        }
    }
}
void AoCMachine::ReadOutput(vector<MachineWord>& Output)
{
    Output.clear();
    for(const auto& o : OutputPort)
        Output.push_back(o);
}

MachineWord AoCMachine::combo()
{
    const ProgramWord value = Program[Registers[PC]];
    switch(value)
    {
    case 0:
    case 1:
    case 2:
    case 3:
        return (MachineWord)value;
    case 4:
        return Registers[A];
    case 5:
        return Registers[B];
    case 6:
        return Registers[C];
    case 7:
    default:
        throw std::runtime_error("Invalid opcode found");
    }
}

/**
The adv instruction (opcode 0) performs division.
The numerator is the value in the A register.
The denominator is found by raising 2 to the power of the instruction's combo operand.
The result of the division operation is truncated to an
integer and then written to the A register.
 */
void AoCMachine::adv()
{
    Registers[PC]++;
    Registers[A] = (MachineWord)(Registers[A] / std::pow(2, combo()));
    Registers[PC]++;
}

/**
The bxl instruction (opcode 1) calculates the bitwise XOR
of register B and the instruction's literal operand,
then stores the result in register B.
 */
void AoCMachine::bxl()
{
    Registers[PC]++;
    Registers[B] = (MachineWord)(Registers[B] ^ Program[Registers[PC]]);
    Registers[PC]++;
}

/**
The bst instruction (opcode 2) calculates the value of its combo operand
modulo 8 (thereby keeping only its lowest 3 bits),
then writes that value to the B register
 */
void AoCMachine::bst()
{
    Registers[PC]++;
    Registers[B] = (MachineWord)(combo() % 8);
    Registers[PC]++;
}

/**
The jnz instruction (opcode 3) does nothing if the A register is 0.
However, if the A register is not zero, it jumps by setting
the instruction pointer to the value of its literal operand;
if this instruction jumps, the instruction pointer
is not increased by 2 after this instruction.
 */
void AoCMachine::jnz()
{
    Registers[PC]++;
    if(Registers[A] != 0) Registers[PC] = Program[Registers[PC]];
    else Registers[PC]++;
}

/**
The bxc instruction (opcode 4) calculates the
bitwise XOR of register B and register C,
then stores the result in register B.
(For legacy reasons, this instruction reads an operand but ignores it.)
 */
void AoCMachine::bxc()
{
    Registers[B] = (MachineWord)(Registers[B] ^ Registers[C]);
    Registers[PC]++;
    Registers[PC]++;
}

/**
 The out instruction (opcode 5) calculates the value
 of its combo operand modulo 8, then outputs that value.
 (If a program outputs multiple values, they are separated by commas.)
*/
void AoCMachine::out()
{
    Registers[PC]++;
    OutputPort.push_back(combo() % 8);
    Registers[PC]++;
}

/**
The bdv instruction (opcode 6) works exactly like the adv instruction
except that the result is stored in the B register.
(The numerator is still read from the A register.)
*/
void AoCMachine::bdv()
{
    Registers[PC]++;
    Registers[B] = (MachineWord)(Registers[A] / std::pow(2, combo()));
    Registers[PC]++;
}

/**
The cdv instruction (opcode 7) works exactly like the adv instruction
except that the result is stored in the C register.
(The numerator is still read from the A register.)
 */
void AoCMachine::cdv()
{
    Registers[PC]++;
    Registers[C] = (MachineWord)(Registers[A] / std::pow(2, combo()));
    Registers[PC]++;
}

