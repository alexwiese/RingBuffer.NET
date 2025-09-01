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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RingBuffer;

namespace RingBufferTests;

[TestClass]
public class GrowingRingBufferTests
{
    [TestMethod]
    public void DefaultConstructor_CreatesBufferWithCapacity4()
    {
        var buffer = new GrowingRingBuffer<int>();
        Assert.AreEqual(4, buffer.Capacity);
        Assert.AreEqual(0, buffer.Size);
    }

    [TestMethod]
    public void ConstructorWithCapacity_CreatesBufferWithSpecifiedCapacity()
    {
        var buffer = new GrowingRingBuffer<int>(10);
        Assert.AreEqual(10, buffer.Capacity);
        Assert.AreEqual(0, buffer.Size);
    }

    [TestMethod]
    public void Put_ExpandsCapacityWhenFull()
    {
        var buffer = new GrowingRingBuffer<int>(2);
        Assert.AreEqual(2, buffer.Capacity);

        // Fill initial capacity
        buffer.Put(1);
        buffer.Put(2);
        Assert.AreEqual(2, buffer.Size);
        Assert.AreEqual(2, buffer.Capacity);

        // Adding one more should expand capacity
        buffer.Put(3);
        Assert.AreEqual(3, buffer.Size);
        Assert.AreEqual(4, buffer.Capacity); // Should double from 2 to 4
    }

    [TestMethod]
    public void Put_ExpandsByOriginalCapacityIncrement()
    {
        var buffer = new GrowingRingBuffer<int>(3);

        // Fill initial capacity
        for (int i = 0; i < 3; i++)
        {
            buffer.Put(i);
        }
        Assert.AreEqual(3, buffer.Capacity);

        // Add one more to trigger expansion
        buffer.Put(3);
        Assert.AreEqual(6, buffer.Capacity); // Should expand by original capacity (3)

        // Add more to trigger another expansion
        for (int i = 4; i < 6; i++)
        {
            buffer.Put(i);
        }
        Assert.AreEqual(6, buffer.Capacity); // Still 6

        // Add one more to trigger second expansion
        buffer.Put(6);
        Assert.AreEqual(9, buffer.Capacity); // Should expand by original capacity again (3)
    }

    [TestMethod]
    public void Put_MaintainsCorrectOrder()
    {
        var buffer = new GrowingRingBuffer<int>(2);

        // Fill and expand
        buffer.Put(1);
        buffer.Put(2);
        buffer.Put(3);
        buffer.Put(4);

        // Verify order
        Assert.AreEqual(1, buffer.Get());
        Assert.AreEqual(2, buffer.Get());
        Assert.AreEqual(3, buffer.Get());
        Assert.AreEqual(4, buffer.Get());
    }

    [TestMethod]
    public void Add_CallsBasePutMethodButDoesNotExpand()
    {
        var buffer = new GrowingRingBuffer<int>(2);

        // Fill initial capacity using Add (which calls base.Put)
        buffer.Add(1);
        buffer.Add(2);

        // This should throw exception because Add calls base.Put, not GrowingRingBuffer.Put
        Assert.ThrowsException<InvalidOperationException>(() => buffer.Add(3));

        // But using Put directly should work and expand
        var buffer2 = new GrowingRingBuffer<int>(2);
        buffer2.Put(1);
        buffer2.Put(2);
        buffer2.Put(3); // Should expand

        Assert.AreEqual(3, buffer2.Size);
        Assert.AreEqual(4, buffer2.Capacity);
    }

    [TestMethod]
    public void MultipleExpansions_WorkCorrectly()
    {
        var buffer = new GrowingRingBuffer<int>(2);

        // Add enough items to cause multiple expansions
        for (int i = 0; i < 10; i++)
        {
            buffer.Put(i);
        }

        Assert.AreEqual(10, buffer.Size);
        Assert.AreEqual(10, buffer.Capacity); // Started at 2, expanded to 4, then 6, then 8, then 10

        // Verify all items are in correct order
        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(i, buffer.Get());
        }
    }

    [TestMethod]
    public void Enumeration_WorksAfterExpansion()
    {
        var buffer = new GrowingRingBuffer<int>(3);

        // Add items causing expansion
        for (int i = 0; i < 7; i++)
        {
            buffer.Put(i);
        }

        // Test enumeration
        int expected = 0;
        foreach (int item in buffer)
        {
            Assert.AreEqual(expected, item);
            expected++;
        }
        Assert.AreEqual(7, expected);
    }

    [TestMethod]
    public void Clear_ResetsAfterExpansion()
    {
        var buffer = new GrowingRingBuffer<int>(2);

        // Add items causing expansion
        for (int i = 0; i < 5; i++)
        {
            buffer.Put(i);
        }

        Assert.AreEqual(5, buffer.Size);
        Assert.AreEqual(6, buffer.Capacity); // 2 -> 4 -> 6

        buffer.Clear();

        Assert.AreEqual(0, buffer.Size);
        Assert.AreEqual(6, buffer.Capacity); // Capacity remains expanded
    }

    [TestMethod]
    public void Contains_WorksAfterExpansion()
    {
        var buffer = new GrowingRingBuffer<int>(2);

        // Add items causing expansion
        for (int i = 0; i < 5; i++)
        {
            buffer.Put(i);
        }

        // Test contains for all items
        for (int i = 0; i < 5; i++)
        {
            Assert.IsTrue(buffer.Contains(i));
        }

        Assert.IsFalse(buffer.Contains(5));
        Assert.IsFalse(buffer.Contains(-1));
    }

    [TestMethod]
    public void CopyTo_WorksAfterExpansion()
    {
        var buffer = new GrowingRingBuffer<int>(2);

        // Add items causing expansion
        for (int i = 0; i < 5; i++)
        {
            buffer.Put(i);
        }

        var array = new int[10];
        buffer.CopyTo(array, 2);

        // Check that items were copied correctly starting at index 2
        for (int i = 0; i < 5; i++)
        {
            Assert.AreEqual(i, array[i + 2]);
        }
    }

    [TestMethod]
    public void Remove_WorksAfterExpansion()
    {
        var buffer = new GrowingRingBuffer<int>(2);

        // Add items causing expansion
        for (int i = 0; i < 5; i++)
        {
            buffer.Put(i);
        }

        Assert.IsTrue(buffer.Remove(2));
        Assert.AreEqual(4, buffer.Size);
        Assert.IsFalse(buffer.Contains(2));

        Assert.IsFalse(buffer.Remove(10)); // Non-existent item
    }
}
