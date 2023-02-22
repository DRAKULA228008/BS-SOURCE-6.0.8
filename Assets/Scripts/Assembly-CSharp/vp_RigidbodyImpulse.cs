using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class vp_RigidbodyImpulse : MonoBehaviour
{
	protected Rigidbody m_Rigidbody;

	public Vector3 RigidbodyForce = new Vector3(0f, 5f, 0f);

	public float RigidbodySpin = 0.2f;

	protected virtual void Awake()
	{
		m_Rigidbody = base.GetComponent<Rigidbody>();
	}

	protected virtual void OnEnable()
	{
		if (!(m_Rigidbody == null))
		{
			if (RigidbodyForce != Vector3.zero)
			{
				m_Rigidbody.AddForce(RigidbodyForce, ForceMode.Impulse);
			}
			if (RigidbodySpin != 0f)
			{
				m_Rigidbody.AddTorque(Random.rotation.eulerAngles * RigidbodySpin);
			}
		}
	}
}
