using UnityEngine;
using UnityEngine.Events;

public class TriggerCallback : MonoBehaviour
{
	public UnityEvent EnterEvent;

	public UnityEvent ExitEvent;

	public static void Set(GameObject go, UnityAction enterAction, UnityAction exitAction)
	{
		if (!(go == null))
		{
			TriggerCallback triggerCallback = go.AddComponent<TriggerCallback>();
			triggerCallback.EnterEvent.AddListener(enterAction);
			triggerCallback.ExitEvent.AddListener(exitAction);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (EnterEvent != null)
		{
			EnterEvent.Invoke();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (ExitEvent != null)
		{
			ExitEvent.Invoke();
		}
	}
}
