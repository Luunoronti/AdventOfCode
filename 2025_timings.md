# Advent Of Code 2025 Benchmarks

### Software: 
BenchmarkDotNet v0.13.12
<br/>
.NET 10.0.0 (10.0.25.52411), X64 RyuJIT AVX2
<br/>
Windows 11 (10.0.26100.7171)

### Hardware: 
Table 1: Intel Core i9-14900KF, 1 CPU, 32 logical and 24 physical cores
<br/>
Table 2: Intel Core i5-8500 CPU 3.00GHz (Coffee Lake), 1 CPU, 6 logical and 6 physical cores


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
Intel Core i9-14900KF:
| Method | Mean     | Mean     | Error   | StdDev  | Median   | Allocated |
|------- |---------:|---------:|--------:|--------:|---------:|----------:|
| Part1  | 0.380 ms | 379.7 us | 0.81 us | 0.68 us | 379.8 us |         - |
| Part2  | 0.728 ms | 727.6 us | 4.07 us | 3.81 us | 726.1 us |         - |

Intel Core i5-8500:
| Method | Mean     | Mean       | Error   | StdDev  | Median     | Allocated |
|------- |---------:|-----------:|--------:|--------:|-----------:|----------:|
| Part1  | 0.795 ms |   795.3 us | 0.71 us | 0.66 us |   795.3 us |         - |
| Part2  | 1.416 ms | 1,416.3 us | 3.61 us | 3.37 us | 1,416.1 us |         - |


## Day 03 - Lobby
| Method | Mean     | Mean     | Error    | StdDev   | Median   | Gen0    | Allocated |
|------- |---------:|---------:|---------:|---------:|---------:|--------:|----------:|
| Part1  | 0.012 ms | 12.32 us | 0.072 us | 0.064 us | 12.29 us |       - |         - |
| Part2  | 0.090 ms | 90.08 us | 0.817 us | 0.765 us | 90.11 us | 25.7568 |  485256 B |


## Day 02 - Gift Shop
| Method | Mean     | Mean     | Error     | StdDev    | Median   | Allocated |
|------- |---------:|---------:|----------:|----------:|---------:|----------:|
| Part1  | 0.001 ms | 1.298 us | 0.0028 us | 0.0026 us | 1.299 us |         - |
| Part2  | 0.003 ms | 3.278 us | 0.0092 us | 0.0086 us | 3.280 us |         - |


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
| Gen0 | GC Generation 0 collects per 1000 operations |
| Gen1 | GC Generation 1 collects per 1000 operations |
| Allocated | Allocated memory per single operation (managed only, inclusive, 1KB = 1024B) |

  1 us      : 1 Microsecond (0.000001 sec)