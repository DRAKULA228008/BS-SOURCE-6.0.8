using System;
using UnityEngine;

public class vp_FPController : vp_Component
{
	public class xRaycastHit
	{
		public Vector3 normal;

		public Vector3 point;

		public Transform transform;

		public GameObject gameObject;
	}

	public vp_FPCamera FPCamera;

	public Action<float> FallImpactEvent;

	public CharacterController m_CharacterController;

	protected Vector3 m_FixedPosition = Vector3.zero;

	protected Vector3 m_SmoothPosition = Vector3.zero;

	protected bool m_Grounded;

	public RaycastHit m_GroundHit;

	protected RaycastHit m_LastGroundHit;

	protected RaycastHit m_WallHit;

	protected float m_FallImpact;

	public CryptoFloatFast MotorAcceleration = 0.18f;

	public CryptoFloatFast MotorDamping = 0.17f;

	public CryptoFloatFast MotorBackwardsSpeed = 0.65f;

	public CryptoFloatFast MotorAirSpeed = 0.35f;

	public CryptoFloatFast MotorSlopeSpeedUp = 1f;

	public CryptoFloatFast MotorSlopeSpeedDown = 1f;

	public bool MotorFreeFly;

	protected Vector3 m_MoveDirection;

	protected float m_SlopeFactor = nValue.float1;

	protected Vector3 m_MotorThrottle;

	protected float m_MotorAirSpeedModifier = nValue.float1;

	protected float m_CurrentAntiBumpOffset;

	protected Vector2 m_MoveVector;

	public CryptoFloatFast MotorJumpForce = 0.18f;

	public CryptoFloatFast MotorJumpForceDamping = 1.08f;

	protected bool m_MotorJumpDone = true;

	public bool MotorDoubleJump;

	private bool MotorDoubleJumpFirst;

	private bool MotorDoubleJumpFirstDown;

	private bool MotorDoubleJumpSecond;

	protected float m_FallSpeed;

	protected float m_LastFallSpeed;

	protected float m_HighestFallSpeed;

	public CryptoFloatFast PhysicsForceDamping = 1.05f;

	public CryptoFloatFast PhysicsGravityModifier = 0;

	public CryptoFloatFast PhysicsSlopeSlideLimit = 30f;

	public CryptoFloatFast PhysicsSlopeSlidiness = 0.15f;

	public CryptoFloatFast PhysicsWallBounce = 0f;

	public CryptoFloatFast PhysicsWallFriction = 0f;

	public bool PhysicsHasCollisionTrigger = true;

	protected GameObject m_Trigger;

	protected Vector3 m_ExternalForce = Vector3.zero;

	protected Vector3[] m_SmoothForceFrame = new Vector3[120];

	protected bool m_Slide;

	protected bool m_SlideFast;

	protected float m_SlideFallSpeed;

	protected float m_OnSteepGroundSince;

	protected float m_SlopeSlideSpeed;

	protected Vector3 m_PredictedPos = Vector3.zero;

	protected Vector3 m_PrevPos = Vector3.zero;

	protected Vector3 m_PrevDir = Vector3.zero;

	protected Vector3 m_NewDir = Vector3.zero;

	protected CryptoFloatFast m_ForceImpact = 0f;

	protected float m_ForceMultiplier;

	protected Vector3 CapsuleBottom = Vector3.zero;

	protected Vector3 CapsuleTop = Vector3.zero;

	protected Transform m_Platform;

	protected Vector3 m_PositionOnPlatform = Vector3.zero;

	protected float m_LastPlatformAngle;

	protected Vector3 m_LastPlatformPos = Vector3.zero;

	private Ray sphereCastRay = new Ray(Vector3.zero, Vector3.down);

	public CharacterController mCharacterController
	{
		get
		{
			if (m_CharacterController == null)
			{
				m_CharacterController = base.gameObject.GetComponent<CharacterController>();
			}
			return m_CharacterController;
		}
	}

	public Vector3 SmoothPosition
	{
		get
		{
			return m_SmoothPosition;
		}
	}

