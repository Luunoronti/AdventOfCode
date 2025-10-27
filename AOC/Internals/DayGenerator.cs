// attempt to keep the code as clean as possible
// most of functionality is in Core and Extensions folder


using System.Net;
using System.Text.RegularExpressions;

static class DayGenerator
{
    public static void GenerateDaysIfRequired()
    {
        var cmdLine = CommandLine.Parse();
        var year = cmdLine.GetResult(CommandLine.Year);
        var day = cmdLine.GetResult(CommandLine.Day);
        var noNewDay = cmdLine.GetResult(CommandLine.NoCode);

        if (year is not null && day is not null && !year.Implicit && !day.Implicit)
        {
            int y = year.GetValueOrDefault<int>();
            int d = day.GetValueOrDefault<int>();

            if (y >= 2016 && y <= DateTime.Now.Year && d >= 1 && d <= 25)
            {
                CreateDayIfDoesNotExist(y, d, noNewDay?.GetValueOrDefault<bool>() ?? false);
                Configuration.SaveExecutionConfiguration();
                return;
            }

            // wrong times: print error
            Console.WriteLine("You can specify year between 2016 and current year, and days between 1 and 25, inclusive.");
            return;
        }

        // else just create one that is today (but make sure we are running December)
        if (DateTime.Now.Month == 12)
        {
            CreateDayIfDoesNotExist(DateTime.Now.Year, DateTime.Now.Day);
            Configuration.SaveExecutionConfiguration();
            return;
        }

        var allSince = cmdLine.GetResult(CommandLine.AllSince);
        if (allSince is not null && !allSince.Implicit)
        {
            int layeryear = DateTime.Now.Month == 12 ? DateTime.Now.Year : DateTime.Now.Year - 1;
            for (int y = allSince.GetValueOrDefault<int>(); y <= layeryear; ++y)
            {
                var lastday = 0;
                if (layeryear == y && DateTime.Now.Month == 12) lastday = DateTime.Now.Day;
                for (int d = 1; d <= lastday; ++d)
                {
                    CreateDayIfDoesNotExist(y, d);
                }
            }
            Configuration.SaveExecutionConfiguration();
        }

        if (Configuration.Execution.Years.Count == 0)
        {
            Console.WriteLine("Please specify target day to work with (using --year and --day options).");

            return;
        }
    }


    private static void CreateDayIfDoesNotExist(int year, int day, bool noCode = false)
    {
        if (day > 25) return;

        var prefix = $"{Configuration.RootPath}{year}\\Day{day:D2}\\";
        Directory.CreateDirectory(prefix);

        if (!noCode && File.Exists(prefix + $"Day{day:D2}.cs") == false)
            File.WriteAllText(prefix + $"Day{day:D2}.cs", DayTemplateCode.Replace("{Year}", year.ToString()).Replace("{Day}", day.ToString("D2")));

        // create configuration if there is none
        if (File.Exists(prefix + $"config.yaml") == false)
        {
            // download the page
            var page = GetMainPage(year, day);

            // parse for title
            var title = ExtractTitle(page);

            File.WriteAllText(prefix + $"config.yaml", DayTemplateConfig
                .Replace("{Year}", year.ToString())
                .Replace("{Day}", day.ToString("D2"))
                .Replace("{Title}", title)
                );
        }
        // and attempt downloading live data
        if (File.Exists(prefix + $"live.txt") == false || File.Exists(prefix + $"live.txt") == false)
            UpdateLiveDataForADay(year, day);

        if (!Configuration.Execution.Years.Any(y => y.Year == year))
            Configuration.Execution.Years.Add(new Configuration.YearConfiguration() { Year = year });

        if (!Configuration.Execution.Years.Single(y => y.Year == year).Days.Any(d => d.Day == day))
            Configuration.Execution.Years.Single(y => y.Year == year).Days.Add(new Configuration.DayConfiguration
            {
                Day = day,
                EnableVisualization = false,
                DebugRun = true,
                Run = true,
            });
    }

    public static string ExtractTitle(string html)
    {
        var match = Regex.Match(html, @"<h2>--- Day \d+: (.*?) ---</h2>");
        return match.Success ? match.Groups[1].Value : "Title not found";
    }

    public static void UpdateLiveDataForADay(int year, int day)
    {
        if (day > 25) return;
        var prefix = $"{Configuration.RootPath}{year}\\Day{day:D2}\\";
        Directory.CreateDirectory(prefix);
        string liveData = GetLiveData(year, day);
        File.WriteAllText(prefix + $"live.txt", liveData);
    }

    // Note: If we are to run live data, download them from AoC. 
    // Huge thanks to Nick Kusters (https://www.youtube.com/@NKCSS) for pointing out that live data should not be kept on GitHub,
    // and allowing to copy his download code.
    private static string GetLiveData(int year, int day)
    {
        string session = "";
        try
        {
            session = File.ReadAllText($"{Configuration.RootPath}session.txt");
        }
        catch
        {
            Console.WriteLine($"Unable to find session file. Please provide a valid session in {Path.GetFullPath(Configuration.RootPath)}session.txt");
            return "no session file";
        }

        try
        {
            Console.WriteLine($"Downloading live data for year {year}, Day {day}");
            string url = $"https://adventofcode.com/{year}/day/{day}/input";

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var wc = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            wc.Headers.Add(HttpRequestHeader.UserAgent, "https://github.com/Luunoronti/AdventOfCode");
            wc.Headers.Add(HttpRequestHeader.Cookie, $"{nameof(session)}={session}");
            string contents = wc.DownloadString(url);
            return contents;

        }
        catch (Exception ex)
        {
            return $"Unable to obtain live data: {ex}";
        }
    }
    private static string GetMainPage(int year, int day)
    {
        try
        {
            Console.WriteLine($"Downloading main page for year {year}, Day {day}");
            string url = $"https://adventofcode.com/{year}/day/{day}";

#pragma warning disable SYSLIB0014 // Type or member is obsolete
            var wc = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
            string contents = wc.DownloadString(url);
            return contents;
        }
        catch (Exception ex)
        {
            return $"Unable to obtain main page: {ex}";
        }
    }


    private const string DayTemplateCode = @"
namespace Year{Year};

class Day{Day}
{
    public string Part1(PartInput Input)
    {
        long response = Input.LineWidth;
        return response.ToString();
    }
    public string Part2(PartInput Input)
    {
        long response = Input.LineWidth;
        return response.ToString();
    }
}
";

    private const string DayTemplateConfig = @"
name: {Title}
year: {Year}
day: {Day}
run: true
debugRun: true
visualization: false
runLive: true
runTests: true

tests:
  - part: 1
    run: true
    debugRun: true
    expected:
    knownErrors:
    visualization: false
    source: 
    
  - part: 2
    run: true
    debugRun: true
    expected:
    knownErrors:
    visualization: false
    source: 
    
live:
  - part: 1
    run: true
    debugRun: true
    expected:
    knownErrors:
    visualization: false
    source: live.txt

  - part: 2
    run: true
    debugRun: true
    expected:
    knownErrors:
    
    visualization: false
    source: live.txt
";

}