namespace AdventOfCode2023
{
    class Day8
    {
       
        public static void Run(string[] lines)
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
            var index = 0;
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
            //var startNodeIndex = nodeidmap["AAA"];
            //var targetNodeIndex = nodeidmap["ZZZ"];
            //var currNodeIndex = startNodeIndex;
            //Console.WriteLine($"Target node index: {targetNodeIndex}");
            //while (true)
            //{
            //    if (currNodeIndex == targetNodeIndex)
            //        break;

            //    var step = steps[stepIndex];
            //    stepIndex++;
            //    if (stepIndex >= steps.Length)
            //    {
            //        stepIndex = 0;
            //    }
            //    stepsCount++;
            //    currNodeIndex = nodeArray[currNodeIndex, step];
            //}

            //Console.WriteLine($"Part 1: {stepsCount}");


            // look for all nodes that end with 'A'
            List<int> searchersList = new List<int>();
            List<int> ends = new List<int>();
            for (int i = 0; i < nodeNames.Length; i++)
            {
                var name = nodeNames[i];
                if (name[2] == 'A') searchersList.Add(i);
                else if (name[2] == 'Z') ends.Add(i);
            }

            Console.WriteLine($"Starting from {searchersList.Count} start points, will look for {ends.Count} end points.");

            stepsCount = 0;
            stepIndex = 0;
            var visitFlags = new bool[nodeArray.Length];
            var searches = searchersList.ToArray();

            while (true)
            {
                // check all end points vs start points
                var hasAll = true;
                var found = ends.Count;
                for (int i = 0; i < ends.Count; i++)
                {
                    if (visitFlags[ends[i]] == false)
                    {
                        hasAll = false;
                        found--;
                    }
                }
                if (found > 2)
                {
                    Console.WriteLine($"Found {found} ends out of {ends.Count} - steps count: {stepsCount} - need {string.Join(", ", ends)}, got: {string.Join(", ", searches)}");
                }

                if (hasAll)
                    break;

                for (int i = 0; i < searches.Length; i++)
                {
                    visitFlags[searches[i]] = false;
                }


                var step = steps[stepIndex];
                stepIndex++;
                if (stepIndex >= steps.Length)
                {
                    stepIndex = 0;
                }
                stepsCount++;

                for (int i = 0; i < searches.Length; i++)
                {
                    searches[i] = nodeArray[searches[i], step];
                    visitFlags[searches[i]] = true;
                }
            }


            Console.WriteLine($"Part 2: {stepsCount}");




        }
    }
}