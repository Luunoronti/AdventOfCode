#include "pch.h"
#include "AoC_2024_24.h"

typedef unordered_map<string, int8_t> Wiring;

class Gate
{
public:
    virtual bool Process(Wiring& Wiring) = 0;
    string Input1;
    string Input2;
    string Output;

    Gate(const string& Input1, const string& Input2, const string& Output)
        : Input1(Input1), Input2(Input2), Output(Output)
    {
    }
};
class AND : public Gate
{
public:
    AND(const string& Input1, const string& Input2, const string& Output)
        : Gate(Input1, Input2, Output)
    {
    }
    // Inherited via Gate
    bool Process(Wiring& Wiring) override
    {
        uint8_t i1 = Wiring[Input1];
        uint8_t i2 = Wiring[Input2];
        if(i1 == 0 || i2 == 0) return false; // input not initialized
        --i1;
        --i2;
        Wiring[Output] = 1 + (i1 && i2);
        return true;
    }
};
class OR : public Gate
{
public:
    OR(const string& Input1, const string& Input2, const string& Output)
        : Gate(Input1, Input2, Output)
    {
    }
    // Inherited via Gate
    bool Process(Wiring& Wiring) override
    {
        uint8_t i1 = Wiring[Input1];
        uint8_t i2 = Wiring[Input2];
        if(i1 == 0 || i2 == 0) return false; // input not initialized
        --i1;
        --i2;
        Wiring[Output] = 1 + (i1 || i2);
        return true;
    }
};
class XOR : public Gate
{
public:
    XOR(const string& Input1, const string& Input2, const string& Output)
        : Gate(Input1, Input2, Output)
    {
    }
    // Inherited via Gate
    bool Process(Wiring& Wiring) override
    {
        uint8_t i1 = Wiring[Input1];
        uint8_t i2 = Wiring[Input2];
        if(i1 == 0 || i2 == 0) return false; // input not initialized
        --i1;
        --i2;
        Wiring[Output] = 1 + (i1 ^ i2);
        return true;
    }
};

typedef vector<shared_ptr<Gate>> Gates;
typedef vector<shared_ptr<AND>> AndGates;
typedef vector<shared_ptr<OR>> OrGates;
typedef vector<shared_ptr<XOR>> XorGates;

class HalfAdder
{
    /*
        Half Adder schematic:

                           +----
        A -------+--------\\     \
                 |         ||XOR  |----- S
        B ----+--|--------//     /
              |  |         +----
              |  |
              |  |         +----
              |  +-------- |     \
              |            | AND  |----- C
              +----------- |     /
                           +----

        Notes:
        * in our case, A must be x00 and B must be y00
        * S must be z00
        * C must go to a Full Adder with x01, y01 inputs and z01 output.
        * C must be connected to second XOR and top AND of Full Adder
    */

public:

    bool TryCreate(const Wiring& Wires, const AndGates& ANDs, const XorGates& XORs, vector<string>& Changes)
    {
        // we need to find two gates, one XOR and one AND.
        // both must have same inputs, x00 and y00
        for(auto& gate : ANDs)
        {
            if((gate->Input1 == "x00" && gate->Input2 == "y00") || (gate->Input2 == "x00" && gate->Input1 == "y00"))
            {
                // this would be our and gate
                AND = gate;
            }
        }

        // do the same for XOR gate
        for(auto& gate : XORs)
        {
            if((gate->Input1 == "x00" && gate->Input2 == "y00") || (gate->Input2 == "x00" && gate->Input1 == "y00"))
            {
                // this would be our xor gate
                XOR = gate;
            }
        }
        return XOR && AND;
    }


    string A;
    string B;
    string S;
    string C;
    shared_ptr<XOR> XOR;
    shared_ptr<AND> AND;
};


