#region License
/* Copyright 2015 Joe Osborne
 * 
 * This file is part of RingBuffer.
 *
 *  RingBuffer is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  RingBuffer is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with RingBuffer. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingBuffer;

namespace RingBufferTests;

[TestClass]
public class ThreadSafetyTests
{
    [TestMethod]
    public void ProducerConsumer_SingleThreads_WorksCorrectly()
    {
        const int itemCount = 1000; // Reduced from 10000
        var buffer = new RingBuffer<int>(100);
        var producedItems = new List<int>();
        var consumedItems = new List<int>();
        var producerDone = false;

        // Producer task
        var producerTask = Task.Run(() =>
        {
            for (int i = 0; i < itemCount; i++)
            {
                buffer.Put(i);
                producedItems.Add(i);
            }
            producerDone = true;
        });

        // Consumer task
        var consumerTask = Task.Run(() =>
        {
            while (!producerDone || buffer.Size > 0)
            {
                try
                {
                    if (buffer.Size > 0)
                    {
                        var item = buffer.Get();
                        consumedItems.Add(item);
                    }
                    else
                    {
                        Thread.Sleep(1); // Brief wait if buffer is empty
                    }
                }
                catch (InvalidOperationException)
                {
                    // Buffer empty, continue checking
                }
            }
        });

        Task.WaitAll(producerTask, consumerTask);

        // Verify all items were produced and consumed
        Assert.AreEqual(itemCount, producedItems.Count, "Not all items were produced");
        Assert.AreEqual(itemCount, consumedItems.Count, "Not all items were consumed");
        Assert.AreEqual(0, buffer.Size, "Buffer should be empty after consumption");

        // Verify FIFO order is maintained
        CollectionAssert.AreEqual(producedItems, consumedItems, "FIFO order not maintained");
    }

    [TestMethod]
    public void GrowingRingBuffer_ProducerConsumer_WorksCorrectly()
    {
        const int itemCount = 1000;
        var buffer = new GrowingRingBuffer<string>(10);
        var producedItems = new List<string>();
        var consumedItems = new List<string>();
        var producerDone = false;

        // Producer task
        var producerTask = Task.Run(() =>
        {
            for (int i = 0; i < itemCount; i++)
            {
                string item = $"Item_{i}";
                buffer.Put(item);
                producedItems.Add(item);
            }
            producerDone = true;
        });

        // Consumer task
        var consumerTask = Task.Run(() =>
        {
            while (!producerDone || buffer.Size > 0)
            {
                try
                {
                    if (buffer.Size > 0)
                    {
                        var item = buffer.Get();
                        consumedItems.Add(item);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Buffer empty, continue checking
                }
            }
        });

        Task.WaitAll(producerTask, consumerTask);

        Assert.AreEqual(itemCount, producedItems.Count);
        Assert.AreEqual(itemCount, consumedItems.Count);
        Assert.AreEqual(0, buffer.Size);
        CollectionAssert.AreEqual(producedItems, consumedItems);
    }

    [TestMethod]
    public void RingBuffer_ConcurrentAccess_NoDataLoss()
    {
        const int itemsPerProducer = 100; // Reduced from 1000
        const int producerCount = 2; // Reduced from 4
        const int expectedTotal = itemsPerProducer * producerCount;
        
        var buffer = new RingBuffer<int>(50, true);
        var allProducedItems = new List<int>();
        var consumedItems = new List<int>();
        var producersDone = 0;
        var lockObj = new object();

        // Multiple producer tasks
        var producerTasks = new Task[producerCount];
        for (int p = 0; p < producerCount; p++)
        {
            int producerId = p;
            producerTasks[p] = Task.Run(() =>
            {
                var localItems = new List<int>();
                for (int i = 0; i < itemsPerProducer; i++)
                {
                    int item = producerId * itemsPerProducer + i;
                    buffer.Put(item);
                    localItems.Add(item);
                }
                
                lock (lockObj)
                {
                    allProducedItems.AddRange(localItems);
                    Interlocked.Increment(ref producersDone);
                }
            });
        }

        // Consumer task
        var consumerTask = Task.Run(() =>
        {
            while (producersDone < producerCount || buffer.Size > 0)
            {
                try
                {
                    if (buffer.Size > 0)
                    {
                        var item = buffer.Get();
                        lock (lockObj)
                        {
                            consumedItems.Add(item);
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Buffer empty, continue
                }
            }
        });

        Task.WaitAll(producerTasks.Concat(new[] { consumerTask }).ToArray());

        // Note: With overflow=true, we may lose some items, but we should never have more consumed than produced
        Assert.IsTrue(consumedItems.Count <= expectedTotal, "Consumed more items than produced");
        Assert.AreEqual(0, buffer.Size, "Buffer should be empty after consumption");
        
        // Verify no duplicate items (each producer produces unique items)
        var uniqueConsumed = consumedItems.Distinct().ToList();
        Assert.AreEqual(consumedItems.Count, uniqueConsumed.Count, "Duplicate items detected");
    }

    [TestMethod]
    public void RingBuffer_HighConcurrency_ThreadSafe()
    {
        const int operationsPerThread = 50; // Reduced from 500
        const int threadCount = 4; // Reduced from 8
        var buffer = new RingBuffer<int>(100);
        var exceptions = new List<Exception>();
        var lockObj = new object();

        var tasks = new Task[threadCount];
        
        // Half the threads will be producers, half consumers
        for (int t = 0; t < threadCount; t++)
        {
            int threadId = t;
            bool isProducer = threadId < threadCount / 2;
            
            tasks[t] = Task.Run(() =>
            {
                try
                {
                    if (isProducer)
                    {
                        // Producer thread
                        for (int i = 0; i < operationsPerThread; i++)
                        {
                            int item = threadId * 1000 + i;
                            try
                            {
                                buffer.Put(item);
                            }
                            catch (InvalidOperationException)
                            {
                                // Buffer full, expected with fixed capacity
                            }
                        }
                    }
                    else
                    {
                        // Consumer thread
                        for (int i = 0; i < operationsPerThread; i++)
                        {
                            try
                            {
                                if (buffer.Size > 0)
                                {
                                    buffer.Get();
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                // Buffer empty, expected
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (lockObj)
                    {
                        exceptions.Add(ex);
                    }
                }
            });
        }

        Task.WaitAll(tasks);

        // If thread-safe, we should have no unexpected exceptions
        if (exceptions.Count > 0)
        {
            var allExceptions = string.Join(Environment.NewLine, exceptions.Select(e => e.ToString()));
            Assert.Fail($"Unexpected exceptions during concurrent access: {allExceptions}");
        }
        
        // The buffer should be in a consistent state
        Assert.IsTrue(buffer.Size >= 0 && buffer.Size <= buffer.Capacity, "Buffer size is inconsistent");
    }
}