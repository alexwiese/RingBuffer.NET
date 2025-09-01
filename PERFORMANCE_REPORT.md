# RingBuffer.NET Performance Report

This document provides a comprehensive analysis of the performance characteristics of RingBuffer.NET library, comparing `RingBuffer<T>` and `GrowingRingBuffer<T>` implementations against .NET's standard `Queue<T>`.

## Test Environment

- **OS**: Linux Ubuntu 24.04.3 LTS (Noble Numbat)  
- **CPU**: AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
- **Runtime**: .NET 9.0.8 (9.0.825.36511), X64 RyuJIT AVX2
- **GC**: Concurrent Workstation
- **Benchmark Framework**: BenchmarkDotNet v0.15.2

## Test Configuration

- **Operation Count**: 1,000 operations per benchmark
- **Iterations**: 5 per benchmark with 3 warmup iterations
- **Buffer Capacities**: Small (100), Large (1,000)
- **Memory Diagnostics**: Enabled (tracks allocations and GC)

## Benchmark Results

### Raw Performance Data

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

## Performance Analysis

### 1. Put/Add Operations

**Winner: Queue (3.3x faster than RingBuffer)**

- **Queue_Enqueue**: 2.004 μs (fastest)
- **RingBuffer_Put**: 6.658 μs (baseline)
- **GrowingRingBuffer_Put**: 6.664 μs (virtually identical to RingBuffer)
- **RingBuffer_Overflow**: 6.380 μs (slightly faster due to overwrite behavior)

**Key Findings:**
- .NET's `Queue<T>` is significantly faster for sequential add operations
- `RingBuffer` and `GrowingRingBuffer` have nearly identical performance for basic Put operations
- Overflow behavior in RingBuffer adds minimal overhead
- All implementations have zero memory allocations during normal operations

### 2. Get/Remove Operations  

**Winner: Queue (3.6x faster than RingBuffer)**

- **Queue_Dequeue**: 3.577 μs (fastest)
- **GrowingRingBuffer_Get**: 12.775 μs
- **RingBuffer_Get**: 12.986 μs

**Key Findings:**
- Queue significantly outperforms both ring buffer implementations
- GrowingRingBuffer is marginally faster than RingBuffer for Get operations
- The circular buffer logic adds substantial overhead compared to Queue's simple pointer management

### 3. Enumeration Performance

**Winner: Tie (Both ring buffers perform identically)**

- **GrowingRingBuffer_Foreach**: 10.702 μs
- **RingBuffer_Foreach**: 10.708 μs

**Key Findings:**
- Both ring buffer types have virtually identical enumeration performance
- Enumeration allocates 40 bytes (likely for iterator state)
- No GC pressure during enumeration

### 4. Memory Expansion Performance

**Critical Finding: GrowingRingBuffer expansion is expensive**

- **GrowingRingBuffer_WithExpansion**: 59.904 μs (9x slower than baseline)
- **Memory Allocated**: 204,400 bytes
- **GC Impact**: Significant (Gen0: 12.2070, Gen1: 0.0610 collections per 1000 operations)

**Key Findings:**
- Dynamic growth comes at a significant performance cost
- Each expansion requires copying the entire buffer
- Heavy memory allocation pressure triggers garbage collection
- Should be avoided in performance-critical scenarios with frequent resizing

### 5. Mixed Operations Performance

**Winner: Tie (Both ring buffers perform similarly)**

- **GrowingRingBuffer_MixedOperations**: 20.097 μs
- **RingBuffer_MixedOperations**: 20.287 μs

**Key Findings:**
- Mixed workloads (Put + Get + Contains) show minimal difference between implementations
- Performance is about 3x slower than baseline Put operations
- Contains operations add measurable overhead

## Recommendations

### Use RingBuffer<T> When:
- ✅ You need fixed-capacity circular buffer behavior
- ✅ Memory usage predictability is important
- ✅ You want overflow handling (overwrite old data)
- ✅ You need zero-allocation operations
- ✅ Buffer size is known at design time

### Use GrowingRingBuffer<T> When:
- ✅ Buffer size cannot be predetermined
- ✅ Occasional growth is acceptable (infrequent)
- ⚠️ **Avoid** when frequent resizing is expected
- ⚠️ **Avoid** in performance-critical hot paths

### Use Queue<T> When:
- ✅ You need maximum performance for FIFO operations
- ✅ Simple enqueue/dequeue pattern without size limits
- ✅ No circular buffer semantics required
- ✅ Memory allocations are acceptable

## Performance Characteristics Summary

| Operation | RingBuffer | GrowingRingBuffer | Queue | Winner |
|-----------|------------|-------------------|--------|--------|
| **Put/Add** | 6.658 μs | 6.664 μs | 2.004 μs | **Queue** (3.3x faster) |
| **Get/Remove** | 12.986 μs | 12.775 μs | 3.577 μs | **Queue** (3.6x faster) |
| **Enumeration** | 10.708 μs | 10.702 μs | N/A | **Tie** |
| **Memory Efficiency** | **Excellent** | Good* | Good | **RingBuffer** |
| **Growth Handling** | Fixed | **Automatic** | Automatic | **GrowingRingBuffer** |

*GrowingRingBuffer memory efficiency degrades significantly during expansion operations.

## Conclusion

The RingBuffer.NET library provides solid circular buffer implementations with distinct use cases:

- **RingBuffer<T>** excels in scenarios requiring predictable memory usage and fixed-capacity circular semantics
- **GrowingRingBuffer<T>** adds flexibility at the cost of potential performance degradation during growth
- **.NET Queue<T>** remains the performance champion for simple FIFO operations

Choose the implementation that best matches your specific requirements for capacity management, performance characteristics, and memory constraints.

---
*Report generated on: 2025-09-01*
*Benchmark data: See `RingBufferBenchmarks/BenchmarkDotNet.Artifacts/results/` for detailed results*