using Microsoft.Diagnostics.Runtime.DacInterface;

namespace AoC;

[DefaultInput("live")]
public static class Solver
{
    static Dictionary<int, List<int>> Graph = new Dictionary<int, List<int>>();
    static Dictionary<string, int> StrToId = new Dictionary<string, int>();
    static int startId = -1;
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
        if (str == "you") startId = nextId;
        else if (str == "out") outId = nextId;
        else if (str == "svr") svrId = nextId;
        else if (str == "dac") dacId = nextId;
        else if (str == "fft") fftId = nextId;
        nextId++;
        return nextId - 1;
    }



    private static void PrepareGraph()
    {
        
    }


    [ExpectedResult("test", 5)]
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

        
        Span<int> pathsKeyBacker = stackalloc int[1024];
        Span<long> pathsValueBacker = stackalloc long[1024];
        Span<byte> pathsStateBacker = stackalloc byte[1024];
        var paths = new SpanDictionary<int, long>(pathsKeyBacker, pathsValueBacker, pathsStateBacker);

        Span<int> queueBacker = stackalloc int[1024];
        var queue = new SpanQueue<int>(queueBacker);

        Span<int> visitedBacker = stackalloc int[1024];
        Span<byte> visitedStateBacker = stackalloc byte[1024];
        var visited = new SpanHashSet<int>(visitedBacker, visitedStateBacker);

        paths[startId] = 1;
        visited.Add(startId);
        queue.Enqueue(startId);
        while (queue.Count > 0)
        {
            var u = queue.Dequeue();
            if (!Graph.TryGetValue(u, out var n)) continue;
            var count = paths[u];
            foreach (var v in n)
            {
                if (paths.TryGetValue(v, out var c)) paths[v] = c + count; else paths[v] = count;
                if (visited.Add(v)) queue.Enqueue(v);
            }
        }
        return paths.TryGetValue(outId, out var result) ? result : 0;
    }

    [ExpectedResult("test", 2)]
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

        // order the graph

        Span<int> queueBack = stackalloc int[nodeCount];
        SpanQueue<int> queue = new SpanQueue<int>(queueBack);

        Span<int> topology = stackalloc int[nodeCount];
        var nextTopologyIndex = 0;

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

        Span<int> dpKeyBacker = stackalloc int[8192];
        Span<long> dpValBacker = stackalloc long[8192];
        Span<byte> stateBacker = stackalloc byte[8192];
        SpanDictionary<int, long> dp = new SpanDictionary<int, long>(dpKeyBacker, dpValBacker, stateBacker);

        static int MakeKey(int node, bool hasDac, bool hasFft)
        {
            var mask = 0;
            if (hasDac) mask |= 1;
            if (hasFft) mask |= 2;
            return (node << 2) | mask;
        }

        dp.TryAdd(MakeKey(svrId, false, false), 1);
        long result = 0;

        var maxQSize = 0;
        foreach (var node in topology)
        {
            if (!Graph.TryGetValue(node, out var neighbors)) continue;

            for (var mask = 0; mask < 4; mask++)
            {
                var hasDac = (mask & 1) != 0;
                var hasFft = (mask & 2) != 0;

                if (!dp.TryGetValue((node << 2) | mask, out var ways) || ways == 0) continue;

                foreach (var v in neighbors)
                {
                    var newHasDac = hasDac || v == dacId;
                    var newHasFft = hasFft || v == fftId;
                    if (v == outId)
                    {
                        if (newHasDac && newHasFft) result += ways;
                    }
                    else
                    {
                        var newKey = MakeKey(v, newHasDac, newHasFft);
                        dp.TryGetValue(newKey, out var cur);
                        dp[newKey] = cur + ways;
                        maxQSize = Math.Max(maxQSize, dp.Count);
                    }
                }
            }
        }

        return result;
    }
}



