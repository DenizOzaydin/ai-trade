using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Core.Collections
{
    public class Deque<T>
    {
        private T[] array;
        public int Capacity { get; private set; }
        private int left = 0;
        private int right = 1;
        public int Size { get; private set; }

        public Deque(int cap)
        {
            Capacity = cap;
            array = new T[cap];
        }

        public void PushBack(T item)
        {
            Size++;
            array[left] = item;
            left = (left - 1 + Capacity) % Capacity;
        }

        public T GetBack()
        {
            return array[(left + 1) % Capacity];
        }

        public T PopBack()
        {
            Size--;
            left = (left + 1) % Capacity;
            return array[left];
        }

        public void PushFront(T item)
        {
            Size++;
            array[right] = item;
            right = (right + 1) % Capacity;
        }

        public T GetFront()
        {
            return array[(right - 1 + Capacity) % Capacity];
        }

        public T PopFront()
        {
            Size--;
            right = (right - 1 + Capacity) % Capacity;
            return array[right];
        }
    }
}
