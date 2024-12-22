using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

string inpt = "123";
string inpt2 = @"1
2
3
2024";

string inpp = @"";

List<long> inputs = [];
Dictionary<string, long> bananas = [];
Dictionary<long, long> precalc = new();
string inp = File.ReadAllText(@"C:\Work\AoC\AdventOfCode\AdventOfCode2024\Live\2024_22_2.txt");
ReadInput(inp);

var ts = Stopwatch.GetTimestamp();
P1();
var ts2 = Stopwatch.GetTimestamp();
P2();
var ts3 = Stopwatch.GetTimestamp();

Console.WriteLine($"P1: {Stopwatch.GetElapsedTime(ts, ts2)}");
Console.WriteLine($"P2: {Stopwatch.GetElapsedTime(ts2, ts3)}");
void P1()
{
    long res = 0;
    long toCalc = 0;
    foreach (var nr in inputs)
    {
        toCalc = nr;
        for (long i = 0; i < 2000; i++)
        {
            toCalc = Calculate(toCalc);
        }

        res += toCalc;
    }
    Console.WriteLine($"P1: {res}");

    //Console.ReadLine();
}
void P2()
{
    long toCalc = 0;

    foreach (var nr in inputs)
    {
        List<long> changes = [];
        HashSet<string> firstchanges = [];
        toCalc = nr;
        for (long i = 0; i < 2000; i++)
        {
            long tmp = toCalc % 10;
            toCalc = Calculate(toCalc);
            long tmp2 = toCalc % 10;
            long change = tmp2 - tmp;
            changes.Add(change);
            if (changes.Count > 3)
            {
                string c = string.Join(',', changes.Skip(changes.Count() - 4));
                if (!firstchanges.Contains(c))
                {
                    firstchanges.Add(c);
                    if (bananas.ContainsKey(c))
                    {
                        bananas[c] += tmp2;
                    }
                    else
                    {
                        bananas.Add(c, tmp2);
                    }
                }
            }
        }
    }
    var r = bananas.OrderByDescending(b => b.Value);
    var res = bananas.Max(b => b.Value);
    Console.WriteLine($"P2:{res}");
    Console.ReadLine();
}

long Calculate(long i)
{
    long res = 0;
    if (precalc.ContainsKey(i))
    {
        res = precalc[i];
    }
    else
    {
        res = ((i << 6) ^ i) % 16777216;
        res = ((res >> 5) ^ res) % 16777216;
        res = ((res << 11) ^ res) % 16777216;

        precalc.Add(i, res);
    }
    return res; ;
}

void ReadInput(string input)
{
    foreach (var l in input.Split('\n'))
    {
        long i;
        if (long.TryParse(l, out i))
        {
            inputs.Add(i);
        }
    }
}