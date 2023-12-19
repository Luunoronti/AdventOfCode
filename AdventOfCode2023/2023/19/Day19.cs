using System.Data;
using System.Runtime.InteropServices;

using RuleSet = System.Collections.Generic.List<(string name, string @default, System.Collections.Generic.List<(char field, char operand, long reqVal, string trg)> rules)>;

namespace AdventOfCode2023
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    [AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(19114)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(167409079868000)] // if != 0, will report failure if expected answer != given answer
    class Day19
    {
        private const string ErrorValue = "E";
        private const string AcceptValue = "A";
        private const string RejectValue = "R";

        private const char Xchar = 'x';
        private const char Mchar = 'm';
        private const char Achar = 'a';
        private const char Schar = 's';

        private const char GTchar = '>';
        private const char LTchar = '<';

        private const int ErrorIndex = -1;
        private const int AcceptIndex = -2;
        private const int RejectIndex = -3;

        private const byte opGreater = 0x10;
        private const byte opLess = 0x20;

        private const byte xField = 1;
        private const byte mField = 2;
        private const byte aField = 3;
        private const byte sField = 4;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Flow
        {
            public static int Size = 5; // count manually, simple enough
            public byte rules;
            public int @default;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Rule
        {
            public static int Size = 13; // count manually, simple enough
            public byte fieldAndOperand;
            public int trgOffset;
            public long reqValue;
        }

        struct Part
        {
            public long x;
            public long m;
            public long a;
            public long s;
            public bool Accepted;
            public long Sum => Accepted ? (x + m + a + s) : 0;
        }

        private static RuleSet ParseRules(string[] input)
        {
            RuleSet parsedRules = new();

            for (int i = 0; i < input.Length; i++)
            {
                var line = input[i];
                if (string.IsNullOrEmpty(input[i]))
                    return parsedRules;

                // for now, we allow standard format, 
                // px{a<2006:qkq,m>2090:A,rfg}

                // so, remove last '}', split by '{'
                var sp1 = input[i][0..^1].Split(new char[] { '{' });

                if (sp1.Length != 2) throw new InvalidDataException($"Unable to parse flow line {line} at index {i}");
                var name = sp1[0];
                var flowStr = sp1[1];

                var defaultRule = ErrorValue; // this is to prevent infinite loops. if we failed parsing, this will show a special value of 'E'.
                // now, split flow
                var flowSp = flowStr.Split(',');

                List<(char, char, long, string)> rules = new();
                foreach (var fs in flowSp)
                {
                    var sp3 = fs.Split(":");
                    if (sp3.Length == 1)
                    {
                        defaultRule = sp3[0];
                    }
                    else
                    {
                        var rule = sp3[0];
                        var trg = sp3[1];

                        var field = rule[0];
                        var op = rule[1];
                        var value = long.Parse(rule[2..]);

                        rules.Add((field, op, value, trg));
                    }
                }
                parsedRules.Add((name, defaultRule, rules));
            }
            return parsedRules;
        }

        private unsafe static byte[] BuildWorkflowsSystem(string[] input)
        {
            var parsed = ParseRules(input);
            var indices = new Dictionary<string, int>();

            indices[ErrorValue] = -1;
            indices[AcceptValue] = -2;
            indices[RejectValue] = -3;

            var totalCount = parsed.Count * Flow.Size + parsed.Select(p => p.rules.Count * Rule.Size).Sum();

            var buffer = new byte[totalCount + 4]; // first 4 bytes indicate initial offset

            int offset = 4;
            fixed (byte* ptr = buffer)
            {
                foreach (var p in parsed)
                {
                    indices.Add(p.name, offset);
                    var flow = (Flow*)(ptr + offset);
                    offset += Flow.Size;
                    flow->rules = (byte)p.rules.Count;
                    foreach (var (field, operand, reqVal, trg) in p.rules)
                    {
                        var rule = (Rule*)(ptr + offset);
                        offset += Rule.Size;
                        rule->fieldAndOperand = GetFieldAndOperand(field, operand);
                        rule->reqValue = reqVal;
                    }
                }
                offset = 4;
                foreach (var p in parsed)
                {
                    if (p.name == "in")
                        *(int*)ptr = offset;

                    var flow = (Flow*)(ptr + offset);
                    offset += Flow.Size;

                    flow->@default = indices[p.@default];
                    foreach (var (field, operand, reqVal, trg) in p.rules)
                    {
                        var rule = (Rule*)(ptr + offset);
                        offset += Rule.Size;
                        rule->trgOffset = indices[trg];
                    }
                }
            }

            return buffer;
            // one liner :)
            // it combines operation and what field operation is to act upon, into one byte
            byte GetFieldAndOperand(char fld, char op) => (byte)(fld switch { Xchar => xField, Mchar => mField, Achar => aField, Schar => sField, _ => throw new NotImplementedException() } | op switch { LTchar => opLess, GTchar => opGreater, _ => throw new NotImplementedException() });
        }
        private static Span<Part> ParseAndBuildParts(string[] input)
        {
            var elIndex = input.ToList().IndexOf("");

            var parts = new Part[input.Length - elIndex - 1];

            for (int i = elIndex + 1; i < input.Length; i++)
            {
                // we assume that values will be in XMAS order
                var sp = input[i][1..^1].Split(new char[] { ',', '=', Xchar, Mchar, Achar, Schar }, StringSplitOptions.RemoveEmptyEntries);
                parts[i - elIndex - 1] = new Part { x = long.Parse(sp[0]), m = long.Parse(sp[1]), a = long.Parse(sp[2]), s = long.Parse(sp[3]) };
            }
            return parts.AsSpan();
        }

        private unsafe static void Process(Span<Part> parts, int partindex, byte[] flowBuffer)
        {
            fixed (byte* ptr = flowBuffer)
            {
                int offset = *((int*)ptr);
                while (true)
                {
                    var flow = (Flow*)(ptr + offset); offset += Flow.Size;
                    var transferred = false;
                    for (int i = 0; i < flow->rules; i++)
                    {
                        var rule = (Rule*)(ptr + offset);
                        offset = Operate(offset, rule, parts[partindex], out var noOp);
                        transferred = !noOp;
                        if (transferred)
                            break;
                    }
                    if (!transferred)
                    {
                        // default here
                        offset = flow->@default;
                    }

                    if (offset == ErrorIndex) throw new InvalidOperationException("Error value detected");
                    if (offset == AcceptIndex)
                    {
                        parts[partindex].Accepted = true;
                        return;
                    }
                    if (offset == RejectIndex)
                    {
                        parts[partindex].Accepted = false;
                        return;
                    }
                }
            }

            unsafe int Operate(int currentOffset, Rule* rule, Part part, out bool noOp)
            {
                noOp = false;
                // get op value
                var opValue = rule->reqValue;
                var opFlags = rule->fieldAndOperand;
                var field = opFlags & 0x0F;

                if ((opFlags & opGreater) == opGreater)
                {
                    if (field == xField && part.x > opValue) return rule->trgOffset;
                    else if (field == mField && part.m > opValue) return rule->trgOffset;
                    else if (field == aField && part.a > opValue) return rule->trgOffset;
                    else if (field == sField && part.s > opValue) return rule->trgOffset;
                }
                else if ((opFlags & opLess) == opLess)
                {
                    if (field == xField && part.x < opValue) return rule->trgOffset;
                    else if (field == mField && part.m < opValue) return rule->trgOffset;
                    else if (field == aField && part.a < opValue) return rule->trgOffset;
                    else if (field == sField && part.s < opValue) return rule->trgOffset;
                }
                noOp = true;
                return currentOffset + Rule.Size;
            }
        }

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(string[] input, int lineWidth, int count)
        {
            // parse and construct
            var flowbuffer = BuildWorkflowsSystem(input);
            var parts = ParseAndBuildParts(input);

            // process
            for (int i = 0; i < parts.Length; i++)
                Process(parts, i, flowbuffer);

            // get answer
            long answer = 0L;
            for (int i = 0; i < parts.Length; i++)
                answer += parts[i].Sum;

            Log.WriteLine($"Used {flowbuffer.Length:N0} bytes for commands");
            return answer;
        }


        private static long Sum(Dictionary<char, (long start, long end)> ranges) => ranges.Aggregate(1L, (a, b) => a *= (b.Value.end - b.Value.start + 1));
        private static long ProcessGreaterThan(Dictionary<char, (long start, long end)> ranges, RuleSet flows, (char field, char operand, long reqVal, string trg) rule)
        {
            if (rule.trg == RejectValue)
                return 0;

            Dictionary<char, (long start, long end)> newRanges = new Dictionary<char, (long start, long end)>(ranges);

            // get max value required by this rule
            var max = Math.Max(newRanges[rule.field].start, rule.reqVal + 1);
            newRanges[rule.field] = (max, newRanges[rule.field].end);

            if (rule.trg == AcceptValue)
                return Sum(newRanges);

            return MakeSumRanges(newRanges, flows, flows.SingleOrDefault(f => f.name == rule.trg));
        }
        private static long ProcessLessThan(Dictionary<char, (long start, long end)> ranges, RuleSet flows, (char field, char operand, long reqVal, string trg) rule)
        {
            if (rule.trg == RejectValue)
                return 0;

            Dictionary<char, (long start, long end)> newRanges = new(ranges);

            // store new min value for this range
            var min = Math.Min(newRanges[rule.field].end, rule.reqVal - 1);
            newRanges[rule.field] = (newRanges[rule.field].start, min);

            if (rule.trg == AcceptValue)
                return Sum(newRanges);

            return MakeSumRanges(newRanges, flows, flows.SingleOrDefault(f => f.name == rule.trg));
        }

        private static long MakeSumRanges(Dictionary<char, (long start, long end)> ranges, RuleSet flows, (string name, string @default, List<(char field, char operand, long reqVal, string trg)> rules) startFlow)
        {
            long sum = 0;

            // process rules
            foreach (var rule in startFlow.rules)
            {
                Dictionary<char, (long start, long end)> newRanges = new(ranges);

                if (rule.operand == GTchar && ranges[rule.field].end > rule.reqVal)
                {
                    sum += ProcessGreaterThan(ranges, flows, rule);
                    ranges[rule.field] = (ranges[rule.field].start, rule.reqVal);
                }
                else if (ranges[rule.field].start < rule.reqVal)
                {
                    sum += ProcessLessThan(ranges, flows, rule);
                    ranges[rule.field] = (rule.reqVal, ranges[rule.field].end);
                }
            }

            if (startFlow.@default == RejectValue)
                return sum;

            if (startFlow.@default == AcceptValue)
                return sum + Sum(ranges);

            return sum + MakeSumRanges(ranges, flows, flows.SingleOrDefault(f => f.name == startFlow.@default));
        }

        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string[] input, int lineWidth, int count)
        {
            var rules = ParseRules(input);

            return MakeSumRanges(new()
            {
                {Xchar, (1, 4000) },
                {Mchar, (1, 4000) },
                {Achar, (1, 4000) },
                {Schar, (1, 4000) }
            }, rules, rules.SingleOrDefault(f => f.name == "in"));
        }
    }
}