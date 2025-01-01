#pragma once
#include "AoC2024.h"

class AoC_2024_17 : public AoC2024
{
public:
    const virtual __forceinline int GetDay() const override { return 17; }
    const int64_t Step1() override;
    const int64_t Step2() override;
};

typedef uint64_t MachineWord;
typedef uint8_t ProgramWord;

class AoCMachine;
class AoCMachineDebugger;

class AoCMachine
{
private:
    const uint8_t A = 0;
    const uint8_t B = 1;
    const uint8_t C = 2;
    const uint8_t PC = 3;

private:
    friend class AoCMachineDebugger;
    vector<ProgramWord> Program;
    vector<MachineWord> Registers;
    vector<ProgramWord> OutputPort;
    MachineWord StepCounter{ 0 };
private:
    // operations
    __forceinline void adv();
    __forceinline void bxl();
    __forceinline void bst();
    __forceinline void jnz();
    __forceinline void bxc();
    __forceinline void out();
    __forceinline void bdv();
    __forceinline void cdv();

    __forceinline MachineWord combo();

public:
    AoCMachine()
    {}

    void LoadProgram(const vector<ProgramWord>& Input);
    void SetRegisters(const vector<MachineWord>& Registers);
    void SetRegister(const int Register, const MachineWord& Value);
    void Run(std::shared_ptr<AoCMachineDebugger> Debugger);
    void Reset();
    void ReadOutput(vector<ProgramWord>& Output);
};

class AoCMachineDebugger
{
public:
    AoCMachineDebugger();
    void OnBeforeStep(AoCMachine* machine);
    void OnAfterStep(AoCMachine* machine);
    const string GetOpCodeName(int opcode);
};