class FullAdder
{
    /*
        Full Adder schematic:

                      +----
    A(x__) -------+--\\     \
                  |   ||XOR  |-G0--\       +----
    B(y__) ----+--|--//     /       ---+--\\     \
               |  |   +----            |   ||XOR  |----------------------- S (z__)
      Cin  ------------------------+---|--//     /
               |  |                |   |   +----
               |  |                |   |
               |  |                |   |  +----
               |  |                |   +--|     \
               |  |                |      | AND  |-G1
               |  |                +------|     /    \     +----
               |  |                       +----       -----\     \
               |  |                                         | OR  |------- Cout
               |  |                       +----       -----/     /
               |  +-----------------------|     \    /     +----
               |                          | AND  |-G2
               +--------------------------|     /
                                          +----

        Notes:
        * First XOR and bottom AND must have x__ and y__ inputs
        * Second XOR must have G0 and Cin inputs
        * First XOR must output to second XOR
        * Top AND must have G0 and Cin inputs
        * First XOR must output to top AND
        * Both ANDs must output to OR
        * OR cannot have inputs from any other source
        * Cin must come in from Half Adder (if our x/y == x01/y01) AND gate
          or from previous full adder (x-1, y-1) OR gate.
        * Cout must go to next full adder's second XOR and top AND gates
    */

    shared_ptr<XOR> FindGateForOutput(const XorGates& XORs, const string& output)
    {
        for(auto& gate : XORs)
        {
            if(gate->Output == output)
                return gate;
        }
        return nullptr;
    }
    shared_ptr<AND> FindGateForOutput(const AndGates& ANDs, const string& output)
    {
        for(auto& gate : ANDs)
        {
            if(gate->Output == output)
                return gate;
        }
        return nullptr;
    }
    shared_ptr<OR> FindGateForOutput(const OrGates& ORs, const string& output)
    {
        for(auto& gate : ORs)
        {
            if(gate->Output == output)
                return gate;
        }
        return nullptr;
    }

    shared_ptr<AND> FindGate(const AndGates& ANDs, const string& anyInput)
    {
        for(auto& gate : ANDs)
        {
            if(gate->Input1 == anyInput || gate->Input2 == anyInput)
                return gate;
        }
        return nullptr;
    }
    shared_ptr<XOR> FindGate(const XorGates& XORs, const string& anyInput)
    {
        for(auto& gate : XORs)
        {
            if(gate->Input1 == anyInput || gate->Input2 == anyInput)
                return gate;
        }
        return nullptr;
    }
    shared_ptr<OR> FindGate(const OrGates& ORs, const string& anyInput)
    {
        for(auto& gate : ORs)
        {
            if(gate->Input1 == anyInput || gate->Input2 == anyInput)
                return gate;
        }
        return nullptr;
    }

