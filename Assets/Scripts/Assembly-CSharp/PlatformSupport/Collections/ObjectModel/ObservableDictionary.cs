using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PlatformSupport.Collections.Specialized;

namespace PlatformSupport.Collections.ObjectModel
{
	public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private const string CountString = "Count";

		private const string IndexerName = "Item[]";

		private const string KeysName = "Keys";

		private const string ValuesName = "Values";

		private IDictionary<TKey, TValue> _Dictionary;

		protected IDictionary<TKey, TValue> Dictionary
		{
			get
			{
				return _Dictionary;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return Dictionary.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return Dictionary.Values;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return Dictionary[key];
			}
			set
			{
				Insert(key, value, false);
			}
		}

		public int Count
		{
			get
			{
				return Dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return Dictionary.IsReadOnly;
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableDictionary()
		{
			_Dictionary = new Dictionary<TKey, TValue>();
		}

		public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
		{
			_Dictionary = new Dictionary<TKey, TValue>(dictionary);
		}

		public ObservableDictionary(IEqualityComparer<TKey> comparer)
		{
			_Dictionary = new Dictionary<TKey, TValue>(comparer);
		}

		public ObservableDictionary(int capacity)
		{
			_Dictionary = new Dictionary<TKey, TValue>(capacity);
		}

		public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
		{
			_Dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
		}

		public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			_Dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)Dictionary).GetEnumerator();
		}

		public void Add(TKey key, TValue value)
		{
			Insert(key, value, true);
		}

		public bool ContainsKey(TKey key)
		{
			return Dictionary.ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			TValue value;
			Dictionary.TryGetValue(key, out value);
			bool flag = Dictionary.Remove(key);
			if (flag)
			{
				OnCollectionChanged();
			}
			return flag;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return Dictionary.TryGetValue(key, out value);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			Insert(item.Key, item.Value, true);
		}

		public void Clear()
		{
			if (Dictionary.Count > 0)
			{
				Dictionary.Clear();
				OnCollectionChanged();
			}
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return Dictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			Dictionary.CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return Remove(item.Key);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return Dictionary.GetEnumerator();
		}

		public void AddRange(IDictionary<TKey, TValue> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			if (items.Count <= 0)
			{
				return;
			}
			if (Dictionary.Count > 0)
			{
				if (items.Keys.Any((TKey k) => Dictionary.ContainsKey(k)))
				{
					throw new ArgumentException("An item with the same key has already been added.");
				}
				foreach (KeyValuePair<TKey, TValue> item in items)
				{
					Dictionary.Add(item);
				}
			}
			else
			{
				_Dictionary = new Dictionary<TKey, TValue>(items);
			}
			OnCollectionChanged(NotifyCollectionChangedAction.Add, items.ToArray());
		}

		private void Insert(TKey key, TValue value, bool add)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			TValue value2;
			if (Dictionary.TryGetValue(key, out value2))
			{
				if (add)
				{
					throw new ArgumentException("An item with the same key has already been added.");
				}
				if (!object.Equals(value2, value))
				{
					Dictionary[key] = value;
					OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value), new KeyValuePair<TKey, TValue>(key, value2));
				}
			}
			else
			{
				Dictionary[key] = value;
				OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value));
			}
		}

		private void OnPropertyChanged()
		{
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnPropertyChanged("Keys");
			OnPropertyChanged("Values");
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void OnCollectionChanged()
		{
			OnPropertyChanged();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> changedItem)
		{
			OnPropertyChanged();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, changedItem));
			}
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<TKey, TValue> newItem, KeyValuePair<TKey, TValue> oldItem)
		{
			OnPropertyChanged();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem));
			}
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, IList newItems)
		{
			OnPropertyChanged();
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, newItems));
			}
		}
	}
}
