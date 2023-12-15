using System.Text;
using Box = System.Collections.Generic.List<(string label, int focalLength)?>;
using BoxDi = System.Collections.Generic.Dictionary<string, int>;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(1320)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(145)] // if != 0, will report failure if expected answer != given answer
    class Day15
    {
        public static long Part1(string[] lines) => string.Join("", lines).Replace("\r", "").Replace("\n", "").Replace(" ", "")
                .Split(',').Select(p => GetHashForLabel(p.AsSpan())).Sum();

        private static int GetHashForLabel(ReadOnlySpan<char> label)
        {
            var current = 0;
            for (int i = 0; i < label.Length; i++)
            {
                current += label[i];
                current *= 17;
                current %= 256;
            }
            return current;
        }
        private static void PrintBoxes(Box[] boxes)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < boxes.Length; i++)
            {
                if (boxes[i].Count == 0) continue;
                sb.AppendLine($"Box {CC.Sys}{i}{CC.Clr}: {string.Join(" ", boxes[i].Select(b => $"[{b?.label} {CC.Val}{b?.focalLength}{CC.Clr}]"))} ");
            }
            Log.WriteLine(sb.ToString());
        }
        private static void PrintBoxes(BoxDi[] boxes)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < boxes.Length; i++)
            {
                if (boxes[i].Count == 0) continue;
                sb.AppendLine($"Box {CC.Sys}{i}{CC.Clr}: {string.Join(" ", boxes[i].Select(b => $"[{b.Key} {CC.Val}{b.Value}{CC.Clr}]"))} ");
            }
            Log.WriteLine(sb.ToString());
        }


        public static long Part2_Di(string[] lines)
        {
            var parts = string.Join("", lines).Replace("\r", "").Replace("\n", "").Replace(" ", "").Split(',');
            // BoxDi is System.Collections.Generic.Dictionary<string, int>
            var boxes = Enumerable.Range(0, 256).Select(i => new BoxDi()).ToArray();

            foreach (var part in parts)
            {
                var sp = part.Split(new char[] { '-', '=' });
                var label = sp[0];
                var arg = sp.Length == 2 ? sp[1] : null;
                var hash = GetHashForLabel(label.AsSpan());
                var box = boxes[hash];

                if (string.IsNullOrEmpty(arg))
                {
                    // - operation
                    if(box.ContainsKey(label))
                        box.Remove(label);
                    //var known = box.FirstOrDefault(l => l != null && l.Value.label == label);
                    //if (known != null)
                    //{
                    //    box.RemoveAt(box.IndexOf(known));
                    //}
                }
                else
                {
                    // = operation
                    // look for box
                    // add lens
                    box[label] = int.Parse(arg);

                    //var known = box.FirstOrDefault(l => l != null && l.Value.label == label);
                    //if (known != null)
                    //{
                    //    box[box.IndexOf(known)] = (label, int.Parse(arg));
                    //}
                    //else
                    //{
                    //    box.Add((label, int.Parse(arg)));
                    //}
                }

                if (Log.Enabled)
                {
                    Log.WriteLine($"Op: {CC.Sys}{part}{CC.Clr}");
                    PrintBoxes(boxes);
                    Log.WriteLine();
                }
            }
            var focusingPower = 0;
            for (int i = 0; i < boxes.Length; i++)
            {
                var box = boxes[i];
                int index = 0;
                foreach(var lens in box)
                {
                    index++;
                    focusingPower += (i + 1) * index * lens.Value;
                }
                //for (int s = 0; s < box.Count; s++)
                //    focusingPower += (i + 1) * (s + 1) * box[s]?.focalLength ?? 0;
            }
            return focusingPower;
        }
        public static long Part2(string[] lines)
        {
            return Part2_Di(lines);
            var parts = string.Join("", lines).Replace("\r", "").Replace("\n", "").Replace(" ", "").Split(',');

            var boxes = Enumerable.Range(0, 256).Select(i => new Box()).ToArray();
            
            foreach (var part in parts)
            {
                var sp = part.Split(new char[] { '-', '=' });
                var label = sp[0];
                var arg = sp.Length == 2 ? sp[1] : null;
                var hash = GetHashForLabel(label.AsSpan());
                var box = boxes[hash];

                if (string.IsNullOrEmpty(arg))
                {
                    // - operation
                    var known = box.FirstOrDefault(l => l != null && l.Value.label == label);
                    if (known != null)
                    {
                        box.RemoveAt(box.IndexOf(known));
                    }
                }
                else
                {
                    // = operation
                    // look for box
                    // add lens

                    var known = box.FirstOrDefault(l => l != null && l.Value.label == label);
                    if (known != null)
                    {
                        box[box.IndexOf(known)] = (label, int.Parse(arg));
                    }
                    else
                    {
                        box.Add((label, int.Parse(arg)));
                    }
                }

                if (Log.Enabled)
                {
                    Log.WriteLine($"Op: {CC.Sys}{part}{CC.Clr}");
                    PrintBoxes(boxes);
                    Log.WriteLine();
                }
            }

            var focusingPower = 0;
            for (int i = 0; i < boxes.Length; i++)
            {
                var box = boxes[i];
                for (int s = 0; s < box.Count; s++)
                    focusingPower += (i + 1) * (s + 1) * box[s]?.focalLength ?? 0;
            }
            return focusingPower;
        }
    }
}