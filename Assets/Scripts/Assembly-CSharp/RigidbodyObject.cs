using System;
using Photon;
using UnityEngine;

public class RigidbodyObject : Photon.MonoBehaviour, IPunObservable
{
	public Rigidbody mRigidbody;

	private Transform mTransform;

	public SyncBuffer syncBuffer;

	private float timeSinceLastSync;

	private Vector3 lastSentVelocity;

	private Vector3 lastSentPosition;

	private bool forceSync;

	private PhotonPlayer LastContactPlayer;

	private void Awake()
	{
		base.photonView.AddPunObservable(this);
		mTransform = base.transform;
	}

	private void Start()
	{
		base.photonView.AddMessage("PhotonForce", PhotonForce);
	}

	private void OnEnable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Combine(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	private void OnDisable()
	{
		PhotonNetwork.onPhotonPlayerConnected = (PhotonNetwork.PhotonPlayerDelegate)Delegate.Remove(PhotonNetwork.onPhotonPlayerConnected, new PhotonNetwork.PhotonPlayerDelegate(OnPhotonPlayerConnected));
	}

	public void Force(Vector3 force)
	{
		PhotonDataWrite data = base.photonView.GetData();
		data.Write(force);
		data.Write(PhotonNetwork.player);
		base.photonView.RPC("PhotonForce", PhotonTargets.MasterClient, data);
	}

	public void Force(Vector3 force, PhotonPlayer player)
	{
		PhotonDataWrite data = base.photonView.GetData();
		data.Write(force);
		data.Write(player);
		base.photonView.RPC("PhotonForce", PhotonTargets.MasterClient, data);
	}

	[PunRPC]
	private void PhotonForce(PhotonMessage message)
	{
		Vector3 force = message.ReadVector3();
		PhotonPlayer lastContactPlayer = PhotonPlayer.Find(message.ReadInt());
		if (message.timestamp + 0.5 > PhotonNetwork.time)
		{
			LastContactPlayer = lastContactPlayer;
			mRigidbody.AddForce(force);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		forceSync = true;
	}

	public void OnPhotonSerializeView(PhotonStream stream)
	{
		if (stream.isWriting)
		{
			if (forceSync || !(mRigidbody.velocity == lastSentVelocity) || !(mRigidbody.position == lastSentPosition))
			{
				forceSync = false;
				stream.Write(timeSinceLastSync);
				stream.Write(mRigidbody.position);
				stream.Write(mRigidbody.rotation);
				stream.Write(mRigidbody.velocity);
				lastSentVelocity = mRigidbody.velocity;
				lastSentPosition = mRigidbody.position;
				timeSinceLastSync = 0f;
			}
		}
		else
		{
			float interpolationTime = Mathf.Max(stream.ReadFloat(), 0.001f);
			Vector3 position = stream.ReadVector3();
			Quaternion rotation = stream.ReadQuaternion();
			Vector3 value = stream.ReadVector3();
			syncBuffer.AddKeyframe(interpolationTime, position, rotation, value);
		}
	}

	private void FixedUpdate()
	{
		if (base.photonView.isMine)
		{
			timeSinceLastSync += Time.deltaTime;
		}
	}

	private void Update()
	{
		if (!base.photonView.isMine && syncBuffer.HasKeyframes)
		{
			syncBuffer.UpdatePlayback(Time.deltaTime);
			mTransform.position = syncBuffer.Position;
			mTransform.rotation = syncBuffer.Rotation;
		}
	}

	public PhotonPlayer GetLastContactPlayer()
	{
		return LastContactPlayer;
	}
}
