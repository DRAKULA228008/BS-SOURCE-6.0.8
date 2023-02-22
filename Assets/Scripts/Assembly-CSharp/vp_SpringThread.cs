using UnityEngine;

public class vp_SpringThread
{
	protected bool m_AutoUpdate = true;

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

	public float time;

	public void FixedUpdate()
	{
		time += 0.02f;
		if (m_VelocityFadeInEndTime > time)
		{
			m_VelocityFadeInCap = Mathf.Clamp01(nValue.float1 - (m_VelocityFadeInEndTime - time) / m_VelocityFadeInLength);
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
		AddForceInternal(force);
	}

	public void AddSoftForce(Vector3 force, float frames)
	{
		frames = Mathf.Clamp(frames, nValue.float1, 120f);
		AddForceInternal(force / frames);
		for (int i = nValue.int0; i < Mathf.RoundToInt(frames) - nValue.int1; i++)
		{
			m_SoftForceFrame[i] += force / frames;
		}
	}

	protected void Move()
	{
		State += m_Velocity;
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
		m_VelocityFadeInEndTime = time + seconds;
		m_VelocityFadeInCap = nValue.float0;
	}
}
