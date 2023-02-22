using UnityEngine;

public class sTweenPosition : MonoBehaviour
{
	public float speed = 1f;

	public Vector3 vector = Vector3.one;

	public float delay;

	public bool debug;

	private Transform mTransform;

	private Vector3 startPosition;

	public bool changePosition;

	public Vector3 position;

	private void Start()
	{
		if (changePosition)
		{
			base.transform.localPosition = position;
		}
		mTransform = base.transform;
		startPosition = base.transform.localPosition;
	}

	private void FixedUpdate()
	{
		mTransform.localPosition = startPosition + vector * Mathf.PingPong((sTweenTime.time + delay) * speed, 1f);
	}

	[ContextMenu("Get Position")]
	private void GetValue()
	{
		position = base.transform.localPosition;
	}
}
