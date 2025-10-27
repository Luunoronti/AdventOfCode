# Getting Started

Welcome to the AdventOfCode project! This guide will help you get started with adding a new puzzle-solving day.

## Quick Start

**You do not need to manually create folders or files for a new day.**

- Simply run the program and specify the desired year and day on the command line, for example:
  ```sh
  dotnet run --year 2024 --day 3
  ```
- If the files and folders for that day do not exist, the program will automatically generate the necessary structure and template files for you.
- You can then open the generated files and start implementing your solution.

## Project Structure

- Each year has its own folder (e.g., `2016/`, `2024/`).
- Each day is a subfolder inside the year (e.g., `2016/Day01/`).
- Each day contains:
  - `DayXX.cs`: The C# implementation for the day's puzzle.
  - `config.yaml`: Configuration for tests and live runs.
  - (Optional) Input files, test files, etc.

## What Gets Generated
- The program will create:
  - The day folder (e.g., `2024/Day03/`)
  - A template `DayXX.cs` file with the correct class and method stubs
  - A `config.yaml` file for configuration
  - Any other required files for the day

## Next Steps
- Open the generated `DayXX.cs` file and implement your solution in the `Part1` and `Part2` methods.
- Use the provided extension methods and runtime utilities for common tasks (see API documentation).
- Check existing days for examples of structure and implementation.

---

If you have questions or need more examples, see the API and Days documentation in this wiki. 