// attempt to keep the code as clean as possible
// most of functionality is in Core and Extensions folder


public static class TimeSpanExtensions
{
    public static string FormatUltraPrecise(this TimeSpan timeSpan)
    {
        var parts = new List<string>();

        if (timeSpan.Days > 0)
            parts.Add($"{timeSpan.Days} d");

        if (timeSpan.Hours > 0)
            parts.Add($"{timeSpan.Hours} h");

        if (timeSpan.Minutes > 0)
            parts.Add($"{timeSpan.Minutes} min");

        if (timeSpan.Seconds > 0)
            parts.Add($"{timeSpan.Seconds} s");

        if (timeSpan.Milliseconds > 0)
            parts.Add($"{timeSpan.Milliseconds} ms");

        long microseconds = (timeSpan.Ticks % TimeSpan.TicksPerMillisecond) * 100 / TimeSpan.TicksPerMillisecond;
        if (microseconds > 0)
            parts.Add($"{microseconds} µs");

        long nanoseconds = (timeSpan.Ticks % TimeSpan.TicksPerMicrosecond) * 100;
        if (nanoseconds > 0)
            parts.Add($"{nanoseconds} ns");

        return parts.Count > 0 ? string.Join(" ", parts) : "0 ns";
    }
}
