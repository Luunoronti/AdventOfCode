using Microsoft.Diagnostics.Runtime.DacInterface;

namespace AoC;

//[DefaultInput("test2.txt")]
[DefaultInput("live")]
public static class Solver
{
    static Dictionary<int, List<int>> Graph = new Dictionary<int, List<int>>();
    static Dictionary<string, int> StrToId = new Dictionary<string, int>();
    static int youId = -1;
    static int outId = -1;
    static int svrId = -1;
    static int dacId = -1;
    static int fftId = -1;
    static int nextId = 0;

    // Get a unique integer id for each string
    private static int AddIdIfNotPresent(string str)
    {
        if (StrToId.TryGetValue(str, out var nId))
            return nId;
        StrToId.Add(str, nextId);

        switch (str)
        {
            case "svr": svrId = nextId; break; // Stackalloc Vibes Required
            case "dac": dacId = nextId; break; // Digital-Analog Converter (?)
            case "fft": fftId = nextId; break; // Fast Fourier Transform :P
            case "you": youId = nextId; break; // Yield Optimization Unit? Yelling Over USB?
            case "out": outId = nextId; break; // Optimized User Tools
        }

        nextId++;
        return nextId - 1;
    }



    private static void PrepareGraph()
    {

    }


    [ExpectedResult("test", 5)]
    [ExpectedResult("test2.txt", 8)]
    [ExpectedResult("live", 603)]
    public static unsafe long SolvePart1(string FilePath)
    {
        // read the file and prepare graph
        var lines = File.ReadAllLines(FilePath);
        foreach (var line in lines)
        {
            var parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var fromId = AddIdIfNotPresent(parts[0]);
            var list = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(AddIdIfNotPresent).ToList();
            Graph[fromId] = list;
        }

        // because we are keeping nodes as ordered ids, we could just use array to keep number of paths
        // instead of dictionary
        Span<long> nodePathLens = stackalloc long[nextId];
        nodePathLens.Clear();

        // BFS queue
        Span<int> queueBacker = stackalloc int[1024];
        var queue = new SpanQueue<int>(queueBacker);

        // was the node visited already?
        Span<byte> visited = stackalloc byte[nextId];
        visited.Clear();

        // init
        var start = youId >= 0 ? youId : svrId;
        nodePathLens[start] = 1;
        visited[start] = 1;
        queue.Enqueue(start);

        // BFS (again :D)
        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            if (!Graph.TryGetValue(u, out var n)) continue;
            var count = nodePathLens[u];
            foreach (var v in n)
            {
                nodePathLens[v] += count;
                if (visited[v] == 0)
                {
                    visited[v] = 1;
                    queue.Enqueue(v);
                }
            }
        }
        return nodePathLens[outId];

    }

    [ExpectedResult("test", 2)]
    [ExpectedResult("test2.txt", 2)]
    [ExpectedResult("live", 380961604031372)]
    public static unsafe long SolvePart2(string FilePath)
    {
        var nodeCount = nextId;

        // counts how many incoming edges each node have
        Span<int> indegree = stackalloc int[nodeCount];
        indegree.Clear();
        foreach (var kv in Graph)
        {
            foreach (var v in kv.Value) indegree[v] = indegree[v] + 1;
        }

        // order the graph topologically
        Span<int> queueBack = stackalloc int[nodeCount];
        SpanQueue<int> queue = new SpanQueue<int>(queueBack);

        Span<int> topology = stackalloc int[nodeCount];
        var nextTopologyIndex = 0;

        // start with nodes that have no incomming edges
        for (var i = 0; i < nodeCount; i++)
            if (indegree[i] == 0)
                queue.Enqueue(i);

        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            topology[nextTopologyIndex++] = u;
            if (!Graph.TryGetValue(u, out var neighbors)) continue;
            foreach (var v in neighbors)
            {
                indegree[v] = indegree[v] - 1;
                if (indegree[v] == 0) queue.Enqueue(v);
            }
        }

        // order complete, prepare DP state
        Span<int> dpKeyBacker = stackalloc int[8192];
        Span<long> dpValBacker = stackalloc long[8192];
        Span<byte> stateBacker = stackalloc byte[8192];
        SpanDictionary<int, long> pathCountsByState = new SpanDictionary<int, long>(dpKeyBacker, dpValBacker, stateBacker);

        // we can't use keys like (int, bool, bool) so we pack them
        static int MakeKey(int node, bool hasDac, bool hasFft)
        {
            var mask = 0;
            if (hasDac) mask |= 1;
            if (hasFft) mask |= 2;
            return (node << 2) | mask;
        }

        // init state
        pathCountsByState.TryAdd(MakeKey(svrId, false, false), 1);
        long result = 0;

        var maxQSize = 0;
        // move in order of topology
        for (var n = 0; n < topology.Length; n++)
        {
            var node = topology[n];
            if (!Graph.TryGetValue(node, out var neighbors)) continue;

            // mask encodes (hasDac, hasFft)
            // 0 = none, 1 = dac, 2 = fft, 3 = both
            for (var mask = 0; mask < 4; mask++)
            {
                var hasDac = (mask & 1) != 0;
                var hasFft = (mask & 2) != 0;

                if (!pathCountsByState.TryGetValue((node << 2) | mask, out var ways) || ways == 0) continue;

                foreach (var v in neighbors)
                {
                    // update flags when moving to v
                    var newHasDac = hasDac || v == dacId;
                    var newHasFft = hasFft || v == fftId;
                    if (v == outId)
                    {
                        // count only paths that have seen both dac and fft
                        if (newHasDac && newHasFft) result += ways;
                    }
                    else
                    {
                        // go to next node
                        var newKey = MakeKey(v, newHasDac, newHasFft);
                        pathCountsByState.TryGetValue(newKey, out var cur);
                        pathCountsByState[newKey] = cur + ways;
                        maxQSize = Math.Max(maxQSize, pathCountsByState.Count);
                    }
                }
            }
        }

        return result;
    }
}



