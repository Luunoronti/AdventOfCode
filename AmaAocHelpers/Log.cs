﻿namespace AmaAocHelpers;

public static partial class Log
{
    public static bool Enabled
    {
        get; set;
    }
    public static void WriteLine(string message)
    {
        if (Enabled) Console.WriteLine(message);
    }
    public static void WriteErrorLine(string message)
    {
        if (Enabled) Console.WriteLine(CC.Error(message));
    }
    public static void WriteLine()
    {
        if (Enabled) Console.WriteLine();
    }
    public static void Write(string message)
    {
        if (Enabled) Console.Write(message);
    }

    
}




