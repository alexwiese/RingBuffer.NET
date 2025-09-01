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
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace RingBuffer;

/// <summary>
/// A generic, thread-safe ring buffer that grows when capacity is reached.
/// Uses lock-free operations where possible, but expansion operations require brief synchronization.
/// </summary>
/// <typeparam name="T">The type of data stored in the buffer</typeparam>
public class GrowingRingBuffer<T> : RingBuffer<T>
{

    private readonly int originalCapacity;
    private readonly object expandLock = new object();

    /// <summary>
    /// Adds an item to the end of the buffer.
    /// This method is thread-safe and will expand capacity if needed.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public new void Put(T item)
    {
        int currentTail = tail;
        int currentHead = head;
        int currentSize = size;

        // Check if buffer is full
        bool isFull = (currentTail == currentHead && currentSize != 0);

        if (isFull)
        {
            ExpandAndAdd(item);
        }
        else
        {
            AddToBuffer(item, currentTail);
        }
    }

    private void ExpandAndAdd(T item)
    {
        lock (expandLock)
        {
            // Double-check pattern - another thread might have already expanded
            int currentTail = tail;
            int currentHead = head;
            int currentSize = size;
            bool stillFull = (currentTail == currentHead && currentSize != 0);

            if (stillFull)
            {
                // Expand the buffer
                var newCapacity = buffer.Length + originalCapacity;
                var newBuffer = new T?[newCapacity];

                // Copy existing items to new buffer starting at index 0
                for (int i = 0; i < currentSize; i++)
                {
                    int sourceIndex = (currentHead + i) % buffer.Length;
                    newBuffer[i] = buffer[sourceIndex];
                }

                // Clear old buffer to prevent memory leaks
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = default;
                }

                // Replace the buffer
                buffer = newBuffer;

                head = 0;
                tail = currentSize;

                Thread.MemoryBarrier(); // Ensure all updates are visible
            }

            // Add the item using the base class method
            base.Put(item);
        }
    }

    #region Constructors
    // Default capacity is 4.
    public GrowingRingBuffer() : this(4) { }
    // Capture the starting capacity, for future expansion.
    public GrowingRingBuffer(int startCapacity)
        : base(startCapacity)
    {
        if (startCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(startCapacity), "Start capacity must be greater than zero");
        }

        originalCapacity = startCapacity;
    }
    #endregion
}
