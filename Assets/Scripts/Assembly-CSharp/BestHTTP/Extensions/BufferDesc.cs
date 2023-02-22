using System;

namespace BestHTTP.Extensions
{
	internal struct BufferDesc
	{
		public static readonly BufferDesc Empty = new BufferDesc(null);

		public byte[] buffer;

		public DateTime released;

		public BufferDesc(byte[] buff)
		{
			buffer = buff;
			released = DateTime.UtcNow;
		}
	}
}
