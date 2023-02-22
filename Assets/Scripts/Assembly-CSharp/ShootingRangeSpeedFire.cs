using System;
using DG.Tweening;
using UnityEngine;

public class ShootingRangeSpeedFire : MonoBehaviour
{
	[Serializable]
	public class RoomData
	{
		public Transform Gate;

		public ShootingRangeTarget[] Targets;
	}

	public RoomData[] Data;

	public int dead;

	public int index;

	public bool isStarting;

	public void StartRoom()
	{
		if (!isStarting)
		{
			index = 0;
			isStarting = true;
			Next();
		}
	}

	public void Dead()
	{
		dead++;
		if (Data[index].Targets.Length == dead)
		{
			dead = 0;
			index++;
			Next();
		}
	}

	public void Next()
	{
		MonoBehaviour.print("Next");
		Data[index].Gate.DOLocalMoveY(-5f, 0.25f);
		for (int i = 0; i < Data[index].Targets.Length; i++)
		{
			Data[index].Targets[i].SetActive(true);
		}
	}
}