	public bool Grounded
	{
		get
		{
			return m_Grounded;
		}
	}

	public Vector3 GroundNormal
	{
		get
		{
			return m_GroundHit.normal;
		}
	}

	public float GroundAngle
	{
		get
		{
			return Vector3.Angle(m_GroundHit.normal, Vector3.up);
		}
	}

	public Transform GroundTransform
	{
		get
		{
			return m_GroundHit.transform;
		}
	}

	public Vector2 OnValue_InputMoveVector
	{
		get
		{
			return m_MoveVector;
		}
		set
		{
			m_MoveVector = ((!(value.y < 0f)) ? value : (value * MotorBackwardsSpeed.GetValue()));
		}
	}

	protected override void Awake()
	{
		base.Awake();
		mCharacterController.center = new Vector3(nValue.float0, mCharacterController.height * nValue.float05, nValue.float0);
		mCharacterController.radius = nValue.float0375;
		if (Physics.gravity.y != nValue.gravity)
		{
			CheckManager.Detected("Error Gravity");
		}
		TimerManager.In(nValue.int5, -1, nValue.int5, CheckValue);
	}

	private void CheckValue()
	{
		MotorAcceleration.CheckValue();
		MotorDamping.CheckValue();
		MotorBackwardsSpeed.CheckValue();
		MotorAirSpeed.CheckValue();
		MotorSlopeSpeedUp.CheckValue();
		MotorSlopeSpeedDown.CheckValue();
		PhysicsForceDamping.CheckValue();
		PhysicsGravityModifier.CheckValue();
		PhysicsSlopeSlideLimit.CheckValue();
		PhysicsSlopeSlidiness.CheckValue();
		PhysicsWallBounce.CheckValue();
		PhysicsWallFriction.CheckValue();
		m_ForceImpact.CheckValue();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		m_Platform = null;
	}

	protected override void Start()
	{
		base.Start();
		SetPosition(base.Transform.position);
		if (PhysicsHasCollisionTrigger)
		{
			m_Trigger = new GameObject("Trigger");
			m_Trigger.transform.parent = m_Transform;
			CapsuleCollider capsuleCollider = m_Trigger.AddComponent<CapsuleCollider>();
			capsuleCollider.isTrigger = true;
			capsuleCollider.radius = mCharacterController.radius + nValue.float008;
			capsuleCollider.height = mCharacterController.height + nValue.float008 * 2f;
			capsuleCollider.center = mCharacterController.center;
			m_Trigger.layer = 30;
			m_Trigger.transform.localPosition = Vector3.zero;
		}
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
	}

	protected override void FixedUpdate()
	{
		UpdateMotor();
		UpdateJump();
		UpdateForces();
		UpdateSliding();
		FixedMove();
		UpdateCollisions();
		UpdatePlatformMove();
		m_PrevPos = base.Transform.position;
	}

	public void OnFixedUpdateThread()
	{
		UpdateJump();
		UpdateForces();
		UpdateSliding();
	}

	protected virtual void UpdateMotor()
	{
		if (!MotorFreeFly)
		{
			UpdateThrottleWalk();
		}
		else
		{
			UpdateThrottleFree();
		}
		m_MotorThrottle = vp_MathUtility.SnapToZero(m_MotorThrottle);
	}

	protected virtual void UpdateThrottleWalk()
	{
		UpdateSlopeFactor();
		if (m_Grounded)
		{
			m_MotorAirSpeedModifier = nValue.float1;
		}
		else
		{
			m_MotorAirSpeedModifier = MotorAirSpeed.GetValue();
		}
		m_MotorThrottle += m_MoveVector.y * (base.Transform.TransformDirection(Vector3.forward * (MotorAcceleration.GetValue() * nValue.float01) * m_MotorAirSpeedModifier) * m_SlopeFactor);
		m_MotorThrottle += m_MoveVector.x * (base.Transform.TransformDirection(Vector3.right * (MotorAcceleration.GetValue() * nValue.float01) * m_MotorAirSpeedModifier) * m_SlopeFactor);
		m_MotorThrottle.x /= nValue.float1 + MotorDamping.GetValue() * m_MotorAirSpeedModifier;
		m_MotorThrottle.z /= nValue.float1 + MotorDamping.GetValue() * m_MotorAirSpeedModifier;
	}

