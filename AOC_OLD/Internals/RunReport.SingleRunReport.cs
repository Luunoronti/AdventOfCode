// attempt to keep the code as clean as possible
// most of functionality is in Core and Extensions folder

partial class RunReport
{
    private class SingleRunReport
    {
        public SingleRunReport(TimeSpan time, int year, int day, int part, bool isTestRun)
        {
            Time = time;
            Year = year;
            Day = day;
            Part = part;
            IsTestRun = isTestRun;
        }

        public ResultResult Result { get; set; }
        public TimeSpan Time { get; }
        public int Year { get; }
        public int Day { get; }
        public int Part { get; }
        public bool IsTestRun { get; }

        public string Name { get; set; }
        public string ResultValue { get; internal set; }

        public override string ToString()
        {
            return $"{Name} ({Day}/{Year}) Part {Part} {(IsTestRun ? "Test" : "Live")}: Result: {Result}, time: {Time.FormatUltraPrecise()}";
        }
    }
}
