using System;

namespace BestHTTP.Futures
{
	public interface IFuture<T>
	{
		FutureState state { get; }

		T value { get; }

		Exception error { get; }

		IFuture<T> OnItem(FutureValueCallback<T> callback);

		IFuture<T> OnSuccess(FutureValueCallback<T> callback);

		IFuture<T> OnError(FutureErrorCallback callback);

		IFuture<T> OnComplete(FutureCallback<T> callback);
	}
}
