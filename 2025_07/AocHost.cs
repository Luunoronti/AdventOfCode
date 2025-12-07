using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
namespace AoC;


public static class AocHost
{
    public static async Task RunAsync(string[] Args, Func<string[], long> SolvePart1, Func<string[], long> SolvePart2)
    {
        var DefaultInputKind = GetDefaultInputKind(SolvePart1.Method);
        AocConfig Config = AocConfig.Parse(Args, DefaultInputKind);

        if (Config.Benchmark)
        {
            var BenchmarkArgs = FilterBenchmarkArgs(Args);
            BenchmarkRunner.Run<AocBenchmarks>(args: BenchmarkArgs);
            return;
        }

        var Lines = await AocInput.LoadAsync(Config);
        var Title = await AocTitle.TryGetTitleAsync(Config);

        var Part1Result = SolvePart1(Lines);
        var Part2Result = SolvePart2(Lines);

        var expected1 = GetExpectedResult(SolvePart1.Method, Config.InputKind);
        var expected2 = GetExpectedResult(SolvePart2.Method, Config.InputKind);

        PrintHeader(Config, Title);
        PrintResult("Part 1", Part1Result, expected1);
        PrintResult("Part 2", Part2Result, expected2);
    }
    private static string? GetDefaultInputKind(MethodInfo Method)
    {
        Type? SolverType = Method.DeclaringType;
        if (SolverType is null)
            return null;

        var Attr = SolverType.GetCustomAttribute<DefaultInputAttribute>(inherit: false);
        return Attr?.InputKind;
    }

    private static string? GetExpectedResult(MethodInfo method, string inputKind)
    {
        var attrs = method.GetCustomAttributes(typeof(ExpectedResultAttribute), inherit: false).Cast<ExpectedResultAttribute>();

        foreach (var attr in attrs)
        {
            if (string.Equals(attr.InputKind, inputKind, StringComparison.OrdinalIgnoreCase))
                return attr.Value;
        }

        return null;
    }

    private static string[] FilterBenchmarkArgs(string[] Args)
    {
        var List = new List<string>(Args.Length);

        foreach (string Arg in Args)
        {
            if (Arg.StartsWith("--benchmark", StringComparison.OrdinalIgnoreCase) ||
                Arg.Equals("--bench", StringComparison.OrdinalIgnoreCase) ||
                Arg.StartsWith("--input=", StringComparison.OrdinalIgnoreCase) ||
                Arg.StartsWith("--inputFile=", StringComparison.OrdinalIgnoreCase) ||
                Arg.StartsWith("--year=", StringComparison.OrdinalIgnoreCase) ||
                Arg.StartsWith("--day=", StringComparison.OrdinalIgnoreCase))
            {
                continue; // these are handled by AocConfig, not BenchmarkDotNet
            }

            List.Add(Arg);
        }

        return List.ToArray();
    }

    private static void PrintHeader(AocConfig Config, string? Title)
    {
        if (!string.IsNullOrEmpty(Title))
            Console.WriteLine($"AoC {Config.Year} Day {Config.Day:00}: {Title} ({Config.InputKind})");
        else
            Console.WriteLine($"AoC {Config.Year} Day {Config.Day:00} ({Config.InputKind})");

        Console.WriteLine(new string('-', 40));
    }
    private static void PrintResult(string Name, object? Value, string? expected)
    {
        var result = Value?.ToString() ?? "<null>";

        bool ok = false;
        if (Value is not null)
        {
            ok = string.Equals(result, expected, StringComparison.OrdinalIgnoreCase);
        }

        var oldC = Console.ForegroundColor;
        Console.ForegroundColor = Value == null ? (oldC) : (ok ? ConsoleColor.Green : ConsoleColor.Red);
        if (result.Contains(Environment.NewLine))
        {
            Console.WriteLine($"{Name}:");
            Console.WriteLine(result);
        }
        else
        {
            Console.WriteLine($"{Name}: {result}");
        }
        Console.ForegroundColor = oldC;
    }
}

// --------------------------------------------------
// Configuration (CLI only)
// --------------------------------------------------

public sealed class AocConfig
{
    public int Year { get; init; }
    public int Day { get; init; }

    /// <summary>
    /// "test" -> test.txt
    /// "live" -> live.txt
    /// otherwise treated as literal file path.
    /// </summary>
    public string InputKind { get; init; } = "live";

    /// <summary>
    /// Optional explicit path. If set, overrides InputKind mapping.
    /// </summary>
    public string? InputFileOverride { get; init; }

    public bool Benchmark { get; init; }

