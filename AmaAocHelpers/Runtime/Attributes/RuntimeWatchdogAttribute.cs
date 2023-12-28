using System.Diagnostics;

namespace AmaAocHelpers;

public class RuntimeWatchdogAttribute : Attribute
{
    public RuntimeWatchdogAttribute(int maximumTimeSeconds)
    {
        if (Debugger.IsAttached == false)
            TimeSeconds = maximumTimeSeconds;
        else
            TimeSeconds = int.MaxValue;
    }
    public long TimeSeconds
    {
        get; set;
    }
}
