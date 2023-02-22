using UnityEngine;

public class FPWeaponTwoHanded : MonoBehaviour
{
	public float PositionSpringStiffness = 0.01f;

	public float PositionSpringDamping = 0.25f;

	public float PositionPivotSpringStiffness = 0.01f;

	public float PositionPivotSpringDamping = 0.25f;

	public float RotationPivotSpringStiffness = 0.01f;

	public float RotationPivotSpringDamping = 0.25f;

	public float RotationSpringStiffness = 0.01f;

	public float RotationSpringDamping = 0.25f;

	public float PositionSpring2Stiffness = 0.95f;

	public float PositionSpring2Damping = 0.25f;

	public float RotationSpring2Stiffness = 0.95f;

	public float RotationSpring2Damping = 0.25f;

	public Vector3 PositionOffset = new Vector3(0.15f, -0.15f, -0.15f);

	public Vector3 RotationOffset = Vector3.zero;

	public Vector3 PositionPivot = Vector3.zero;

	public Vector3 RotationPivot = Vector3.zero;

	public CryptoInt KnifeDelayForce = 50;

	public CryptoVector3 KnifeDelayForcePosition;

	public CryptoVector3 KnifeDelayForceRotation;

	public CryptoInt KnifeAttackForce = 50;

	public CryptoVector3 KnifeAttackForcePosition;

	public CryptoVector3 KnifeAttackForceRotation;

	private vp_Spring m_PositionSpring;

	private vp_Spring m_PositionSpring2;

	private vp_Spring m_PositionPivotSpring;

	private vp_Spring m_RotationPivotSpring;

	private vp_Spring m_RotationSpring;

	private vp_Spring m_RotationSpring2;

	private Transform m_Transform;

	private void Start()
	{
		m_Transform = base.transform;
		m_PositionSpring = new vp_Spring(m_Transform.parent, vp_Spring.UpdateMode.Position);
		m_PositionSpring.RestState = PositionOffset;
		m_PositionPivotSpring = new vp_Spring(m_Transform, vp_Spring.UpdateMode.Position);
		m_PositionPivotSpring.RestState = PositionPivot;
		m_PositionSpring2 = new vp_Spring(m_Transform, vp_Spring.UpdateMode.PositionAdditive);
		m_PositionSpring2.MinVelocity = 1E-05f;
		m_RotationSpring = new vp_Spring(m_Transform.parent, vp_Spring.UpdateMode.Rotation);
		m_RotationSpring.RestState = RotationOffset;
		m_RotationPivotSpring = new vp_Spring(m_Transform, vp_Spring.UpdateMode.Rotation);
		m_RotationPivotSpring.RestState = RotationPivot;
		m_RotationSpring2 = new vp_Spring(m_Transform.parent, vp_Spring.UpdateMode.RotationAdditive);
		m_RotationSpring2.MinVelocity = 1E-05f;
		SnapSprings();
		Refresh();
	}

	private void FixedUpdate()
	{
		UpdateSprings();
	}

	private void UpdateSprings()
	{
		m_PositionSpring.FixedUpdate();
		m_RotationSpring.FixedUpdate();
		m_PositionPivotSpring.FixedUpdate();
		m_RotationPivotSpring.FixedUpdate();
		m_PositionSpring2.FixedUpdate();
		m_RotationSpring2.FixedUpdate();
	}

	public virtual void AddForce2(Vector3 positional, Vector3 angular)
	{
		m_PositionSpring2.AddForce(positional);
		m_RotationSpring2.AddForce(angular);
	}

	public void AddSoftForce(Vector3 positional, Vector3 angular, int frames)
	{
		m_PositionSpring.AddSoftForce(positional, frames);
		m_RotationSpring.AddSoftForce(angular, frames);
	}

	public void AddSoftForceKnifeDelay()
	{
		m_PositionSpring.AddSoftForce(KnifeDelayForcePosition, (int)KnifeDelayForce);
		m_RotationSpring.AddSoftForce(KnifeDelayForceRotation, (int)KnifeDelayForce);
	}

	public void AddSoftForceKnifeAttack()
	{
		m_PositionSpring.AddSoftForce(KnifeAttackForcePosition, (int)KnifeAttackForce);
		m_RotationSpring.AddSoftForce(KnifeAttackForceRotation, (int)KnifeAttackForce);
	}

