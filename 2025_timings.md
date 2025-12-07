# Advent Of Code 2025 Timings

### Software: 
BenchmarkDotNet v0.13.12
<br/>
.NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX2
<br/>
Windows 11 (10.0.26100.7171)

### Hardware: 
Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores


> All tinimgs ran in-process with default job.

<br/>


## Day 07 - Laboratories
| Method | Mean     | Mean      | Error     | StdDev    | Median    | Allocated |
|------- |---------:|----------:|----------:|----------:|----------:|----------:|
| Part1  | 0.010 ms |  9.858 us | 0.4967 us | 0.7434 us |  9.896 us |         - |
| Part2  | 0.012 ms | 11.964 us | 1.1914 us | 1.7087 us | 10.527 us |         - |


## Day 06 - Trash Compactor
| Method | Mean     | Mean     | Error    | StdDev   | Median   | Allocated |
|------- |---------:|---------:|---------:|---------:|---------:|----------:|
| Part1  | 0.015 ms | 15.29 us | 0.085 us | 0.079 us | 15.29 us |         - |
| Part2  | 0.016 ms | 15.71 us | 0.052 us | 0.048 us | 15.71 us |         - |


## Day 05 - Cafeteria
| Method | Mean     | Mean      | Error     | StdDev    | Median    | Gen0   | Allocated |
|------- |---------:|----------:|----------:|----------:|----------:|-------:|----------:|
| Part1  | 0.023 ms | 22.717 us | 0.0771 us | 0.0643 us | 22.696 us | 1.1597 |  21.46 KB |
| Part2  | 0.005 ms |  5.376 us | 0.0174 us | 0.0155 us |  5.379 us | 1.1673 |  21.46 KB |


## Day 04 - Printing Department
| Method | Mean     | Mean     | Error   | StdDev  | Median   | Gen0   | Allocated |
|------- |---------:|---------:|--------:|--------:|---------:|-------:|----------:|
| Part1  | 0.396 ms | 396.4 us | 1.07 us | 1.00 us | 396.3 us | 1.9531 |  38.33 KB |
| Part2  | 0.732 ms | 732.3 us | 1.75 us | 1.46 us | 731.9 us | 5.8594 | 114.95 KB |


## Day 03 - Lobby
| Method | Mean     | Mean     | Error    | StdDev   | Median   | Gen0    | Allocated |
|------- |---------:|---------:|---------:|---------:|---------:|--------:|----------:|
| Part1  | 0.012 ms | 12.32 us | 0.072 us | 0.064 us | 12.29 us |       - |         - |
| Part2  | 0.090 ms | 90.08 us | 0.817 us | 0.765 us | 90.11 us | 25.7568 |  485256 B |


## Day 02 - Gift Shop
| Method | Mean     | Mean     | Error     | StdDev    | Median   | Gen0   | Gen1   | Allocated |
|------- |---------:|---------:|----------:|----------:|---------:|-------:|-------:|----------:|
| Part1  | 0.001 ms | 1.301 us | 0.0021 us | 0.0019 us | 1.301 us |      - |      - |         - |
| Part2  | 0.004 ms | 4.500 us | 0.0272 us | 0.0255 us | 4.503 us | 1.8311 | 0.1755 |   34480 B |


## Day 01 - Secret Entrance
| Method | Mean     | Mean     | Error     | StdDev    | Median   | Allocated |
|------- |---------:|---------:|----------:|----------:|---------:|----------:|
| Part1  | 0.004 ms | 3.664 us | 0.0091 us | 0.0085 us | 3.662 us |         - |
| Part2  | 0.005 ms | 5.161 us | 0.0193 us | 0.0181 us | 5.160 us |         - |


<br/>
<br/>
<br/>

### Legend
| Column | Description  |
|------- |---------|
| Mean  | Arithmetic mean of all measurements |
| Error | Half of 99.9% confidence interval |
| StdDev | Standard deviation of all measurements |
| Median  | Value separating the higher half of all measurements (50th percentile) |
| Allocated | Allocated memory per single operation (managed only, inclusive, 1KB = 1024B) |

  1 us      : 1 Microsecond (0.000001 sec)