// attempt to keep the code as clean as possible
// most of functionality is in Core and Extensions folder

using static DayRunner;

partial class RunReport
{
    private static List<SingleRunReport> results = [];

    public static void AddResult(string result, TimeSpan time, RunLocalConfiguration localConfig, RunLocalConfiguration.PartCfg part, bool RunTests)
    {
        results.Add(new SingleRunReport(time, localConfig.Year, localConfig.Day, part.Part, RunTests)
        {
            Name = localConfig.Name,
            Result = GetResultResult(part, RunTests, result),
            ResultValue = result,
        });
    }

    enum ResultResult
    {
        Unknown,
        Good,
        TooLow,
        TooLowKnown,
        TooHigh,
        TooHighKnown,
        UnknownBad,
        KnownBad,
    }

    private static ResultResult GetResultResult(RunLocalConfiguration.PartCfg part, bool isTest, string actualResult)
    {
        if (part == null) return ResultResult.Good;
        if (actualResult == part.Expected) return ResultResult.Good;
        if (string.IsNullOrEmpty(part.Expected) && part.KnownErrors.Count == 0) return ResultResult.Unknown;

        bool known = part.KnownErrors.Any(r => r == actualResult);

        return long.TryParse(actualResult, out var reslong) && long.TryParse(part.Expected, out var resexp)
            ? reslong < resexp
                ? known ? ResultResult.TooLowKnown : ResultResult.TooLow
                : known ? ResultResult.TooHighKnown : ResultResult.TooHigh
            : known ? ResultResult.KnownBad : ResultResult.UnknownBad;
    }
    public static void PrintResults()
    {
        Table table = new();
        table.AddColumn("Part");
        table.AddColumn("Result");
        table.AddColumn("Result Remarks");
        table.AddColumn("Time");

        var lastDay = -1;
        var lastYear = -1;

        foreach (var result in results)
        {
            if (lastDay != result.Day && lastYear != result.Year)
            {
                lastDay = result.Day;
                lastYear = result.Year;
                table.AddDayEntry(lastDay, lastYear, result.Name);
            }
            table.AddValue("Part", $"{result.Part} - {(result.IsTestRun ? "Test" : "Live")}", result.IsTestRun ? ConsoleColor.Yellow : ConsoleColor.White);

            table.AddValue("Result", $"{result.ResultValue}",
                result.Result switch
                {
                    ResultResult.Good => ConsoleColor.Green,
                    ResultResult.Unknown => ConsoleColor.Cyan,
                    _ => ConsoleColor.Red,
                });

            table.AddValue("Result Remarks", $"{result.Result switch
            {
                ResultResult.TooLow => "too low",
                ResultResult.TooLowKnown => "known, too low",
                ResultResult.TooHigh => "too high",
                ResultResult.TooHighKnown => "known, too high",
                ResultResult.KnownBad => "known",
                ResultResult.UnknownBad => "not known",
                _ => "",
            }}",
               result.Result switch
               {
                   ResultResult.Good => ConsoleColor.Green,
                   ResultResult.TooLow => ConsoleColor.Red,
                   ResultResult.TooLowKnown => ConsoleColor.Magenta,
                   ResultResult.TooHigh => ConsoleColor.Red,
                   ResultResult.TooHighKnown => ConsoleColor.Magenta,
                   ResultResult.UnknownBad => ConsoleColor.Red,
                   ResultResult.KnownBad => ConsoleColor.Magenta,
                   ResultResult.Unknown => ConsoleColor.Cyan,
                   _ => ConsoleColor.White,
               });

            table.AddValue("Time", result.Time.FormatUltraPrecise(), ConsoleColor.Gray);
        }

        table.Format();
        table.Print();
    }
}
