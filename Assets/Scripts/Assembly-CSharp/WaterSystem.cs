using UnityEngine;

public class WaterSystem : MonoBehaviour
{
	public bool freeGravity;

	private BoxCollider boxCollider;

	private void Start()
	{
		base.gameObject.layer = 2;
		boxCollider = GetComponent<BoxCollider>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component == null)
		{
			return;
		}
		if (boxCollider != null)
		{
			if (boxCollider.bounds.Intersects(component.mCharacterController.bounds))
			{
				component.SetWater(true, freeGravity);
			}
		}
		else
		{
			component.SetWater(true, freeGravity);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (!(component == null))
			{
				component.SetWater(false);
			}
		}
	}
}
