using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
	public UnityEvent TriggerEnter;

	public UnityEvent TriggerExit;

	private BoxCollider boxCollider;

	private void Start()
	{
		boxCollider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (!(component == null) && !(boxCollider == null) && boxCollider.bounds.Intersects(component.mCharacterController.bounds))
			{
				TriggerEnter.Invoke();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (!(component == null))
			{
				TriggerExit.Invoke();
			}
		}
	}
}
