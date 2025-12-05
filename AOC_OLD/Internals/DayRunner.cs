// attempt to keep the code as clean as possible
// most of functionality is in Core and Extensions folder


using System.IO;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using TermGlass;
using System.Diagnostics;

static class DayRunner
{
    delegate string PartInputHandler(PartInput input);

    public readonly struct HandlerBenchResult
    {
        public long Calls
        {
            get;
        }
        public TimeSpan Total
        {
            get;
        }
        public double NsPerCall
        {
            get;
        }
        public double OpsPerSecond
        {
            get;
        }

        public HandlerBenchResult(long calls, TimeSpan total)
        {
            Calls = calls;
            Total = total;
            NsPerCall = calls == 0 ? 0.0 : (total.TotalMilliseconds * 1_000_000.0) / calls;
            OpsPerSecond = total.TotalSeconds == 0 ? double.PositiveInfinity : calls / total.TotalSeconds;
        }

        public override string ToString()
            => $"{NsPerCall:N1} ns/op, {OpsPerSecond:N0} ops/s (total {Total.TotalMilliseconds:N2} ms)";
    }

    private static HandlerBenchResult BenchmarkHandler(
        PartInputHandler handler,
        PartInput input,
        int warmupIterations = 3,
        int outerIterations = 10,
        int innerIterations = 1_000)
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));
        if (outerIterations <= 0) throw new ArgumentOutOfRangeException(nameof(outerIterations));
        if (innerIterations <= 0) throw new ArgumentOutOfRangeException(nameof(innerIterations));

        // 1. Warmup – JIT and touch data
        object? last = null;
        for (int i = 0; i < warmupIterations; i++)
        {
            last = handler(input);
        }

        GC.KeepAlive(last);

        // Optional: reduce GC noise a bit
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long calls = (long)outerIterations * innerIterations;

        // 2. Timed region
        long start = Stopwatch.GetTimestamp();
        for (int o = 0; o < outerIterations; o++)
        {
            for (int i = 0; i < innerIterations; i++)
            {
                last = handler(input);
            }
        }
        TimeSpan elapsed = Stopwatch.GetElapsedTime(start);
        GC.KeepAlive(last);

        return new HandlerBenchResult(calls, elapsed);
    }


    public class PartRunner
    {
        internal PartRunner()
        {
            Current = this;
        }
        public RunLocalConfiguration Config
        {
            get; internal set;
        }
        public RunLocalConfiguration.PartCfg PartConfig
        {
            get; internal set;
        }
        public bool IsTestRun
        {
            get; internal set;
        }
        public int TestNumber
        {
            get; internal set;
        }
        private bool LoadSourceFromFile(string file, out string output)
        {
            try
            {
                var path = $"{AoCApp.RootPath}{Config.Year}\\Day{Config.Day:D2}\\{file}";
                output = File.ReadAllText(path);
                return true;
            }
            catch { }
            output = "";
            return false;
        }
        private void TimedRun(bool RunTests, PartInputHandler Handler, PartInput Input)
        {
            Visualiser.ts = TimeSpan.Zero;
            Visualiser.RanUsingVisualizer = false;
            var startTime = Stopwatch.GetTimestamp();
            var result = Handler(Input);
            var time = Visualiser.RanUsingVisualizer ? Visualiser.ts : Stopwatch.GetElapsedTime(startTime);

            if (Config.Benchmark)
            {
                Console.WriteLine($"Benchmarking {Config.Name} - {PartConfig.Part} ({(RunTests ? "Test" : "Live")})");
                var bench = BenchmarkHandler(Handler, Input,
                                             warmupIterations: 3,
                                             outerIterations: 20,
                                             innerIterations: 1_000);

                time = TimeSpan.FromTicks((long)(bench.NsPerCall / 100.0));
            }

            RunReport.AddResult(result, time, Config, PartConfig, RunTests);
        }

        private void RunInternal(bool RunTests)
        {
            this.IsTestRun = RunTests;
            var tests = RunTests ? Config.Tests : Config.Live;
            for (int i = 0; i < tests.Count; i++)
            {
                var test = tests[i];
#if DEBUG
                if (!test.DebugRun)
                    continue;
#else
                if (!test.Run)
                    continue;
#endif
                PartConfig = test;
                PartInputHandler Handler = LoadHandler();

                if (Handler == null)
                    continue;

                Console.Title = $"Advent of Code by Amarthdae | Year {Config.Year} | Day {Config.Day} | {(RunTests ? $"Test" : "Live")} {i + 1} / {tests.Count} | Part {PartConfig.Part}";

                // attempt to load source as a file
                // if it fails, attempt to load in place
                if (!LoadSourceFromFile(test.Source, out var output))
                    output = test.Source;
                if (string.IsNullOrEmpty(output))
                    continue;

                this.TestNumber = i;
                PartInput input = new()
                {
                    FullString = output
                };
                input.Lines = input.FullString.Replace("\r", "").Split('\n');
                input.Span = input.FullString.AsSpan();
                input.Count = input.Lines.Length;
                input.LineWidth = input.Lines == null ? 0 : (input.Lines.Length > 0 ? input.Lines[0].Length : 0);

                TimedRun(RunTests, Handler, input);
            }
        }

        public void Run(bool RunTests)
        {
            RunInternal(RunTests);
        }
        private PartInputHandler LoadHandler()
        {
            int Year = Config.Year;
            int Day = Config.Day;
            int Part = PartConfig.Part;
            var type = Type.GetType($"Year{Year}.Day{Day:D2}");
            if (type == null)
            {
                Console.WriteLine($"Unable to find day type Year_{Year}.Day{Day:D2}");
                return null;
            }
            var part = type.GetMethod($"Part{Part}");
            if (part == null)
            {
                Console.WriteLine($"Unable to find Part{Part} method in type {type}");
                return null;
            }

            var obj = Activator.CreateInstance(type);
            if (obj == null)
            {
                Console.WriteLine($"Unable to create an instance from type {type}");
                return null;
            }

            return (PartInputHandler)part.CreateDelegate(typeof(PartInputHandler), obj);
        }

        public static PartRunner Current
        {
            get; private set;
        }

    }


    public static void Run()
    {
        if (RunDayFromCommandLine())
            return;
    }


    private static IDeserializer YamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    private static bool RunDay(int Year, int Day)
    {
        // load local configuration from yaml file
        var file = $"{AoCApp.RootPath}{Year}\\Day{Day:D2}\\config.yaml";
        var localConfig = YamlDeserializer.Deserialize<RunLocalConfiguration>(File.ReadAllText(file));
        localConfig?.OnAfterDeserialize();

        // create part runner
        PartRunner runner = new() { Config = localConfig };

        // create report for this day
        runner.Run(true);
        runner.Run(false);

        return true;
    }


    private static bool RunDayFromCommandLine()
    {
        var cmdLine = CommandLine.Parse();
        var year = cmdLine.GetResult(CommandLine.Year);
        var day = cmdLine.GetResult(CommandLine.Day);

        if (year is not null && day is not null && !year.Implicit && !day.Implicit)
        {
            int y = year.GetValueOrDefault<int>();
            int d = day.GetValueOrDefault<int>();

            // get this day from config. if not found, we skip
            // now load the day 

            if (y >= 2016 && y <= DateTime.Now.Year && d >= 1 && d <= 25)
            {
                if (RunDay(y, d))
                    return true;
            }
            else
            {
                Console.WriteLine("You can specify year between 2016 and current year, and days between 1 and 25, inclusive.");
                return true;
            }
        }
        return false;
    }

    internal class RunLocalConfiguration
    {
        internal void OnAfterDeserialize()
        {
            Tests ??= [];
            Live ??= [];

            foreach (var t in Tests)
                t.KnownErrors ??= [];
            foreach (var l in Live)
                l.KnownErrors ??= [];
        }
        internal class PartCfg
        {
            public int Part
            {
                get; set;
            }
            public bool Run
            {
                get; set;
            }
            public bool DebugRun
            {
                get; set;
            }
            public string Expected { get; set; } = "";
            public bool ShowVisualisation { get; set; } = false;
            public List<string> KnownErrors { get; set; } = [];
            public string Source { get; set; } = "";
        }
        public string Name { get; set; } = "";
        public int Year
        {
            get; set;
        }
        public int Day
        {
            get; set;
        }

        public bool Benchmark
        {
            get; set;
        } = false;
        public List<PartCfg> Tests { get; set; } = [];
        public List<PartCfg> Live { get; set; } = [];
    }
}
