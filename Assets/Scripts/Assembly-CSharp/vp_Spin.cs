using UnityEngine;

public class vp_Spin : MonoBehaviour
{
	public Vector3 RotationSpeed = new Vector3(0f, 90f, 0f);

	private Transform m_Transform;

	protected virtual void Start()
	{
		m_Transform = base.transform;
	}

	protected virtual void Update()
	{
		m_Transform.Rotate(RotationSpeed * Time.deltaTime);
	}
}
