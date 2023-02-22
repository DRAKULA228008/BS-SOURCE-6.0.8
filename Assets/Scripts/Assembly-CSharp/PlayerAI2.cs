using System;
using UnityEngine;

public class PlayerAI2 : MonoBehaviour
{
	[Serializable]
	public class AttackData
	{
		public float damage = 20f;

		public float distance = 5f;

		public float speed = 0.5f;
	}

	public Transform target;

	public float navigatorTime = 0.1f;

	public AttackData attack;

	public UnityEngine.AI.NavMeshAgent nav;

	private void Start()
	{
		TimerManager.In(navigatorTime, -1, navigatorTime, UpdateNavigator);
		TimerManager.In(attack.speed, -1, attack.speed, CheckAttackDistance);
	}

	private void UpdateNavigator()
	{
		if (target != null)
		{
			nav.SetDestination(target.position);
		}
		else if (nav.remainingDistance < 1f)
		{
			nav.SetDestination(base.transform.position + new Vector3(UnityEngine.Random.Range(-5f, 5f), 0f, UnityEngine.Random.Range(-5f, 5f)));
		}
	}

	private void Update()
	{
	}

	private void CheckAttackDistance()
	{
		if (!(target == null) && Vector3.Distance(target.position, base.transform.position) <= attack.distance)
		{
			Attack();
		}
	}

	private void Attack()
	{
		MonoBehaviour.print("Attack");
	}

	public void SetTarget(Transform t)
	{
		target = t;
	}
}
