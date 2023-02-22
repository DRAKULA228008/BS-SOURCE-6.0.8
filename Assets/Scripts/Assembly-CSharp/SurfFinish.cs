using UnityEngine;

public class SurfFinish : MonoBehaviour
{
	public CryptoInt XP;

	public CryptoInt Money;

	private BoxCollider boxCollider;

	private Bounds bounds;

	private void Start()
	{
		boxCollider = GetComponent<BoxCollider>();
		bounds = boxCollider.bounds;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && bounds.Intersects(component.mCharacterController.bounds))
			{
				SurfMode.FinishMap(XP, Money);
			}
		}
	}
}
