

public class Configuration
{
    public static ProgramConfiguration Program { get; } = LoadProgramConfiguration();
    //public static ExecutionConfiguration Execution { get; } = LoadExecutionConfiguration();

    private static ProgramConfiguration LoadProgramConfiguration()
    {
        try
        {
            return JsonConvert.DeserializeObject<ProgramConfiguration>(File.ReadAllText($"{RootPath}/Configuration/ProgramConfig.json")) ?? new ProgramConfiguration();
        }
        catch
        {
        }
        // create empty program config
        try
        {
            Directory.CreateDirectory($"{RootPath}/Configuration/");
            File.WriteAllText($"{RootPath}/Configuration/ProgramConfig.json", "{}");
        }
        catch { }
        return new ProgramConfiguration();
    }
    //private static ExecutionConfiguration LoadExecutionConfiguration()
    //{
    //    try
    //    {
    //        return JsonConvert.DeserializeObject<ExecutionConfiguration>(File.ReadAllText($"{RootPath}/Configuration/ExecutionConfig.json")) ?? new ExecutionConfiguration();
    //    }
    //    catch
    //    {
    //    }
    //    // create empty program config
    //    try
    //    {
    //        Directory.CreateDirectory($"{RootPath}/Configuration/");
    //        File.WriteAllText($"{RootPath}/Configuration/ProgramConfig.json", "{}");
    //    }
    //    catch { }
    //    return new ExecutionConfiguration();
    //}
    public static void SaveExecutionConfiguration()
    {
        //try
        //{
        //    // sort years such that last year is going to be the first
        //    // in the file, for simpler editing
        //    Execution.Years?.Sort((y1, y2) => y2.Year - y1.Year);

        //    // and do the same for days, so last day (the one we will probably work on)
        //    // is going to be the first in the file
        //    Execution.Years?.ForEach(y => y.Days?.Sort((d1, d2) => d2.Day - d1.Day));

        //    Directory.CreateDirectory($"{RootPath}/Configuration/");
        //    File.WriteAllText($"{RootPath}/Configuration/ExecutionConfig.json", JsonConvert.SerializeObject(Execution, Formatting.Indented));
        //    return;
        //}
        //catch
        //{
        //}
        //// create empty program config
        //try
        //{
        //    Directory.CreateDirectory($"{RootPath}/Configuration/");
        //    File.WriteAllText($"{RootPath}/Configuration/ExecutionConfig.json", "{}");
        //}
        //catch { }
    }

    private static string rootPath;
    public static string RootPath
    {
        get
        {
            // introduce some advanced logic herein - later on
            rootPath ??= File.Exists(".\\session.txt") ? ".\\" : "..\\..\\..\\";
            return rootPath;
        }
    }

    [JsonObject]
    public class ProgramConfiguration
    {
        [JsonProperty]
        public bool RunAllInDebug
        {
            get; set;
        }
        [JsonProperty]
        public bool RunAllInRelease
        {
            get; set;
        }
        [JsonProperty]
        public VisualizerConfiguration Visualizer
        {
            get; set;
        } = new VisualizerConfiguration();
    }
    [JsonObject]
    public class VisualizerConfiguration
    {
        public bool AllowMouseCapture
        {
            get; set;
        }
        public bool AlternateBuffer
        {
            get; set;
        }
        public bool HideCursor
        {
            get; set;
        }
        public bool Force16colorsMode
        {
            get; set;
        }
        public bool ClearScreenOnExit
        {
            get; set;
        }
        public int LegendVisibility
        {
            get; set;
        }
        public int CameraMoveMouseButton
        {
            get; set;
        }
        public int ClearMode
        {
            get; set;
        }
        public ConfigColor ClearColor
        {
            get; set;
        }
        public ConfigColor InfoColor
        {
            get; set;
        }
        public ConfigColor GradientStartColor
        {
            get; set;
        }
        public ConfigColor GradientEndColor
        {
            get; set;
        }
    }
    [JsonObject]
    public struct ConfigColor
    {
        public int r
        {
            get; set;
        }
        public int g
        {
            get; set;
        }
        public int b
        {
            get; set;
        }
    }

    [JsonObject]
    public class ExecutionConfiguration
    {
        public List<YearConfiguration> Years { get; set; } = [];
    }
    [JsonObject]
    public class YearConfiguration
    {
        public int Year { get; set; }
        public List<DayConfiguration> Days { get; set; } = [];
    }
    [JsonObject]
    public class DayConfiguration
    {
        public bool Run { get; set; }
        public bool DebugRun { get; set; }
        public int Day { get; set; }
        public bool EnableVisualization { get; set; }
    }

}