	protected virtual void UpdateThrottleFree()
	{
		m_MotorThrottle += m_MoveVector.y * base.Transform.TransformDirection(base.Transform.InverseTransformDirection(FPCamera.Forward) * (MotorAcceleration.GetValue() * nValue.float01));
		m_MotorThrottle += m_MoveVector.x * base.Transform.TransformDirection(Vector3.right * (MotorAcceleration.GetValue() * nValue.float01));
		m_MotorThrottle.x /= nValue.float1 + MotorDamping.GetValue();
		m_MotorThrottle.z /= nValue.float1 + MotorDamping.GetValue();
	}

	protected virtual void UpdateJump()
	{
		m_MotorThrottle.y /= MotorJumpForceDamping.GetValue();
	}

	protected virtual void UpdateForces()
	{
		if (m_Grounded && m_FallSpeed <= nValue.float0)
		{
			m_FallSpeed = nValue.gravity * (PhysicsGravityModifier.GetValue() * nValue.float0002);
		}
		else
		{
			m_FallSpeed += nValue.gravity * (PhysicsGravityModifier.GetValue() * nValue.float0002);
		}
		if (m_FallSpeed < m_LastFallSpeed)
		{
			m_HighestFallSpeed = m_FallSpeed;
		}
		m_LastFallSpeed = m_FallSpeed;
		if (m_SmoothForceFrame[0] != Vector3.zero)
		{
			AddForceInternal(m_SmoothForceFrame[0]);
			for (int i = 0; i < 120; i++)
			{
				m_SmoothForceFrame[i] = ((i >= 119) ? Vector3.zero : m_SmoothForceFrame[i + 1]);
				if (m_SmoothForceFrame[i] == Vector3.zero)
				{
					break;
				}
			}
		}
		m_ExternalForce /= PhysicsForceDamping.GetValue();
	}

	protected virtual void UpdateSliding()
	{
		bool slideFast = m_SlideFast;
		m_Slide = false;
		if (!m_Grounded)
		{
			m_OnSteepGroundSince = nValue.float0;
			m_SlideFast = false;
		}
		else if (GroundAngle > PhysicsSlopeSlideLimit.GetValue())
		{
			m_Slide = true;
			if (GroundAngle <= 45f)
			{
				m_SlopeSlideSpeed = Mathf.Max(m_SlopeSlideSpeed, PhysicsSlopeSlidiness.GetValue() * nValue.float001);
				m_OnSteepGroundSince = nValue.float0;
				m_SlideFast = false;
				m_SlopeSlideSpeed = ((!(Mathf.Abs(m_SlopeSlideSpeed) < nValue.float00001)) ? (m_SlopeSlideSpeed / 1.05f) : nValue.float0);
			}
			else
			{
				if (m_SlopeSlideSpeed > nValue.float001)
				{
					m_SlideFast = true;
				}
				if (m_OnSteepGroundSince == nValue.float0)
				{
					m_OnSteepGroundSince = Time.time;
				}
				m_SlopeSlideSpeed += PhysicsSlopeSlidiness.GetValue() * nValue.float001 * ((Time.time - m_OnSteepGroundSince) * nValue.float0125);
				m_SlopeSlideSpeed = Mathf.Max(PhysicsSlopeSlidiness.GetValue() * nValue.float001, m_SlopeSlideSpeed);
			}
			AddForce(Vector3.Cross(Vector3.Cross(GroundNormal, Vector3.down), GroundNormal) * m_SlopeSlideSpeed);
		}
		else
		{
			m_OnSteepGroundSince = nValue.float0;
			m_SlideFast = false;
			m_SlopeSlideSpeed = nValue.float0;
		}
		if (m_MotorThrottle != Vector3.zero)
		{
			m_Slide = false;
		}
		if (m_SlideFast)
		{
			m_SlideFallSpeed = base.Transform.position.y;
		}
		else if (slideFast && !Grounded)
		{
			m_FallSpeed = base.Transform.position.y - m_SlideFallSpeed;
			m_FallSpeed = Mathf.Clamp(m_FallSpeed, nValue.float0, nValue.float001);
		}
	}

