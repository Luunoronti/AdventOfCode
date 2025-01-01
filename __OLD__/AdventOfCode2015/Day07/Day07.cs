
using AmaAocHelpers;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode0;

//[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
//[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
//[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
//[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
//[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
[ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
[ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
class Day07
{
    private static int __globalId = 0;
    enum GateType
    {
        And,
        Or,
        LShift,
        RShift,
        Not,
        Move,
        ValueSource,
        Destination
    }

    class Edge
    {
        public string? Name;
        public Node? Source;
        public List<Node>? Destinations = new();
        public ushort Value;
        public bool IsValueLocked;

        public void Set(ushort value)
        {
            Value = value;
            IsValueLocked = true;
        }
    }
    class Node
    {
        public int Id;
        public GateType Type;
        public ushort Value;
        public bool processed = false;
        public Edge? Input1;
        public Edge? Input2;
        public Edge? Output;

        public bool Process()
        {
            if (processed) return false;

            switch (Type)
            {
                case GateType.Destination:
                    processed = true;
                    Value = Input1!.Value;
                    return true;

                case GateType.LShift:
                    if (Input1!.IsValueLocked == false || Input2!.IsValueLocked == false) return false;
                    Output!.Set((ushort)(Input1.Value << Input2.Value));
                    processed = true;
                    return true;
                case GateType.RShift:
                    if (Input1!.IsValueLocked == false || Input2!.IsValueLocked == false) return false;
                    Output!.Set((ushort)(Input1.Value >> Input2.Value));
                    processed = true;
                    return true;
                case GateType.And:
                    if (Input1!.IsValueLocked == false || Input2!.IsValueLocked == false) return false;
                    Output!.Set((ushort)(Input1.Value & Input2.Value));
                    processed = true;
                    return true;
                case GateType.Or:
                    if (Input1!.IsValueLocked == false || Input2!.IsValueLocked == false) return false;
                    Output!.Set((ushort)(Input1.Value | Input2.Value));
                    processed = true;
                    return true;
                case GateType.Not:
                    if (Input1!.IsValueLocked == false) return false;
                    Output!.Set((ushort)(~Input1.Value));
                    processed = true;
                    return true;
                case GateType.Move:
                    if (Input1!.IsValueLocked == false) return false;
                    Output!.Set(Input1.Value);
                    processed = true;
                    return true;
                case GateType.ValueSource:
                    Output!.Set(Value);
                    processed = true;
                    return true;
            }
            return true;
        }

    }



    class Device
    {
        public List<Node> nodes = new();
        public List<Edge> edges = new();

        public Edge GetEdge(string name)
        {
            if (ushort.TryParse(name, out var val))
            {
                var vedge = new Edge
                {
                    Name = name,
                };
                var node = new Node
                {
                    Id = ++__globalId,
                    Type = GateType.ValueSource,
                    Value = val,
                    Output = vedge,
                };
                nodes.Add(node);
                edges.Add(vedge);
                vedge.Source = node;

                return vedge;
            }

            var ee = edges.FirstOrDefault(e => e.Name == name);
            if (ee != null) return ee;

            var edge = new Edge
            {
                Name = name,
            };
            edges.Add(edge);
            return edge;
        }

        public Device(string[] input)
        {
            // put our machine together
            foreach (var l in input)
            {
                var s1 = l.Split("->", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                if (s1[0].Contains(" AND "))
                {
                    var ss = s1[0].Split(" AND ");

                    var i1 = GetEdge(ss[0]);
                    var i2 = GetEdge(ss[1]);
                    var o1 = GetEdge(s1[1]);

                    var n = new Node
                    {
                        Id = ++__globalId,
                        Type = GateType.And,
                        Input1 = i1,
                        Input2 = i2,
                        Output = o1,
                    };

                    i1.Destinations!.Add(n);
                    i2.Destinations!.Add(n);
                    o1.Source = n;

                    nodes.Add(n);
                }
                else if (s1[0].Contains(" OR "))
                {
                    var ss = s1[0].Split(" OR ");

                    var i1 = GetEdge(ss[0]);
                    var i2 = GetEdge(ss[1]);
                    var o1 = GetEdge(s1[1]);

                    var n = new Node
                    {
                        Id = ++__globalId,
                        Type = GateType.Or,
                        Input1 = i1,
                        Input2 = i2,
                        Output = o1,
                    };

                    i1.Destinations!.Add(n);
                    i2.Destinations!.Add(n);
                    o1.Source = n;

                    nodes.Add(n);
                }
                else if (s1[0].Contains(" LSHIFT "))
                {
                    var ss = s1[0].Split(" LSHIFT ");

                    var i1 = GetEdge(ss[0]);
                    var i2 = GetEdge(ss[1]);
                    var o1 = GetEdge(s1[1]);

                    var n = new Node
                    {
                        Id = ++__globalId,
                        Type = GateType.LShift,
                        Input1 = i1,
                        Input2 = i2,
                        Output = o1,
                    };

                    i1.Destinations!.Add(n);
                    i2.Destinations!.Add(n);
                    o1.Source = n;

                    nodes.Add(n);
                }
                else if (s1[0].Contains(" RSHIFT "))
                {
                    var ss = s1[0].Split(" RSHIFT ");

                    var i1 = GetEdge(ss[0]);
                    var i2 = GetEdge(ss[1]);
                    var o1 = GetEdge(s1[1]);

                    var n = new Node
                    {
                        Id = ++__globalId,
                        Type = GateType.RShift,
                        Input1 = i1,
                        Input2 = i2,
                        Output = o1,
                    };

                    i1.Destinations!.Add(n);
                    i2.Destinations!.Add(n);
                    o1.Source = n;

                    nodes.Add(n);
                }
                else if (s1[0].StartsWith("NOT "))
                {
                    var i1 = GetEdge(s1[0][4..]);
                    var o1 = GetEdge(s1[1]);
                    var n = new Node
                    {
                        Id = ++__globalId,
                        Type = GateType.Not,
                        Input1 = i1,
                        Output = o1,
                    };

                    i1.Destinations!.Add(n);
                    o1.Source = n;
                    nodes.Add(n);
                }
                else
                {
                    if (ushort.TryParse(s1[0], out var val))
                    {
                        var o1 = GetEdge(s1[1]);
                        var n = new Node
                        {
                            Id = ++__globalId,
                            Type = GateType.ValueSource,
                            Output = o1,
                            Value = val,
                        };

                        o1.Source = n;
                        nodes.Add(n);
                    }
                    else
                    {
                        var i1 = GetEdge(s1[0]);
                        var o1 = GetEdge(s1[1]);
                        var n = new Node
                        {
                            Id = ++__globalId,
                            Type = GateType.Move,
                            Input1 = i1,
                            Output = o1,
                        };

                        i1.Destinations!.Add(n);
                        o1.Source = n;
                        nodes.Add(n);
                    }
                }
            }

            foreach (var edge in edges)
            {
                if (edge.Destinations!.Count == 0)
                {
                    var node = new Node { Id = ++__globalId, Input1 = edge, Type = GateType.Destination };
                    edge.Destinations.Add(node);
                    nodes.Add(node);
                }
            }

        }

        public bool Process()
        {
            var processAny = false;
            int count = 0;
            foreach (var gate in nodes)
            {
                if (gate.Process())
                {
                    processAny = true;
                    count++;
                }
            }
            Console.CursorLeft = 0;
            Log.Write($"Gates processed: {count,5} / {nodes.Count}");
            return processAny;
        }

        public void SendDiagram()
        {
            //DiagramContext dc = new DiagramContext();

            //foreach (var node in nodes)
            //{
            //    var color = node.processed ? Color.DarkViolet : DiagramContext.DefaultColor;

            //    if (node.Type == GateType.ValueSource)
            //        dc.AddNode(node.Id, $"[ {node.Value} >>>", 0, 0, 1, color: color);
            //    else if (node.Type == GateType.Destination)
            //        dc.AddNode(node.Id, $" >>> {node.Input1.Name} ({node.Value}) ]", 0, 0, 1, color: color);
            //    else
            //        dc.AddNode(node.Id, node.Type.ToString(), 0, 0, 1, color: color);
            //}

            //foreach (var edge in edges)
            //{
            //    //if (edge.Source != null && edge.Destination != null)
            //    foreach (var dst in edge.Destinations)
            //    {

            //        if (edge.IsValueLocked)
            //        {
            //            dc.AddConnector(edge.Source?.Id ?? -1, dst?.Id ?? -1, $"{edge.Name}: {edge.Value}", Color.DarkOrange, 2);
            //        }
            //        else
            //        {
            //            dc.AddConnector(edge.Source?.Id ?? -1, dst?.Id ?? -1, edge.Name);
            //        }

            //    }
            //}
            //dc.Send();
        }


    }

    public static long Part1(string[] input)
    {
        var device = new Device(input);

        device.SendDiagram();

        while (true)
        {
            if (!device.Process())
            {
                device.SendDiagram();
                break;
            }
            device.SendDiagram();
        }
        return device.GetEdge("a").Value;
    }

    public static long Part2(string[] input)
    {
        return 0;
    }
}
