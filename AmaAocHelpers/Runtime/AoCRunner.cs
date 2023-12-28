using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using StringSpan = System.ReadOnlySpan<char>;

namespace AmaAocHelpers.Runtime;
public class AoCRunner
{
    private static int currentYear;
    private static bool testDataInUseMsgPrinted;

    private delegate long RunMethodStrParam(string input);
    private delegate long RunMethodStrLinesParam(string[] input);
    private delegate long RunMethodSpanParam(StringSpan input);

    private delegate long RunMethodSpanWidthHeigthParam(StringSpan input, int lineWidth, int lineCount);
    private delegate long RunMethodStringWidthHeigthParam(string input, int lineWidth, int lineCount);
    private delegate long RunMethodStringArrWidthHeigthParam(string[] input, int lineWidth, int lineCount);

    private static void SetupThreadPool()
    {
        var maxthreads = Math.Max(1, Environment.ProcessorCount);
        ThreadPool.SetMaxThreads(maxthreads, Environment.ProcessorCount);
        Console.WriteLine($"Max ThreadPool threads set to {CC.Sys}{maxthreads}{CC.Clr}");
    }
    private static int GetAocDay(string[] cmdLine)
    {
        var index = cmdLine.ToList().IndexOf("-day");
        if (index != -1) return int.Parse(cmdLine[index + 1]);
        if (DateTime.Now.Month != 12) return 60;
        return DateTime.Now.Day;
    }

    private static bool GetUseTestData(Type dayType)
    {
        var useTestData = Debugger.IsAttached ? dayType.GetCustomAttribute<UseLiveDataInDebugAttribute>() == null : dayType.GetCustomAttribute<AlwaysUseTestDataAttribute>() != null;
        if (!testDataInUseMsgPrinted)
            Console.WriteLine($"{CC.Sys}{(useTestData ? "Test" : "Live")}{CC.Clr} data is in use");
        testDataInUseMsgPrinted = true;
        return useTestData;
    }
    private static bool GetLogEnabled(Type dayType)
    {
        var enabled = Debugger.IsAttached ? dayType.GetCustomAttribute<UseLiveDataInDebugAttribute>() == null : dayType.GetCustomAttribute<AlwaysUseTestDataAttribute>() != null;
        Console.WriteLine($"Logger is {CC.Sys}{(enabled ? "on" : "off")}{CC.Clr}");
        return enabled;
    }
    private static Watchdog GetAndStartWatchdog(Type dayType)
    {
        var wda = dayType.GetCustomAttribute<RuntimeWatchdogAttribute>();
        if (wda != null)
        {
            var wd = new Watchdog();
            wd.Start((int)wda.TimeSeconds);
        }
        return default!;
    }
    private static void PrintAnswer(int part, long answer, Type dayType)
    {
        Console.WriteLine();
        Console.Write($"{CC.Att}===>{CC.Clr} Part {CC.Sys}{part}{CC.Clr} answer: {CC.Ans}{answer:N0}{CC.Clr}");
        var expectedAsnwer1 = GetUseTestData(dayType) ? dayType.GetCustomAttribute<ExpectedTestAnswerPart1Attribute>()?.Answer ?? 0L : 0L;

        if (expectedAsnwer1 > 0 && expectedAsnwer1 != answer)
        {
            Console.Write($"    {CC.Err}PART 1 FAILED!{CC.Clr} Expected answer: {CC.Ans}{expectedAsnwer1:N0}{CC.Clr}");
            if (expectedAsnwer1 > answer)
                Console.Write($" (Too small)");
            else
                Console.Write($" (Too big)");
        }
    }
    private static Type? GetTypeToProcess(string name) =>
        Assembly.GetEntryAssembly()!.GetTypes()
        .Where(t => t.GetCustomAttribute<ForceAttribute>() != null)
        .ForEach(t => Console.WriteLine($"Processing of type {CC.Cls}{t.Namespace}.{t.Name}{CC.Clr} is being forced via attribute."))
        .FirstOrDefault()
        ??
        Assembly.GetEntryAssembly()!.GetTypes()
        .Where(t => t.Name == name)
        .ForEach(t => Console.WriteLine($"Processing of type {CC.Cls}{t.Namespace}.{t.Name}{CC.Clr} is being selected by name."))
        .FirstOrDefault()
        ;
    private static string ProcessInputForMethod(string input, MethodInfo method)
    {
        if (method.GetCustomAttribute<RemoveNewLinesFromInputAttribute>() != null) input = input.Replace("\n", "").Replace("\r", "");
        if (method.GetCustomAttribute<RemoveSpacesFromInputAttribute>() != null) input = input.Replace(" ", "");
        return input;
    }
    private static bool HasParameters(MethodInfo method, params Type[] types)
    {
        var methodParameters = method.GetParameters();
        if (methodParameters.Length != types.Length)
            return false;
        for (var i = 0; i < types.Length; i++)
        {
            if (methodParameters[i].ParameterType != types[i])
                return false;
        }
        return true;
    }

