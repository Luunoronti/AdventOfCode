using AoC.Core;
using TextCopy;

var grid = new Grid<int>(5, 5);
grid[2, 2] = 42;

// Put a breakpoint here and inspect `grid` in VS
Console.WriteLine("Ready to debug!");

await ClipboardService.SetTextAsync("Text to place in clipboard #1");

await ClipboardService.SetTextAsync("Text to place in clipboard #2");

