using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VoiDPlugins.Library
{
    public class RingBuffer<T> : IEnumerable<T>
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

        public void Clear()
        {
            this.dataStream = new T[this.Size];
            this.head = 0;
        }

        private int Wrap(int index)
        {
            return (index + this.Size) % this.Size;
        }

        IEnumerator<T> RingGetEnumerator()
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

        public T this[int index]
        {
            get => this.dataStream[Wrap(index + this.head)];
            set => this.dataStream[Wrap(index + this.head)] = value;
        }

        public T this[Index index]
        {
            get => this.dataStream[Wrap(index.IsFromEnd ? Wrap(this.head - index.Value) : Wrap(index.Value + this.head))];
            set => this.dataStream[Wrap(index.IsFromEnd ? Wrap(this.head - index.Value) : Wrap(index.Value + this.head))] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!this.IsFilled)
                return (IEnumerator<T>)dataStream.GetEnumerator();
            else
                return RingGetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            string a = "";
            foreach (var item in this.SkipLast(1))
                a += $"{item}, ";

            a += this[^1];
            return a;
        }
    }
}