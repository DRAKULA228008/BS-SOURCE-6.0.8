using DG.Tweening;
using UnityEngine;

public class TrapPosition : MonoBehaviour
{
	[Range(1f, 30f)]
	public int Key = 1;

	public Transform Target;

	public Vector3 PositionIn;

	public Vector3 PositionOut;

	public float Duration;

	public float DelayIn;

	public float DelayOut = 3f;

	public int Loop;

	private Tweener Tween;

	private bool Activated;

	private void Start()
	{
		if (Target == null)
		{
			Target = base.transform;
		}
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener("Button" + Key, ActiveTrap);
		StartRound();
	}

	[ContextMenu("Get Transform")]
	private void GetTarget()
	{
		Target = base.transform;
	}

	[ContextMenu("Get Position")]
	private void GetValue()
	{
		PositionIn = Target.localPosition;
		PositionOut = Target.localPosition;
	}

	private void ActiveTrap()
	{
		if (!Activated)
		{
			Tween = Target.DOLocalMove(PositionOut, Duration).OnComplete(ResetTrap).SetDelay(DelayIn)
				.SetLoops(Loop, LoopType.Yoyo);
			Activated = true;
		}
	}

	private void ResetTrap()
	{
		if (Activated && DelayOut != 0f)
		{
			Tween = Target.DOLocalMove(PositionIn, Duration).SetDelay(DelayOut);
		}
	}

	private void StartRound()
	{
		if (Tween != null)
		{
			Tween.Kill();
		}
		Target.localPosition = PositionIn;
		Activated = false;
	}
}
