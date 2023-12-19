using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

//using RuleSet = System.Collections.Generic.List<(string name, string @default, System.Collections.Generic.List<(int field, byte operand, long reqVal, string trg)> rules)>;

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

        private const int Xid = 0;
        private const int Mid = 1;
        private const int Aid = 2;
        private const int Sid = 3;

        private const char GTchar = '>';
        private const char LTchar = '<';
        private enum Operation { GreaterThan, LessThan }

        private const int ErrorIndex = -1;
        private const int AcceptIndex = -2;
        private const int RejectIndex = -3;

        private const byte opGreater = 0x10;
        private const byte opLess = 0x20;

        private const byte xField = 1;
        private const byte mField = 2;
        private const byte aField = 3;
        private const byte sField = 4;

        // just want to play with direct memory representation, and see if it's going to be faster than
        // using collections
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

        private static List<(string name, string @default, List<(int field, Operation operand, long reqVal, string trg)> rules)> 
            ParseRules(string[] input)
        {
            List<(string name, string @default, List<(int field, Operation operand, long reqVal, string trg)> rules)> parsedRules = new();

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

                List<(int, Operation, long, string)> rules = new();
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

                        rules.Add((GetIndex(field), GetOperand(op), value, trg));
                    }
                }
                parsedRules.Add((name, defaultRule, rules));
            }
            return parsedRules;
        }

        private static int GetIndex(char ch) => ch switch
        {
            Xchar => 0,
            Mchar => 1,
            Achar => 2,
            Schar => 3,
            _ => throw new NotImplementedException(),
        };
        private static Operation GetOperand(char op)
        {
            if (op == GTchar) return Operation.GreaterThan;
            return Operation.LessThan;
        }

        private unsafe static byte[] BuildWorkflowsSystem(string[] input)
        {
            var parsed = ParseRules(input);
            var indices = new Dictionary<string, int>
            {
                [ErrorValue] = ErrorIndex,
                [AcceptValue] = AcceptIndex,
                [RejectValue] = RejectIndex
            };

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
        }

        // one liner :)
        // it combines operation code and which field is to act upon, into one byte
        private static byte GetFieldAndOperand(int fld, Operation op) => (byte)(fld switch { Xid => xField, Mid => mField, Aid => aField, Sid => sField, _ => throw new NotImplementedException() } | op switch { Operation.LessThan => opLess, Operation.GreaterThan => opGreater, _ => throw new NotImplementedException() });

        private static Span<Part> ParseAndBuildParts(string[] input)
        {
            var elIndex = input.ToList().IndexOf("");
            // we assume that values will be in XMAS order
            return input[(elIndex + 1)..]
                .Select(l => l[1..^1].Split(new char[] { ',', '=', Xchar, Mchar, Achar, Schar }, StringSplitOptions.RemoveEmptyEntries))
                .Select(sp => new Part { x = long.Parse(sp[0]), m = long.Parse(sp[1]), a = long.Parse(sp[2]), s = long.Parse(sp[3]) })
                .ToArray().AsSpan();
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

            // moved this to own method, but we want our compiler to optimize it as much as possible
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [RemoveSpacesFromInput]
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


        private static long Aggregate(List<(long start, long end)> ranges) => ranges.Aggregate(1L, (acc, v) => acc *= v.end - v.start + 1);
        private static long ProcessGreaterThan((int field, Operation operand, long reqVal, string trg) rule, List<(string name, string @default, List<(int field, Operation operand, long reqVal, string trg)> rules)> flows, List<(long start, long end)> ranges)
        {
            // clone our ranges before we go any deeper
            List<(long start, long end)> newRanges = new(ranges);

            // get max value required by this rule
            var s = ranges[rule.field].start;
            var max = Math.Max(rule.reqVal + 1, s);
            newRanges[rule.field] = (max, ranges[rule.field].end);
            
            // if our rule is Accept, return our ranges
            if (rule.trg == AcceptValue)
                return Aggregate(newRanges);

            // it's not accept, nor reject, so go deeper
            return MakeSumRanges(flows.SingleOrDefault(f => f.name == rule.trg), flows, newRanges);
        }
        private static long ProcessLessThan((int field, Operation operand, long reqVal, string trg) rule, List<(string name, string @default, List<(int field, Operation operand, long reqVal, string trg)> rules)> flows, List<(long start, long end)> ranges)
        {
            List<(long start, long end)> newRanges = new(ranges);

            // store new min value for this range
            var e = ranges[rule.field].end;
            var min = Math.Min(rule.reqVal - 1, e);
            newRanges[rule.field] = (ranges[rule.field].start, min);

            if (rule.trg == AcceptValue)
                return Aggregate(newRanges);

            return MakeSumRanges(flows.SingleOrDefault(f => f.name == rule.trg), flows, newRanges);
        }

        private static long MakeSumRanges(
            (string name, string @default, List<(int field, Operation operand, long reqVal, string trg)> rules) current, 
            List<(string name, string @default, List<(int field, Operation operand, long reqVal, string trg)> rules)> flows, 
            List<(long start, long end)> ranges)
        {
            long sum = 0;

            // process rules, and step into all inner rule sets recurrently 
            foreach (var rule in current.rules)
            {
                var index = rule.field;

                if (rule.reqVal < ranges[index].end && rule.reqVal > ranges[index].start)
                {
                    if (rule.operand == Operation.GreaterThan)
                    {
                        if (rule.trg != RejectValue)
                            sum += ProcessGreaterThan(rule, flows, ranges);
                        ranges[index] = (ranges[index].start, rule.reqVal);
                    }
                    else 
                    {
                        if (rule.trg != RejectValue) 
                            sum += ProcessLessThan(rule, flows, ranges);
                        ranges[index] = (rule.reqVal, ranges[index].end);
                    }
                }
            }

            if (current.@default == RejectValue)
                return sum;

            if (current.@default == AcceptValue)
                return sum + Aggregate(ranges);

            return sum + MakeSumRanges(flows.SingleOrDefault(f => f.name == current.@default), flows, ranges);
        }

      
        [RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(string[] input, int lineWidth, int count)
        {
            var rules = ParseRules(input);
            return MakeSumRanges(rules.SingleOrDefault(f => f.name == "in"), rules, Enumerable.Range(1, 4).Select(i => (1L, 4000L)).ToList());
        }
    }
}