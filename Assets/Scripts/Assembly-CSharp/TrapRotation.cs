using DG.Tweening;
using UnityEngine;

public class TrapRotation : MonoBehaviour
{
	[Range(1f, 30f)]
	public int Key = 1;

	public Transform Target;

	public bool setStartPosition;

	public Vector3 startPosition;

	public int repeatTimes;

	private int repeat;

	public Vector3 RotationIn;

	public Vector3 RotationOut;

	public RotateMode Mode;

	public float Duration;

	public float DelayIn;

	public float DelayOut = 3f;

	private Tweener Tween;

	public bool Activated;

	private void Start()
	{
		if (Target == null)
		{
			Target = base.transform;
		}
		if (setStartPosition)
		{
			base.transform.position = startPosition;
		}
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener("Button" + Key, ActiveTrap);
	}

	[ContextMenu("Get Object")]
	private void GetObject()
	{
		Target = base.transform;
	}

	[ContextMenu("Get Start Position")]
	private void GetStart()
	{
		startPosition = base.transform.position;
	}

	[ContextMenu("Get Rotation")]
	private void GetValue()
	{
		RotationIn = Target.localEulerAngles;
		RotationOut = Target.localEulerAngles;
	}

	private void ActiveTrap()
	{
		if (!Activated)
		{
			Tween = Target.DORotate(RotationOut, Duration, Mode).OnComplete(ResetTrap).SetDelay(DelayIn);
			Activated = true;
		}
	}

	private void ResetTrap()
	{
		if (Activated && repeat == 0)
		{
			Tween = Target.DORotate(RotationIn, Duration, Mode).SetDelay(DelayOut);
		}
		else if (repeat > 0)
		{
			Tween = Target.DORotate(RotationIn, Duration, Mode).OnComplete(ActiveTrap).SetDelay(DelayOut);
			repeat--;
			Activated = false;
		}
	}

	private void StartRound()
	{
		if (Tween != null)
		{
			Tween.Kill();
		}
		Target.localEulerAngles = RotationIn;
		repeat = repeatTimes;
		Activated = false;
	}
}
