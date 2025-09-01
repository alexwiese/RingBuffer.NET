```

BenchmarkDotNet v0.15.2, Linux Ubuntu 24.04.3 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.304
  [Host]     : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
  Job-NTRUNJ : .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2

IterationCount=5  WarmupCount=3  

```
| Method                            | Mean       | Error     | StdDev    | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|---------------------------------- |-----------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|------------:|
| RingBuffer_Foreach                |  10.233 μs | 0.1116 μs | 0.0290 μs |  1.56 |    0.00 |  0.2441 |      - |    4096 B |          NA |
| GrowingRingBuffer_Foreach         |  10.201 μs | 0.0892 μs | 0.0232 μs |  1.56 |    0.00 |  0.2441 |      - |    4096 B |          NA |
| RingBuffer_Get                    |  12.452 μs | 0.0498 μs | 0.0077 μs |  1.90 |    0.00 |       - |      - |         - |          NA |
| GrowingRingBuffer_Get             |  12.458 μs | 0.0275 μs | 0.0071 μs |  1.90 |    0.00 |       - |      - |         - |          NA |
| Queue_Dequeue                     |   3.581 μs | 0.0140 μs | 0.0036 μs |  0.55 |    0.00 |       - |      - |         - |          NA |
| GrowingRingBuffer_WithExpansion   | 141.696 μs | 8.4932 μs | 2.2057 μs | 21.62 |    0.31 | 12.2070 | 0.2441 |  204480 B |          NA |
| RingBuffer_MixedOperations        |  12.529 μs | 0.0536 μs | 0.0139 μs |  1.91 |    0.00 |       - |      - |         - |          NA |
| GrowingRingBuffer_MixedOperations |  12.041 μs | 0.0246 μs | 0.0038 μs |  1.84 |    0.00 |       - |      - |         - |          NA |
| RingBuffer_Overflow               |   6.250 μs | 0.0131 μs | 0.0020 μs |  0.95 |    0.00 |       - |      - |         - |          NA |
| RingBuffer_Put                    |   6.555 μs | 0.0141 μs | 0.0036 μs |  1.00 |    0.00 |       - |      - |         - |          NA |
| GrowingRingBuffer_Put             |   6.561 μs | 0.0232 μs | 0.0036 μs |  1.00 |    0.00 |       - |      - |         - |          NA |
| Queue_Enqueue                     |   1.984 μs | 0.0028 μs | 0.0007 μs |  0.30 |    0.00 |       - |      - |         - |          NA |
