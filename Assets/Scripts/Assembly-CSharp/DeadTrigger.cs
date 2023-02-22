using UnityEngine;

public class DeadTrigger : MonoBehaviour
{
	public bool secure;

	private BoxCollider boxCollider;

	private Bounds bounds;

	private bool isTrigger;

	private void Start()
	{
		base.gameObject.layer = 2;
		if (secure)
		{
			boxCollider = GetComponent<BoxCollider>();
			bounds = boxCollider.bounds;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("Player"))
		{
			return;
		}
		DamageInfo damageInfo = DamageInfo.Get(nValue.int10000, Vector3.zero, Team.None, nValue.int0, nValue.int0, -nValue.int1, false);
		PlayerInput.instance.Damage(damageInfo);
		if (!secure)
		{
			return;
		}
		TimerManager.In(0.1f, delegate
		{
			if (!GameManager.player.Dead && bounds.Intersects(GameManager.player.mCharacterController.bounds))
			{
				PlayerInput.instance.Damage(damageInfo);
			}
		});
	}
}
