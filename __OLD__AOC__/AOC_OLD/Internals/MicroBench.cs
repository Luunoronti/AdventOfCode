using System.Diagnostics;

public readonly struct BenchResult
{
    public string Name { get; }
    public long OperationCount { get; }
    public double TotalMilliseconds { get; }
    public double NanosecondsPerOp { get; }
    public double OpsPerSecond { get; }

    public BenchResult(string name, long operationCount, double totalMs)
    {
        Name = name;
        OperationCount = operationCount;
        TotalMilliseconds = totalMs;
        NanosecondsPerOp = (totalMs * 1_000_000.0) / operationCount;
        OpsPerSecond = operationCount / (totalMs / 1000.0);
    }

    public override string ToString()
        => $"{Name}: {NanosecondsPerOp:N1} ns/op, {OpsPerSecond:N0} ops/s (total {TotalMilliseconds:N2} ms)";
}

public static class MicroBench
{
    /// <summary>
    /// Benchmarks a method with a single input using Stopwatch.
    /// It does a short warmup and then runs multiple iterations.
    /// </summary>
    public static BenchResult Benchmark<TIn, TOut>(
        string name,
        Func<TIn, TOut> func,
        TIn input,
        int warmupIterations = 3,
        int outerIterations = 10,
        int innerIterations = 1)
    {
        if (func == null) throw new ArgumentNullException(nameof(func));
        if (outerIterations <= 0) throw new ArgumentOutOfRangeException(nameof(outerIterations));
        if (innerIterations <= 0) throw new ArgumentOutOfRangeException(nameof(innerIterations));

        // 1. Warmup: JIT the method and touch data.
        TOut last = default!;
        for (int i = 0; i < warmupIterations; i++)
        {
            last = func(input);
        }
        GC.KeepAlive(last);

        // Optional: clean up GC noise a bit
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // 2. Actual measurement.
        long opCount = (long)outerIterations * innerIterations;

        var sw = Stopwatch.StartNew();
        for (int outer = 0; outer < outerIterations; outer++)
        {
            for (int inner = 0; inner < innerIterations; inner++)
            {
                last = func(input);
            }
        }
        sw.Stop();
        GC.KeepAlive(last); // stop JIT from eliding the call

        return new BenchResult(name, opCount, sw.Elapsed.TotalMilliseconds);
    }

    /// <summary>
    /// Convenience overload for void-like actions (no result).
    /// </summary>
    public static BenchResult Benchmark<TIn>(
        string name,
        Action<TIn> action,
        TIn input,
        int warmupIterations = 3,
        int outerIterations = 10,
        int innerIterations = 1)
    {
        if (action == null) throw new ArgumentNullException(nameof(action));
        if (outerIterations <= 0) throw new ArgumentOutOfRangeException(nameof(outerIterations));
        if (innerIterations <= 0) throw new ArgumentOutOfRangeException(nameof(innerIterations));

        // Warmup
        for (int i = 0; i < warmupIterations; i++)
        {
            action(input);
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long opCount = (long)outerIterations * innerIterations;

        var sw = Stopwatch.StartNew();
        for (int outer = 0; outer < outerIterations; outer++)
        {
            for (int inner = 0; inner < innerIterations; inner++)
            {
                action(input);
            }
        }
        sw.Stop();

        return new BenchResult(name, opCount, sw.Elapsed.TotalMilliseconds);
    }
}
