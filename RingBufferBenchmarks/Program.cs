using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using RingBuffer;
using System.Collections.Generic;

namespace RingBufferBenchmarks;

class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<RingBufferBenchmark>();
    }
}

[MemoryDiagnoser]
[SimpleJob]
public class RingBufferBenchmark
{
    private const int OperationCount = 10000;
    private const int SmallCapacity = 100;
    private const int MediumCapacity = 1000;
    private const int LargeCapacity = 10000;

    private RingBuffer<int> _smallRingBuffer = null!;
    private RingBuffer<int> _mediumRingBuffer = null!;
    private RingBuffer<int> _largeRingBuffer = null!;
    private RingBuffer<int> _overflowBuffer = null!;

    private GrowingRingBuffer<int> _smallGrowingBuffer = null!;
    private GrowingRingBuffer<int> _mediumGrowingBuffer = null!;
    private GrowingRingBuffer<int> _largeGrowingBuffer = null!;

    private Queue<int> _queue = null!;
    private List<int> _list = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallRingBuffer = new RingBuffer<int>(SmallCapacity);
        _mediumRingBuffer = new RingBuffer<int>(MediumCapacity);
        _largeRingBuffer = new RingBuffer<int>(LargeCapacity);
        _overflowBuffer = new RingBuffer<int>(SmallCapacity, true); // Allow overflow

        _smallGrowingBuffer = new GrowingRingBuffer<int>(SmallCapacity);
        _mediumGrowingBuffer = new GrowingRingBuffer<int>(MediumCapacity);
        _largeGrowingBuffer = new GrowingRingBuffer<int>(LargeCapacity);

