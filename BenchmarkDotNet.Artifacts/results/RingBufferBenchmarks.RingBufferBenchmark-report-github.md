```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.304
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  Job-NTRUNJ : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
| Method                            | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0    | Allocated | Alloc Ratio |
|---------------------------------- |----------:|----------:|----------:|------:|--------:|--------:|----------:|------------:|
| RingBuffer_Foreach                | 10.721 μs | 0.1555 μs | 0.0241 μs |  1.61 |    0.00 |       - |      40 B |          NA |
| GrowingRingBuffer_Foreach         | 10.716 μs | 0.0211 μs | 0.0033 μs |  1.61 |    0.00 |       - |      40 B |          NA |
| RingBuffer_Get                    | 12.989 μs | 0.0066 μs | 0.0017 μs |  1.95 |    0.00 |       - |         - |          NA |
| GrowingRingBuffer_Get             | 12.776 μs | 0.0266 μs | 0.0041 μs |  1.92 |    0.00 |       - |         - |          NA |
| Queue_Dequeue                     |  3.576 μs | 0.0048 μs | 0.0012 μs |  0.54 |    0.00 |       - |         - |          NA |
| GrowingRingBuffer_WithExpansion   | 60.127 μs | 7.5808 μs | 1.9687 μs |  9.03 |    0.27 | 12.2070 |  204400 B |          NA |
| RingBuffer_MixedOperations        | 20.318 μs | 0.0373 μs | 0.0097 μs |  3.05 |    0.00 |       - |         - |          NA |
| GrowingRingBuffer_MixedOperations | 20.018 μs | 0.0094 μs | 0.0024 μs |  3.00 |    0.00 |       - |         - |          NA |
| RingBuffer_Overflow               |  6.396 μs | 0.0041 μs | 0.0006 μs |  0.96 |    0.00 |       - |         - |          NA |
| RingBuffer_Put                    |  6.662 μs | 0.0116 μs | 0.0030 μs |  1.00 |    0.00 |       - |         - |          NA |
| GrowingRingBuffer_Put             |  6.664 μs | 0.0135 μs | 0.0035 μs |  1.00 |    0.00 |       - |         - |          NA |
| Queue_Enqueue                     |  1.998 μs | 0.0110 μs | 0.0029 μs |  0.30 |    0.00 |       - |         - |          NA |
