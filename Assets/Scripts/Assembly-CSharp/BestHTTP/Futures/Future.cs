using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace BestHTTP.Futures
{
	public class Future<T> : IFuture<T>
	{
		private volatile FutureState _state;

		private T _value;

		private Exception _error;

		private Func<T> _processFunc;

		private readonly List<FutureValueCallback<T>> _itemCallbacks = new List<FutureValueCallback<T>>();

		private readonly List<FutureValueCallback<T>> _successCallbacks = new List<FutureValueCallback<T>>();

		private readonly List<FutureErrorCallback> _errorCallbacks = new List<FutureErrorCallback>();

		private readonly List<FutureCallback<T>> _complationCallbacks = new List<FutureCallback<T>>();

		public FutureState state
		{
			get
			{
				return _state;
			}
		}

		public T value
		{
			get
			{
				if (_state != FutureState.Success && _state != FutureState.Processing)
				{
					throw new InvalidOperationException("value is not available unless state is Success or Processing.");
				}
				return _value;
			}
		}

		public Exception error
		{
			get
			{
				if (_state != FutureState.Error)
				{
					throw new InvalidOperationException("error is not available unless state is Error.");
				}
				return _error;
			}
		}

		public Future()
		{
			_state = FutureState.Pending;
		}

		public IFuture<T> OnItem(FutureValueCallback<T> callback)
		{
			if (_state < FutureState.Success && !_itemCallbacks.Contains(callback))
			{
				_itemCallbacks.Add(callback);
			}
			return this;
		}

		public IFuture<T> OnSuccess(FutureValueCallback<T> callback)
		{
			if (_state == FutureState.Success)
			{
				callback(value);
			}
			else if (_state != FutureState.Error && !_successCallbacks.Contains(callback))
			{
				_successCallbacks.Add(callback);
			}
			return this;
		}

		public IFuture<T> OnError(FutureErrorCallback callback)
		{
			if (_state == FutureState.Error)
			{
				callback(error);
			}
			else if (_state != FutureState.Success && !_errorCallbacks.Contains(callback))
			{
				_errorCallbacks.Add(callback);
			}
			return this;
		}

		public IFuture<T> OnComplete(FutureCallback<T> callback)
		{
			if (_state == FutureState.Success || _state == FutureState.Error)
			{
				callback(this);
			}
			else if (!_complationCallbacks.Contains(callback))
			{
				_complationCallbacks.Add(callback);
			}
			return this;
		}

		public IFuture<T> Process(Func<T> func)
		{
			if (_state != 0)
			{
				throw new InvalidOperationException("Cannot process a future that isn't in the Pending state.");
			}
			BeginProcess();
			_processFunc = func;
			ThreadPool.QueueUserWorkItem(ThreadFunc);
			return this;
		}

		private void ThreadFunc(object param)
		{
			try
			{
				AssignImpl(_processFunc());
			}
			catch (Exception ex)
			{
				FailImpl(ex);
			}
			finally
			{
				_processFunc = null;
			}
		}

		public void Assign(T value)
		{
			if (_state != 0 && _state != FutureState.Processing)
			{
				throw new InvalidOperationException("Cannot assign a value to a future that isn't in the Pending or Processing state.");
			}
			AssignImpl(value);
		}

		public void BeginProcess([Optional] T initialItem)
		{
			_state = FutureState.Processing;
			_value = initialItem;
		}

		public void AssignItem(T value)
		{
			_value = value;
			_error = null;
			foreach (FutureValueCallback<T> itemCallback in _itemCallbacks)
			{
				itemCallback(this.value);
			}
		}

		public void Fail(Exception error)
		{
			if (_state != 0 && _state != FutureState.Processing)
			{
				throw new InvalidOperationException("Cannot fail future that isn't in the Pending or Processing state.");
			}
			FailImpl(error);
		}

		private void AssignImpl(T value)
		{
			_value = value;
			_error = null;
			_state = FutureState.Success;
			FlushSuccessCallbacks();
		}

		private void FailImpl(Exception error)
		{
			_value = default(T);
			_error = error;
			_state = FutureState.Error;
			FlushErrorCallbacks();
		}

		private void FlushSuccessCallbacks()
		{
			foreach (FutureValueCallback<T> successCallback in _successCallbacks)
			{
				successCallback(value);
			}
			FlushComplationCallbacks();
		}

		private void FlushErrorCallbacks()
		{
			foreach (FutureErrorCallback errorCallback in _errorCallbacks)
			{
				errorCallback(error);
			}
			FlushComplationCallbacks();
		}

		private void FlushComplationCallbacks()
		{
			foreach (FutureCallback<T> complationCallback in _complationCallbacks)
			{
				complationCallback(this);
			}
			ClearCallbacks();
		}

		private void ClearCallbacks()
		{
			_itemCallbacks.Clear();
			_successCallbacks.Clear();
			_errorCallbacks.Clear();
			_complationCallbacks.Clear();
		}
	}
}
