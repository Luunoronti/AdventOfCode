namespace TermGlass;


public static class Visualizer
{
    public static void Run(VizConfig cfg, Func<bool> process, Action<Frame, bool> draw, TooltipProvider? info = null, Func<string>? status = null)
    {
        using var term = new Terminal(cfg.ColorMode);
        var loop = new MainLoop(term, cfg, process, draw, info, status);
        loop.Run();
    }
}