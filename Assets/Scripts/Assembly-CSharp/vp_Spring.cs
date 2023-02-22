using UnityEngine;

public class vp_Spring
{
	public enum UpdateMode
	{
		Position,
		PositionAdditive,
		Rotation,
		RotationAdditive,
		Scale,
		ScaleAdditive
	}

	protected delegate void UpdateDelegate();

	protected UpdateMode Mode;

	protected bool m_AutoUpdate = true;

	protected UpdateDelegate m_UpdateFunc;

	public Vector3 State = Vector3.zero;

	protected Vector3 m_Velocity = Vector3.zero;

	public Vector3 RestState = Vector3.zero;

	public Vector3 Stiffness = new Vector3(nValue.float05, nValue.float05, nValue.float05);

	public Vector3 Damping = new Vector3(0.75f, 0.75f, 0.75f);

	protected float m_VelocityFadeInCap = nValue.float1;

	protected float m_VelocityFadeInEndTime = nValue.float0;

	protected float m_VelocityFadeInLength = nValue.float0;

	protected Vector3[] m_SoftForceFrame = new Vector3[120];

	public float MaxVelocity = nValue.int10000;

	public float MinVelocity = 1E-07f;

	public Vector3 MaxState = new Vector3(nValue.int10000, nValue.int10000, nValue.int10000);

	public Vector3 MinState = new Vector3(-nValue.int10000, -nValue.int10000, -nValue.int10000);

	protected Transform m_Transform;

	public Transform Transform
	{
		set
		{
			m_Transform = value;
			RefreshUpdateMode();
		}
	}

	public vp_Spring(Transform transform, UpdateMode mode, bool autoUpdate = true)
	{
		Mode = mode;
		Transform = transform;
		m_AutoUpdate = autoUpdate;
	}

	public void FixedUpdate()
	{
		if (m_VelocityFadeInEndTime > Time.time)
		{
			m_VelocityFadeInCap = Mathf.Clamp01(nValue.float1 - (m_VelocityFadeInEndTime - Time.time) / m_VelocityFadeInLength);
		}
		else
		{
			m_VelocityFadeInCap = nValue.float1;
		}
		if (m_SoftForceFrame[nValue.int0] != Vector3.zero)
		{
			AddForceInternal(m_SoftForceFrame[nValue.int0]);
			for (int i = nValue.int0; i < nValue.int120; i++)
			{
				m_SoftForceFrame[i] = ((i >= 119) ? Vector3.zero : m_SoftForceFrame[i + nValue.int1]);
				if (m_SoftForceFrame[i] == Vector3.zero)
				{
					break;
				}
			}
		}
		Calculate();
		m_UpdateFunc();
	}

	private void Position()
	{
		m_Transform.localPosition = State;
	}

	private void Rotation()
	{
		m_Transform.localEulerAngles = State;
	}

	private void Scale()
	{
		m_Transform.localScale = State;
	}

	private void PositionAdditive()
	{
		m_Transform.localPosition += State;
	}

	private void RotationAdditive()
	{
		m_Transform.localEulerAngles += State;
	}

	private void ScaleAdditive()
	{
		m_Transform.localScale += State;
	}

	private void None()
	{
	}

	protected void RefreshUpdateMode()
	{
		m_UpdateFunc = None;
		switch (Mode)
		{
		case UpdateMode.Position:
			State = m_Transform.localPosition;
			if (m_AutoUpdate)
			{
				m_UpdateFunc = Position;
			}
			break;
		case UpdateMode.Rotation:
			State = m_Transform.localEulerAngles;
			if (m_AutoUpdate)
			{
				m_UpdateFunc = Rotation;
			}
			break;
		case UpdateMode.Scale:
			State = m_Transform.localScale;
			if (m_AutoUpdate)
			{
				m_UpdateFunc = Scale;
			}
			break;
		case UpdateMode.PositionAdditive:
			State = m_Transform.localPosition;
			if (m_AutoUpdate)
			{
				m_UpdateFunc = PositionAdditive;
			}
			break;
		case UpdateMode.RotationAdditive:
			State = m_Transform.localEulerAngles;
			if (m_AutoUpdate)
			{
				m_UpdateFunc = RotationAdditive;
			}
			break;
		case UpdateMode.ScaleAdditive:
			State = m_Transform.localScale;
			if (m_AutoUpdate)
			{
				m_UpdateFunc = ScaleAdditive;
			}
			break;
		}
		RestState = State;
	}

	protected void Calculate()
	{
		if (!(State == RestState))
		{
			m_Velocity += Vector3.Scale(RestState - State, Stiffness);
			m_Velocity = Vector3.Scale(m_Velocity, Damping);
			m_Velocity = Vector3.ClampMagnitude(m_Velocity, MaxVelocity);
			if (m_Velocity.sqrMagnitude > MinVelocity * MinVelocity)
			{
				Move();
			}
			else
			{
				Reset();
			}
		}
	}

	private void AddForceInternal(Vector3 force)
	{
		force *= m_VelocityFadeInCap;
		m_Velocity += force;
		m_Velocity = Vector3.ClampMagnitude(m_Velocity, MaxVelocity);
		Move();
	}

	public void AddForce(Vector3 force)
	{
		if (Time.timeScale < nValue.float1)
		{
			AddSoftForce(force, nValue.float1);
		}
		else
		{
			AddForceInternal(force);
		}
	}

	public void AddSoftForce(Vector3 force, float frames)
	{
		force /= Time.timeScale;
		frames = Mathf.Clamp(frames, nValue.float1, 120f);
		AddForceInternal(force / frames);
		for (int i = nValue.int0; i < Mathf.RoundToInt(frames) - nValue.int1; i++)
		{
			m_SoftForceFrame[i] += force / frames;
		}
	}

	protected void Move()
	{
		State += m_Velocity * Time.timeScale;
		State.x = Mathf.Clamp(State.x, MinState.x, MaxState.x);
		State.y = Mathf.Clamp(State.y, MinState.y, MaxState.y);
		State.z = Mathf.Clamp(State.z, MinState.z, MaxState.z);
	}

	public void Reset()
	{
		m_Velocity = Vector3.zero;
		State = RestState;
	}

	public void Stop(bool includeSoftForce = false)
	{
		m_Velocity = Vector3.zero;
		if (includeSoftForce)
		{
			StopSoftForce();
		}
	}

	public void StopSoftForce()
	{
		for (int i = nValue.int0; i < nValue.int120; i++)
		{
			m_SoftForceFrame[i] = Vector3.zero;
		}
	}

	public void ForceVelocityFadeIn(float seconds)
	{
		m_VelocityFadeInLength = seconds;
		m_VelocityFadeInEndTime = Time.time + seconds;
		m_VelocityFadeInCap = nValue.float0;
	}
}