    shared_ptr<AND> FindGate(const AndGates& ANDs, const string& input1, const string& input2)
    {
        for(auto& gate : ANDs)
        {
            if((gate->Input1 == input1 && gate->Input2 == input2) || (gate->Input2 == input1 && gate->Input1 == input2))
                return gate;
        }
        return nullptr;
    }
    shared_ptr<XOR> FindGate(const XorGates& XORs, const string& input1, const string& input2)
    {
        for(auto& gate : XORs)
        {
            if((gate->Input1 == input1 && gate->Input2 == input2) || (gate->Input2 == input1 && gate->Input1 == input2))
                return gate;
        }
        return nullptr;
    }
    shared_ptr<OR> FindGate(const OrGates& ORs, const string& input1, const string& input2)
    {
        for(auto& gate : ORs)
        {
            if((gate->Input1 == input1 && gate->Input2 == input2) || (gate->Input2 == input1 && gate->Input1 == input2))
                return gate;
        }
        return nullptr;
    }

public:
    bool TryCreate(const Wiring& Wires, const AndGates& ANDs, const XorGates& XORs, const OrGates& ORs, const int inputIndex, vector<string>& Changes)
    {
        string xIn = "";
        string yIn = "";
        string zout = "";
        {
            std::ostringstream oss;
            oss << "x" << std::setw(2) << std::setfill('0') << inputIndex;
            xIn = oss.str();
        }
        {
            std::ostringstream oss;
            oss << "y" << std::setw(2) << std::setfill('0') << inputIndex;
            yIn = oss.str();
        }
        {
            std::ostringstream oss;
            oss << "z" << std::setw(2) << std::setfill('0') << inputIndex;
            zout = oss.str();
        }

        // attempt to get up XOR and down AND gates
        UpXOR = FindGate(XORs, xIn, yIn);
        DownAND = FindGate(ANDs, xIn, yIn);

        if(!UpXOR || !DownAND) return false;

        // we don't yet know the Cin name, but we know downXOR and upAND
        // must have one input set as output of UpXOR
        DownXOR = FindGate(XORs, UpXOR->Output);
        UpAND = FindGate(ANDs, UpXOR->Output);

        if(!DownXOR || !UpAND)
        {
            // try to reverse gates, maybe that would help
            std::swap(DownAND->Output, UpXOR->Output);
            DownXOR = FindGate(XORs, UpXOR->Output);
            UpAND = FindGate(ANDs, UpXOR->Output);

            if(!DownXOR || !UpAND)
                return false;

            Changes.push_back(UpXOR->Output);
            Changes.push_back(DownAND->Output);
        }

        // down XOR' must be 's output is an output of the adder
        if(DownXOR->Output != zout)
        {
            // if DownAND Output is proper, this might be a switch here
            if(DownAND->Output == zout)
            {
                DownAND->Output = DownXOR->Output;
                DownXOR->Output = zout;

                Changes.push_back(DownXOR->Output);
                Changes.push_back(DownAND->Output);
            }
            else
            {
                // look for XOR with our zout. if found, switch
                if(auto other = FindGateForOutput(XORs, zout))
                {
                    std::swap(other->Output, DownXOR->Output);
                    Changes.push_back(other->Output);
                    Changes.push_back(DownXOR->Output);
                }
                else if(auto other = FindGateForOutput(ANDs, zout))
                {
                    std::swap(other->Output, DownXOR->Output);
                    Changes.push_back(other->Output);
                    Changes.push_back(DownXOR->Output);
                }
                else if(auto other = FindGateForOutput(ORs, zout))
                {
                    std::swap(other->Output, DownXOR->Output);
                    Changes.push_back(other->Output);
                    Changes.push_back(DownXOR->Output);
                }
                else
                    return false;
            }
        }

        // last gate is OR, with inputs from up and down AND
        Or = FindGate(ORs, UpAND->Output, DownAND->Output);

        return Or != nullptr;
    }

    shared_ptr<XOR> UpXOR;
    shared_ptr<XOR> DownXOR;
    shared_ptr<AND> UpAND;
    shared_ptr<AND> DownAND;
    shared_ptr<OR> Or;
};




static void CreateWiring(const vector<string>& input, Wiring& Wires, Gates& Gates)
{
    bool isWiringInitialState = true;

    // to simplify wiring init, we proceed with two loops
    // first one will read all wires and gates
    // and second one will read initial wiring state

    for(const string& line : input)
    {
        if(line.size() == 0)
        {
            isWiringInitialState = false;
            continue;
        }
        if(isWiringInitialState)
        {
            continue;
        }

        const string& wire1 = line.substr(0, 3);
        string wire2;
        string outWire;

        if(line[4] == 'O') // OR gate
        {
            wire2 = line.substr(7, 3);
            outWire = line.substr(14, 3);
        }
        else // XOR / AND gate
        {
            wire2 = line.substr(8, 3);
            outWire = line.substr(15, 3);
        }

        Wires[wire1] = 0;
        Wires[wire2] = 0;
        Wires[outWire] = 0;

        if(line[4] == 'O') // OR gate
            Gates.push_back(make_shared<OR>(wire1, wire2, outWire));
        else if(line[4] == 'X') // XOR gate
            Gates.push_back(make_shared<XOR>(wire1, wire2, outWire));
        else // AND gate
            Gates.push_back(make_shared<AND>(wire1, wire2, outWire));
    }

    for(const string& line : input)
    {
        if(line.size() == 0)
        {
            return;
        }
        Wires[line.substr(0, 3)] = 1 + (line[5] == '1');
    }
}
static bool Check(Wiring& Wires)
{
    for(auto& wire : Wires)
        if(wire.second == 0 && wire.first[0] == 'z')
            return false;
    return true;
}
static bool Process(Wiring& Wires, Gates& Gates)
{
    for(auto& gate : Gates)
        gate->Process(Wires);
    return Check(Wires);
}

static bool CustomComparator(const std::pair<std::string, int>& a, const std::pair<std::string, int>& b)
{
    int numA = std::stoi(a.first.substr(1));
    int numB = std::stoi(b.first.substr(1));
    return numA > numB;
}

