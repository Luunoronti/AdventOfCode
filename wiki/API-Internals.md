# Internals API

This page documents the main classes and APIs in the `Internals` folder.

---

## DayRunner
Handles running a specific Advent of Code day/part, including test/live input, timing, and reporting.
- Loads the correct day/part handler using reflection.
- Handles input loading (from file or inline).
- Times execution and records results.
- Used internally by the framework.

## Configuration
Loads and saves program and execution configuration from JSON files.
- `Program`: Main program configuration (global settings).
- `Execution`: Execution configuration (years, days, etc).
- `RootPath`: Determines the root path for config files.
- Usage: Access via `Configuration.Program` or `Configuration.Execution`.

## PartInput
Represents the input for a puzzle part.
- `FullString`: The full input as a string.
- `Lines`: The input split into lines.
- `Span`: The input as a `ReadOnlySpan<char>`.
- `LineWidth`: Width of a line.
- `Count`: Number of lines.
- Used as the parameter to part solution methods.

## RunReport, RunReport.Table, RunReport.SingleRunReport, RunReport.TableColumn
Classes for collecting and formatting run results, including timing and output. Used internally for reporting after running solutions.

## GlobalUsings
Provides global using directives for common types and namespaces, including aliases for `Vector2D`, `Location`, and `Direction`. 