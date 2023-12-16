using AdventOfCode2023;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using StringSpan = System.ReadOnlySpan<char>;

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

        // if we are to run live data, download them from AoC. 
        // thanks to Nick Kusters (https://www.youtube.com/@NKCSS) for pointing out that live data should not be kept on GitHub

        var a1 = RunMethod(type, "Part1", useTestData);
        var a2 = RunMethod(type, "Part2", useTestData);

        Console.WriteLine();
        Console.Write($"{CC.Att}===>{CC.Clr} Part {CC.Sys}1{CC.Clr} answer: {CC.Ans}{a1}{CC.Clr}");
        var expectedAsnwer1 = useTestData ? type.GetCustomAttribute<ExpectedTestAnswerPart1Attribute>()?.Answer ?? 0L : 0L;

        if (expectedAsnwer1 > 0 && expectedAsnwer1 != a1)
            Console.Write($"    {CC.Err}PART 1 FAILED!{CC.Clr} Expected answer is: {CC.Ans}{expectedAsnwer1}{CC.Clr}");
        Console.WriteLine();
        Console.Write($"{CC.Att}===>{CC.Clr} Part {CC.Sys}2{CC.Clr} answer: {CC.Ans}{a2}{CC.Clr}");
        var expectedAsnwer2 = useTestData ? type.GetCustomAttribute<ExpectedTestAnswerPart2Attribute>()?.Answer ?? 0L : 0L;

        if (expectedAsnwer2 > 0 && expectedAsnwer2 != a2)
            Console.Write($"    {CC.Err}PART 2 FAILED!{CC.Clr} Expected answer is: {CC.Ans}{expectedAsnwer2}{CC.Clr}");
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

    private delegate long RunMethodStrParam(string input);
    private delegate long RunMethodStrLinesParam(string[] input);
    private delegate long RunMethodSpanParam(StringSpan input);


    private delegate long RunMethodSpanWidthHeigthParam(StringSpan input, int lineWidth, int lineCount);
    private delegate long RunMethodStringWidthHeigthParam(string input, int lineWidth, int lineCount);
    private delegate long RunMethodStringArrWidthHeigthParam(string[] input, int lineWidth, int lineCount);


    private static long RunMethod(Type type, string methodName, bool useTestData)
    {
        Console.WriteLine($"{CC.Att}===>{CC.Clr} Running {CC.Cls}{type.Namespace}.{type.Name}.{methodName}{CC.Clr}...");
        var met = type.GetMethod(methodName);
        if (met == null) return -1;

        var logStatus = Log.Enabled;
        if (Debugger.IsAttached && met.GetCustomAttribute<DisableLogInDebugAttribute>() != null)
            Log.Enabled = false;
        if (!Debugger.IsAttached && met.GetCustomAttribute<AlwaysEnableLogAttribute>() != null)
            Log.Enabled = true;

        var method = type.GetMethod(methodName);
        if (method == null)
        {
            Log.WriteLine($"{CC.Err}Unable to find method {CC.Sys}{methodName}{CC.Err} on type {CC.Sys}{type.Namespace}.{type.Name}{CC.Clr}");
            return 0;
        }

        var methodParameters = method.GetParameters();
        var answer = 0L;
        Stopwatch sw = new();
        
        bool HasParameters(params Type[] types)
        {
            if (methodParameters.Length != types.Length)
                return false;
            for (int i = 0; i < types.Length; i++)
            {
                if (methodParameters[i].ParameterType != types[i])
                    return false;
            }
            return true;
        }
        string ProcessInputForMethod(string input)
        {
            if (method.GetCustomAttribute<RemoveNewLinesFromInputAttribute>() != null)
                input = input.Replace("\n", "").Replace("\r", "");
            if (method.GetCustomAttribute<RemoveSpacesFromInputAttribute>() != null)
                input = input.Replace(" ", "");
            return input;
        }

        if (HasParameters(typeof(StringSpan), typeof(int), typeof(int)))
        {
            var lines = ReadInputLines(type, useTestData: useTestData);
            var width = lines[0].Length;
            var count = lines.Length;
            var input = ProcessInputForMethod(string.Join("", lines).Replace("\n", "").Replace("\r", ""));

            var del = method.CreateDelegate<RunMethodSpanWidthHeigthParam>();
            sw.Start();
            answer = del(input.AsSpan(), width, count);
        }
        else if (HasParameters(typeof(string), typeof(int), typeof(int)))
        {
            var lines = ReadInputLines(type, useTestData: useTestData);
            var width = lines[0].Length;
            var count = lines.Length;
            var input = ProcessInputForMethod(string.Join("", lines).Replace("\n", "").Replace("\r", ""));

            var del = method.CreateDelegate<RunMethodStringWidthHeigthParam>();
            sw.Start();
            answer = del(input, width, count);
        }
        else if (HasParameters(typeof(string[]), typeof(int), typeof(int)))
        {
            var lines = ReadInputLines(type, useTestData: useTestData);
            var width = lines[0].Length;
            var count = lines.Length;
            var del = method.CreateDelegate<RunMethodStringArrWidthHeigthParam>();
            sw.Start();
            answer = del(lines, width, count);
        }
        else if (HasParameters(typeof(string[])))
        {
            var lines = ReadInputLines(type, useTestData: useTestData);
            var del = method.CreateDelegate<RunMethodStrLinesParam>();

            sw.Start();
            answer = del(lines);
        }
        else if (HasParameters(typeof(string)))
        {
            var text = ReadInputText(type, useTestData: useTestData);
            text = ProcessInputForMethod(text);
            var del = method.CreateDelegate<RunMethodStrParam>();

            sw.Start();
            answer = del(text);
        }
        else if (HasParameters(typeof(StringSpan)))
        {
            var text = ReadInputText(type, useTestData: useTestData);
            text = ProcessInputForMethod(text);
            var span = text.AsSpan();
            var del = method.CreateDelegate<RunMethodSpanParam>();

            sw.Start();
            answer = del(span);
        }

        sw.Stop();
        Console.WriteLine($"{CC.Att}===> {CC.Cls}{type.Namespace}.{type.Name}.{method} {CC.Clr}completed in {CC.Sys}{sw.ElapsedMilliseconds} ms ({sw.Elapsed}){CC.Clr}\n");

        Log.Enabled = logStatus;
        return answer;
    }

    private static string[] ReadInputLines(Type dayClassType, bool useTestData)
    {
        var year = int.Parse(dayClassType.Namespace?.Replace("AdventOfCode", "") ?? "0");
        var day = int.Parse(dayClassType.Name.Replace("Day", ""));

        var fileName = $"..\\..\\..\\{year}\\{day:D2}\\{(useTestData ? "test" : "live")}.txt";

        if (useTestData == false)
        {
            if (File.Exists(fileName) == false || new FileInfo(fileName).Length == 0)
            {
                // reconstruct year and data from type name
                var content = GetLiveCode(year, day);
                File.WriteAllText(fileName, content);
            }
        }

        var lines = File.ReadAllLines(fileName);
        if (lines.Length == 0)
        {
            Console.WriteLine($"{CC.Err}There are no lines to process. Did you forget to fill in data into the file?{CC.Clr}");
            Console.WriteLine($"{fileName}");
        }
        Console.WriteLine($"{CC.Att}===> {CC.Sys}{lines.Length}{CC.Clr} lines of data read from {CC.Sys}{Path.GetFileName(fileName)}{CC.Clr}");
        return lines;
    }
    private static string ReadInputText(Type dayClassType, bool useTestData)
    {
        var year = int.Parse(dayClassType.Namespace?.Replace("AdventOfCode", "") ?? "0");
        var day = int.Parse(dayClassType.Name.Replace("Day", ""));

        var fileName = $"..\\..\\..\\{year}\\{day:D2}\\{(useTestData ? "test" : "live")}.txt";

        if (useTestData == false)
        {
            if (File.Exists(fileName) == false || new FileInfo(fileName).Length == 0)
            {
                // reconstruct year and data from type name
                var content = GetLiveCode(year, day);
                File.WriteAllText(fileName, content);
            }
        }

        var lines = File.ReadAllText(fileName);
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
    }

    private static Dictionary<(int, int), string> liveCode = new();

    // Note: If we are to run live data, download them from AoC. 
    // Huge thanks to Nick Kusters (https://www.youtube.com/@NKCSS) for pointing out that live data should not be kept on GitHub,
    // and allowing to copy his download code.
    internal static string GetLiveCode(int year, int day)
    {
        string session = File.ReadAllText($"..\\..\\..\\session.txt");
        string url = $"https://adventofcode.com/{year}/day/{day}/input";

#pragma warning disable SYSLIB0014 // Type or member is obsolete
        var wc = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        wc.Headers.Add(HttpRequestHeader.UserAgent, "https://github.com/Luunoronti/AdventOfCode");
        wc.Headers.Add(HttpRequestHeader.Cookie, $"{nameof(session)}={session}");
        string contents = wc.DownloadString(url);
        return contents;
    }

    private const string DayTemplateCode = @"
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode{Year}
{
    //[Force]                    // uncomment to force processing this type (regardless of which day it is according to DateTime)
    //[AlwaysEnableLog]          // if uncommented, Log.Write() and Log.WriteLine() will still be honored in runs without a debugger (do not confuse with Debug/Release configuration)
    //[DisableLogInDebug]        // if uncommented, Log will be disabled even when under debugger
    //[UseLiveDataInDeug]        // if uncommented and under a debug session, will use live data (problem data) instead of test data
    //[AlwaysUseTestData]        // if uncommented, will use test data in both debugging session and non-debugging session
    [ExpectedTestAnswerPart1(0)] // if != 0, will report failure if expected answer != given answer
    [ExpectedTestAnswerPart2(0)] // if != 0, will report failure if expected answer != given answer
    class Day{Day}
    {
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part1(StringSpan input, int lineWidth, int count)
        {
            return 0;
        }
        //[RemoveSpacesFromInput]
        //[RemoveNewLinesFromInput]
        // change to string or string[] to get other types of input
        public static long Part2(StringSpan input, int lineWidth, int count)
        {
            return 0;
        }
    }
}";


}