static void LoadGates(const vector<string>& input, AndGates& ANDs, XorGates& XORs, OrGates& ORs, Wiring& Wires)
{
    bool isWiringInitialState = true;

    for(const string& line : input)
    {
        if(line.size() == 0)
        {
            isWiringInitialState = false;
            continue;
        }
        if(isWiringInitialState)
        {
            continue;
        }

        const string& wire1 = line.substr(0, 3);
        string wire2;
        string outWire;

        if(line[4] == 'O') // OR gate
        {
            wire2 = line.substr(7, 3);
            outWire = line.substr(14, 3);
        }
        else // XOR / AND gate
        {
            wire2 = line.substr(8, 3);
            outWire = line.substr(15, 3);
        }
        if(line[4] == 'O') // OR gate
            ORs.push_back(make_shared<OR>(wire1, wire2, outWire));
        else if(line[4] == 'X') // XOR gate
            XORs.push_back(make_shared<XOR>(wire1, wire2, outWire));
        else // AND gate
            ANDs.push_back(make_shared<AND>(wire1, wire2, outWire));

        Wires[wire1] = 1;
        Wires[wire2] = 1;
        Wires[outWire] = 1;
    }
}



const int64_t AoC_2024_24::Step1()
{
    // !! these are the states of the wires:
    // 0: not initialized
    // 1: logic 0
    // 2: logic 1

    TIME_PART;
    vector<string> input;
    aoc::AoCStream() >> input;

    Wiring Wires;
    Gates Gates;
    CreateWiring(input, Wires, Gates);

    while(!Process(Wires, Gates)) {};

    // get all the wiring with z
    vector<std::pair<string, uint8_t>> zs;
    for(const auto& wire : Wires)
    {
        if(wire.first[0] == 'z')
            zs.push_back(make_pair(wire.first, wire.second - 1));
    }

    // sort
    std::sort(zs.begin(), zs.end(), CustomComparator);

    // sum
    int64_t sum = 0;
    for(const auto& pair : zs)
    {
        sum = (sum << 1) | pair.second;
    }

    return sum;
};
const int64_t AoC_2024_24::Step2()
{
    if(IsTest())
    {
        cout << endl << "Test Input is not valid for P2" << endl;
        return 1;
    }

    vector<string> input;
    aoc::AoCStream() >> input;

    Wiring Wires;
    AndGates ANDs;
    OrGates ORs;
    XorGates XORs;
    LoadGates(input, ANDs, XORs, ORs, Wires);

    TIME_PART;

    // count wires that start with x, y and z
    mutil::IntVector3 letterCount;
    for(const auto& [w, s] : Wires)
    {
        if(w[0] == 'x') letterCount.x++;
        else if(w[0] == 'y') letterCount.y++;
        else if(w[0] == 'z') letterCount.z++;
    }
    if(letterCount.x != letterCount.y)
    {
        cout << ERR << "Input count mismatch." << endl;
        return 0;
    }
    cout << OK << "Input count correct." << endl;


    vector<string> Changes;

    HalfAdder halfAdder;
    if(!halfAdder.TryCreate(Wires, ANDs, XORs, Changes))
    {
        cout << ERR << "Unable to create a valid half adder." << endl;
        return 0;
    }
    cout << OK << "Half adder created." << endl;

    // for each x and y (except x00 and y00)
    // we attempt to create a full adder

    for(int i = 1; i < letterCount.x; i++)
    {
        FullAdder adder;
        if(!adder.TryCreate(Wires, ANDs, XORs, ORs, i, Changes))
        {
            cout << ERR << "Unable to create a valid full adder at index " << i << "." << endl;
            return 0;
        }
    }
    cout << OK << "Full adders created." << endl;
    
    // this is where we would have to check for connections between adders.
    // however, the problem turned out to be swaps inside adders only.
    // we got 4 swaps (8 strings in total) already.
    // but looking for proper interconnections would be just the same
    // as looking for gates to swap in FullAdder::TryCreate()


    std::sort(Changes.begin(), Changes.end());
    for(auto c : Changes)
    {
        cout << c << ",";
    }
    return 1;
};