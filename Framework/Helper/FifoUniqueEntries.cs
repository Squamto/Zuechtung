// -----------------------------------------------------------------------
// <copyright file="FifoUniqueEntries.cs" company="IB Hermann">
// Copyright (c) IB Hermann Mirow. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Framework.Helper
{
	using System;

	/// <summary>
	/// Defines a fifo stack of strings with unique entrys.
	/// </summary>
	public class FifoUniqueEntries
    {
        private readonly int capacity;
        private readonly List<string> queue;
        private readonly HashSet<string> set;

        public FifoUniqueEntries(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be greater than zero.");
            }

            this.capacity = capacity;
            this.queue = new List<string>();
            this.set = new HashSet<string>();
        }

        public void Enqueue(string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                throw new ArgumentException("Item cannot be null or empty.");
            }

            if (this.set.Contains(item))
            {
                // Remove the existing item from the queue
                this.queue.Remove(item);
                this.set.Remove(item);
            }
            else if (this.queue.Count >= this.capacity)
            {
                string removed = this.queue[0];
                this.queue.RemoveAt(0);
                set.Remove(removed);
            }

            this.queue.Add(item);
            this.set.Add(item);
        }

        public string GetElementAt(int index)
        {
            if (index < 0 || index >= this.queue.Count)
            {
                throw new ArgumentOutOfRangeException("Index is out of range.");
            }

            return this.queue[index];
        }

        public int Count => queue.Count;
    }
}
