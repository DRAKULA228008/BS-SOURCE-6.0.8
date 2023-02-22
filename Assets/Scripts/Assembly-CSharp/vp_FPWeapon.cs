using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_FPWeapon : vp_Component
{
	private static bool loadCustomData = false;

	private static float customRenderingFieldOfView = -20f;

	private static bool customBob = true;

	public static Vector3 customPositionOffset = Vector3.zero;

	public bool Spectator;

	public float SpectatorVelocity;

	public GameObject WeaponPrefab;

	protected GameObject m_WeaponModel;

	public CharacterController Controller;

	public CryptoFloat RenderingZoomDamping = 0.5f;

	protected float m_FinalZoomTime = nValue.float0;

	public CryptoFloat RenderingFieldOfView = 35f;

	public CryptoVector2 RenderingClippingPlanes = new Vector2(0.01f, 10f);

	public CryptoFloat RenderingZScale = 1f;

	public CryptoVector3 PositionOffset = new Vector3(0.15f, -0.15f, -0.15f);

	public CryptoFloat PositionSpringStiffness = 0.01f;

	public CryptoFloat PositionSpringDamping = 0.25f;

	public CryptoFloat PositionFallRetract = 1f;

	public CryptoFloat PositionPivotSpringStiffness = 0.01f;

	public CryptoFloat PositionPivotSpringDamping = 0.25f;

	public CryptoFloat PositionSpring2Stiffness = 0.95f;

	public CryptoFloat PositionSpring2Damping = 0.25f;

	public CryptoFloat PositionKneeling = 0.06f;

	public CryptoInt PositionKneelingSoftness = 1;

	public CryptoVector3 PositionWalkSlide = new Vector3(0.5f, 0.75f, 0.5f);

	public CryptoVector3 PositionPivot = Vector3.zero;

	public CryptoVector3 RotationPivot = Vector3.zero;

	public CryptoFloat PositionInputVelocityScale = 1f;

	public CryptoFloat PositionMaxInputVelocity = 25f;

	protected vp_SpringThread m_PositionSpring;

	protected vp_SpringThread m_PositionSpring2;

	protected vp_SpringThread m_PositionPivotSpring;

	protected vp_SpringThread m_RotationPivotSpring;

	protected GameObject m_WeaponCamera;

	protected GameObject m_WeaponGroup;

	protected GameObject m_Pivot;

	protected Transform m_WeaponGroupTransform;

	public CryptoVector3 RotationOffset = Vector3.zero;

	public CryptoFloat RotationSpringStiffness = 0.01f;

	public CryptoFloat RotationSpringDamping = 0.25f;

	public CryptoFloat RotationPivotSpringStiffness = 0.01f;

	public CryptoFloat RotationPivotSpringDamping = 0.25f;

	public CryptoFloat RotationSpring2Stiffness = 0.95f;

	public CryptoFloat RotationSpring2Damping = 0.25f;

	public CryptoFloat RotationKneeling = 0f;

	public CryptoInt RotationKneelingSoftness = 1;

	public CryptoVector3 RotationLookSway = new Vector3(1f, 0.7f, 0f);

	public CryptoVector3 RotationStrafeSway = new Vector3(0.3f, 1f, 1.5f);

	public CryptoVector3 RotationFallSway = new Vector3(1f, -0.5f, -3f);

	public CryptoFloat RotationSlopeSway = 0.5f;

	public CryptoFloat RotationInputVelocityScale = 1f;

	public CryptoFloat RotationMaxInputVelocity = 15f;

	protected vp_SpringThread m_RotationSpring;

	protected vp_SpringThread m_RotationSpring2;

	protected Vector3 m_SwayVel = Vector3.zero;

	protected Vector3 m_FallSway = Vector3.zero;

	public CryptoFloat RetractionDistance = 0f;

	public CryptoVector2 RetractionOffset = new Vector2(0f, 0f);

	public CryptoFloat RetractionRelaxSpeed = 0.25f;

	protected bool m_DrawRetractionDebugLine;

	public CryptoFloat ShakeSpeed = 0.05f;

	public CryptoVector3 ShakeAmplitude = new Vector3(0.25f, 0f, 2f);

	protected Vector3 m_Shake = Vector3.zero;

	public CryptoVector4 BobRate = new Vector4(0.9f, 0.45f, 0f, 0f);

	public CryptoVector4 BobAmplitude = new Vector4(0.35f, 0.5f, 0f, 0f);

	protected float m_LastBobSpeed;

	protected Vector4 m_CurrentBobAmp = Vector4.zero;

	protected Vector4 m_CurrentBobVal = Vector4.zero;

	protected float m_BobSpeed;

	public CryptoVector3 StepPositionForce = new Vector3(0f, -0.0012f, -0.0012f);

	public CryptoVector3 StepRotationForce = new Vector3(0f, 0f, 0f);

	public CryptoInt StepSoftness = 4;

	public CryptoFloat StepMinVelocity = 0f;

	public CryptoFloat StepPositionBalance = 0f;

	public CryptoFloat StepRotationBalance = 0f;

	public CryptoFloat StepForceScale = 1f;

	protected float m_LastUpBob;

	protected bool m_BobWasElevating;

	protected Vector3 m_PosStep = Vector3.zero;

	protected Vector3 m_RotStep = Vector3.zero;

	public AudioClip SoundWield;

	public AudioClip SoundUnWield;

	public AnimationClip AnimationWield;

	public AnimationClip AnimationUnWield;

	public List<UnityEngine.Object> AnimationAmbient = new List<UnityEngine.Object>();

	protected List<bool> m_AmbAnimPlayed = new List<bool>();

	public Vector2 AmbientInterval = new Vector2(2.5f, 7.5f);

	protected int m_CurrentAmbientAnimation;

	protected int m_AnimationAmbientTimer;

	public CryptoVector3 PositionExitOffset = new Vector3(0f, -1f, 0f);

	public CryptoVector3 RotationExitOffset = new Vector3(40f, 0f, 0f);

	protected bool m_Wielded = true;

	protected Vector2 m_MouseMove = Vector2.zero;

	private Vector3 controllerVelocity;

	private bool controllerIsGrounded;

	private Vector3 swayLocalVelocity;

	private float bobTime;

	private Vector2 LookAxis;

	public bool Wielded
	{
		get
		{
			return m_Wielded && base.Rendering;
		}
	}

	public GameObject WeaponCamera
	{
		get
		{
			return m_WeaponCamera;
		}
	}

	public GameObject WeaponModel
	{
		get
		{
			return m_WeaponModel;
		}
	}

	public CryptoVector3 DefaultPosition
	{
		get
		{
			return (CryptoVector3)base.DefaultState.Preset.GetFieldValue("PositionOffset");
		}
	}

	public CryptoVector3 DefaultRotation
	{
		get
		{
			return (CryptoVector3)base.DefaultState.Preset.GetFieldValue("RotationOffset");
		}
	}

	public bool DrawRetractionDebugLine
	{
		get
		{
			return m_DrawRetractionDebugLine;
		}
		set
		{
			m_DrawRetractionDebugLine = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (base.transform.parent == null)
		{
			Debug.LogError(string.Concat("Error (", this, ") Must not be placed in scene root. Disabling self."));
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		if (PlayerInput.instance != null)
		{
			Controller = PlayerInput.instance.mCharacterController;
		}
		else
		{
			Controller = base.Transform.root.GetComponent<CharacterController>();
		}
		if (Controller == null)
		{
			Debug.LogError(string.Concat("Error (", this, ") Could not find CharacterController. Disabling self."));
			vp_Utility.Activate(base.gameObject, false);
			return;
		}
		base.Transform.eulerAngles = Vector3.zero;
		Camera camera = null;
		foreach (Transform item in base.Transform.parent)
		{
			camera = (Camera)item.GetComponent(typeof(Camera));
			if (camera != null)
			{
				m_WeaponCamera = camera.gameObject;
				break;
			}
		}
		if (GetComponent<Collider>() != null)
		{
			GetComponent<Collider>().enabled = false;
		}
		if (!loadCustomData)
		{
			customRenderingFieldOfView = GameConsole.Load("weapon_fov", 0f);
			customBob = GameConsole.Load("weapon_bobup", true);
			loadCustomData = true;
		}
	}

	protected override void Start()
	{
		base.Start();
		InstantiateWeaponModel();
		m_WeaponGroup = new GameObject(base.name + "Transform");
		m_WeaponGroupTransform = m_WeaponGroup.transform;
		m_WeaponGroupTransform.parent = base.Transform.parent;
		m_WeaponGroupTransform.localPosition = PositionOffset + customPositionOffset;
		vp_Layer.Set(m_WeaponGroup, 31);
		base.Transform.parent = m_WeaponGroupTransform;
		base.Transform.localPosition = Vector3.zero;
		m_WeaponGroupTransform.localEulerAngles = RotationOffset;
		if (m_WeaponCamera != null && vp_Utility.IsActive(m_WeaponCamera.gameObject))
		{
			vp_Layer.Set(base.gameObject, 31, true);
		}
		m_Pivot = new GameObject("Pivot");
		m_Pivot.gameObject.transform.localScale = new Vector3(nValue.float01, nValue.float01, nValue.float01);
		m_Pivot.transform.parent = m_WeaponGroupTransform;
		m_Pivot.transform.localPosition = Vector3.zero;
		m_Pivot.layer = 31;
		vp_Utility.Activate(m_Pivot.gameObject, false);
		m_PositionSpring = new vp_SpringThread();
		m_PositionSpring.RestState = PositionOffset + customPositionOffset;
		m_PositionPivotSpring = new vp_SpringThread();
		m_PositionPivotSpring.RestState = PositionPivot;
		m_PositionSpring2 = new vp_SpringThread();
		m_PositionSpring2.MinVelocity = nValue.float000001;
		m_RotationSpring = new vp_SpringThread();
		m_RotationSpring.RestState = RotationOffset;
		m_RotationPivotSpring = new vp_SpringThread();
		m_RotationPivotSpring.RestState = RotationPivot;
		m_RotationSpring2 = new vp_SpringThread();
		m_RotationSpring2.MinVelocity = nValue.float000001;
		SnapSprings();
		Refresh();
	}

	public virtual void InstantiateWeaponModel()
	{
		if (WeaponPrefab != null)
		{
			if (m_WeaponModel != null && m_WeaponModel != base.gameObject)
			{
				UnityEngine.Object.Destroy(m_WeaponModel);
			}
			m_WeaponModel = (GameObject)UnityEngine.Object.Instantiate(WeaponPrefab);
			m_WeaponModel.transform.parent = base.transform;
			m_WeaponModel.transform.localPosition = Vector3.zero;
			m_WeaponModel.transform.localScale = new Vector3(1f, 1f, RenderingZScale);
			m_WeaponModel.transform.localEulerAngles = Vector3.zero;
			if (m_WeaponCamera != null && vp_Utility.IsActive(m_WeaponCamera.gameObject))
			{
				vp_Layer.Set(m_WeaponModel, 31, true);
			}
		}
		else
		{
			m_WeaponModel = base.gameObject;
		}
		CacheRenderers();
	}

	protected override void Init()
	{
		base.Init();
		ScheduleAmbientAnimation();
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Combine(InputManager.GetAxisEvent, new InputManager.AxisDelegate(GetAxis));
		vp_FPController fPController = PlayerInput.instance.FPController;
		fPController.FallImpactEvent = (Action<float>)Delegate.Combine(fPController.FallImpactEvent, new Action<float>(OnMessage_FallImpact));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		InputManager.GetAxisEvent = (InputManager.AxisDelegate)Delegate.Remove(InputManager.GetAxisEvent, new InputManager.AxisDelegate(GetAxis));
		vp_FPController fPController = PlayerInput.instance.FPController;
		fPController.FallImpactEvent = (Action<float>)Delegate.Remove(fPController.FallImpactEvent, new Action<float>(OnMessage_FallImpact));
		LookAxis = Vector2.zero;
	}

	private void GetAxis(string name, float value)
	{
		if (!Spectator)
		{
			switch (name)
			{
			case "Mouse X":
				LookAxis.x = value;
				break;
			case "Mouse Y":
				LookAxis.y = value;
				break;
			}
		}
	}

	protected override void Update()
	{
		base.Update();
		OnUpdateThread();
		if (m_WeaponGroupTransform.localPosition != m_PositionSpring.State)
		{
			m_WeaponGroupTransform.localPosition = m_PositionSpring.State;
		}
		m_WeaponGroupTransform.localEulerAngles = m_RotationSpring.State + m_RotationSpring2.State;
		base.Transform.localPosition = m_PositionPivotSpring.State + m_PositionSpring2.State;
		if (base.Transform.localEulerAngles != m_RotationPivotSpring.State)
		{
			base.Transform.localEulerAngles = m_RotationPivotSpring.State;
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		controllerVelocity = Controller.velocity;
		controllerIsGrounded = Controller.isGrounded;
		swayLocalVelocity = base.Transform.InverseTransformDirection(m_SwayVel / 60f);
		OnFixedUpdateThread();
	}

	public void OnUpdateThread()
	{
		UpdateMouseLook();
	}

	public void OnFixedUpdateThread()
	{
		UpdateSwaying();
		UpdateBob();
		m_PositionSpring.FixedUpdate();
		m_PositionPivotSpring.FixedUpdate();
		m_RotationPivotSpring.FixedUpdate();
		m_PositionSpring2.FixedUpdate();
		m_RotationSpring.FixedUpdate();
		m_RotationSpring2.FixedUpdate();
	}

	public virtual void AddForce2(Vector3 positional, Vector3 angular)
	{
		m_PositionSpring2.AddForce(positional);
		m_RotationSpring2.AddForce(angular);
	}

	public virtual void AddForce2(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
	{
		AddForce2(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot));
	}

	public virtual void AddForce(Vector3 force)
	{
		m_PositionSpring.AddForce(force);
	}

	public virtual void AddForce(float x, float y, float z)
	{
		AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce(Vector3 positional, Vector3 angular)
	{
		m_PositionSpring.AddForce(positional);
		m_RotationSpring.AddForce(angular);
	}

	public virtual void AddForce(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot)
	{
		AddForce(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot));
	}

	public virtual void AddSoftForce(Vector3 force, int frames)
	{
		m_PositionSpring.AddSoftForce(force, frames);
	}

	public virtual void AddSoftForce(float x, float y, float z, int frames)
	{
		AddSoftForce(new Vector3(x, y, z), frames);
	}

	public virtual void AddSoftForce(Vector3 positional, Vector3 angular, int frames)
	{
		m_PositionSpring.AddSoftForce(positional, frames);
		m_RotationSpring.AddSoftForce(angular, frames);
	}

	public virtual void AddSoftForce(float xPos, float yPos, float zPos, float xRot, float yRot, float zRot, int frames)
	{
		AddSoftForce(new Vector3(xPos, yPos, zPos), new Vector3(xRot, yRot, zRot), frames);
	}

	protected virtual void UpdateMouseLook()
	{
		m_MouseMove.x = LookAxis.x / (Time.deltaTime * 60f);
		m_MouseMove.y = LookAxis.y / (Time.deltaTime * 60f);
		m_MouseMove *= (float)RotationInputVelocityScale;
		m_MouseMove = Vector3.Min(m_MouseMove, Vector3.one * RotationMaxInputVelocity);
		m_MouseMove = Vector3.Max(m_MouseMove, Vector3.one * (0f - (float)RotationMaxInputVelocity));
	}

	protected virtual void UpdateZoom()
	{
		if (!(m_FinalZoomTime <= Time.time) && m_Wielded)
		{
			RenderingZoomDamping = Mathf.Max(RenderingZoomDamping, nValue.float001);
			float t = nValue.float1 - (m_FinalZoomTime - Time.time) / (float)RenderingZoomDamping;
			if (m_WeaponCamera != null && vp_Utility.IsActive(m_WeaponCamera.gameObject))
			{
				m_WeaponCamera.GetComponent<Camera>().fieldOfView = Mathf.SmoothStep(m_WeaponCamera.gameObject.GetComponent<Camera>().fieldOfView, (float)RenderingFieldOfView + customRenderingFieldOfView, t);
			}
		}
	}

	public virtual void Zoom()
	{
		m_FinalZoomTime = Time.time + (float)RenderingZoomDamping;
	}

	public virtual void SnapZoom()
	{
		if (m_WeaponCamera != null && vp_Utility.IsActive(m_WeaponCamera.gameObject))
		{
			m_WeaponCamera.GetComponent<Camera>().fieldOfView = (float)RenderingFieldOfView + customRenderingFieldOfView;
		}
	}

	protected virtual void UpdateShakes()
	{
		if ((float)ShakeSpeed != 0f)
		{
			m_Shake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(ShakeSpeed), ShakeAmplitude);
			m_RotationSpring.AddForce(m_Shake);
		}
	}

	protected virtual void UpdateRetraction(bool firstIteration = true)
	{
		if ((float)RetractionDistance != 0f)
		{
			Vector3 vector = WeaponModel.transform.TransformPoint(new Vector3(RetractionOffset.x, RetractionOffset.y, 0f));
			Vector3 end = vector + WeaponModel.transform.forward * RetractionDistance;
			RaycastHit hitInfo;
			if (Physics.Linecast(vector, end, out hitInfo, -1749041173) && !hitInfo.collider.isTrigger)
			{
				WeaponModel.transform.position = hitInfo.point - (hitInfo.point - vector).normalized * ((float)RetractionDistance * 0.99f);
				WeaponModel.transform.localPosition = Vector3.forward * Mathf.Min(WeaponModel.transform.localPosition.z, 0f);
			}
			else if (firstIteration && WeaponModel.transform.localPosition != Vector3.zero && WeaponModel != base.gameObject)
			{
				WeaponModel.transform.localPosition = Vector3.forward * Mathf.SmoothStep(WeaponModel.transform.localPosition.z, 0f, RetractionRelaxSpeed);
				UpdateRetraction(false);
			}
		}
	}

	protected virtual void UpdateBob()
	{
		if (customBob && !(BobAmplitude == Vector4.zero) && !(BobRate == Vector4.zero))
		{
			if (Spectator)
			{
				m_BobSpeed = SpectatorVelocity;
			}
			else
			{
				m_BobSpeed = (controllerIsGrounded ? controllerVelocity.sqrMagnitude : 0f);
			}
			if (m_BobSpeed > 100f)
			{
				m_BobSpeed = 100f;
			}
			m_BobSpeed = Mathf.Round(m_BobSpeed * 1000f) / 1000f;
			if (m_BobSpeed == 0f)
			{
				m_BobSpeed = m_LastBobSpeed * 0.93f;
			}
			bobTime += 0.02f;
			m_CurrentBobAmp.x = m_BobSpeed * (BobAmplitude.x * -0.0001f);
			m_CurrentBobVal.x = Mathf.Cos(bobTime * (BobRate.x * 10f)) * m_CurrentBobAmp.x;
			m_CurrentBobAmp.y = m_BobSpeed * (BobAmplitude.y * 0.0001f);
			m_CurrentBobVal.y = Mathf.Cos(bobTime * (BobRate.y * 10f)) * m_CurrentBobAmp.y;
			m_PositionSpring.AddForce(m_CurrentBobVal);
			m_LastBobSpeed = m_BobSpeed;
		}
	}

	protected virtual void UpdateSprings()
	{
		m_PositionSpring.FixedUpdate();
		m_PositionPivotSpring.FixedUpdate();
		m_RotationPivotSpring.FixedUpdate();
		m_PositionSpring2.FixedUpdate();
		m_RotationSpring.FixedUpdate();
		m_RotationSpring2.FixedUpdate();
	}

	protected virtual void UpdateStep()
	{
		if ((float)StepMinVelocity <= 0f || !Controller.isGrounded || Controller.velocity.sqrMagnitude < (float)StepMinVelocity)
		{
			return;
		}
		bool flag = ((m_LastUpBob < m_CurrentBobVal.x) ? true : false);
		m_LastUpBob = m_CurrentBobVal.x;
		if (flag && !m_BobWasElevating)
		{
			if (Mathf.Cos(Time.time * (BobRate.x * 5f)) > 0f)
			{
				m_PosStep = StepPositionForce - StepPositionForce * StepPositionBalance;
				m_RotStep = StepRotationForce - StepPositionForce * StepRotationBalance;
			}
			else
			{
				m_PosStep = StepPositionForce + StepPositionForce * StepPositionBalance;
				m_RotStep = Vector3.Scale(StepRotationForce - StepPositionForce * StepRotationBalance, -Vector3.one + Vector3.right * 2f);
			}
			AddSoftForce(m_PosStep * StepForceScale, m_RotStep * StepForceScale, StepSoftness);
		}
		m_BobWasElevating = flag;
	}

	protected virtual void UpdateSwaying()
	{
		m_SwayVel = controllerVelocity * PositionInputVelocityScale;
		m_SwayVel = Vector3.Min(m_SwayVel, Vector3.one * PositionMaxInputVelocity);
		m_SwayVel = Vector3.Max(m_SwayVel, Vector3.one * (0f - (float)PositionMaxInputVelocity));
		m_RotationSpring.AddForce(new Vector3(m_MouseMove.y * (RotationLookSway.x * nValue.float0025), m_MouseMove.x * (RotationLookSway.y * (0f - nValue.float0025)), m_MouseMove.x * (RotationLookSway.z * (0f - nValue.float0025))));
		m_FallSway = RotationFallSway * (m_SwayVel.y * 0.005f);
		if (controllerIsGrounded)
		{
			m_FallSway *= (float)RotationSlopeSway;
		}
		m_FallSway.z = Mathf.Max(0f, m_FallSway.z);
		m_RotationSpring.AddForce(m_FallSway);
		m_PositionSpring.AddForce(Vector3.forward * (0f - Mathf.Abs(m_SwayVel.y * ((float)PositionFallRetract * 2.5E-05f))));
		m_PositionSpring.AddForce(new Vector3(swayLocalVelocity.x * (PositionWalkSlide.x * nValue.float00016), 0f - Mathf.Abs(swayLocalVelocity.x * (PositionWalkSlide.y * nValue.float00016)), (0f - swayLocalVelocity.z) * (PositionWalkSlide.z * nValue.float00016)));
		m_RotationSpring.AddForce(new Vector3(0f - Mathf.Abs(swayLocalVelocity.x * (RotationStrafeSway.x * nValue.float016)), 0f - swayLocalVelocity.x * (RotationStrafeSway.y * nValue.float016), swayLocalVelocity.x * (RotationStrafeSway.z * nValue.float016)));
	}

	public virtual void ResetSprings(float positionReset, float rotationReset, float positionPauseTime = 0f, float rotationPauseTime = 0f)
	{
		m_PositionSpring.State = Vector3.Lerp(m_PositionSpring.State, m_PositionSpring.RestState, positionReset);
		m_RotationSpring.State = Vector3.Lerp(m_RotationSpring.State, m_RotationSpring.RestState, rotationReset);
		m_PositionPivotSpring.State = Vector3.Lerp(m_PositionPivotSpring.State, m_PositionPivotSpring.RestState, positionReset);
		m_RotationPivotSpring.State = Vector3.Lerp(m_RotationPivotSpring.State, m_RotationPivotSpring.RestState, rotationReset);
		if (positionPauseTime != 0f)
		{
			m_PositionSpring.ForceVelocityFadeIn(positionPauseTime);
		}
		if (rotationPauseTime != 0f)
		{
			m_RotationSpring.ForceVelocityFadeIn(rotationPauseTime);
		}
		if (positionPauseTime != 0f)
		{
			m_PositionPivotSpring.ForceVelocityFadeIn(positionPauseTime);
		}
		if (rotationPauseTime != 0f)
		{
			m_RotationPivotSpring.ForceVelocityFadeIn(rotationPauseTime);
		}
	}

	public override void Refresh()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (m_PositionSpring != null)
		{
			m_PositionSpring.Stiffness = new Vector3(PositionSpringStiffness, PositionSpringStiffness, PositionSpringStiffness);
			m_PositionSpring.Damping = Vector3.one - new Vector3(PositionSpringDamping, PositionSpringDamping, PositionSpringDamping);
			m_PositionSpring.RestState = PositionOffset + customPositionOffset - PositionPivot;
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
		if (base.Rendering)
		{
			if (m_WeaponCamera != null && vp_Utility.IsActive(m_WeaponCamera.gameObject))
			{
				m_WeaponCamera.GetComponent<Camera>().nearClipPlane = RenderingClippingPlanes.x;
				m_WeaponCamera.GetComponent<Camera>().farClipPlane = RenderingClippingPlanes.y;
			}
			Zoom();
		}
	}

	public override void Activate()
	{
		m_Wielded = true;
		base.Rendering = true;
		TimerManager.Cancel(m_DeactivationTimer);
		SnapZoom();
		if (m_WeaponGroup != null && !vp_Utility.IsActive(m_WeaponGroup))
		{
			vp_Utility.Activate(m_WeaponGroup);
		}
		SetPivotVisible(false);
	}

	public override void Deactivate()
	{
		m_Wielded = false;
		if (m_WeaponGroup != null && vp_Utility.IsActive(m_WeaponGroup))
		{
			vp_Utility.Activate(m_WeaponGroup, false);
		}
	}

	public virtual void SnapPivot()
	{
		if (m_PositionSpring != null)
		{
			m_PositionSpring.RestState = PositionOffset + customPositionOffset - PositionPivot;
			m_PositionSpring.State = PositionOffset + customPositionOffset - PositionPivot;
		}
		if (m_WeaponGroup != null)
		{
			m_WeaponGroupTransform.localPosition = PositionOffset + customPositionOffset - PositionPivot;
		}
		if (m_PositionPivotSpring != null)
		{
			m_PositionPivotSpring.RestState = PositionPivot;
			m_PositionPivotSpring.State = PositionPivot;
		}
		if (m_RotationPivotSpring != null)
		{
			m_RotationPivotSpring.RestState = RotationPivot;
			m_RotationPivotSpring.State = RotationPivot;
		}
		base.Transform.localPosition = PositionPivot;
		base.Transform.localEulerAngles = RotationPivot;
	}

	public virtual void SetPivotVisible(bool visible)
	{
		if (!(m_Pivot == null))
		{
			vp_Utility.Activate(m_Pivot.gameObject, visible);
		}
	}

	public virtual void SnapToExit()
	{
		RotationOffset = RotationExitOffset;
		PositionOffset = PositionExitOffset;
		SnapSprings();
		SnapPivot();
	}

	public virtual void SnapSprings()
	{
		if (m_PositionSpring != null)
		{
			m_PositionSpring.RestState = PositionOffset + customPositionOffset - PositionPivot;
			m_PositionSpring.State = PositionOffset + customPositionOffset - PositionPivot;
			m_PositionSpring.Stop(true);
		}
		if (m_WeaponGroup != null)
		{
			m_WeaponGroupTransform.localPosition = PositionOffset + customPositionOffset - PositionPivot;
		}
		if (m_PositionPivotSpring != null)
		{
			m_PositionPivotSpring.RestState = PositionPivot;
			m_PositionPivotSpring.State = PositionPivot;
			m_PositionPivotSpring.Stop(true);
		}
		base.Transform.localPosition = PositionPivot;
		if (m_PositionSpring2 != null)
		{
			m_PositionSpring2.RestState = Vector3.zero;
			m_PositionSpring2.State = Vector3.zero;
			m_PositionSpring2.Stop(true);
		}
		if (m_RotationPivotSpring != null)
		{
			m_RotationPivotSpring.RestState = RotationPivot;
			m_RotationPivotSpring.State = RotationPivot;
			m_RotationPivotSpring.Stop(true);
		}
		base.Transform.localEulerAngles = RotationPivot;
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
	}

	public virtual void StopSprings()
	{
		if (m_PositionSpring != null)
		{
			m_PositionSpring.Stop(true);
		}
		if (m_PositionPivotSpring != null)
		{
			m_PositionPivotSpring.Stop(true);
		}
		if (m_PositionSpring2 != null)
		{
			m_PositionSpring2.Stop(true);
		}
		if (m_RotationSpring != null)
		{
			m_RotationSpring.Stop(true);
		}
		if (m_RotationPivotSpring != null)
		{
			m_RotationPivotSpring.Stop(true);
		}
		if (m_RotationSpring2 != null)
		{
			m_RotationSpring2.Stop(true);
		}
	}

	public virtual void Wield(bool showWeapon = true)
	{
		if (showWeapon)
		{
			SnapToExit();
		}
		if (showWeapon)
		{
			PositionOffset = DefaultPosition;
			RotationOffset = DefaultRotation;
		}
		else
		{
			PositionOffset = PositionExitOffset;
			RotationOffset = RotationExitOffset;
		}
		m_Wielded = showWeapon;
		Refresh();
		base.StateManager.CombineStates();
		if (base.Audio != null)
		{
		}
		if (((!showWeapon) ? AnimationUnWield : AnimationWield) != null && vp_Utility.IsActive(base.gameObject))
		{
			m_WeaponModel.GetComponent<Animation>().CrossFade(((!showWeapon) ? AnimationUnWield : AnimationWield).name);
		}
	}

	public virtual void ScheduleAmbientAnimation()
	{
		if (AnimationAmbient.Count == 0 || !vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		m_AnimationAmbientTimer = TimerManager.In(UnityEngine.Random.Range(AmbientInterval.x, AmbientInterval.y), delegate
		{
			if (vp_Utility.IsActive(base.gameObject))
			{
				m_CurrentAmbientAnimation = UnityEngine.Random.Range(0, AnimationAmbient.Count);
				if (AnimationAmbient[m_CurrentAmbientAnimation] != null)
				{
					m_WeaponModel.GetComponent<Animation>().CrossFadeQueued(AnimationAmbient[m_CurrentAmbientAnimation].name);
					ScheduleAmbientAnimation();
				}
			}
		});
	}

	private void OnMessage_FallImpact(float impact)
	{
		if (!Spectator)
		{
			if (m_PositionSpring != null)
			{
				m_PositionSpring.AddSoftForce(Vector3.down * impact * PositionKneeling, (int)PositionKneelingSoftness);
			}
			if (m_RotationSpring != null)
			{
				m_RotationSpring.AddSoftForce(Vector3.right * impact * RotationKneeling, (int)RotationKneelingSoftness);
			}
		}
	}
}
