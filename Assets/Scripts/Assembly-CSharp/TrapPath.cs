using DG.Tweening;
using UnityEngine;

public class TrapPath : MonoBehaviour
{
	public Transform Target;

	public Transform Target2;

	public Transform[] Points;

	public float Duration = 60f;

	public float Delay = 20f;

	public float Delay2 = 25f;

	public Color GizmosColor = Color.white;

	private Tween tween;

	private Tween tween2;

	private Vector3 StartPosition;

	private Quaternion StartRotation;

	private void Start()
	{
		StartPosition = Target.position;
		StartRotation = Target.rotation;
		EventManager.AddListener("StartRound", StartRound);
	}

	private void OnWaypointChange(int waypointIndex)
	{
		if (Points.Length != waypointIndex)
		{
			Target.DOLookAt(Points[waypointIndex].position, nValue.float03);
		}
	}

	private void OnWaypointChange2(int waypointIndex)
	{
		if (Points.Length != waypointIndex)
		{
			Target2.DOLookAt(Points[waypointIndex].position, nValue.float03);
		}
	}

	private void StartRound()
	{
		if (tween != null)
		{
			tween.Kill();
			Target.position = StartPosition;
			Target.rotation = StartRotation;
		}
		if (tween2 != null && Target2 != null)
		{
			tween2.Kill();
			Target2.position = StartPosition;
			Target2.rotation = StartRotation;
		}
		Vector3[] array = new Vector3[Points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Points[i].position;
		}
		tween = Target.DOPath(array, Duration).SetDelay(Delay).OnWaypointChange(OnWaypointChange);
		if (Target2 != null)
		{
			tween2 = Target2.DOPath(array, Duration).SetDelay(Delay2).OnWaypointChange(OnWaypointChange2);
		}
	}
}
