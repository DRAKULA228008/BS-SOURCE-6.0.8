using System;
using System.IO;

namespace BestHTTP.Extensions
{
	public class ReadOnlyBufferedStream : Stream
	{
		public const int READBUFFER = 8192;

		private Stream stream;

		private byte[] buf;

		private int available;

		private int pos;

		public bool CheckForDataAvailability { get; set; }

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool CanWrite
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long Length
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public ReadOnlyBufferedStream(Stream nstream)
		{
			stream = nstream;
			buf = VariableSizedBufferPool.Get(8192L, true);
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			//Discarded unreachable code: IL_00c3
			if (size <= available)
			{
				Array.Copy(buf, pos, buffer, offset, size);
				available -= size;
				pos += size;
				return size;
			}
			int num = 0;
			if (available > 0)
			{
				Array.Copy(buf, pos, buffer, offset, available);
				offset += available;
				num += available;
				available = 0;
				pos = 0;
			}
			int num2;
			while (true)
			{
				try
				{
					available = stream.Read(buf, 0, buf.Length);
					pos = 0;
				}
				catch (Exception ex)
				{
					if (num > 0)
					{
						return num;
					}
					throw ex;
				}
				if (available < 1)
				{
					if (num > 0)
					{
						return num;
					}
					return 0;
				}
				num2 = size - num;
				if (num2 <= available)
				{
					break;
				}
				Array.Copy(buf, pos, buffer, offset, available);
				offset += available;
				num += available;
				pos = 0;
				available = 0;
			}
			Array.Copy(buf, pos, buffer, offset, num2);
			available -= num2;
			pos += num2;
			return num + num2;
		}

		public override int ReadByte()
		{
			//Discarded unreachable code: IL_006c
			if (available > 0)
			{
				available--;
				pos++;
				return buf[pos - 1];
			}
			try
			{
				available = stream.Read(buf, 0, buf.Length);
				pos = 0;
			}
			catch
			{
				return -1;
			}
			if (available < 1)
			{
				return -1;
			}
			available--;
			pos++;
			return buf[pos - 1];
		}

		protected override void Dispose(bool disposing)
		{
			if (buf != null)
			{
				VariableSizedBufferPool.Release(buf);
			}
			buf = null;
		}

		public override void Flush()
		{
			throw new NotImplementedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
	}
}
