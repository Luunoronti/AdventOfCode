namespace AdventOfCode2023
{
    class Day8
    {
        public static long Part1(string[] lines)
        {
            var steps = lines[0].Trim().Select(c => c == 'L' ? 0 : 1).ToArray();

            var nodes = lines.TakeLast(lines.Length - 2)
                .Select(l => l.Replace("(", "").Replace(")", "").Trim())
                .Select(l => new
                {
                    id = l.SplitAtAsString('=', 0).Trim(),
                    lid = l.SplitAtAsString('=', 1).Trim().SplitAtAsString(',', 0),
                    rid = l.SplitAtAsString('=', 1).Trim().SplitAtAsString(',', 1)
                });

            var nodeidmap = nodes.Select((n, i) => (n, i)).ToDictionary(i => i.n.id, i => i.i);
            var nodeArray = new int[nodeidmap.Count, 2];
            foreach (var node in nodes)
            {
                nodeArray[nodeidmap[node.id], 0] = nodeidmap[node.lid];
                nodeArray[nodeidmap[node.id], 1] = nodeidmap[node.rid];
            }

            var nodeNames = new string[nodeidmap.Count];
            foreach (var nim in nodeidmap)
            {
                nodeNames[nim.Value] = nim.Key;
            }
            long stepsCount = 0;
            int stepIndex = 0;
            var startNodeIndex = nodeidmap["AAA"];
            var targetNodeIndex = nodeidmap["ZZZ"];
            var currNodeIndex = startNodeIndex;
            Console.WriteLine($"Target node index: {targetNodeIndex}");
            while (true)
            {
                if (currNodeIndex == targetNodeIndex)
                    break;

                var step = steps[stepIndex];
                stepIndex++;
                if (stepIndex >= steps.Length)
                {
                    stepIndex = 0;
                }
                stepsCount++;
                currNodeIndex = nodeArray[currNodeIndex, step];
            }

            return stepsCount;
        }
        public static long Part2(string[] lines)
        {
            var steps = lines[0].Trim().Select(c => c == 'L' ? 0 : 1).ToArray();

            var nodes = lines.TakeLast(lines.Length - 2)
                .Select(l => l.Replace("(", "").Replace(")", "").Trim())
                .Select(l => new
                {
                    id = l.SplitAtAsString('=', 0).Trim(),
                    lid = l.SplitAtAsString('=', 1).Trim().SplitAtAsString(',', 0),
                    rid = l.SplitAtAsString('=', 1).Trim().SplitAtAsString(',', 1)
                });

            var nodeidmap = nodes.Select((n, i) => (n, i)).ToDictionary(i => i.n.id, i => i.i);
            var nodeArray = new int[nodeidmap.Count, 2];
            foreach (var node in nodes)
            {
                nodeArray[nodeidmap[node.id], 0] = nodeidmap[node.lid];
                nodeArray[nodeidmap[node.id], 1] = nodeidmap[node.rid];
            }

            var nodeNames = new string[nodeidmap.Count];
            foreach (var nim in nodeidmap)
                nodeNames[nim.Value] = nim.Key;

            // look for all nodes that end with 'A'
            List<int> searchersList = new();
            List<int> ends = new();
            for (int i = 0; i < nodeNames.Length; i++)
            {
                var name = nodeNames[i];
                if (name[2] == 'A') searchersList.Add(i);
                else if (name[2] == 'Z') ends.Add(i);
            }

            var visitFlags = new bool[nodeArray.Length];
            var stepCount = steps.Length;
            long fctr = 1L;
            searchersList.Select(l => GetPath(l, 0)).Select(l => l / stepCount)
                .ForEach(l => fctr *= l);

            return fctr * stepCount;

            int GetPath(int node, int stepIndex)
            {
                int stepsCount = 0;
                while (true)
                {
                    if (ends.Contains(node))
                        break;
                    var step = steps[stepIndex];
                    stepIndex++;
                    if (stepIndex >= steps.Length)
                        stepIndex = 0;
                    stepsCount++;
                    node = nodeArray[node, step];
                }
                return stepsCount;
            }
        }
    }
}