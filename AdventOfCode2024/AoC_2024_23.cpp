#include "pch.h"
#include "AoC_2024_23.h"


class Connection
{
public:
    string C1;
    string C2;
    Connection(const string& C1, const string& C2) : C1(C1), C2(C2) {}

    bool operator==(const Connection& other) const
    {
        return (C1 == other.C1 && C2 == other.C2) || (C1 == other.C2 && C2 == other.C1);
    }
};


struct ConnectionHash
{
    size_t operator()(const Connection& conn) const
    {
        return hash<string>()(conn.C1) ^ hash<string>()(conn.C2);
    }
};

struct ConnectionComparator
{
    bool operator()(const Connection& lhs, const Connection& rhs) const
    {
        if(lhs.C1 == rhs.C1) { return lhs.C2 < rhs.C2; }
        else if(lhs.C1 == rhs.C2) { return lhs.C2 < rhs.C1; }
        else if(lhs.C2 == rhs.C1) { return lhs.C1 < rhs.C2; }
        else if(lhs.C2 == rhs.C2) { return lhs.C1 < rhs.C1; }
        else
        {
            return std::tie(lhs.C1, lhs.C2) < std::tie(rhs.C1, rhs.C2);
        }
    }
};


const int64_t AoC_2024_23::Step1()
{
    TIME_PART;
    vector<string> input;
    aoc::AoCStream() >> input;

    unordered_map<string, set<string>> connMap;
    set<tuple<string, string, string>> setMap;

    for(string line : input)
    {
        connMap[line.substr(0, 2)].insert(line.substr(3, 2));
        connMap[line.substr(3, 2)].insert(line.substr(0, 2));
    }
    for(const auto& [a, conns] : connMap)
    {
        for(const auto& b : conns)
        {
            if(b > a)
            {
                for(const auto& c : connMap[b])
                {
                    if(c > b && conns.find(c) != conns.end())
                    {
                        setMap.insert(make_tuple(a, b, c));
                    }
                }
            }
        }
    }
    int64_t sum = 0;
    for(const auto& t : setMap)
    {
        if(get<0>(t)[0] == 't' || get<1>(t)[0] == 't' || get<2>(t)[0] == 't')
            sum += 1;
    }

    return sum;
};


const int64_t AoC_2024_23::Step2()
{
    TIME_PART;

    if(this->Context->Visualizer)
        ::Sleep(100000000);


    set<string> largest;
    vector<string> sorted(largest.begin(), largest.end()); 
    sort(sorted.begin(), sorted.end()); 
    for (const auto& node : sorted) 
        cout << node << ","; 
    cout << endl;

    return 0;
};




