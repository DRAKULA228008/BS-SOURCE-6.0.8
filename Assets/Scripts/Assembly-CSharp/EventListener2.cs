using UnityEngine;
using UnityEngine.Events;

public class EventListener2 : MonoBehaviour
{
	public UnityEvent onEnable;

	public UnityEvent onDisable;

	public UnityEvent onBecameVisible;

	public UnityEvent onBecameInvisible;

	private void OnEnable()
	{
		if (onEnable != null)
		{
			onEnable.Invoke();
		}
	}

	private void OnDisable()
	{
		if (onDisable != null)
		{
			onDisable.Invoke();
		}
	}

	private void OnBecameVisible()
	{
		if (onBecameVisible != null)
		{
			onBecameVisible.Invoke();
		}
	}

	private void OnBecameInvisible()
	{
		if (onBecameInvisible != null)
		{
			onBecameInvisible.Invoke();
		}
	}
}
