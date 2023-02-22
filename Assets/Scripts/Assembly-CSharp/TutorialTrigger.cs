using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
	public UnityEvent Callback;

	private bool Activated;

	private void OnTriggerEnter(Collider other)
	{
		if (!Activated && other.CompareTag("Player"))
		{
			Callback.Invoke();
			Activated = true;
		}
	}
}