	[ContextMenu("Refresh")]
	public void Refresh()
	{
		if (m_PositionSpring != null)
		{
			m_PositionSpring.Stiffness = new Vector3(PositionSpringStiffness, PositionSpringStiffness, PositionSpringStiffness);
			m_PositionSpring.Damping = Vector3.one - new Vector3(PositionSpringDamping, PositionSpringDamping, PositionSpringDamping);
			m_PositionSpring.RestState = PositionOffset - PositionPivot;
		}
		if (m_PositionPivotSpring != null)
		{
			m_PositionPivotSpring.Stiffness = new Vector3(PositionPivotSpringStiffness, PositionPivotSpringStiffness, PositionPivotSpringStiffness);
			m_PositionPivotSpring.Damping = Vector3.one - new Vector3(PositionPivotSpringDamping, PositionPivotSpringDamping, PositionPivotSpringDamping);
			m_PositionPivotSpring.RestState = PositionPivot;
		}
		if (m_RotationPivotSpring != null)
		{
			m_RotationPivotSpring.Stiffness = new Vector3(RotationPivotSpringStiffness, RotationPivotSpringStiffness, RotationPivotSpringStiffness);
			m_RotationPivotSpring.Damping = Vector3.one - new Vector3(RotationPivotSpringDamping, RotationPivotSpringDamping, RotationPivotSpringDamping);
			m_RotationPivotSpring.RestState = RotationPivot;
		}
		if (m_PositionSpring2 != null)
		{
			m_PositionSpring2.Stiffness = new Vector3(PositionSpring2Stiffness, PositionSpring2Stiffness, PositionSpring2Stiffness);
			m_PositionSpring2.Damping = Vector3.one - new Vector3(PositionSpring2Damping, PositionSpring2Damping, PositionSpring2Damping);
			m_PositionSpring2.RestState = Vector3.zero;
		}
		if (m_RotationSpring != null)
		{
			m_RotationSpring.Stiffness = new Vector3(RotationSpringStiffness, RotationSpringStiffness, RotationSpringStiffness);
			m_RotationSpring.Damping = Vector3.one - new Vector3(RotationSpringDamping, RotationSpringDamping, RotationSpringDamping);
			m_RotationSpring.RestState = RotationOffset;
		}
		if (m_RotationSpring2 != null)
		{
			m_RotationSpring2.Stiffness = new Vector3(RotationSpring2Stiffness, RotationSpring2Stiffness, RotationSpring2Stiffness);
			m_RotationSpring2.Damping = Vector3.one - new Vector3(RotationSpring2Damping, RotationSpring2Damping, RotationSpring2Damping);
			m_RotationSpring2.RestState = Vector3.zero;
		}
	}

	public void SnapSprings()
	{
		if (m_PositionSpring != null)
		{
			m_PositionSpring.RestState = PositionOffset - PositionPivot;
			m_PositionSpring.State = PositionOffset - PositionPivot;
			m_PositionSpring.Stop(true);
		}
		if (m_PositionSpring2 != null)
		{
			m_PositionSpring2.RestState = Vector3.zero;
			m_PositionSpring2.State = Vector3.zero;
			m_PositionSpring2.Stop(true);
		}
		if (m_RotationSpring != null)
		{
			m_RotationSpring.RestState = RotationOffset;
			m_RotationSpring.State = RotationOffset;
			m_RotationSpring.Stop(true);
		}
		if (m_RotationSpring2 != null)
		{
			m_RotationSpring2.RestState = Vector3.zero;
			m_RotationSpring2.State = Vector3.zero;
			m_RotationSpring2.Stop(true);
		}
		if (m_PositionPivotSpring != null)
		{
			m_PositionPivotSpring.RestState = PositionPivot;
			m_PositionPivotSpring.State = PositionPivot;
			m_PositionPivotSpring.Stop(true);
		}
		m_Transform.localPosition = PositionPivot;
		if (m_RotationPivotSpring != null)
		{
			m_RotationPivotSpring.RestState = RotationPivot;
			m_RotationPivotSpring.State = RotationPivot;
			m_RotationPivotSpring.Stop(true);
		}
		m_Transform.localEulerAngles = RotationPivot;
	}

	public void StopSprings()
	{
		if (m_PositionSpring != null)
		{
			m_PositionSpring.Stop(true);
		}
		if (m_PositionPivotSpring != null)
		{
			m_PositionPivotSpring.Stop(true);
		}
		if (m_RotationSpring != null)
		{
			m_RotationSpring.Stop(true);
		}
		if (m_RotationPivotSpring != null)
		{
			m_RotationPivotSpring.Stop(true);
		}
		if (m_PositionSpring2 != null)
		{
			m_PositionSpring2.Stop(true);
		}
		if (m_RotationSpring2 != null)
		{
			m_RotationSpring2.Stop(true);
		}
	}

	public void Wield()
	{
		Refresh();
	}
}
