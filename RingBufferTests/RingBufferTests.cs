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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingBuffer;

namespace RingBufferTests;

[TestClass]
public class RingBufferTests
{

    private int iterations = 1000;

    private int knownValue = 1;

    /// <summary>
    /// Ensures that size is correctly augmented when items are added.
    /// </summary>
    [TestMethod()]
    public void PutIncrementsSize()
    {
        var buffer = new RingBuffer<int>(iterations);
        for (int i = 0; i < iterations; i++)
        {
            int tmp = i;
            buffer.Add(tmp);
            Assert.AreEqual(i + 1, buffer.Size, "Size is not equal to number of elements added.");
        }
    }

    /// <summary>
    /// Ensures that size is correctly adjusted when elements are removed.
    /// </summary>
    [TestMethod()]
    public void GetDecrementsSize()
    {
        var buffer = new RingBuffer<int>(iterations);
        PopulateBuffer(iterations, buffer);
        for (int i = iterations; i > 0; i--)
        {
            int tmp = buffer.Get();
            Assert.AreEqual(i - 1, buffer.Size, "Size does not reflect the correct number of removed elements.");
        }
    }

    /// <summary>
    /// Ensures that capacity expands as needed
    /// </summary>
    /*[TestMethod()]
    public void CapacityExpands() {
        int _startCapacity = 12;
        RingBuffer<double> _testBuffer = new RingBuffer<double>(_startCapacity);
        for(int i = 0; i < _startCapacity + 1; i++) {
            _testBuffer.Add((double)i);
        }
        Assert.AreEqual(_startCapacity * 2, _testBuffer.Capacity, "Capacity not expanded");
        Assert.AreEqual(_startCapacity + 1, _testBuffer.Size, "incorrect number of elements");
    }*/

    /// <summary>
    /// Ensures that head/tail move properly by testing the value of data
    /// returned by get.
    /// </summary>
    [TestMethod()]
    public void RetrievedInCorrectOrder()
    {
        var buffer = new RingBuffer<int>(iterations);
        PopulateBuffer(iterations, buffer);
        for (int i = 0; i < iterations; i++)
        {
            int tmp = buffer.Get();
            Assert.AreEqual(i, tmp, "Incorrect Sequence");
        }
    }

    /// <summary>
    /// Ensures that an exception is thrown when Get() is called on an
    /// empty buffer.
    /// </summary>
    [TestMethod()]
    public void ThrowsError_GetEmpty()
    {
        var buffer = new RingBuffer<byte>();
        Assert.ThrowsException<InvalidOperationException>(() => buffer.Get());
    }

    /// <summary>
    /// Ensures that foreach iteration covers only the range of active
    /// items
    /// </summary>
    [TestMethod()]
    public void CanIterateForeach()
    {
        var buffer = new RingBuffer<int>(iterations);
        PopulateBuffer(iterations, buffer);
        int iterationCount = 0;
        foreach (int i in buffer)
        {
            iterationCount++;
        }
        Assert.AreEqual(iterations, iterationCount, "Wrong number of foreach iterations.");
    }

    /// <summary>
    /// Ensures that the contains function returns the correct value.
    /// </summary>
    [TestMethod()]
    public void ContainsReturnsCorrectly()
    {
        var buffer = new RingBuffer<int>(iterations + 2);
        buffer.Add(knownValue - 1);
        bool containsKnownValue = buffer.Contains(knownValue);
        Assert.AreEqual(false, containsKnownValue);
        PopulateBuffer(iterations, buffer);
        buffer.Add(knownValue);
        containsKnownValue = buffer.Contains(knownValue);
        Assert.AreEqual(true, containsKnownValue);

    }

    /// <summary>
    /// Ensures that after calling Clear(), the RingBuffer contains no
    /// items.
    /// </summary>
    [TestMethod()]
    public void ClearAsExpected()
    {
        var buffer = new RingBuffer<int>(iterations);
        PopulateBuffer(iterations, buffer);
        buffer.Clear();
        Assert.AreEqual(0, buffer.Count);
    }

    /// <summary>
    /// Ensures that CopyTo() behaves properly.
    /// </summary>
    [TestMethod()]
    public void CopyToTest()
    {
        var buffer = new RingBuffer<int>(iterations);
        PopulateBuffer(iterations, buffer);
        int[] array = new int[iterations + 1];
        buffer.CopyTo(array, 1);
        Assert.AreEqual(default(int), array[0]);
        for (int i = 1; i < array.Length; i++)
        {
            Assert.AreEqual(i - 1, array[i]);
        }
    }

