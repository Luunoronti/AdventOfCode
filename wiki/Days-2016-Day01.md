# 2016 Day 1

## Implementation Summary
- **Part 1:**
  - Uses a `Traveller` starting at the origin facing North.
  - Parses instructions (e.g., 'R2', 'L3'), updates direction, and walks the specified number of steps.
  - Returns the Manhattan distance from the origin after all moves.
- **Part 2:**
  - Similar to Part 1, but tracks all visited locations.
  - Stops at the first location visited twice and returns its Manhattan distance from the origin.

## Example Usage
```csharp
var day = new Year2016.Day01();
var result1 = day.Part1(new PartInput { FullString = "R2, L3" }); // "5"
var result2 = day.Part2(new PartInput { FullString = "R8, R4, R4, R8" }); // "4"
``` 