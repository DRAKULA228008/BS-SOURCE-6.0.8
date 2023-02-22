using UnityEngine;

public class sTweenRotation : MonoBehaviour
{
	public Vector3 speed = Vector3.forward;

	private Transform mTransform;

	private Vector3 startEulerAngles;

	public bool changePosition;

	public Vector3 position;

	private void Start()
	{
		if (changePosition)
		{
			base.transform.localPosition = position;
		}
		mTransform = base.transform;
		startEulerAngles = base.transform.localEulerAngles;
	}

	private void FixedUpdate()
	{
		mTransform.localEulerAngles = startEulerAngles + speed * Time.time * 10f;
	}

	[ContextMenu("Get Position")]
	private void GetValue()
	{
		position = base.transform.localPosition;
	}
}
