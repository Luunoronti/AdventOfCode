using System.Diagnostics;
using System.Reflection;

internal partial class Program
{
    private static void Main(string[] _)
    {
        var maxthreads = Math.Max(1, Environment.ProcessorCount);
        ThreadPool.SetMaxThreads(maxthreads, Environment.ProcessorCount);
        Console.WriteLine($"Max ThreadPool threads set to {CC.Sys}{maxthreads}{CC.Clr}");

        var year = DateTime.Now.Year;
        var ns = $"AdventOfCode{year}";
        var day = DateTime.Now.Day;
        var dn = $"Day{day}";

        CreateDayIfDoesNotExist(year, day);

        var typeSet =
            Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.Namespace == ns && t.Name == dn)
            .Select(t => (type: t, name: t.Name))
            .FirstOrDefault();

        if (typeSet.type == null)
        {
            Console.WriteLine($"{CC.Err}Type {dn} for year {year} does not exist. It has been recreated, please build your project and run it again. {CC.Clr}");
            return;
        }

        var type = typeSet.type;
        var name = typeSet.name;

        var test = (bool)(type.GetProperty("TestData")?.GetValue(null) ?? true);
        if (Debugger.IsAttached == false)
            test = false;

        Log.Enabled = test;
        Console.WriteLine($"Logger is {CC.Sys}{(Log.Enabled ? "on" : "off")}{CC.Clr}");

        var lines = ReadLines(test, day, year);

        Log.Enabled = test; 

        var a1 = RunMethod(name, type, "Part1", lines);
        var a2 = RunMethod(name, type, "Part2", lines);

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine($"Day {CC.Sys}{day}{CC.Clr} part {CC.Sys}1{CC.Clr} answer: {CC.Ans}{a1}{CC.Clr}");
        Console.WriteLine($"Day {CC.Sys}{day}{CC.Clr} part {CC.Sys}2{CC.Clr} answer: {CC.Ans}{a2}{CC.Clr}");
        Console.WriteLine();
    }

    private static long RunMethod(string name, Type type, string method, string[] lines)
    {
        Console.WriteLine($"{CC.Sys}===> Running {name} {method}...{CC.Clr}");
        var sw = Stopwatch.StartNew();
        var answer = (long)(type.GetMethod(method)?.Invoke(null, new object[] { lines }) ?? -1);
        sw.Stop();
        Console.WriteLine($"{CC.Sys}===> {name} {method} completed in {sw.ElapsedMilliseconds} ms ({sw.Elapsed}){CC.Clr}");
        return answer;
    }

    private static string[] ReadLines(bool test, int day, int year)
    {
        Console.WriteLine($"Invoking Run with {CC.Sys}{(test ? "test" : "problem")}{CC.Clr} data on {CC.Sys}{day}{CC.Clr}");
        var fileName = $"..\\..\\..\\{year}\\{day:D2}\\{(test ? "test" : "problem")}.txt";
        var lines = File.ReadAllLines(fileName);
        if (lines.Length == 0)
        {
            Console.WriteLine($"{CC.Err}There are no lines to process. Did you forget to fill in data into the file?{CC.Clr}");
            Console.WriteLine($"{fileName}");
        }
        return lines;
    }

    private static void CreateDayIfDoesNotExist(int year, int day)
    {
        var prefix = $"..\\..\\..\\{year}\\{day:D2}\\";
        Directory.CreateDirectory(prefix);

        if (File.Exists(prefix + $"Day{day}.cs") == false)
            File.WriteAllText(prefix + $"Day{day}.cs", DayTemplateCode.Replace("{Year}", year.ToString()).Replace("{Day}", day.ToString()));
        if (File.Exists(prefix + $"test.txt") == false)
            File.WriteAllText(prefix + $"test.txt", "");
        if (File.Exists(prefix + $"problem.txt") == false)
            File.WriteAllText(prefix + $"problem.txt", "");
    }

    private const string DayTemplateCode = @"namespace AdventOfCode{Year}
{
    class Day{Day}
    {
        public static bool TestData => true;
        
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