using Microsoft.Diagnostics.Runtime.DacInterface;

namespace AoC;

//[DefaultInput("test2.txt")]
[DefaultInput("live")]
public static class Solver
{
    static Dictionary<int, List<int>> Graph = new Dictionary<int, List<int>>();
    static int youId = -1;
    static int outId = -1;
    static int svrId = -1;
    static int dacId = -1;
    static int fftId = -1;
    static int nextId = 0;

    [ExpectedResult("test", 5)]
    [ExpectedResult("test2.txt", 8)]
    [ExpectedResult("live", 603)]
    public static unsafe long SolvePart1(string FilePath)
    {
        //TODO: natywny win32 jak wszystko pójdzie na stack
        var lines = File.ReadAllLines(FilePath);
        var maxNodes = lines.Length * 2; // jedna linia to jedno źródło i kilka destów, czyli jeden int na linię by starczył

        #region Build graph
        
        Span<int> nameKeys = stackalloc int[maxNodes];
        nameKeys.Clear();

        const int svrKey = ('s' << 16) | ('v' << 8) | 'r'; // Stackalloc Vibes Required
        const int dacKey = ('d' << 16) | ('a' << 8) | 'c'; // Digital-Analog Converter
        const int fftKey = ('f' << 16) | ('f' << 8) | 't'; // Fast Fourier Transform
        const int youKey = ('y' << 16) | ('o' << 8) | 'u'; // Yield Optimization Unit? Yelling Over USB?
        const int outKey = ('o' << 16) | ('u' << 8) | 't'; // Optimized User Tools

        static int MakeNameKey(ReadOnlySpan<char> s) => (s[0] << 16) | (s[1] << 8) | s[2];
        int AddIdIfNotPresent(ReadOnlySpan<char> name, Span<int> nameKeys)
        {
            var key = MakeNameKey(name);
            for (var i = 0; i < nextId; i++) if (nameKeys[i] == key) return i;
            var id = nextId;
            nameKeys[id] = key;
            nextId++;

            if (key == youKey) youId = id;
            else if (key == outKey) outId = id;
            else if (key == svrKey) svrId = id;
            else if (key == dacKey) dacId = id;
            else if (key == fftKey) fftId = id;

            return id;
        }

        // read the file and prepare graph
        //TODO: skan po span i brak linq
        foreach (var line in lines)
        {
            var parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var fromId = AddIdIfNotPresent(parts[0], nameKeys);
            var splits = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var grList = new List<int>();
            foreach (var s in splits)
            {
                grList.Add(AddIdIfNotPresent(s, nameKeys));
            }
            Graph[fromId] = grList;
        }
        #endregion




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



