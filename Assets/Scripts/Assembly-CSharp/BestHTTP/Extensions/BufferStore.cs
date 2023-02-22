using System.Collections.Generic;

namespace BestHTTP.Extensions
{
	internal struct BufferStore
	{
		public readonly long Size;

		public List<BufferDesc> buffers;

		public BufferStore(long size)
		{
			Size = size;
			buffers = new List<BufferDesc>();
		}

		public BufferStore(long size, byte[] buffer)
			: this(size)
		{
			buffers.Add(new BufferDesc(buffer));
		}
	}
}
