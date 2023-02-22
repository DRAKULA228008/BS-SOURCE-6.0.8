namespace BestHTTP
{
	public sealed class HTTPRange
	{
		public int FirstBytePos { get; private set; }

		public int LastBytePos { get; private set; }

		public int ContentLength { get; private set; }

		public bool IsValid { get; private set; }

		internal HTTPRange()
		{
			ContentLength = -1;
			IsValid = false;
		}

		internal HTTPRange(int contentLength)
		{
			ContentLength = contentLength;
			IsValid = false;
		}

		internal HTTPRange(int firstBytePosition, int lastBytePosition, int contentLength)
		{
			FirstBytePos = firstBytePosition;
			LastBytePos = lastBytePosition;
			ContentLength = contentLength;
			IsValid = FirstBytePos <= LastBytePos && ContentLength > LastBytePos;
		}

		public override string ToString()
		{
			return string.Format("{0}-{1}/{2} (valid: {3})", FirstBytePos, LastBytePos, ContentLength, IsValid);
		}
	}
}
