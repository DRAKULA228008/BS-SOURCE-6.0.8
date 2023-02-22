using System;
using System.Collections.Generic;
using System.Text;
using BestHTTP.Logger;

namespace BestHTTP.Extensions
{
	public static class VariableSizedBufferPool
	{
		public static readonly byte[] NoData;

		public static volatile bool _isEnabled;

		public static TimeSpan RemoveOlderThan;

		public static TimeSpan RunMaintenanceEvery;

		public static long MinBufferSize;

		public static long MaxBufferSize;

		public static long MaxPoolSize;

		public static bool RemoveEmptyLists;

		public static bool IsDoubleReleaseCheckEnabled;

		private static List<BufferStore> FreeBuffers;

		private static DateTime lastMaintenance;

		private static volatile int PoolSize;

		private static volatile uint GetBuffers;

		private static volatile uint ReleaseBuffers;

		private static StringBuilder statiscticsBuilder;

		public static bool IsEnabled
		{
			get
			{
				return _isEnabled;
			}
			set
			{
				_isEnabled = value;
				if (!_isEnabled)
				{
					Clear();
				}
			}
		}

		static VariableSizedBufferPool()
		{
			NoData = new byte[0];
			_isEnabled = true;
			RemoveOlderThan = TimeSpan.FromSeconds(30.0);
			RunMaintenanceEvery = TimeSpan.FromSeconds(10.0);
			MinBufferSize = 256L;
			MaxBufferSize = long.MaxValue;
			MaxPoolSize = 10485760L;
			RemoveEmptyLists = true;
			IsDoubleReleaseCheckEnabled = false;
			FreeBuffers = new List<BufferStore>();
			lastMaintenance = DateTime.MinValue;
			PoolSize = 0;
			GetBuffers = 0u;
			ReleaseBuffers = 0u;
			statiscticsBuilder = new StringBuilder();
			IsDoubleReleaseCheckEnabled = false;
		}

		public static byte[] Get(long size, bool canBeLarger)
		{
			//Discarded unreachable code: IL_00ce
			if (!_isEnabled)
			{
				return new byte[size];
			}
			if (size == 0L)
			{
				return NoData;
			}
			lock (FreeBuffers)
			{
				if (FreeBuffers.Count == 0)
				{
					return new byte[size];
				}
				BufferDesc bufferDesc = FindFreeBuffer(size, canBeLarger);
				if (bufferDesc.buffer == null)
				{
					if (canBeLarger)
					{
						if (size < MinBufferSize)
						{
							size = MinBufferSize;
						}
						else if (!IsPowerOfTwo(size))
						{
							size = NextPowerOf2(size);
						}
					}
					return new byte[size];
				}
				GetBuffers++;
				PoolSize -= bufferDesc.buffer.Length;
				return bufferDesc.buffer;
			}
		}

		public static void Release(List<byte[]> buffers)
		{
			if (_isEnabled && buffers != null && buffers.Count != 0)
			{
				for (int i = 0; i < buffers.Count; i++)
				{
					Release(buffers[i]);
				}
			}
		}

		public static void Release(byte[] buffer)
		{
			if (!_isEnabled || buffer == null)
			{
				return;
			}
			int num = buffer.Length;
			if (num == 0 || num > MaxBufferSize)
			{
				return;
			}
			lock (FreeBuffers)
			{
				if (PoolSize + num <= MaxPoolSize)
				{
					PoolSize += num;
					ReleaseBuffers++;
					AddFreeBuffer(buffer);
				}
			}
		}

		public static byte[] Resize(ref byte[] buffer, int newSize, bool canBeLarger)
		{
			if (!_isEnabled)
			{
				Array.Resize(ref buffer, newSize);
				return buffer;
			}
			byte[] array = Get(newSize, canBeLarger);
			Array.Copy(buffer, 0, array, 0, Math.Min(array.Length, buffer.Length));
			Release(buffer);
			return buffer = array;
		}

