using UnityEngine;

public class RadiationTrigger : MonoBehaviour
{
	public Team PlayerTeam;

	public int DamageMin;

	public int DamageMax;

	public float Period = 1.5f;

	private BoxCollider boxCollider;

	private Bounds bounds;

	private void Start()
	{
		base.gameObject.layer = 2;
		boxCollider = GetComponent<BoxCollider>();
		bounds = boxCollider.bounds;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == PlayerTeam && bounds.Intersects(component.mCharacterController.bounds))
			{
				InvokeRepeating("PlayerDamage", 0.2f, Period);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			PlayerInput component = other.GetComponent<PlayerInput>();
			if (component != null && component.PlayerTeam == PlayerTeam)
			{
				CancelInvoke("PlayerDamage");
			}
		}
	}

	private void PlayerDamage()
	{
		PlayerInput instance = PlayerInput.instance;
		if (instance.PlayerTeam == PlayerTeam)
		{
			DamageInfo damageInfo = DamageInfo.Get(Random.Range(DamageMin, DamageMax), instance.PlayerTransform.position, Team.None, 0, 0, -1, false);
			instance.Damage(damageInfo);
		}
		else
		{
			CancelInvoke("PlayerDamage");
		}
	}
}