    private static long RunMethod(Type type, string methodName)
    {
        var logStatus = Log.Enabled;
        try
        {
            Console.WriteLine($"{CC.Att}===>{CC.Clr} Running {CC.Cls}{type.Namespace}.{type.Name}.{methodName}{CC.Clr}...");
            var method = type.GetMethod(methodName);
            if (method == null)
            {
                Log.WriteLine($"{CC.Err}Unable to find method {CC.Sys}{methodName}{CC.Err} on type {CC.Sys}{type.Namespace}.{type.Name}{CC.Clr}");
                return 0;
            }

            if (Debugger.IsAttached && method.GetCustomAttribute<DisableLogInDebugAttribute>() != null) Log.Enabled = false;
            if (!Debugger.IsAttached && method.GetCustomAttribute<AlwaysEnableLogAttribute>() != null) Log.Enabled = true;

            type.GetProperty("IsTest")?.SetValue(null, (object)GetUseTestData(type));

            var methodParameters = method.GetParameters();
            var answer = 0L;
            Stopwatch sw = new();

            if (HasParameters(method, typeof(StringSpan), typeof(int), typeof(int)))
            {
                var lines = ReadInputLines(type);
                if (lines.Length == 0)
                    return -1;

                var width = lines[0].Length;
                var count = lines.Length;
                var input = ProcessInputForMethod(string.Join("", lines).Replace("\n", "").Replace("\r", ""), method);

                var del = method.CreateDelegate<RunMethodSpanWidthHeigthParam>();
                sw.Start();
                answer = del(input.AsSpan(), width, count);
            }
            else if (HasParameters(method, typeof(string), typeof(int), typeof(int)))
            {
                var lines = ReadInputLines(type);
                if (lines.Length == 0)
                    return -1;
                var width = lines[0].Length;
                var count = lines.Length;
                var input = ProcessInputForMethod(string.Join("", lines).Replace("\n", "").Replace("\r", ""), method);

                var del = method.CreateDelegate<RunMethodStringWidthHeigthParam>();
                sw.Start();
                answer = del(input, width, count);
            }
            else if (HasParameters(method, typeof(string[]), typeof(int), typeof(int)))
            {
                var lines = ReadInputLines(type);
                if (lines.Length == 0)
                    return -1;
                var width = lines[0].Length;
                var count = lines.Length;
                var del = method.CreateDelegate<RunMethodStringArrWidthHeigthParam>();
                sw.Start();
                answer = del(lines, width, count);
            }
            else if (HasParameters(method, typeof(string[])))
            {
                var lines = ReadInputLines(type);
                if (lines.Length == 0)
                    return -1;
                var del = method.CreateDelegate<RunMethodStrLinesParam>();

                sw.Start();
                answer = del(lines);
            }
            else if (HasParameters(method, typeof(string)))
            {
                var text = ReadInputText(type);
                text = ProcessInputForMethod(text, method);
                var del = method.CreateDelegate<RunMethodStrParam>();

                sw.Start();
                answer = del(text);
            }
            else if (HasParameters(method, typeof(StringSpan)))
            {
                var text = ReadInputText(type);
                text = ProcessInputForMethod(text, method);
                var span = text.AsSpan();
                var del = method.CreateDelegate<RunMethodSpanParam>();

                sw.Start();
                answer = del(span);
            }

            sw.Stop();
            Console.WriteLine($"{CC.Att}===> {CC.Cls}{type.Namespace}.{type.Name}.{method} {CC.Clr}completed in {CC.Sys}{sw.ElapsedMilliseconds} ms ({sw.Elapsed}){CC.Clr}\n");

            return answer;
        }
        finally
        {
            Log.Enabled = logStatus;

        }
    }
    private static string[] ReadInputLines(Type dayType)
    {

        var day = int.Parse(dayType.Name.Replace("Day", ""));

        var fileName = $"{RootPath}Day{day:D2}\\{(GetUseTestData(dayType) ? "test" : "live")}.txt";
        if (GetUseTestData(dayType) == false)
        {
            if (File.Exists(fileName) == false || new FileInfo(fileName).Length == 0)
            {
                // reconstruct year and data from type name
                var content = GetLiveCode(day);
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
    private static string ReadInputText(Type dayType)
    {
        var day = int.Parse(dayType.Name.Replace("Day", ""));

        var fileName = $"{RootPath}Day{day:D2}\\{(GetUseTestData(dayType) ? "test" : "live")}.txt";

        if (GetUseTestData(dayType) == false)
        {
            if (File.Exists(fileName) == false || new FileInfo(fileName).Length == 0)
            {
                // reconstruct year and data from type name
                var content = GetLiveCode(day);
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
    private static string? __rootPath;
    public static string RootPath
    {
        get
        {
            if (__rootPath == null)
            {
                if (File.Exists(".\\session.txt"))
                    __rootPath = ".\\";
                else if (File.Exists("..\\..\\..\\session.txt"))
                    __rootPath = "..\\..\\..\\";
                else
                {
                    throw new InvalidDataException("Unable to find root path. File [session.txt] not found.");
                }
            }
            return __rootPath;
        }
    }

    // Note: If we are to run live data, download them from AoC. 
    // Huge thanks to Nick Kusters (https://www.youtube.com/@NKCSS) for pointing out that live data should not be kept on GitHub,
    // and allowing to copy his download code.
    private static string GetLiveCode(int day)
    {
        var session = File.ReadAllText($"{RootPath}session.txt");
        var url = $"https://adventofcode.com/{currentYear}/day/{day}/input";

#pragma warning disable SYSLIB0014 // Type or member is obsolete
        var wc = new WebClient();
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        wc.Headers.Add(HttpRequestHeader.UserAgent, "https://github.com/Luunoronti/AdventOfCode");
        wc.Headers.Add(HttpRequestHeader.Cookie, $"{nameof(session)}={session}");
        var contents = wc.DownloadString(url);

        Log.WriteLine("");
        Console.WriteLine($"{CC.Sys}{Encoding.UTF8.GetByteCount(contents)}{CC.Clr} bytes of live data downloaded.");
        return contents;
    }

    private static void CreateDayIfDoesNotExist(int day)
    {
        if (day > 25) return;

        var prefix = $"{RootPath}Day{day:D2}\\";
        Directory.CreateDirectory(prefix);

        if (File.Exists(prefix + $"Day{day:D2}.cs") == false)
            File.WriteAllText(prefix + $"Day{day:D2}.cs", DayTemplateCode.Replace("{Year}", currentYear.ToString()).Replace("{Day}", day.ToString("D2")));
        if (File.Exists(prefix + $"test.txt") == false)
            File.WriteAllText(prefix + $"test.txt", "");
    }


    public static void Run(int year, string[] args, int dayOverride = -1)
    {
        currentYear = year;

        SetupThreadPool();

        var day = dayOverride == -1 ? GetAocDay(args) : dayOverride;
        var dayTypeName = $"Day{day:D2}";

        CreateDayIfDoesNotExist(day);

        var dayType = GetTypeToProcess(dayTypeName);
        if (dayType == null)
        {
            Console.WriteLine($"{CC.Err}Type {dayTypeName} for year {year} does not exist. It has been recreated, please build your project and run it again. {CC.Clr}");
            return;
        }

        Log.Enabled = GetLogEnabled(dayType);

        var watchDog = GetAndStartWatchdog(dayType);
        var a1 = RunMethod(dayType, "Part1");
        watchDog?.Stop();

        watchDog = GetAndStartWatchdog(dayType);
        var a2 = RunMethod(dayType, "Part2");
        watchDog?.Stop();


        PrintAnswer(1, a1, dayType);
        PrintAnswer(2, a2, dayType);

        Console.WriteLine();
        Console.WriteLine();

        if (a2 != 0)
        {
            //Clipboard.SetText(a2.ToString());
            //Console.WriteLine($"Answer {CC.Sys}2{CC.Clr} ({CC.Ans}{a2}{CC.Clr}) has been copied to clipboard automatically.");
        }
        else if (a1 != 0)
        {
            //Clipboard.SetText(a1.ToString());
            //Console.WriteLine($"Answer {CC.Sys}1{CC.Clr} ({CC.Ans}{a1}{CC.Clr}) has been copied to clipboard automatically.");
        }
    }



    private const string DayTemplateCode = @"
using AmaAocHelpers;
using StringSpan = System.ReadOnlySpan<char>;

namespace AdventOfCode{Year};

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
    public static long Part1(StringSpan input, int width, int height)
    {
        return 0;
    }
    //[RemoveSpacesFromInput]
    //[RemoveNewLinesFromInput]
    // change to string or string[] to get other types of input
    public static long Part2(StringSpan input, int width, int height)
    {
        return 0;
    }
}
";

}

