using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    [AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(54)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    class Day25
    {

        class Node
        {
            public string Name { get; set; }
            public List<Edge> Edges { get; } = new List<Edge>();
        }
        class Edge
        {
            public Node NodeA { get; set; }
            public Node NodeB { get; set; }
        }

        class Graph
        {
            public List<Node> nodes = new();
            public List<Edge> edges = new();

            public Node GetOrCreateNode(string name)
            {
                var node = nodes.SingleOrDefault(n => n.Name == name);
                if (node == null)
                {
                    nodes.Add(node = new Node { Name = name });
                }
                return node;
            }
            public Edge GetOrCreateEdge(Node a, Node b)
            {
                var edge = edges.SingleOrDefault(e => (e.NodeA == a && e.NodeB == b) || (e.NodeA == b && e.NodeB == a));
                if (edge == null)
                    edges.Add(edge = new Edge { NodeA = a, NodeB = b });

                if (a.Edges.Contains(edge) == false)
                    a.Edges.Add(edge);

                if (b.Edges.Contains(edge) == false)
                    b.Edges.Add(edge);
                return edge;
            }
        }

        private static Graph GetGraph(string[] input)
        {
            var ret = new Graph();

            foreach (var line in input)
            {
                var sp1 = line.Split(": ");

                var node = ret.GetOrCreateNode(sp1[0]);
                var sp2 = sp1[1].Split(' ');
                foreach (var c in sp2)
                {
                    var chnode = ret.GetOrCreateNode(c);
                    ret.GetOrCreateEdge(node, chnode);
                }
            }
            Log.WriteLine($"Graph created with {ret.nodes.Count} nodes and {ret.edges.Count} edges.");
            return ret;
        }


        private static List<HashSet<Node>> FindDisconnectGroups(Graph graph, int maxGroupsCount, int cuts)
        {
            var nodes = graph.nodes;
            var edges = graph.edges;

            while (true) // we assume there is a solution to be found
            {
                var subsets = nodes.Select(n => new HashSet<Node> { { n } }).ToList();

                // cut graph randomly (Karger's algorithm) https://en.wikipedia.org/wiki/Karger%27s_algorithm
                // this is actually a terrible algorithm, times are very random (as expected)
                // and searching for solution can take as low as 500ms and as much as 20 seconds
                // It think I'll want Stoer–Wagner algorithm instead
                // https://en.wikipedia.org/wiki/Stoer%E2%80%93Wagner_algorithm
                while (subsets.Count > maxGroupsCount)
                {
                    var rnd = new Random().Next(edges.Count);

                    var ss1 = subsets.FirstOrDefault(s => s.Contains(edges[rnd].NodeA));
                    var ss2 = subsets.FirstOrDefault(s => s.Contains(edges[rnd].NodeB));

                    if (ss1 == null)
                        throw new InvalidProgramException("ss1 is null");
                    if (ss2 == null)
                        throw new InvalidProgramException("ss1 is null");

                    if (ss1 == ss2) continue;

                    subsets.Remove(ss2);
                    foreach (var s in ss2)
                    {
                        ss1.Add(s);
                    }
                }

                // count cuts
                var c = 0;
                for (int e = 0; e < edges.Count; e++)
                {
                    var ss1 = subsets.FirstOrDefault(s => s.Contains(edges[e].NodeA));
                    var ss2 = subsets.FirstOrDefault(s => s.Contains(edges[e].NodeB));
                    if (ss1 != ss2) c++;
                }

                if (c == cuts && subsets.Count == maxGroupsCount)
                    return subsets;
            }
        }





        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] input, int lineWidth, int count)
        {
            var g2 = GetSWGraph(input);
            var minCut = MinCut(g2);

            var graph = GetGraph(input);
            var sets = FindDisconnectGroups(graph, maxGroupsCount: 2, cuts: 3);
            return sets.Aggregate(1, (a, s) => a * s.Count);
        }


        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int lineWidth, int count)
        {
            return 0;
        }


        public class SW_Edge
        {
            public int n1; // source vertex
            public int n2; // destination vertex
            public int w; // weight of the edge
        }

        private static List<SW_Edge> GetSWGraph(string[] input)
        {
            HashSet<string> allPossibleNodex = new HashSet<string>();
            foreach (var line in input)
            {
                var sp1 = line.Split(": ");
                var sp2 = sp1[1].Split(' ');

                allPossibleNodex.Add(sp1[0]);
                foreach (var c in sp2)
                    allPossibleNodex.Add(c);
            }

            var nodes = allPossibleNodex.ToList();
            var ret = new List<SW_Edge>();

            foreach (var line in input)
            {
                var sp1 = line.Split(": ");
                var sp2 = sp1[1].Split(' ');

                var n = sp1[0];
                var ni = nodes.IndexOf(n);
                for (int i = 0; i < sp2.Length; i++)
                {
                    string? n2 = sp2[i];
                    ret.Add(new SW_Edge { n1 = ni, n2 = nodes.IndexOf(n2), w = i + 1 });
                }
            }
            return ret;
        }
        private static int MinCut(List<SW_Edge> edges)
        {
            var count = edges.Count;

            // Initialize the minimum cut value to infinity
            int minCut = int.MaxValue;

            // Initialize an array to store the contracted vertices
            Span<int> contracted = new int[count];

            // Initialize an array to store the weights of the vertices
            Span<int> weights = new int[count];

            // Initialize an array to store the visited vertices
            Span<bool> visited = new bool[count];

            // Loop until only one vertex is left
            for (int i = 0; i < count - 1; i++)
            {
                // Initialize the weight and visited arrays to zero and false
                weights.Clear();
                visited.Clear();

                // Pick a random vertex as the starting vertex
                int start = new Random().Next(count);

                // Mark the starting vertex as visited
                visited[start] = true;

                // Initialize the last added vertex to the starting vertex
                int last = start;

                // Loop n - i - 1 times to find the most tightly connected vertices
                for (int j = 0; j < count - i - 1; j++)
                {
                    // Update the weights of the vertices adjacent to the last added vertex
                    foreach (SW_Edge e in edges)
                    {
                        if (e.n1 == last || e.n2 == last)
                        {
                            int other = e.n1 == last ? e.n2 : e.n1;
                            if (!visited[other])
                            {
                                weights[other] += e.w;
                            }
                        }
                    }

                    // Find the vertex with the maximum weight that is not visited
                    int maxWeight = -1;
                    int maxVertex = -1;
                    for (int k = 0; k < count; k++)
                    {
                        if (!visited[k] && weights[k] > maxWeight)
                        {
                            maxWeight = weights[k];
                            maxVertex = k;
                        }
                    }

                    // Mark the vertex with the maximum weight as visited
                    visited[maxVertex] = true;

                    // Update the last added vertex to the vertex with the maximum weight
                    last = maxVertex;
                }

                // Update the minimum cut value if the weight of the last added vertex is smaller
                if (weights[last] < minCut)
                {
                    minCut = weights[last];
                }

                // Merge the last two added vertices into one
                contracted[last] = start;

                // Update the edges to reflect the merging
                foreach (SW_Edge e in edges)
                {
                    if (e.n1 == last)
                    {
                        e.n1 = start;
                    }
                    if (e.n2 == last)
                    {
                        e.n2 = start;
                    }
                }

                // Remove the self-loops
                edges.RemoveAll(e => e.n1 == e.n2);
            }

            // Return the minimum cut value
            return minCut;
        }


    }
}