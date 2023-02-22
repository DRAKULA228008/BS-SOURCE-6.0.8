using UnityEngine;

public class TrapParticle : MonoBehaviour
{
	[Range(1f, 30f)]
	public int Key = 1;

	public ParticleSystem Target;

	public float DelayIn;

	public float DelayOut = 3f;

	private bool isTrigger;

	private bool Active;

	private bool Activated;

	private int Timer;

	private void Start()
	{
		Target.Stop();
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener("Button" + Key, ActiveTrap);
	}

	private void ActiveTrap()
	{
		if (Activated)
		{
			return;
		}
		Activated = true;
		Timer = TimerManager.In(DelayIn, delegate
		{
			Active = true;
			Target.Play();
			if (isTrigger)
			{
				DamageInfo damageInfo = DamageInfo.Get(1000, Vector3.zero, Team.None, 0, 0, -1, false);
				PlayerInput.instance.Damage(damageInfo);
			}
			if (DelayOut != 0f)
			{
				TimerManager.In(DelayOut, delegate
				{
					if (Activated)
					{
						Active = false;
						Target.Stop();
					}
				});
			}
		});
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isTrigger = true;
			if (Active)
			{
				DamageInfo damageInfo = DamageInfo.Get(1000, Vector3.zero, Team.None, 0, 0, -1, false);
				PlayerInput.instance.Damage(damageInfo);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isTrigger = false;
		}
	}

	private void StartRound()
	{
		Target.Stop();
		Activated = false;
		Active = false;
		TimerManager.Cancel(Timer);
	}
}
