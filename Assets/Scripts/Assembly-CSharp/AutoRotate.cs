using UnityEngine;

public class AutoRotate : MonoBehaviour
{
	public Vector3 Rotate;

	private Transform Target;

	private void Start()
	{
		Target = base.transform;
	}

	private void Update()
	{
		Target.Rotate(Rotate);
	}
}
