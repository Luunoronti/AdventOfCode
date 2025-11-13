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

    class PartRunner
    {
        public RunLocalConfiguration Config { get; internal set; }
        public RunLocalConfiguration.PartCfg PartConfig { get; internal set; }

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
            // warm up
#if !DEBUG
            _ = Handler(Input);
#endif
            Visualizer.ts = TimeSpan.Zero;
            Visualizer.RanUsingVisualizer = false;
            var startTime = Stopwatch.GetTimestamp();
            var result = Handler(Input);
            RunReport.AddResult(result, Visualizer.RanUsingVisualizer ? Visualizer.ts : Stopwatch.GetElapsedTime(startTime), Config, PartConfig, RunTests);
        }
        private void RunInternal(bool RunTests)
        {

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

                PartInput input = new()
                {
                    FullString = output
                };
                input.Lines = input.FullString.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
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
            public int Part { get; set; }
            public bool Run { get; set; }
            public bool DebugRun { get; set; }
            public string Expected { get; set; } = "";
            public List<string> KnownErrors { get; set; } = [];
            public string Source { get; set; } = "";
        }
        public string Name { get; set; } = "";
        public int Year { get; set; }
        public int Day { get; set; }

        public List<PartCfg> Tests { get; set; } = [];
        public List<PartCfg> Live { get; set; } = [];
    }
}