		public static string GetStatistics(bool showEmptyBuffers = true)
		{
			//Discarded unreachable code: IL_0103
			lock (FreeBuffers)
			{
				statiscticsBuilder.Length = 0;
				statiscticsBuilder.AppendFormat("Pooled array reused count: {0:N0}\n", GetBuffers);
				statiscticsBuilder.AppendFormat("Release call count: {0:N0}\n", ReleaseBuffers);
				statiscticsBuilder.AppendFormat("PoolSize: {0:N0}\n", PoolSize);
				statiscticsBuilder.AppendFormat("Buffers: {0}\n", FreeBuffers.Count);
				for (int i = 0; i < FreeBuffers.Count; i++)
				{
					BufferStore bufferStore = FreeBuffers[i];
					List<BufferDesc> buffers = bufferStore.buffers;
					if (showEmptyBuffers || buffers.Count > 0)
					{
						statiscticsBuilder.AppendFormat("- Size: {0:N0} Count: {1:N0}\n", bufferStore.Size, buffers.Count);
					}
				}
				return statiscticsBuilder.ToString();
			}
		}

		public static void Clear()
		{
			lock (FreeBuffers)
			{
				FreeBuffers.Clear();
				PoolSize = 0;
			}
		}

		internal static void Maintain()
		{
			DateTime utcNow = DateTime.UtcNow;
			if (!_isEnabled || lastMaintenance + RunMaintenanceEvery > utcNow)
			{
				return;
			}
			lastMaintenance = utcNow;
			DateTime dateTime = utcNow - RemoveOlderThan;
			lock (FreeBuffers)
			{
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					HTTPManager.Logger.Information("VariableSizedBufferPool", "Before Maintain: " + GetStatistics());
				}
				for (int i = 0; i < FreeBuffers.Count; i++)
				{
					BufferStore bufferStore = FreeBuffers[i];
					List<BufferDesc> buffers = bufferStore.buffers;
					for (int num = buffers.Count - 1; num >= 0; num--)
					{
						if (buffers[num].released < dateTime)
						{
							int num2 = num + 1;
							buffers.RemoveRange(0, num2);
							PoolSize -= (int)(num2 * bufferStore.Size);
							break;
						}
					}
					if (RemoveEmptyLists && buffers.Count == 0)
					{
						FreeBuffers.RemoveAt(i--);
					}
				}
				if (HTTPManager.Logger.Level == Loglevels.All)
				{
					HTTPManager.Logger.Information("VariableSizedBufferPool", "After Maintain: " + GetStatistics());
				}
			}
		}

		private static bool IsPowerOfTwo(long x)
		{
			return (x & (x - 1)) == 0;
		}

		private static long NextPowerOf2(long x)
		{
			long num;
			for (num = 1L; num <= x; num *= 2)
			{
			}
			return num;
		}

		private static BufferDesc FindFreeBuffer(long size, bool canBeLarger)
		{
			for (int i = 0; i < FreeBuffers.Count; i++)
			{
				BufferStore bufferStore = FreeBuffers[i];
				if (bufferStore.buffers.Count > 0 && (bufferStore.Size == size || (canBeLarger && bufferStore.Size > size)))
				{
					BufferDesc result = bufferStore.buffers[bufferStore.buffers.Count - 1];
					bufferStore.buffers.RemoveAt(bufferStore.buffers.Count - 1);
					return result;
				}
			}
			return BufferDesc.Empty;
		}

		private static void AddFreeBuffer(byte[] buffer)
		{
			int num = buffer.Length;
			for (int i = 0; i < FreeBuffers.Count; i++)
			{
				BufferStore bufferStore = FreeBuffers[i];
				if (bufferStore.Size == num)
				{
					if (IsDoubleReleaseCheckEnabled)
					{
						for (int j = 0; j < bufferStore.buffers.Count; j++)
						{
							if (bufferStore.buffers[j].buffer == buffer)
							{
								HTTPManager.Logger.Error("VariableSizedBufferPool", "Buffer already added to the pool!");
								return;
							}
						}
					}
					bufferStore.buffers.Add(new BufferDesc(buffer));
					return;
				}
				if (bufferStore.Size > num)
				{
					FreeBuffers.Insert(i, new BufferStore(num, buffer));
					return;
				}
			}
			FreeBuffers.Add(new BufferStore(num, buffer));
		}
	}
}
