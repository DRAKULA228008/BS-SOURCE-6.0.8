using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ShootingRangeTarget : MonoBehaviour
{
	[Serializable]
	public class PositionClass
	{
		public bool Use;

		public Vector3 Default;

		public Vector3 Activated;

		public float Duration = 1f;

		public bool Loop;

		public Tweener Tween;
	}

	[Serializable]
	public class RotationClass
	{
		public bool Use;

		public Vector3 Default;

		public Vector3 Activated;

		public float Duration = 1f;

		public Tweener Tween;
	}

	[Serializable]
	public class RandomPositionClass
	{
		public bool Use;

		public Vector3[] Positions;
	}

	public PositionClass Position;

	public RotationClass Rotation;

	public RandomPositionClass RandomPosition;

	public int Health = 100;

	public UnityEvent DeadCallback;

	private bool Activated;

	private Transform CacheTransform;

	private void Start()
	{
		CacheTransform = base.transform;
	}

	public void SetActive(bool active)
	{
		Activated = active;
		Health = 100;
		if (CacheTransform == null)
		{
			CacheTransform = base.transform;
		}
		if (Activated)
		{
			if (Position.Use)
			{
				CacheTransform.position = Position.Default;
				if (Position.Loop)
				{
					CacheTransform.DOMove(Position.Activated, Position.Duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
				}
				else
				{
					CacheTransform.DOMove(Position.Activated, Position.Duration);
				}
			}
			if (Rotation.Use)
			{
				CacheTransform.eulerAngles = Rotation.Default;
				CacheTransform.DORotate(Rotation.Activated, Rotation.Duration);
			}
			if (RandomPosition.Use)
			{
				CacheTransform.position = RandomPosition.Positions[UnityEngine.Random.Range(0, RandomPosition.Positions.Length)];
			}
			return;
		}
		if (Position.Use)
		{
			if (Position.Loop)
			{
				CacheTransform.DOKill();
			}
			else
			{
				CacheTransform.DOMove(Position.Default, Position.Duration);
			}
		}
		if (Rotation.Use)
		{
			CacheTransform.DORotate(Rotation.Default, Rotation.Duration);
		}
	}

	public bool GetActive()
	{
		return Activated;
	}

	public void Damage(DamageInfo damageInfo)
	{
		if (Activated)
		{
			Health -= damageInfo.damage;
			Health = Mathf.Max(Health, 0);
			if (Health == 0)
			{
				DeadCallback.Invoke();
				SetActive(false);
			}
		}
	}
}
