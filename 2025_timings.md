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


## Day 7 - Laboratories
Intel Core i9-14900KF:
| Method | Mean     | Mean      | Error     | StdDev    | Median    | Allocated |
|------- |---------:|----------:|----------:|----------:|----------:|----------:|
| Part1  | 0.010 ms |  9.858 us | 0.4967 us | 0.7434 us |  9.896 us |         - |
| Part2  | 0.012 ms | 11.964 us | 1.1914 us | 1.7087 us | 10.527 us |         - |

Intel Core i5-8500:
| Method | Mean     | Mean     | Error    | StdDev   | Median   | Allocated |
|------- |---------:|---------:|---------:|---------:|---------:|----------:|
| Part1  | 0.037 ms | 37.16 us | 0.143 us | 0.111 us | 37.12 us |         - |
| Part2  | 0.042 ms | 42.31 us | 0.109 us | 0.102 us | 42.32 us |         - |

<br/>

## Day 6 - Trash Compactor
Intel Core i9-14900KF:
| Method | Mean     | Mean     | Error    | StdDev   | Median   | Allocated |
|------- |---------:|---------:|---------:|---------:|---------:|----------:|
| Part1  | 0.015 ms | 15.29 us | 0.085 us | 0.079 us | 15.29 us |         - |
| Part2  | 0.016 ms | 15.71 us | 0.052 us | 0.048 us | 15.71 us |         - |

Intel Core i5-8500:
| Method | Mean     | Mean     | Error    | StdDev   | Median   | Allocated |
|------- |---------:|---------:|---------:|---------:|---------:|----------:|
| Part1  | 0.055 ms | 54.69 us | 0.128 us | 0.120 us | 54.70 us |         - |
| Part2  | 0.042 ms | 42.27 us | 0.228 us | 0.214 us | 42.28 us |         - |

<br/>

## Day 5 - Cafeteria
Intel Core i9-14900KF:
| Method | Mean     | Mean      | Error     | StdDev    | Median    | Gen0   | Allocated |
|------- |---------:|----------:|----------:|----------:|----------:|-------:|----------:|
| Part1  | 0.023 ms | 22.717 us | 0.0771 us | 0.0643 us | 22.696 us | 1.1597 |  21.46 KB |
| Part2  | 0.005 ms |  5.376 us | 0.0174 us | 0.0155 us |  5.379 us | 1.1673 |  21.46 KB |

Intel Core i5-8500:
| Method | Mean     | Mean     | Error    | StdDev   | Median   | Gen0   | Allocated |
|------- |---------:|---------:|---------:|---------:|---------:|-------:|----------:|
| Part1  | 0.073 ms | 72.92 us | 0.338 us | 0.316 us | 72.86 us | 4.6387 |  21.46 KB |
| Part2  | 0.016 ms | 16.31 us | 0.234 us | 0.219 us | 16.30 us | 4.6692 |  21.46 KB |

<br/>

## Day 4 - Printing Department
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

<br/>

## Day 3 - Lobby
Intel Core i9-14900KF:
| Method | Mean     | Mean     | Error    | StdDev   | Median   | Gen0    | Allocated |
|------- |---------:|---------:|---------:|---------:|---------:|--------:|----------:|
| Part1  | 0.012 ms | 12.32 us | 0.072 us | 0.064 us | 12.29 us |       - |         - |
| Part2  | 0.090 ms | 90.08 us | 0.817 us | 0.765 us | 90.11 us | 25.7568 |  485256 B |

Intel Core i5-8500:
| Method | Mean     | Mean      | Error    | StdDev   | Median    | Gen0     | Gen1   | Allocated |
|------- |---------:|----------:|---------:|---------:|----------:|---------:|-------:|----------:|
| Part1  | 0.034 ms |  34.21 us | 0.072 us | 0.068 us |  34.21 us |        - |      - |         - |
| Part2  | 0.149 ms | 148.73 us | 2.810 us | 2.491 us | 148.05 us | 103.0273 | 0.2441 |  485256 B |

<br/>

## Day 2 - Gift Shop
Intel Core i9-14900KF:

| Method | Mean     | Mean     | Error     | StdDev    | Median   | Allocated |
|------- |---------:|---------:|----------:|----------:|---------:|----------:|
| Part1  | 0.001 ms | 1.298 us | 0.0028 us | 0.0026 us | 1.299 us |         - |
| Part2  | 0.003 ms | 3.278 us | 0.0092 us | 0.0086 us | 3.280 us |         - |

Intel Core i5-8500:
| Method | Mean     | Mean     | Error     | StdDev    | Median   | Allocated |
|------- |---------:|---------:|----------:|----------:|---------:|----------:|
| Part1  | 0.003 ms | 2.971 us | 0.0123 us | 0.0115 us | 2.969 us |         - |
| Part2  | 0.010 ms | 9.848 us | 0.0574 us | 0.0509 us | 9.844 us |         - |

<br/>

## Day 1 - Secret Entrance
Intel Core i9-14900KF:
| Method | Mean     | Mean     | Error     | StdDev    | Median   | Allocated |
|------- |---------:|---------:|----------:|----------:|---------:|----------:|
| Part1  | 0.004 ms | 3.704 us | 0.0097 us | 0.0091 us | 3.702 us |         - |
| Part2  | 0.004 ms | 3.778 us | 0.0132 us | 0.0124 us | 3.780 us |         - |

Intel Core i5-8500:
| Method | Mean     | Mean     | Error    | StdDev   | Median   | Allocated |
|------- |---------:|---------:|---------:|---------:|---------:|----------:|
| Part1  | 0.021 ms | 20.87 us | 0.115 us | 0.102 us | 20.86 us |         - |
| Part2  | 0.026 ms | 26.18 us | 0.167 us | 0.156 us | 26.21 us |         - |


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