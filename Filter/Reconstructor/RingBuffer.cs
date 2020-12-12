using System;
using System.Collections;
using System.Collections.Generic;

namespace VoiDPlugins.Filter
{
    internal class RingBuffer<T> : IEnumerable<T>
    {
        public int Size { private set; get; }
        public bool IsFilled { private set; get; }

        private T[] dataStream;
        private int head;

        public RingBuffer(int size)
        {
            this.Size = size;
            this.dataStream = new T[size];
        }

        public void Insert(T item)
        {
            this.dataStream[this.head++] = item;
            if (this.head == this.Size)
            {
                this.head = 0;
                this.IsFilled = true;
            }
        }

        public T PeekFirst()
        {
            var currentPosition = this.head;
            return this.dataStream[currentPosition];
        }

        public T PeekLast()
        {
            var last = Math.Abs(this.head - 1);
            return this.dataStream[last];
        }

        public void Clear()
        {
            this.dataStream = new T[this.Size];
            this.head = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (this.head == 0)
            {
                foreach (var item in this.dataStream)
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in this.dataStream[this.head..^0])
                {
                    yield return item;
                }
                foreach (var item in this.dataStream[0..this.head])
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}