	protected virtual void FixedMove()
	{
		m_MoveDirection = Vector3.zero;
		m_MoveDirection += m_ExternalForce;
		m_MoveDirection += m_MotorThrottle;
		m_MoveDirection.y += m_FallSpeed;
		if (MotorDoubleJump && MotorDoubleJumpFirst && Grounded)
		{
			MotorDoubleJumpFirst = false;
			MotorDoubleJumpFirstDown = false;
			MotorDoubleJumpSecond = false;
		}
		m_CurrentAntiBumpOffset = nValue.float0;
		if (m_Grounded && m_MotorThrottle.y <= nValue.float0001)
		{
			m_CurrentAntiBumpOffset = Mathf.Max(mCharacterController.stepOffset, Vector3.Scale(m_MoveDirection, Vector3.one - Vector3.up).magnitude);
			m_MoveDirection += m_CurrentAntiBumpOffset * Vector3.down;
		}
		m_PredictedPos = base.Transform.position + m_MoveDirection * base.Delta;
		if (m_Platform != null && m_PositionOnPlatform != Vector3.zero)
		{
			mCharacterController.Move(m_Platform.TransformPoint(m_PositionOnPlatform) - m_Transform.position);
		}
		mCharacterController.Move(m_MoveDirection * base.Delta);
		sphereCastRay.origin = base.Transform.position + Vector3.up * mCharacterController.radius;
		Physics.SphereCast(sphereCastRay, mCharacterController.radius, out m_GroundHit, nValue.float008 + nValue.float0001, -1749041173);
		m_Grounded = m_GroundHit.collider != null;
		if (m_GroundHit.transform == null && m_LastGroundHit.transform != null)
		{
			if (m_Platform != null && m_PositionOnPlatform != Vector3.zero)
			{
				AddForce(m_Platform.position - m_LastPlatformPos);
				m_Platform = null;
			}
			if (m_CurrentAntiBumpOffset != nValue.float0)
			{
				mCharacterController.Move(m_CurrentAntiBumpOffset * Vector3.up * base.Delta);
				m_PredictedPos += m_CurrentAntiBumpOffset * Vector3.up * base.Delta;
				m_MoveDirection += m_CurrentAntiBumpOffset * Vector3.up;
			}
		}
	}

	protected virtual void UpdateCollisions()
	{
		if (m_GroundHit.transform != null && m_GroundHit.transform != m_LastGroundHit.transform)
		{
			if (!MotorFreeFly)
			{
				m_FallImpact = 0f - m_HighestFallSpeed;
			}
			else
			{
				m_FallImpact = 0f - mCharacterController.velocity.y * nValue.float001;
			}
			m_SmoothPosition.y = base.Transform.position.y;
			DeflectDownForce();
			m_HighestFallSpeed = nValue.float0;
			if (FallImpactEvent != null)
			{
				FallImpactEvent(m_FallImpact);
			}
			m_MotorThrottle.y = nValue.float0;
			if (m_GroundHit.collider.gameObject.layer == 28)
			{
				m_Platform = m_GroundHit.transform;
				m_LastPlatformAngle = m_Platform.eulerAngles.y;
			}
			else
			{
				m_Platform = null;
			}
		}
		else
		{
			m_FallImpact = nValue.float0;
		}
		m_LastGroundHit = m_GroundHit;
		if (m_PredictedPos.x != base.Transform.position.x || (m_PredictedPos.z != base.Transform.position.z && m_ExternalForce != Vector3.zero))
		{
			DeflectHorizontalForce();
		}
	}