        _queue = new Queue<int>();
        _list = new List<int>();
    }

    #region RingBuffer Put Benchmarks

    [Benchmark]
    [BenchmarkCategory("RingBuffer", "Put", "Small")]
    public void RingBuffer_Put_Small()
    {
        _smallRingBuffer.Clear();
        for (int i = 0; i < SmallCapacity; i++)
        {
            _smallRingBuffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("RingBuffer", "Put", "Medium")]
    public void RingBuffer_Put_Medium()
    {
        _mediumRingBuffer.Clear();
        for (int i = 0; i < MediumCapacity; i++)
        {
            _mediumRingBuffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("RingBuffer", "Put", "Large")]
    public void RingBuffer_Put_Large()
    {
        _largeRingBuffer.Clear();
        for (int i = 0; i < LargeCapacity; i++)
        {
            _largeRingBuffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("RingBuffer", "Put", "Overflow")]
    public void RingBuffer_Put_Overflow()
    {
        _overflowBuffer.Clear();
        for (int i = 0; i < OperationCount; i++) // Much more than capacity
        {
            _overflowBuffer.Put(i);
        }
    }

    #endregion

    #region RingBuffer Get Benchmarks

    [Benchmark]
    [BenchmarkCategory("RingBuffer", "Get")]
    public void RingBuffer_Get_PreFilled()
    {
        // Fill buffer first
        _largeRingBuffer.Clear();
        for (int i = 0; i < LargeCapacity; i++)
        {
            _largeRingBuffer.Put(i);
        }

        // Now benchmark getting
        for (int i = 0; i < LargeCapacity; i++)
        {
            _largeRingBuffer.Get();
        }
    }

    #endregion

    #region GrowingRingBuffer Benchmarks

    [Benchmark]
    [BenchmarkCategory("GrowingRingBuffer", "Put", "Small")]
    public void GrowingRingBuffer_Put_Small()
    {
        _smallGrowingBuffer.Clear();
        for (int i = 0; i < SmallCapacity; i++)
        {
            _smallGrowingBuffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("GrowingRingBuffer", "Put", "Medium")]
    public void GrowingRingBuffer_Put_Medium()
    {
        _mediumGrowingBuffer.Clear();
        for (int i = 0; i < MediumCapacity; i++)
        {
            _mediumGrowingBuffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("GrowingRingBuffer", "Put", "Large")]
    public void GrowingRingBuffer_Put_Large()
    {
        _largeGrowingBuffer.Clear();
        for (int i = 0; i < LargeCapacity; i++)
        {
            _largeGrowingBuffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("GrowingRingBuffer", "Put", "Expansion")]
    public void GrowingRingBuffer_Put_WithExpansion()
    {
        var buffer = new GrowingRingBuffer<int>(10); // Small initial capacity
        for (int i = 0; i < OperationCount; i++) // Force many expansions
        {
            buffer.Put(i);
        }
    }

    [Benchmark]
    [BenchmarkCategory("GrowingRingBuffer", "Get")]
    public void GrowingRingBuffer_Get_PreFilled()
    {
        // Fill buffer first
        _largeGrowingBuffer.Clear();
        for (int i = 0; i < LargeCapacity; i++)
        {
            _largeGrowingBuffer.Put(i);
        }

        // Now benchmark getting
        for (int i = 0; i < LargeCapacity; i++)
        {
            _largeGrowingBuffer.Get();
        }
    }

    #endregion

    #region Comparison with .NET Collections

    [Benchmark]
    [BenchmarkCategory("Comparison", "Queue")]
    public void Queue_EnqueueDequeue()
    {
        _queue.Clear();
        
        // Fill queue
        for (int i = 0; i < LargeCapacity; i++)
        {
            _queue.Enqueue(i);
        }

        // Dequeue all
        while (_queue.Count > 0)
        {
            _queue.Dequeue();
        }
    }

    [Benchmark]
    [BenchmarkCategory("Comparison", "List")]
    public void List_AddRemove()
    {
        _list.Clear();
        
        // Fill list
        for (int i = 0; i < LargeCapacity; i++)
        {
            _list.Add(i);
        }

        // Remove from beginning (expensive for List)
        for (int i = 0; i < LargeCapacity; i++)
        {
            if (_list.Count > 0)
                _list.RemoveAt(0);
        }
    }

    #endregion

    #region Enumeration Benchmarks

    [Benchmark]
    [BenchmarkCategory("Enumeration", "RingBuffer")]
    public void RingBuffer_Enumeration()
    {
        _largeRingBuffer.Clear();
        for (int i = 0; i < LargeCapacity; i++)
        {
            _largeRingBuffer.Put(i);
        }

        int sum = 0;
        foreach (int item in _largeRingBuffer)
        {
            sum += item;
        }
    }

    [Benchmark]
    [BenchmarkCategory("Enumeration", "GrowingRingBuffer")]
    public void GrowingRingBuffer_Enumeration()
    {
        _largeGrowingBuffer.Clear();
        for (int i = 0; i < LargeCapacity; i++)
        {
            _largeGrowingBuffer.Put(i);
        }

        int sum = 0;
        foreach (int item in _largeGrowingBuffer)
        {
            sum += item;
        }
    }

    [Benchmark]
    [BenchmarkCategory("Enumeration", "Queue")]
    public void Queue_Enumeration()
    {
        _queue.Clear();
        for (int i = 0; i < LargeCapacity; i++)
        {
            _queue.Enqueue(i);
        }

        int sum = 0;
        foreach (int item in _queue)
        {
            sum += item;
        }
    }

    #endregion

    #region Memory Usage Benchmarks

    [Benchmark]
    [BenchmarkCategory("Memory", "RingBuffer")]
    public RingBuffer<int> RingBuffer_MemoryAllocation()
    {
        var buffer = new RingBuffer<int>(LargeCapacity);
        for (int i = 0; i < LargeCapacity; i++)
        {
            buffer.Put(i);
        }
        return buffer;
    }

    [Benchmark]
    [BenchmarkCategory("Memory", "GrowingRingBuffer")]
    public GrowingRingBuffer<int> GrowingRingBuffer_MemoryAllocation()
    {
        var buffer = new GrowingRingBuffer<int>(100); // Start small
        for (int i = 0; i < LargeCapacity; i++)
        {
            buffer.Put(i);
        }
        return buffer;
    }

    [Benchmark]
    [BenchmarkCategory("Memory", "Queue")]
    public Queue<int> Queue_MemoryAllocation()
    {
        var queue = new Queue<int>();
        for (int i = 0; i < LargeCapacity; i++)
        {
            queue.Enqueue(i);
        }
        return queue;
    }

    #endregion

    #region Mixed Operations Benchmarks

    [Benchmark]
    [BenchmarkCategory("Mixed", "RingBuffer")]
    public void RingBuffer_MixedOperations()
    {
        _mediumRingBuffer.Clear();
        
        // Mixed put/get operations
        for (int i = 0; i < OperationCount; i++)
        {
            _mediumRingBuffer.Put(i);
            if (i % 3 == 0 && _mediumRingBuffer.Size > 0)
            {
                _mediumRingBuffer.Get();
            }
            if (i % 7 == 0)
            {
                _mediumRingBuffer.Contains(i / 2);
            }
        }
    }

    [Benchmark]
    [BenchmarkCategory("Mixed", "GrowingRingBuffer")]
    public void GrowingRingBuffer_MixedOperations()
    {
        _mediumGrowingBuffer.Clear();
        
        // Mixed put/get operations
        for (int i = 0; i < OperationCount; i++)
        {
            _mediumGrowingBuffer.Put(i);
            if (i % 3 == 0 && _mediumGrowingBuffer.Size > 0)
            {
                _mediumGrowingBuffer.Get();
            }
            if (i % 7 == 0)
            {
                _mediumGrowingBuffer.Contains(i / 2);
            }
        }
    }

    #endregion
}
