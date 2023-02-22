using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeFreeFire : MonoBehaviour
{
	public List<ShootingRangeTarget> Targets;

	private List<ShootingRangeTarget> ActivateTarget = new List<ShootingRangeTarget>();

	private List<ShootingRangeTarget> DeactiveTarget = new List<ShootingRangeTarget>();

	public int MaxTargets = 5;

	private bool Activated;

	private void Start()
	{
		for (int i = 0; i < Targets.Count; i++)
		{
			DeactiveTarget.Add(Targets[i]);
		}
		SetActive(true);
	}

	private void SetActive(bool active)
	{
		if (Activated == active)
		{
			return;
		}
		Activated = !Activated;
		if (Activated)
		{
			for (int i = 0; i < MaxTargets; i++)
			{
				ShootingRangeTarget shootingRangeTarget = DeactiveTarget[Random.Range(0, DeactiveTarget.Count)];
				shootingRangeTarget.SetActive(true);
				DeactiveTarget.Remove(shootingRangeTarget);
				ActivateTarget.Add(shootingRangeTarget);
			}
		}
		else
		{
			ActivateTarget.Clear();
			DeactiveTarget.Clear();
			for (int j = 0; j < Targets.Count; j++)
			{
				Targets[j].SetActive(false);
				DeactiveTarget.Add(Targets[j]);
			}
		}
	}

	public void DeadTarget(ShootingRangeTarget target)
	{
		ShootingRangeTarget shootingRangeTarget = DeactiveTarget[Random.Range(0, DeactiveTarget.Count)];
		shootingRangeTarget.SetActive(true);
		DeactiveTarget.Remove(shootingRangeTarget);
		ActivateTarget.Add(shootingRangeTarget);
		if (ActivateTarget.Contains(target))
		{
			ActivateTarget.Remove(target);
			DeactiveTarget.Add(target);
		}
	}
}
