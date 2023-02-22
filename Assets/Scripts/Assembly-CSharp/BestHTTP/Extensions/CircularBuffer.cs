using System;

namespace BestHTTP.Extensions
{
	public sealed class CircularBuffer<T>
	{
		private T[] buffer;

		private int startIdx;

		private int endIdx;

		public int Capacity { get; private set; }

		public int Count { get; private set; }

		public T this[int idx]
		{
			get
			{
				int num = (startIdx + idx) % Capacity;
				return buffer[num];
			}
			set
			{
				int num = (startIdx + idx) % Capacity;
				buffer[num] = value;
			}
		}

		public CircularBuffer(int capacity)
		{
			Capacity = capacity;
		}

		public void Add(T element)
		{
			if (buffer == null)
			{
				buffer = new T[Capacity];
			}
			buffer[endIdx] = element;
			endIdx = (endIdx + 1) % Capacity;
			if (endIdx == startIdx)
			{
				startIdx = (startIdx + 1) % Capacity;
			}
			Count = Math.Min(Count + 1, Capacity);
		}

		public void Clear()
		{
			startIdx = (endIdx = 0);
		}
	}
}
