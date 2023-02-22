using UnityEngine;

public class ClimbSystem : MonoBehaviour
{
	private CryptoVector3 center;

	private CryptoVector3 size;

	private BoxCollider boxCollider;

	private Bounds bounds;

	private void Start()
	{
		base.gameObject.layer = 2;
		boxCollider = GetComponent<BoxCollider>();
		if (!(boxCollider == null))
		{
			center = boxCollider.center;
			size = boxCollider.size;
			bounds = boxCollider.bounds;
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && boxCollider != null)
		{
			boxCollider.center = center;
			boxCollider.size = size;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (!(component == null) && bounds.Intersects(component.mCharacterController.bounds))
		{
			component.SetClimb(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (!(component == null))
		{
			component.SetClimb(false);
		}
	}
}
