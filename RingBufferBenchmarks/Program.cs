using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using RingBuffer;

namespace RingBufferBenchmarks;

class Program
{
    static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
            .AddExporter(MarkdownExporter.GitHub)
            .AddExporter(CsvExporter.Default)
            .AddJob(Job.Default.WithIterationCount(5).WithWarmupCount(3)); // Faster execution

        var summary = BenchmarkRunner.Run<RingBufferBenchmark>(config);
    }
}

[MemoryDiagnoser]
public class RingBufferBenchmark
{
    private const int OperationCount = 1000;
    private const int SmallCapacity = 100;
    private const int LargeCapacity = 1000;

    private RingBuffer<int> _ringBuffer = null!;
    private RingBuffer<int> _overflowBuffer = null!;
    private GrowingRingBuffer<int> _growingBuffer = null!;
    private Queue<int> _queue = null!;

    [GlobalSetup]
    public void Setup()
    {
        _ringBuffer = new RingBuffer<int>(LargeCapacity);
        _overflowBuffer = new RingBuffer<int>(SmallCapacity, true);
        _growingBuffer = new GrowingRingBuffer<int>(SmallCapacity);
        _queue = new Queue<int>();
    }

    [Benchmark(Baseline = true)]
    [BenchmarkCategory("Put")]
    public void RingBuffer_Put()
    {
        _ringBuffer.Clear();
        for (int i = 0; i < OperationCount; i++)
        {
            _ringBuffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Put")]
    public void GrowingRingBuffer_Put()
    {
        _growingBuffer.Clear();
        for (int i = 0; i < OperationCount; i++)
        {
            _growingBuffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Put")]
    public void Queue_Enqueue()
    {
        _queue.Clear();
        for (int i = 0; i < OperationCount; i++)
        {
            _queue.Enqueue(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Get")]
    public void RingBuffer_Get()
    {
        // Pre-fill
        _ringBuffer.Clear();
        for (int i = 0; i < OperationCount; i++)
        {
            _ringBuffer.Put(i);
        }

        // Benchmark get
        for (int i = 0; i < OperationCount; i++)
        {
            _ringBuffer.Get();
        }
    }

    [Benchmark]
    [BenchmarkCategory("Get")]
    public void GrowingRingBuffer_Get()
    {
        // Pre-fill
        _growingBuffer.Clear();
        for (int i = 0; i < OperationCount; i++)
        {
            _growingBuffer.Put(i);
        }

        // Benchmark get
        for (int i = 0; i < OperationCount; i++)
        {
            _growingBuffer.Get();
        }
    }

    [Benchmark]
    [BenchmarkCategory("Get")]
    public void Queue_Dequeue()
    {
        // Pre-fill
        _queue.Clear();
        for (int i = 0; i < OperationCount; i++)
        {
            _queue.Enqueue(i);
        }

        // Benchmark dequeue
        for (int i = 0; i < OperationCount; i++)
        {
            _queue.Dequeue();
        }
    }

    [Benchmark]
    [BenchmarkCategory("Overflow")]
    public void RingBuffer_Overflow()
    {
        _overflowBuffer.Clear();
        for (int i = 0; i < OperationCount; i++)
        {
            _overflowBuffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Growth")]
    public void GrowingRingBuffer_WithExpansion()
    {
        var buffer = new GrowingRingBuffer<int>(10);
        for (int i = 0; i < OperationCount; i++)
        {
            buffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Enumeration")]
    public void RingBuffer_Foreach()
    {
        _ringBuffer.Clear();
        for (int i = 0; i < OperationCount; i++)
        {
            _ringBuffer.Put(i);
        }

        int sum = 0;
        foreach (int item in _ringBuffer)
        {
            sum += item;
        }
    }

    [Benchmark]
    [BenchmarkCategory("Enumeration")]
    public void GrowingRingBuffer_Foreach()
    {
        _growingBuffer.Clear();
        for (int i = 0; i < OperationCount; i++)
        {
            _growingBuffer.Put(i);
        }

        int sum = 0;
        foreach (int item in _growingBuffer)
        {
            sum += item;
        }
    }

    [Benchmark]
    [BenchmarkCategory("Mixed")]
    public void RingBuffer_MixedOperations()
    {
        _ringBuffer.Clear();

        for (int i = 0; i < OperationCount / 2; i++)
        {
            _ringBuffer.Put(i);
            if (i % 3 == 0 && _ringBuffer.Size > 0)
            {
                _ringBuffer.Get();
            }
            if (i % 5 == 0)
            {
                _ringBuffer.Contains(i / 2);
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("Mixed")]
    public void GrowingRingBuffer_MixedOperations()
    {
        _growingBuffer.Clear();

        for (int i = 0; i < OperationCount / 2; i++)
        {
            _growingBuffer.Put(i);
            if (i % 3 == 0 && _growingBuffer.Size > 0)
            {
                _growingBuffer.Get();
            }
            if (i % 5 == 0)
            {
                _growingBuffer.Contains(i / 2);
            }
        }
    }
}
