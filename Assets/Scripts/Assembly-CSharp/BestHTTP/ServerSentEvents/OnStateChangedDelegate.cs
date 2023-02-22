namespace BestHTTP.ServerSentEvents
{
	public delegate void OnStateChangedDelegate(EventSource eventSource, States oldState, States newState);
}