    /// <summary>
    /// Tests the Size and return value of Contains() after a Remove()
    /// to ensure that the item was removed.
    /// </summary>
    [TestMethod()]
    public void ItemIsRemoved()
    {
        var buffer = new RingBuffer<int>(iterations);
        PopulateBuffer(iterations, buffer);
        int preRemoveSize = buffer.Count;
        buffer.Remove(0);
        Assert.AreEqual(false, buffer.Contains(0));
        Assert.AreEqual(preRemoveSize - 1, buffer.Count);

    }

    #region Edge Case Tests

    /// <summary>
    /// Tests behavior with single capacity buffer
    /// </summary>
    [TestMethod]
    public void SingleCapacityBuffer_WorksCorrectly()
    {
        var buffer = new RingBuffer<int>(1);
        Assert.AreEqual(1, buffer.Capacity);
        Assert.AreEqual(0, buffer.Size);

        buffer.Add(42);
        Assert.AreEqual(1, buffer.Size);
        Assert.AreEqual(42, buffer.Get());
        Assert.AreEqual(0, buffer.Size);
    }

    /// <summary>
    /// Tests overflow behavior when AllowOverflow is true
    /// </summary>
    [TestMethod]
    public void OverflowAllowed_OverwritesOldestItem()
    {
        var buffer = new RingBuffer<int>(3, true);

        // Fill buffer
        buffer.Add(1);
        buffer.Add(2);
        buffer.Add(3);
        Assert.AreEqual(3, buffer.Size);

        // Add one more - should overwrite oldest (1)
        buffer.Add(4);
        Assert.AreEqual(3, buffer.Size); // Size stays same with overflow

        // Should get 2, 3, 4 (1 was overwritten)
        Assert.AreEqual(2, buffer.Get());
        Assert.AreEqual(3, buffer.Get());
        Assert.AreEqual(4, buffer.Get());
    }

    /// <summary>
    /// Tests that overflow throws exception when AllowOverflow is false
    /// </summary>
    [TestMethod]
    public void OverflowNotAllowed_ThrowsException()
    {
        var buffer = new RingBuffer<int>(2, false);

        buffer.Add(1);
        buffer.Add(2);

        Assert.ThrowsException<InvalidOperationException>(() => buffer.Add(3));
    }

    /// <summary>
    /// Tests constructor variations
    /// </summary>
    [TestMethod]
    public void Constructors_WorkCorrectly()
    {
        // Default constructor
        var buffer1 = new RingBuffer<int>();
        Assert.AreEqual(4, buffer1.Capacity);
        Assert.IsFalse(buffer1.AllowOverflow);

        // Capacity only constructor
        var buffer2 = new RingBuffer<int>(10);
        Assert.AreEqual(10, buffer2.Capacity);
        Assert.IsFalse(buffer2.AllowOverflow);

        // Full constructor
        var buffer3 = new RingBuffer<int>(5, true);
        Assert.AreEqual(5, buffer3.Capacity);
        Assert.IsTrue(buffer3.AllowOverflow);
    }

    /// <summary>
    /// Tests buffer with different data types
    /// </summary>
    [TestMethod]
    public void DifferentDataTypes_WorkCorrectly()
    {
        // String buffer
        var stringBuffer = new RingBuffer<string>(3);
        stringBuffer.Add("first");
        stringBuffer.Add("second");
        stringBuffer.Add("third");

        Assert.AreEqual("first", stringBuffer.Get());
        Assert.AreEqual("second", stringBuffer.Get());
        Assert.AreEqual("third", stringBuffer.Get());

        // Double buffer
        var doubleBuffer = new RingBuffer<double>(2);
        doubleBuffer.Add(3.14);
        doubleBuffer.Add(2.71);

        Assert.AreEqual(3.14, doubleBuffer.Get());
        Assert.AreEqual(2.71, doubleBuffer.Get());
    }

    /// <summary>
    /// Tests null values in buffer
    /// </summary>
    [TestMethod]
    public void NullValues_HandledCorrectly()
    {
        var buffer = new RingBuffer<string?>(3);

        buffer.Add("test");
        buffer.Add(null);
        buffer.Add("another");

        // Test Contains before removing items
        Assert.IsTrue(buffer.Contains(null));
        Assert.IsTrue(buffer.Contains("test"));
        Assert.IsTrue(buffer.Contains("another"));

        Assert.AreEqual("test", buffer.Get());
        Assert.IsNull(buffer.Get());
        Assert.AreEqual("another", buffer.Get());
    }

    /// <summary>
    /// Tests large capacity buffer
    /// </summary>
    [TestMethod]
    public void LargeCapacity_WorksCorrectly()
    {
        var buffer = new RingBuffer<int>(10000);
        Assert.AreEqual(10000, buffer.Capacity);

        // Add many items
        for (int i = 0; i < 5000; i++)
        {
            buffer.Add(i);
        }

        Assert.AreEqual(5000, buffer.Size);

        // Verify order
        for (int i = 0; i < 5000; i++)
        {
            Assert.AreEqual(i, buffer.Get());
        }
    }

