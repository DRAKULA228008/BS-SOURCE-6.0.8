using System.Collections.Generic;
using Photon;
using UnityEngine;

public class GrenadeObject : Photon.MonoBehaviour, IPunObservable
{
	public static List<GrenadeObject> list = new List<GrenadeObject>();

	public float time = 3f;

	public float speed = 5f;

	public ParticleSystem effect;

	public Transform effectTransform;

	public GameObject cachedGameObject;

	public Transform cachedTransform;

	public Rigidbody cachedRigidbody;

	public GameObject cachedGameObjectModel;

	private Vector3 PhotonPosition = Vector3.zero;

	private Quaternion PhotonRotation = Quaternion.identity;

	private bool isMine;

	private bool isActive;

	private void Awake()
	{
		base.photonView.AddPunObservable(this);
		base.photonView.AddMessage("SetTime", SetTime);
	}

	private void OnEnable()
	{
		list.Add(this);
		isActive = true;
		isMine = base.photonView.isMine;
		PhotonRotation = Quaternion.identity;
		PhotonPosition = Vector3.zero;
		cachedGameObjectModel.SetActive(true);
		cachedRigidbody.isKinematic = !base.photonView.isMine;
		if (!cachedRigidbody.isKinematic)
		{
			cachedRigidbody.velocity = Vector3.zero;
		}
		cachedRigidbody.useGravity = base.photonView.isMine;
		if (isMine)
		{
			base.photonView.RPC("SetTime", PhotonTargets.All);
			PhotonPosition = cachedRigidbody.position;
			PhotonRotation = cachedRigidbody.rotation;
		}
	}

	private void OnDisable()
	{
		list.Remove(this);
		isActive = false;
	}

	[PunRPC]
	private void SetTime(PhotonMessage message)
	{
		if (isActive)
		{
			float num = time - (float)(PhotonNetwork.time - message.timestamp);
			if (num > 0f)
			{
				TimerManager.In(time - (float)(PhotonNetwork.time - message.timestamp), Bomb);
			}
			else
			{
				Clear();
			}
		}
		else
		{
			Clear();
		}
	}

	private void Bomb()
	{
		int num = (int)Vector3.Distance(PlayerInput.instance.PlayerTransform.position, cachedTransform.position);
		int value = (nValue.int12 - num) * nValue.int6;
		value = Mathf.Clamp(value, nValue.int0, nValue.int80);
		if (value > nValue.int0 && PhotonNetwork.player.GetTeam() != base.photonView.owner.GetTeam())
		{
			DamageInfo damageInfo = DamageInfo.Get(value, Vector3.zero, base.photonView.owner.GetTeam(), 46, nValue.int0, base.photonView.owner.ID, false);
			PlayerInput.instance.Damage(damageInfo);
			PlayerInput.instance.FPCamera.AddRollForce(Random.Range((float)(-value) * 0.03f, (float)value * 0.03f));
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != this && list[i].photonView.isMine)
			{
				list[i].cachedRigidbody.AddExplosionForce(150f, cachedTransform.position, 15f);
			}
		}
		cachedGameObjectModel.SetActive(false);
		effectTransform.eulerAngles = new Vector3(-90f, 0f, 0f);
		effect.Play(true);
		SoundClip soundClip = SoundManager.Play3D("Explosion", cachedTransform.position);
		if (soundClip != null)
		{
			soundClip.GetSource().rolloffMode = AudioRolloffMode.Linear;
		}
		TimerManager.In(0.2f, Clear);
	}

	private void Clear()
	{
		if (isActive && base.photonView.isMine)
		{
			PhotonNetwork.Destroy(cachedGameObject);
		}
	}

	private void Update()
	{
		if (!isMine)
		{
			cachedRigidbody.MovePosition(Vector3.Lerp(cachedRigidbody.position, PhotonPosition, Time.deltaTime * speed));
			cachedRigidbody.MoveRotation(Quaternion.Lerp(cachedRigidbody.rotation, PhotonRotation, Time.deltaTime * speed));
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream)
	{
		if (stream.isWriting)
		{
			stream.Write(cachedRigidbody.position);
			stream.Write(cachedRigidbody.rotation);
		}
		else
		{
			PhotonPosition = stream.ReadVector3();
			PhotonRotation = stream.ReadQuaternion();
		}
	}
}
