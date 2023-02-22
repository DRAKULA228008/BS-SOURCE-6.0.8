using UnityEngine;

public class vp_Billboard : MonoBehaviour
{
	public Transform m_CameraTransform;

	private Transform m_Transform;

	protected virtual void Start()
	{
		m_Transform = base.transform;
		if (m_CameraTransform == null)
		{
			m_CameraTransform = Camera.main.transform;
		}
	}

	protected virtual void Update()
	{
		if (m_CameraTransform != null)
		{
			m_Transform.localEulerAngles = m_CameraTransform.eulerAngles;
		}
		m_Transform.localEulerAngles = (Vector2)m_Transform.localEulerAngles;
	}
}
