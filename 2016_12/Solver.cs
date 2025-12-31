using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    [ExpectedResult("test", 42)]
    [ExpectedResult("live", 318077)]
    public static unsafe long SolvePart1(string FilePath) => Solve(FilePath, 0);

    [ExpectedResult("test", 42)]
    [ExpectedResult("live", 9227731)]
    public static unsafe long SolvePart2(string FilePath) => Solve(FilePath, 1);

    private static unsafe long Solve(string FilePath, int RegCinitValue)
    {
        var UsedStackMemory = 0;

        var FileSize = FileIO.GetFileSize(FilePath);
        ReadOnlySpan<byte> Buffer = stackalloc byte[(int)FileSize];
        UsedStackMemory += Buffer.Length;
        FileIO.ReadToBuffer(FilePath, Buffer);

        // count the number of instructions
        var instructionCounter = 0;
        for (var i = 0; i < Buffer.Length; i++)
        {
            if (Buffer[i] == '\n')
                instructionCounter++;
        }
        instructionCounter++;

        // allocate instructions buffer
        Span<int> Instructions = stackalloc int[instructionCounter];
        UsedStackMemory += 4 * instructionCounter;

        // compile the input
        CompileProgram(Buffer, Instructions);

        // from assembunny CPU design, we know we have 5 registers
        // A, B, C, D, PC
        // technically, we could keep PC and general registers as variables inside
        // ExecuteProgram() method, but I like to have a CPU struct :)
        Span<int> CPU = stackalloc int[5];
        UsedStackMemory += 4 * 5;

        // initialize CPU
        CPU.Clear();
        CPU[Registers.C] = RegCinitValue;

        // execute instructions
        ExecuteProgram(Instructions, CPU);

        return CPU[Registers.A];
    }

    private static void CompileProgram(ReadOnlySpan<byte> Buffer, Span<int> Instructions)
    {
        var instructionIndex = 0;

        // each asmbunno code is 3 chars, so we can
        // utilize that to our advantage
        // also, operators and operands do not bleed between lines
        var index = 0;
        while (index < Buffer.Length)
        {
            SkipWhitespace(Buffer, ref index);

            if (index >= Buffer.Length)
                break;

            // get the operator
            var @operator = (char)Buffer[index];
            index += 4; // skip operator itself and a space

            var opcode = 0;
            // first operand is always present
            // but second operand is optional, depends on operator

            if (GetOperand2(Buffer, ref index, out var op1))
                opcode = OpCodes.Op1IsRegAddress;


            var opcodeValid = false;
            if (@operator is 'j' or 'c')
            {
                opcodeValid = true;
                opcode |= @operator == 'j' ? OpCodes.JumpNotZero : OpCodes.Copy;
                if (GetOperand2(Buffer, ref index, out var op2))
                    opcode |= OpCodes.Op2IsRegAddress;
                opcode = (opcode << 16) | ((op1 & 0xFF) << 8) | (op2 & 0xFF);
            }
            else if (@operator is 'i' or 'd')
            {
                opcodeValid = true;
                opcode |= @operator == 'i' ? OpCodes.Increment : OpCodes.Decrement;
                opcode = (opcode << 16) | ((op1 & 0xFF) << 8);
            }

            if (opcodeValid)
                Instructions[instructionIndex++] = opcode;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool GetOperand2(ReadOnlySpan<byte> Buffer, ref int index, out sbyte op)
        {
            SkipWhitespace(Buffer, ref index);

            var first = (char)Buffer[index++];
            if (first >= 'a' && first <= 'd')
            {
                op = (sbyte)(first - 'a');
                return true;
            }

            var sign = first == '-' ? -1 : 1;
            if (sign >= 0) index--;

            var value = 0;
            while (true)
            {
                if (index < Buffer.Length)
                {
                    var c = (char)Buffer[index++];
                    if (c >= '0' && c <= '9')
                    {
                        value = (value * 10) + (c - '0');
                        continue;
                    }
                }
                op = (sbyte)(value * sign);
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void SkipWhitespace(ReadOnlySpan<byte> Buffer, ref int index)
        {
            while (index < Buffer.Length)
            {
                var c = Buffer[index];
                if (c == (byte)' ' || c == (byte)'\t' || c == (byte)'\r' || c == (byte)'\n')
                {
                    index++;
                    continue;
                }
                break;
            }
        }
    }
    private static void ExecuteProgram(ReadOnlySpan<int> Instructions, Span<int> CPU)
    {
        while (CPU[Registers.PC] < Instructions.Length)
        {
            var opCode = Instructions[CPU[Registers.PC]];
            var op = (opCode >> 16) & 0xFF;
            int op1 = (sbyte)(opCode >> 8);
            int op2 = (sbyte)opCode;
            var ov1 = (op & OpCodes.Op1IsRegAddress) != 0 ? CPU[op1] : op1;
            var ov2 = (op & OpCodes.Op2IsRegAddress) != 0 ? CPU[op2] : op2;

            switch (op & 0x0F)
            {
                case OpCodes.Copy:
                    CPU[op2] = ov1;
                    break;
                case OpCodes.Decrement:
                    CPU[op1]--;
                    break;
                case OpCodes.Increment:
                    CPU[op1]++;
                    break;
                case OpCodes.JumpNotZero:
                    CPU[Registers.PC] += ov1 != 0 ? (ov2 - 1) : 0;
                    break;
            }

            CPU[Registers.PC]++;
        }
    }

    private static class Registers
    {
        public const int A = 0;
        public const int B = 1;
        public const int C = 2;
        public const int D = 3;
        public const int PC = 4;
    }
    private static class OpCodes
    {
        public const int JumpNotZero = 0x04;
        public const int Copy = 0x01;
        public const int Increment = 0x03;
        public const int Decrement = 0x02;

        public const int Op1IsRegAddress = 0x10;
        public const int Op2IsRegAddress = 0x20;
    }
}
