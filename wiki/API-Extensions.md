# Extensions API

This page documents all extension methods provided in the `Extensions` folder.

---

## StringExtentions

### SplitTrim(this string, char separator): string[]
Splits a string by the given separator and trims whitespace from each resulting substring.

**Usage:**
```csharp
var parts = "a, b, c".SplitTrim(',');
// parts = ["a", "b", "c"]
```

---

## Vector2DExtentions

### ToFlooredLong(this Vector2D): Vector2D
Converts a `Vector2D` to a new vector with its X and Y values floored and cast to long.

### ManhattanDistance(this Vector2D): double
Returns the Manhattan distance from the origin.

### ManhattanDistance(this Vector2D, Vector2D other): double
Returns the Manhattan distance between two vectors.

**Usage:**
```csharp
var v1 = new Vector2D(3.7, 4.2);
var v2 = new Vector2D(1.0, 2.0);
var dist = v1.ManhattanDistance(v2);
```

---

## TimeSpanExtensions

### FormatUltraPrecise(this TimeSpan): string
Formats a `TimeSpan` into a human-readable string with days, hours, minutes, seconds, milliseconds, microseconds, and nanoseconds.

**Usage:**
```csharp
var ts = TimeSpan.FromMilliseconds(1234.5678);
var formatted = ts.FormatUltraPrecise();
// e.g., "1 s 234 ms 56 µs 78 ns"
``` 