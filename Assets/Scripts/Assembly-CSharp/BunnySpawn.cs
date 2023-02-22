using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BunnySpawn : MonoBehaviour
{
	public bool FinishSpawn;

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
		if (!other.CompareTag("Player"))
		{
			return;
		}
		PlayerInput component = other.GetComponent<PlayerInput>();
		if (component != null && bounds.Intersects(component.mCharacterController.bounds))
		{
			if (FinishSpawn)
			{
				BunnyHop.FinishMap(XP, Money);
				return;
			}
			SpawnManager.GetTeamSpawn().cachedTransform.position = base.transform.position;
			SpawnManager.GetTeamSpawn().cachedTransform.rotation = base.transform.rotation;
		}
	}
}
