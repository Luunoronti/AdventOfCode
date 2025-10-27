# Runtime API

This page documents the main classes and APIs in the `Runtime` folder.

---

## Traveller
Simulates movement on a grid, tracking position, direction, and visited locations.
- `Location`: Current position.
- `CardinalDirection`: Current facing direction.
- `VisitedLocations`: Dictionary of locations visited and their counts.
- `Walk(int steps, Func<Traveller, StepPred> CanStepPred = null, Func<Traveller, bool> TookStepPred = null): WalkResult` — Moves the traveller, optionally using predicates to control/observe each step.

**Usage:**
```csharp
var traveller = new Traveller();
traveller.Walk(5);
```

---

## CardinalDirection
Represents one of the four cardinal directions (North, East, South, West).
- `Left`, `Right`, `Back`: Returns a new direction after turning.
- `IsNorth`, `IsEast`, etc.: Checks direction.

**Usage:**
```csharp
var dir = CardinalDirection.North.Right; // East
```

---

## DoubleVector2 / LongVector2
2D vector structs for double and long precision.
- Arithmetic operators (+, -, *).
- `Length`, `LengthSquared`, `ManhattanLength` (for LongVector2).
- `Distance` methods.

**Usage:**
```csharp
var v = new DoubleVector2(1.5, 2.5);
var len = v.Length;
```

---

## WalkResult
Enum: `Completed`, `Cancelled` — Indicates the result of a walk operation. 