using AdventOfCode2023;
using System.Diagnostics;
using System.Reflection;

internal partial class Program
{
    [STAThread]
    private static void Main(string[] _)
    {
        var maxthreads = Math.Max(1, Environment.ProcessorCount);
        ThreadPool.SetMaxThreads(maxthreads, Environment.ProcessorCount);
        Console.WriteLine($"Max ThreadPool threads set to {CC.Sys}{maxthreads}{CC.Clr}");

        var year = DateTime.Now.Year;
        var ns = $"AdventOfCode{year}";
        var day = DateTime.Now.Day;
        var dn = $"Day{day:D2}";

        CreateDayIfDoesNotExist(year, day);

        var type = GetTypeToProcess(ns, dn);

        if (type == null)
        {
            Console.WriteLine($"{CC.Err}Type {dn} for year {year} does not exist. It has been recreated, please build your project and run it again. {CC.Clr}");
            return;
        }

        var useTestData = Debugger.IsAttached
            ? type.GetCustomAttribute<UseLiveDataInDeugAttribute>() == null
            : type.GetCustomAttribute<AlwaysUseTestDataAttribute>() != null;

        Console.WriteLine($"{CC.Sys}{(useTestData ? "Test" : "Live")}{CC.Clr} data is in use");

        Log.Enabled = Debugger.IsAttached
            ? type.GetCustomAttribute<DisableLogInDebugAttribute>() == null
            : type.GetCustomAttribute<AlwaysEnableLogAttribute>() != null;

        Console.WriteLine($"Logger is {CC.Sys}{(Log.Enabled ? "on" : "off")}{CC.Clr}");

        Console.WriteLine();

        var lines = ReadLines(type.GetProperty(useTestData ? "TestFile" : "LiveFile")?.GetValue(null) as string);
        var a1 = RunMethod(type, "Part1", lines);

        lines = ReadLines(type.GetProperty(useTestData ? "TestFile" : "LiveFile")?.GetValue(null) as string);
        var a2 = RunMethod(type, "Part2", lines);

        Console.WriteLine();
        Console.WriteLine($"{CC.Att}===>{CC.Clr} Part {CC.Sys}1{CC.Clr} answer: {CC.Ans}{a1}{CC.Clr}");
        Console.WriteLine($"{CC.Att}===>{CC.Clr} Part {CC.Sys}2{CC.Clr} answer: {CC.Ans}{a2}{CC.Clr}");
        Console.WriteLine();
        Console.WriteLine();

        if (a2 != 0)
        {
            Clipboard.SetText(a2.ToString());
            Console.WriteLine($"Answer {CC.Sys}2{CC.Clr} ({CC.Ans}{a2}{CC.Clr}) has been copied to clipboard automatically.");
        }
        else if (a1 != 0)
        {
            Clipboard.SetText(a1.ToString());
            Console.WriteLine($"Answer {CC.Sys}1{CC.Clr} ({CC.Ans}{a1}{CC.Clr}) has been copied to clipboard automatically.");
        }


        if (!Debugger.IsAttached)
        {
            Console.WriteLine($"Press {CC.Sys}1{CC.Clr} to copy first answer to clipboard, {CC.Sys}2{CC.Clr} to copy second answer, any other key to quit.");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.D1)
                {
                    Clipboard.SetText(a1.ToString());
                    Console.WriteLine($"Answer {CC.Sys}1{CC.Clr} ({CC.Ans}{a1}{CC.Clr}) copied.");
                }
                else if (key.Key == ConsoleKey.D2)
                {
                    Clipboard.SetText(a2.ToString());
                    Console.WriteLine($"Answer {CC.Sys}2{CC.Clr} ({CC.Ans}{a2}{CC.Clr}) copied.");
                }
                else break;
            }
        }
    }

    private static Type? GetTypeToProcess(string @namespace, string name) =>
            Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<ForceAttribute>() != null)
            .ForEach(t => Console.WriteLine($"Processing of type {CC.Cls}{t.Namespace}.{t.Name}{CC.Clr} is being forced via attribute."))
            .FirstOrDefault()
            ??
            Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.Namespace == @namespace && t.Name == name)
            .FirstOrDefault()
            ;

    private static long RunMethod(Type type, string method, string[] lines)
    {
        Console.WriteLine($"{CC.Att}===>{CC.Clr} Running {CC.Cls}{type.Namespace}.{type.Name}.{method}{CC.Clr}...");
        var sw = Stopwatch.StartNew();
        var met = type.GetMethod(method);
        if (met == null) return -1;

        var logStatus = Log.Enabled;
        if (Debugger.IsAttached && met.GetCustomAttribute<DisableLogInDebugAttribute>() != null)
            Log.Enabled = false;
        if (!Debugger.IsAttached && met.GetCustomAttribute<AlwaysEnableLogAttribute>() != null)
            Log.Enabled = true;

        var answer = (long)(type.GetMethod(method)?.Invoke(null, new object[] { lines }) ?? -1);
        sw.Stop();
        Console.WriteLine($"{CC.Att}===> {CC.Cls}{type.Namespace}.{type.Name}.{method} {CC.Clr}completed in {CC.Sys}{sw.ElapsedMilliseconds} ms ({sw.Elapsed}){CC.Clr}\n");

        Log.Enabled = logStatus;
        return answer;
    }

    private static string[] ReadLines(string? file)
    {
        var fileName = $"..\\..\\..\\{file}";
        var lines = File.ReadAllLines(fileName);
        if (lines.Length == 0)
        {
            Console.WriteLine($"{CC.Err}There are no lines to process. Did you forget to fill in data into the file?{CC.Clr}");
            Console.WriteLine($"{fileName}");
        }
        Console.WriteLine($"{CC.Att}===> {CC.Sys}{lines.Length}{CC.Clr} lines of data read from {CC.Sys}{Path.GetFileName(fileName)}{CC.Clr}");
        return lines;
    }

    private static void CreateDayIfDoesNotExist(int year, int day)
    {
        var prefix = $"..\\..\\..\\{year}\\{day:D2}\\";
        Directory.CreateDirectory(prefix);

        if (File.Exists(prefix + $"Day{day:D2}.cs") == false)
            File.WriteAllText(prefix + $"Day{day:D2}.cs", DayTemplateCode.Replace("{Year}", year.ToString()).Replace("{Day}", day.ToString("D2")));
        if (File.Exists(prefix + $"test.txt") == false)
            File.WriteAllText(prefix + $"test.txt", "");
        if (File.Exists(prefix + $"live.txt") == false)
            File.WriteAllText(prefix + $"live.txt", "");
    }

    private const string DayTemplateCode = @"namespace AdventOfCode{Year}
{
    //[Force] // uncomment to force processing this type
    //[AlwaysEnableLog]
    //[DisableLogInDebug]
    //[UseLiveDataInDeug]
    //[AlwaysUseTestData]
    class Day{Day}
    {
        public static string TestFile => ""{Year}\\{Day}\\test.txt"";
        public static string LiveFile => ""{Year}\\{Day}\\live.txt"";
        
        public static long Part1(string[] lines)
        {
            return 0;
        }
        public static long Part2(string[] lines)
        {
            return 0;
        }
    }
}";

}