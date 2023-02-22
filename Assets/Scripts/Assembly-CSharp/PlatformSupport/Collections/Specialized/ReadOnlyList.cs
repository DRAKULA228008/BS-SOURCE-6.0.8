using System;
using System.Collections;

namespace PlatformSupport.Collections.Specialized
{
	internal sealed class ReadOnlyList : IList, ICollection, IEnumerable
	{
		private readonly IList _list;

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return _list.IsSynchronized;
			}
		}

		public object this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public object SyncRoot
		{
			get
			{
				return _list.SyncRoot;
			}
		}

		internal ReadOnlyList(IList list)
		{
			_list = list;
		}

		public int Add(object value)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(object value)
		{
			return _list.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			_list.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return _list.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			throw new NotSupportedException();
		}

		public void Remove(object value)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
	}
}