    /// <summary>
    /// AoC session cookie value (e.g. from AOC_SESSION env var).
    /// </summary>
    public string? SessionToken { get; init; }

    public static AocConfig Parse(string[] args, string? DefaultInputKind = null)
    {
        // Defaults baked into the template (replaced by dotnet new)
        int year = 2025;
        int day = 7;

        string InputKind = string.IsNullOrEmpty(DefaultInputKind) ? "live" : DefaultInputKind;
        string? inputFileOverride = null;
        bool benchmark = false;

        string? session = Environment.GetEnvironmentVariable("AOC_SESSION");

        foreach (string arg in args)
        {
            if (arg.StartsWith("--year=", StringComparison.OrdinalIgnoreCase))
            {
                year = int.Parse(arg.AsSpan("--year=".Length));
            }
            else if (arg.StartsWith("--day=", StringComparison.OrdinalIgnoreCase))
            {
                day = int.Parse(arg.AsSpan("--day=".Length));
            }
            else if (arg.StartsWith("--input=", StringComparison.OrdinalIgnoreCase))
            {
                InputKind = arg.Substring("--input=".Length);
            }
            else if (arg.StartsWith("--inputFile=", StringComparison.OrdinalIgnoreCase))
            {
                inputFileOverride = arg.Substring("--inputFile=".Length);
            }
            else if (arg.Equals("--bench", StringComparison.OrdinalIgnoreCase) || arg.Equals("--benchmark", StringComparison.OrdinalIgnoreCase))
            {
                benchmark = true;
            }
        }

        return new AocConfig
        {
            Year = year,
            Day = day,
            InputKind = InputKind,
            InputFileOverride = inputFileOverride,
            Benchmark = benchmark,
            SessionToken = session
        };
    }
}

// --------------------------------------------------
// Input loader + AoC downloader
// --------------------------------------------------

public static class AocInput
{
    internal static string ProjectRoot => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    
    public static async Task<string[]> LoadAsync(AocConfig config)
    {
        string filePath = ResolveInputPath(config);

        if (File.Exists(filePath))
            return await File.ReadAllLinesAsync(filePath);

        // If it's the live input file and doesn't exist yet: download it.
        if (IsLiveFile(config, filePath))
        {
            if (string.IsNullOrEmpty(config.SessionToken))
                {
                    Console.WriteLine("==========================================================================");
                    Console.WriteLine("Session cookie environment variable is not set.");
                    Console.WriteLine("You can set it from command line, using: ");
                    Console.WriteLine("     setx AOC_SESSION \"your-long-session-cookie\"   (Windows)    ");
                    Console.WriteLine("\nNote: you may need to restart terminal session.");
                    Console.WriteLine("==========================================================================");

                    throw new InvalidOperationException("AOC_SESSION environment variable (session cookie) is not set.");
                }

            await DownloadLiveAsync(config, filePath);
            return await File.ReadAllLinesAsync(filePath);
        }

        throw new FileNotFoundException($"Input file not found: {filePath}");
    }

   private static string ResolveInputPath(AocConfig Config)
    {
        string Root = ProjectRoot;

        if (!string.IsNullOrEmpty(Config.InputFileOverride))
        {
            return Path.IsPathRooted(Config.InputFileOverride) ? Config.InputFileOverride : Path.Combine(Root, Config.InputFileOverride);
        }

        string FileName = Config.InputKind switch
        {
            "test" => "test.txt",
            "live" => "live.txt",
            _      => Config.InputKind // treat as literal file name
        };

        return Path.IsPathRooted(FileName) ? FileName : Path.Combine(Root, FileName);
    }


    private static bool IsLiveFile(AocConfig config, string filePath) => config.InputKind.Equals("live", StringComparison.OrdinalIgnoreCase) || string.Equals(Path.GetFileName(filePath), "live.txt", StringComparison.OrdinalIgnoreCase);

    private static async Task DownloadLiveAsync(AocConfig config, string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? ".");

        string url = $"https://adventofcode.com/{config.Year}/day/{config.Day}/input";

        var handler = new HttpClientHandler
        {
            CookieContainer = new CookieContainer()
        };

        handler.CookieContainer.Add(new Uri("https://adventofcode.com"), new Cookie("session", config.SessionToken!));

        using var client = new HttpClient(handler);
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AocRunner", "1.0"));
        string body = await client.GetStringAsync(url);
        await File.WriteAllTextAsync(filePath, body);
    }
}