	protected virtual void UpdateSlopeFactor()
	{
		if (!m_Grounded)
		{
			m_SlopeFactor = nValue.float1;
			return;
		}
		m_SlopeFactor = nValue.float1 + (nValue.float1 - Vector3.Angle(m_GroundHit.normal, m_MotorThrottle) / (float)nValue.int90);
		if (Mathf.Abs(nValue.float1 - m_SlopeFactor) < nValue.float001)
		{
			m_SlopeFactor = nValue.float1;
			return;
		}
		if (m_SlopeFactor > nValue.float1)
		{
			if (MotorSlopeSpeedDown.GetValue() == nValue.float1)
			{
				m_SlopeFactor = nValue.float1 / m_SlopeFactor;
				m_SlopeFactor *= nValue.float12;
			}
			else
			{
				m_SlopeFactor *= MotorSlopeSpeedDown.GetValue();
			}
			return;
		}
		if (MotorSlopeSpeedUp.GetValue() == nValue.float1)
		{
			m_SlopeFactor *= nValue.float12;
		}
		else
		{
			m_SlopeFactor *= MotorSlopeSpeedUp.GetValue();
		}
		if (GroundAngle > mCharacterController.slopeLimit)
		{
			m_SlopeFactor = nValue.float0;
		}
	}

	protected virtual void UpdatePlatformMove()
	{
		if (!(m_Platform == null))
		{
			m_PositionOnPlatform = m_Platform.InverseTransformPoint(m_Transform.position);
			FPCamera.Angle = new Vector2(FPCamera.Angle.x, FPCamera.Angle.y - Mathf.DeltaAngle(m_Platform.eulerAngles.y, m_LastPlatformAngle));
			m_LastPlatformAngle = m_Platform.eulerAngles.y;
			m_LastPlatformPos = m_Platform.position;
			m_SmoothPosition = base.Transform.position;
		}
	}

	public virtual void SetPosition(Vector3 position)
	{
		base.Transform.position = position;
		m_PrevPos = position;
		m_SmoothPosition = position;
		m_Platform = null;
	}

	protected virtual void AddForceInternal(Vector3 force)
	{
		m_ExternalForce += force;
	}

