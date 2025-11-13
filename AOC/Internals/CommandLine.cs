

class CommandLine
{
    public static Option Day { get; } = new Option<int>("--day", "Day");
    public static Option Year { get; } = new Option<int>("--year", "Year");
    public static Option AllSince { get; } = new Option<int>("--allsince", "AllSince");
    public static Option NoCode { get; } = new Option<bool>("--nocode", "NoCode");
    public static ParseResult Parse()
    {
        return Parse(Environment.GetCommandLineArgs());
    }
    static ParseResult _result;
    public static ParseResult Parse(string[] args)
    {
        if (_result == null)
        { }
        var rootCommand = new RootCommand("Advent of Code");
        rootCommand.Options.Add(Day);
        rootCommand.Options.Add(Year);
        rootCommand.Options.Add(AllSince);
        rootCommand.Options.Add(NoCode);
        _result = rootCommand.Parse(args);
        return _result;
    }
}