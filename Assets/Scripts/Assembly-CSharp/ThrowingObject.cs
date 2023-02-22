using Photon;
using UnityEngine;

public class ThrowingObject : Photon.MonoBehaviour, IPunObservable
{
	public float time = 3f;

	public float speed = 5f;

	public GameObject cachedGameObject;

	public Transform cachedTransform;

	public Rigidbody cachedRigidbody;

	public GameObject cachedGameObjectModel;

	private Vector3 PhotonPosition = Vector3.zero;

	private Quaternion PhotonRotation = Quaternion.identity;

	private bool isMine;

	private bool isActive;

	private void Start()
	{
		isMine = base.photonView.isMine;
		cachedRigidbody.isKinematic = !base.photonView.isMine;
		cachedRigidbody.useGravity = base.photonView.isMine;
		if (isMine)
		{
			base.photonView.RPC("SetTime", PhotonTargets.All);
			PhotonPosition = cachedRigidbody.position;
			PhotonRotation = cachedRigidbody.rotation;
		}
	}

	private void OnEnable()
	{
		isActive = true;
	}

	private void OnDisable()
	{
		isActive = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.name, other.gameObject);
		if (isMine && !other.CompareTag("Player"))
		{
			cachedRigidbody.isKinematic = true;
			cachedRigidbody.useGravity = false;
		}
	}

	[PunRPC]
	private void SetTime(PhotonMessageInfo info)
	{
		if (isActive)
		{
			float num = time - (float)(PhotonNetwork.time - info.timestamp);
			if (num > 0f)
			{
				TimerManager.In(time - (float)(PhotonNetwork.time - info.timestamp), Clear);
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
