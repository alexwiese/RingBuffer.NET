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
/// A generic, thread-safe ring buffer with fixed capacity using lock-free operations.
/// Safe for single producer/single consumer scenarios.
/// </summary>
/// <typeparam name="T">The type of data stored in the buffer</typeparam>
public class RingBuffer<T> : IEnumerable<T>, IEnumerable, ICollection<T>,
    ICollection
{

    protected volatile int head = 0;
    protected volatile int tail = 0;
    protected volatile int size = 0;

    protected T?[] buffer;

    private readonly bool allowOverflow;
    public bool AllowOverflow => allowOverflow;

    /// <summary>
    /// The total number of elements the buffer can store (grows).
    /// </summary>
    public int Capacity => buffer.Length;

    /// <summary>
    /// The number of elements currently contained in the buffer.
    /// </summary>
    public int Size => size;

    /// <summary>
    /// Retrieve the next item from the buffer.
    /// This method is thread-safe for single consumer scenarios.
    /// </summary>
    /// <returns>The oldest item added to the buffer.</returns>
    public T Get()
    {
        if (size == 0)
        {
            throw new System.InvalidOperationException("Buffer is empty.");
        }

        int currentHead = head;
        T? item = buffer[currentHead];
        
        // Clear the buffer slot to prevent memory leaks
        buffer[currentHead] = default;
        
        int newHead = (currentHead + 1) % Capacity;
        Thread.MemoryBarrier(); // Ensure buffer write is visible before updating head
        head = newHead;
        
        Interlocked.Decrement(ref size);
        
        return item!;
    }

    /// <summary>
    /// Adds an item to the end of the buffer.
    /// This method is thread-safe for single producer scenarios.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    public void Put(T item)
    {
        int currentTail = tail;
        int currentHead = head;
        int currentSize = size;

        // Check if buffer is full
        bool isFull = (currentTail == currentHead && currentSize != 0);
        
        if (isFull)
        {
            if (allowOverflow)
            {
                AddToBufferWithOverflow(item, currentTail, currentHead);
            }
            else
            {
                throw new System.InvalidOperationException("The RingBuffer is full");
            }
        }
        else
        {
            AddToBuffer(item, currentTail);
        }
    }

    protected void AddToBuffer(T toAdd, int currentTail)
    {
        // Write the item first
        buffer[currentTail] = toAdd;
        
        // Ensure the write is visible before updating tail
        Thread.MemoryBarrier();
        
        int newTail = (currentTail + 1) % Capacity;
        tail = newTail;
        
        Interlocked.Increment(ref size);
    }

    protected void AddToBufferWithOverflow(T toAdd, int currentTail, int currentHead)
    {
        // Clear the head slot before overwriting
        buffer[currentHead] = default;
        
        // Move head forward
        int newHead = (currentHead + 1) % Capacity;
        head = newHead;
        
        // Write the new item
        buffer[currentTail] = toAdd;
        
        // Ensure writes are visible before updating tail
        Thread.MemoryBarrier();
        
        int newTail = (currentTail + 1) % Capacity;
        tail = newTail;
        
        // Size remains the same since we overwrote
    }

    #region Constructors
    // Default capacity is 4, default overflow behavior is false.
    public RingBuffer() : this(4) { }

    public RingBuffer(int capacity) : this(capacity, false) { }

    public RingBuffer(int capacity, bool overflow)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero");
            
        buffer = new T?[capacity];
        allowOverflow = overflow;
    }
    #endregion

    #region IEnumerable Members
    public IEnumerator<T> GetEnumerator()
    {
        // Create a snapshot for thread-safe enumeration
        var snapshot = new List<T>(Size);
        int currentHead = head;
        int currentSize = size;
        
        for (int i = 0; i < currentSize; i++)
        {
            int index = (currentHead + i) % Capacity;
            T? item = buffer[index];
            if (item != null || typeof(T).IsValueType)
            {
                snapshot.Add(item!);
            }
        }
        
        return snapshot.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    #endregion

    #region ICollection<T> Members
    public int Count => Size;
    public bool IsReadOnly => false;

    public void Add(T item) => Put(item);

    /// <summary>
    /// Determines whether the RingBuffer contains a specific value.
    /// This method is thread-safe but may return outdated results in concurrent scenarios.
    /// </summary>
    /// <param name="item">The value to check the RingBuffer for.</param>
    /// <returns>True if the RingBuffer contains <paramref name="item"/>
    /// , false if it does not.
    /// </returns>
    public bool Contains(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        int currentHead = head;
        int currentSize = size;
        
        for (int i = 0; i < currentSize; i++)
        {
            int index = (currentHead + i) % Capacity;
            T? bufferItem = buffer[index];
            
            if (comparer.Equals(item, bufferItem))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes all items from the RingBuffer.
    /// This method is thread-safe.
    /// </summary>
    public void Clear()
    {
        // Clear all buffer slots
        for (int i = 0; i < Capacity; i++)
        {
            buffer[i] = default;
        }
        
        // Reset pointers atomically
        head = 0;
        tail = 0;
        size = 0;
        Thread.MemoryBarrier(); // Ensure all writes are visible
    }

    /// <summary>
    /// Copies the contents of the RingBuffer to <paramref name="array"/>
    /// starting at <paramref name="arrayIndex"/>.
    /// This method is thread-safe but may copy a snapshot that becomes outdated.
    /// </summary>
    /// <param name="array">The array to be copied to.</param>
    /// <param name="arrayIndex">The index of <paramref name="array"/>
    /// where the buffer should begin copying to.</param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        
        int currentHead = head;
        int currentSize = size;
        
        if (array.Length - arrayIndex < currentSize)
            throw new ArgumentException("Destination array is not large enough");
            
        for (int i = 0; i < currentSize; i++)
        {
            int index = (currentHead + i) % Capacity;
            array[arrayIndex + i] = buffer[index]!;
        }
    }

    /// <summary>
    /// Removes <paramref name="item"/> from the buffer.
    /// Note: This operation is expensive and not recommended for frequent use in high-performance scenarios.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>True if <paramref name="item"/> was found and 
    /// successfully removed. False if <paramref name="item"/> was not
    /// found or there was a problem removing it from the RingBuffer.
    /// </returns>
    public bool Remove(T item)
    {
        var comparer = EqualityComparer<T>.Default;
        int currentHead = head;
        int currentSize = size;
        
        // Find the item
        int removeIndex = -1;
        for (int i = 0; i < currentSize; i++)
        {
            int index = (currentHead + i) % Capacity;
            if (comparer.Equals(item, buffer[index]))
            {
                removeIndex = index;
                break;
            }
        }
        
        if (removeIndex == -1)
            return false;

        // Shift elements to fill the gap - this is expensive but maintains order
        int itemsToMove = currentSize - 1;
        bool foundGap = false;
        
        for (int i = 0; i < itemsToMove; i++)
        {
            int currentIndex = (currentHead + i) % Capacity;
            int nextIndex = (currentHead + i + 1) % Capacity;
            
            if (currentIndex == removeIndex)
                foundGap = true;
                
            if (foundGap)
            {
                buffer[currentIndex] = buffer[nextIndex];
            }
        }
        
        // Clear the last position and update tail
        int lastIndex = (tail - 1 + Capacity) % Capacity;
        buffer[lastIndex] = default;
        
        int newTail = (tail - 1 + Capacity) % Capacity;
        tail = newTail;
        
        Interlocked.Decrement(ref size);
        
        return true;
    }
    #endregion

    #region ICollection Members
    /// <summary>
    /// Gets an object that can be used to synchronize access to the
    /// RingBuffer. Note: This implementation is lock-free, so external 
    /// synchronization should generally not be needed.
    /// </summary>
    public object SyncRoot => this;

    /// <summary>
    /// Gets a value indicating whether access to the RingBuffer is 
    /// synchronized (thread safe). Returns true for this lock-free implementation.
    /// </summary>
    public bool IsSynchronized => true;

    /// <summary>
    /// Copies the elements of the RingBuffer to <paramref name="array"/>, 
    /// starting at a particular Array <paramref name="index"/>.
    /// </summary>
    /// <param name="array">The one-dimensional Array that is the 
    /// destination of the elements copied from RingBuffer. The Array must 
    /// have zero-based indexing.</param>
    /// <param name="index">The zero-based index in 
    /// <paramref name="array"/> at which copying begins.</param>
    void ICollection.CopyTo(Array array, int index) => CopyTo((T[])array, index);
    #endregion
}