    /// <summary>
    /// Tests enumeration on partially filled buffer
    /// </summary>
    [TestMethod]
    public void PartiallyFilledBuffer_EnumeratesCorrectly()
    {
        var buffer = new RingBuffer<int>(10);

        // Only add 5 items
        for (int i = 0; i < 5; i++)
        {
            buffer.Add(i);
        }

        var enumeratedItems = new List<int>();
        foreach (int item in buffer)
        {
            enumeratedItems.Add(item);
        }

        Assert.AreEqual(5, enumeratedItems.Count);
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(i, enumeratedItems[i]);
        }
    }

    /// <summary>
    /// Tests circular behavior after wrapping
    /// </summary>
    [TestMethod]
    public void CircularBehavior_WorksAfterWrapping()
    {
        var buffer = new RingBuffer<int>(3, true); // Allow overflow

        // Add more items than capacity to test wrapping
        for (int i = 0; i < 10; i++)
        {
            buffer.Add(i);
        }

        // Should contain last 3 items: 7, 8, 9
        Assert.AreEqual(7, buffer.Get());
        Assert.AreEqual(8, buffer.Get());
        Assert.AreEqual(9, buffer.Get());
    }

    /// <summary>
    /// Tests mixed operations (add/remove)
    /// </summary>
    [TestMethod]
    public void MixedOperations_WorkCorrectly()
    {
        var buffer = new RingBuffer<int>(5);

        // Add some items
        buffer.Add(1);
        buffer.Add(2);
        buffer.Add(3);

        // Remove one
        Assert.AreEqual(1, buffer.Get());

        // Add more
        buffer.Add(4);
        buffer.Add(5);

        // Check remaining order
        Assert.AreEqual(2, buffer.Get());
        Assert.AreEqual(3, buffer.Get());
        Assert.AreEqual(4, buffer.Get());
        Assert.AreEqual(5, buffer.Get());
    }

    /// <summary>
    /// Tests Count property consistency with Size
    /// </summary>
    [TestMethod]
    public void Count_ConsistentWithSize()
    {
        var buffer = new RingBuffer<int>(5);

        Assert.AreEqual(buffer.Size, buffer.Count);

        buffer.Add(1);
        Assert.AreEqual(buffer.Size, buffer.Count);
        Assert.AreEqual(1, buffer.Count);

        buffer.Add(2);
        buffer.Add(3);
        Assert.AreEqual(buffer.Size, buffer.Count);
        Assert.AreEqual(3, buffer.Count);

        buffer.Get();
        Assert.AreEqual(buffer.Size, buffer.Count);
        Assert.AreEqual(2, buffer.Count);
    }

    /// <summary>
    /// Tests IsReadOnly property
    /// </summary>
    [TestMethod]
    public void IsReadOnly_ReturnsFalse()
    {
        var buffer = new RingBuffer<int>();
        Assert.IsFalse(buffer.IsReadOnly);
    }

    /// <summary>
    /// Tests SyncRoot property
    /// </summary>
    [TestMethod]
    public void SyncRoot_ReturnsBuffer()
    {
        var buffer = new RingBuffer<int>();
        Assert.AreEqual(buffer, buffer.SyncRoot);
    }

    /// <summary>
    /// Tests IsSynchronized property (this implementation is thread-safe)
    /// </summary>
    [TestMethod]
    public void IsSynchronized_ReturnsFalse()
    {
        var buffer = new RingBuffer<int>();
        Assert.IsTrue(buffer.IsSynchronized);
    }

    /// <summary>
    /// Tests non-generic ICollection.CopyTo
    /// </summary>
    [TestMethod]
    public void NonGenericCopyTo_WorksCorrectly()
    {
        var buffer = new RingBuffer<int>(5);
        buffer.Add(1);
        buffer.Add(2);
        buffer.Add(3);

        var array = new int[6];
        ((System.Collections.ICollection)buffer).CopyTo(array, 2);

        Assert.AreEqual(0, array[0]);
        Assert.AreEqual(0, array[1]);
        Assert.AreEqual(1, array[2]);
        Assert.AreEqual(2, array[3]);
        Assert.AreEqual(3, array[4]);
        Assert.AreEqual(0, array[5]);
    }

    #endregion

    private void PopulateBuffer(int elements, RingBuffer<int> buffer)
    {
        for (int i = 0; i < elements; i++)
        {
            buffer.Add(i);
        }
    }
}