// --------------------------------------------------
// BenchmarkDotNet harness
// --------------------------------------------------
[Config(typeof(AoCConfig))]
[MemoryDiagnoser]
public class AocBenchmarks
{
    private class AoCConfig : ManualConfig
    {
        public AoCConfig()
        {
            AddColumn(StatisticColumn.Mean, StatisticColumn.Error, StatisticColumn.StdDev, StatisticColumn.Median);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddColumn(MeanMsColumn.Instance);
        }
    }

    private string[] _lines = Array.Empty<string>();
    private AocConfig config;
    [GlobalSetup]
    public async Task Setup()
    {
        // Uses template defaults (2025 / 7),
        config = AocConfig.Parse(Array.Empty<string>());
        //_lines = await AocInput.LoadAsync(config);
    }

    [Benchmark]
    public void Part1()
    {
        var lines = AocInput.LoadAsync(config).Result;
        Solver.SolvePart1(lines);
    }

    [Benchmark]
    public void Part2()
    {
        var lines = AocInput.LoadAsync(config).Result;
        Solver.SolvePart2(lines);
    }
}

public sealed class MeanMsColumn : IColumn
{
    public static IColumn Instance { get; } = new MeanMsColumn();

    public string Id => "MeanMs";
    public string ColumnName => "Mean";
    public string Legend => "";//"Mean time per op in milliseconds";
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Statistics;
    public int PriorityInCategory => -10;
    public bool IsNumeric => true;
    public UnitType UnitType => UnitType.Time;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase) => GetValue(summary, benchmarkCase, summary.Style);

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        var report = summary[benchmarkCase];
        var statistics = report?.ResultStatistics;
        if (statistics == null || double.IsNaN(statistics.Mean)) return "NA";
        var meanNanoseconds = statistics.Mean;
        var meanMilliseconds = meanNanoseconds / 1_000_000.0;
        return $"{meanMilliseconds.ToString("F3", style.CultureInfo)} ms";
    }

    public bool IsAvailable(Summary summary) => true;
    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
}



public static class AocTitle
{
    public static async Task<string?> TryGetTitleAsync(AocConfig Config)
    {
        string FilePath = Path.Combine(AocInput.ProjectRoot, "title.txt");

        // 1) Try cached title first
        if (File.Exists(FilePath))
        {
            string Cached = await File.ReadAllTextAsync(FilePath);
            if (!string.IsNullOrWhiteSpace(Cached))
                return Cached.Trim();
        }

        // 2) Download from AoC
        string Url = $"https://adventofcode.com/{Config.Year}/day/{Config.Day}";

        using var Client = new HttpClient();
        Client.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("AocRunner", "1.0"));

        string Html;
        try
        {
            Html = await Client.GetStringAsync(Url);
        }
        catch
        {
            // Network / HTTP error – just skip the title
            return null;
        }

        string? Title = ExtractTitle(Html);
        if (!string.IsNullOrEmpty(Title))
        {
            try
            {
                await File.WriteAllTextAsync(FilePath, Title);
            }
            catch
            {
                // Ignore caching errors
            }
        }

        return Title;
    }

    private static string? ExtractTitle(string Html)
    {
        const string H2Start = "<h2>";
        const string H2End = "</h2>";

        int StartIndex = Html.IndexOf(H2Start, StringComparison.OrdinalIgnoreCase);
        if (StartIndex < 0) return null;

        StartIndex += H2Start.Length;
        int EndIndex = Html.IndexOf(H2End, StartIndex, StringComparison.OrdinalIgnoreCase);
        if (EndIndex < 0) return null;

        string Inner = Html.Substring(StartIndex, EndIndex - StartIndex).Trim();

        // Typically: "--- Day 4: Title ---"
        if (Inner.StartsWith("---"))
            Inner = Inner.Trim('-').Trim();

        int ColonIndex = Inner.IndexOf(':');
        if (ColonIndex >= 0 && ColonIndex + 1 < Inner.Length)
            Inner = Inner[(ColonIndex + 1)..].Trim();

        // Remove trailing "---" if still present
        Inner = Inner.Trim('-').Trim();

        return Inner;
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ExpectedResultAttribute : Attribute
{
    public string InputKind { get; }
    public string Value { get; }

    public ExpectedResultAttribute(string inputKind, string value)
    {
        InputKind = inputKind;
        Value = value;
    }

    public ExpectedResultAttribute(string inputKind, long value)
    {
        InputKind = inputKind;
        Value = value.ToString();
    }

    public ExpectedResultAttribute(string inputKind, int value)
    {
        InputKind = inputKind;
        Value = value.ToString();
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class DefaultInputAttribute : Attribute
{
    public string InputKind { get; }

    public DefaultInputAttribute(string inputKind)
    {
        InputKind = inputKind;
    }
}