using System.Diagnostics;

namespace AmaAocHelpers.Runtime;

public class Watchdog
{
    private int maxTime;
    private bool killApp;

    public void Start(int maxTimeS)
    {
        killApp = true;
        maxTime = maxTimeS;
        new Thread(ThreadFunc) { IsBackground = true }.Start();
    }
    public void Stop()
    {
        killApp = false;
    }
    private void ThreadFunc()
    {
        var sw = Stopwatch.StartNew();
        while (killApp)
        {
            sw.Stop();
            if (sw.Elapsed.TotalSeconds > maxTime)
            {
                Console.WriteLine($"{CC.Err}App is being killed by Watchdog ({maxTime} seconds have passed).{CC.Clr}");
                Environment.Exit(-1);
            }
            sw.Start();
            Thread.Sleep(100);
        }
    }
}

