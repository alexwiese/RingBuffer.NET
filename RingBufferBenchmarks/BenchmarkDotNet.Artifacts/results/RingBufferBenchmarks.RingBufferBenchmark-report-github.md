```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.304
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  Job-NTRUNJ : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
| Method                            | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|---------------------------------- |----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
| RingBuffer_Foreach                | 10.708 μs | 0.0324 μs | 0.0050 μs |  1.61 |    0.00 |       - |      - |      40 B |          NA |
| GrowingRingBuffer_Foreach         | 10.702 μs | 0.0172 μs | 0.0027 μs |  1.61 |    0.00 |       - |      - |      40 B |          NA |
| RingBuffer_Get                    | 12.986 μs | 0.0234 μs | 0.0061 μs |  1.95 |    0.00 |       - |      - |         - |          NA |
| GrowingRingBuffer_Get             | 12.775 μs | 0.0203 μs | 0.0053 μs |  1.92 |    0.00 |       - |      - |         - |          NA |
| Queue_Dequeue                     |  3.577 μs | 0.0142 μs | 0.0037 μs |  0.54 |    0.00 |       - |      - |         - |          NA |
| GrowingRingBuffer_WithExpansion   | 59.904 μs | 0.6409 μs | 0.1664 μs |  9.00 |    0.02 | 12.2070 | 0.0610 |  204400 B |          NA |
| RingBuffer_MixedOperations        | 20.287 μs | 0.0103 μs | 0.0016 μs |  3.05 |    0.00 |       - |      - |         - |          NA |
| GrowingRingBuffer_MixedOperations | 20.097 μs | 0.0025 μs | 0.0004 μs |  3.02 |    0.00 |       - |      - |         - |          NA |
| RingBuffer_Overflow               |  6.380 μs | 0.0051 μs | 0.0013 μs |  0.96 |    0.00 |       - |      - |         - |          NA |
| RingBuffer_Put                    |  6.658 μs | 0.0156 μs | 0.0024 μs |  1.00 |    0.00 |       - |      - |         - |          NA |
| GrowingRingBuffer_Put             |  6.664 μs | 0.0041 μs | 0.0006 μs |  1.00 |    0.00 |       - |      - |         - |          NA |
| Queue_Enqueue                     |  2.004 μs | 0.0091 μs | 0.0024 μs |  0.30 |    0.00 |       - |      - |         - |          NA |