	public virtual void AddForce(float x, float y, float z)
	{
		AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce(Vector3 force)
	{
		AddForceInternal(force);
	}

	public virtual void AddSoftForce(Vector3 force, float frames)
	{
		frames = Mathf.Clamp(frames, nValue.float1, 120f);
		AddForceInternal(force / frames);
		for (int i = 0; i < Mathf.RoundToInt(frames) - nValue.int1; i++)
		{
			m_SmoothForceFrame[i] += force / frames;
		}
	}

	public virtual void StopSoftForce()
	{
		for (int i = 0; i < 120 && !(m_SmoothForceFrame[i] == Vector3.zero); i++)
		{
			m_SmoothForceFrame[i] = Vector3.zero;
		}
	}

	public virtual void Stop()
	{
		mCharacterController.Move(Vector2.zero);
		m_MotorThrottle = Vector3.zero;
		m_ExternalForce = Vector2.zero;
		StopSoftForce();
		m_MoveVector = Vector2.zero;
		m_FallSpeed = nValue.float0;
		m_LastFallSpeed = nValue.float0;
		m_HighestFallSpeed = nValue.float0;
		m_SmoothPosition = base.Transform.position;
	}

	public virtual void DeflectDownForce()
	{
		if (GroundAngle > PhysicsSlopeSlideLimit.GetValue())
		{
			m_SlopeSlideSpeed = m_FallImpact * nValue.float025;
		}
		if (GroundAngle > 85f)
		{
			m_MotorThrottle += vp_3DUtility.HorizontalVector(GroundNormal * m_FallImpact);
			m_Grounded = false;
		}
	}

	protected virtual void DeflectHorizontalForce()
	{
		m_PredictedPos.y = base.Transform.position.y;
		m_PrevPos.y = base.Transform.position.y;
		m_PrevDir = (m_PredictedPos - m_PrevPos).normalized;
		CapsuleBottom = m_PrevPos + Vector3.up * mCharacterController.radius;
		CapsuleTop = CapsuleBottom + Vector3.up * (mCharacterController.height - mCharacterController.radius * 2f);
		if (Physics.CapsuleCast(CapsuleBottom, CapsuleTop, mCharacterController.radius, m_PrevDir, out m_WallHit, Vector3.Distance(m_PrevPos, m_PredictedPos), -1749041173))
		{
			m_NewDir = Vector3.Cross(m_WallHit.normal, Vector3.up).normalized;
			if (Vector3.Dot(Vector3.Cross(m_WallHit.point - base.Transform.position, m_PrevPos - base.Transform.position), Vector3.up) > nValue.float0)
			{
				m_NewDir = -m_NewDir;
			}
			m_ForceMultiplier = Mathf.Abs(Vector3.Dot(m_PrevDir, m_NewDir)) * (nValue.float1 - PhysicsWallFriction.GetValue());
			if (PhysicsWallBounce.GetValue() > nValue.float0)
			{
				m_NewDir = Vector3.Lerp(m_NewDir, Vector3.Reflect(m_PrevDir, m_WallHit.normal), PhysicsWallBounce.GetValue());
				m_ForceMultiplier = Mathf.Lerp(m_ForceMultiplier, nValue.float1, PhysicsWallBounce.GetValue() * (nValue.float1 - PhysicsWallFriction.GetValue()));
			}
			m_ForceImpact = nValue.float0;
			float y = m_ExternalForce.y;
			m_ExternalForce.y = nValue.float0;
			m_ForceImpact = m_ExternalForce.magnitude;
			m_ExternalForce = m_NewDir * m_ExternalForce.magnitude * m_ForceMultiplier;
			m_ForceImpact = (float)m_ForceImpact - m_ExternalForce.magnitude;
			for (int i = 0; i < 120 && !(m_SmoothForceFrame[i] == Vector3.zero); i++)
			{
				m_SmoothForceFrame[i] = m_SmoothForceFrame[i].magnitude * m_NewDir * m_ForceMultiplier;
			}
			m_ExternalForce.y = y;
		}
	}

	public bool CanStartJump()
	{
		if (MotorDoubleJump)
		{
			if (!MotorDoubleJumpFirst)
			{
				return true;
			}
			if (MotorDoubleJumpFirst && !MotorDoubleJumpFirstDown)
			{
				return false;
			}
			if (MotorDoubleJumpFirst && MotorDoubleJumpFirstDown && !MotorDoubleJumpSecond)
			{
				return true;
			}
			if (MotorDoubleJumpFirst && MotorDoubleJumpSecond)
			{
				return false;
			}
		}
		if (MotorFreeFly)
		{
			return true;
		}
		if (!m_Grounded)
		{
			return false;
		}
		if (!m_MotorJumpDone)
		{
			return false;
		}
		if (GroundAngle > mCharacterController.slopeLimit)
		{
			return false;
		}
		return true;
	}

	public void OnStartJump()
	{
		m_MotorJumpDone = false;
		if (MotorDoubleJump)
		{
			if (!MotorDoubleJumpFirst)
			{
				MotorDoubleJumpFirst = true;
			}
			else
			{
				m_FallSpeed = nValue.float0;
				m_LastFallSpeed = nValue.float0;
				m_HighestFallSpeed = nValue.float0;
				MotorDoubleJumpSecond = true;
			}
		}
		if (!MotorFreeFly || Grounded)
		{
			m_MotorThrottle.y = MotorJumpForce.GetValue();
			m_SmoothPosition.y = base.Transform.position.y;
		}
	}

	public void OnStopJump()
	{
		m_MotorJumpDone = true;
		if (MotorDoubleJump && MotorDoubleJumpFirst)
		{
			MotorDoubleJumpFirstDown = true;
		}
	}
}
